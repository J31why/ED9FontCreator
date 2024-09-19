using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Chinese;
using CommunityToolkit.Mvvm.Input;
using ED9FontCreator.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ED9FontCreator.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
#if DEBUG
            _fntPath = @"E:\Games\ED9A\asset\common\font\font_0.fnt.bak";
            _fontName = "Source Han Serif CN";
#endif
            OutDir = System.IO.Path.Combine(Environment.CurrentDirectory, "out");
            if (!Directory.Exists(OutDir))
                Directory.CreateDirectory(OutDir);
            if (File.Exists(ReplaceTxtFile))
                ReplaceText = File.ReadAllText(ReplaceTxtFile);
        }

        [RelayCommand]
        private void AnalyzeFntFile()
        {
            FntHelper.GetFnt(FntPath, out var fnt);
            this.Fnt = fnt;
        }

        [RelayCommand]
        private void SearchFntChar(string? text)
        {
            if (text == null || Fnt == null) return;
            var code = FntHelper.GetCode(text);
            SearchedFntChar = Fnt.Chars.FirstOrDefault(x => x.Code == code);
        }

        [RelayCommand]
        private void PreviewText(string? text)
        {
            if (text == null) return;
            InitReplaceGroup();
            var charArr = text.ToCharArray();
            var font = new FontFamily(FontName + ",Segoe UI Symbol");
            var wp = new WrapPanel();
            foreach (var c in charArr)
            {
                var code = FntHelper.GetCode(c.ToString());
                var t = GenerateCharBlock(code, font, Brushes.White, Brushes.Magenta);
                var border = new Border
                {
                    Child = t,
                    Margin = new Thickness(FntXOffset, FntYOffset, FntNextCharExOffset, 0)
                };
                wp.Children.Add(border);
            }

            PreviewBorder.Child = wp;
        }

        [RelayCommand]
        private void DefaultUserSetting()
        {
            FontName = "SimSun";
            FontSize = 42;
            FontWeight = nameof(Avalonia.Media.FontWeight.Normal);
            FontStyle = nameof(Avalonia.Media.FontStyle.Normal);
            FontClipL = 0;
            FontClipR = 0;
            FontClipT = 0;
            FontClipB = 0;
            FntXOffset = 0;
            FntYOffset = 0xC;
            FntNextCharExOffset = 2;
        }

        [RelayCommand]
        private void GenerateRedChars()
        {
            GenerateChars(true);
        }

        [RelayCommand]
        private void GenerateGreenChars()
        {
            GenerateChars(false);
        }

        [RelayCommand]
        private void ExportCharPng()
        {
            if (CharBorder.Child == null) return;
            var size = new PixelSize((int)CharBorder.Bounds.Width, (int)CharBorder.Bounds.Height);
            CharBorder.UpdateLayout();
            CharBorder.Measure(new Size(CharBorder.Bounds.Width, CharBorder.Bounds.Height));
            CharBorder.Arrange(new Rect(CharBorder.DesiredSize));
            using RenderTargetBitmap bitmap = new(size, new Vector(96, 96));
            bitmap.Render(CharBorder);
            var file = System.IO.Path.Combine(OutDir, IsRedChars ? "r.png" : "g.png");
            bitmap.Save(file);
            if (IsRedChars)
                TempFntData.Clear();
            SaveTempFnt();
        }

        private void SaveTempFnt()
        {
            var wp = CharBorder.Child as WrapPanel;
            if (wp == null) return;
            foreach (var control in wp.Children)
            {
                var tb = (TextBlock)control;
                var c = new FntChar
                {
                    Code = (int)tb.Tag!,
                    Type = 1,
                    X = (short)Math.Ceiling(tb.Bounds.Left < 0 ? 0 : tb.Bounds.Left + FontClipL),
                    Y = (short)Math.Ceiling(tb.Bounds.Top < 0 ? 0 : tb.Bounds.Top + FontClipT),
                    Width = (short)Math.Ceiling(tb.Width - FontClipL - FontClipR),
                    Height = (short)Math.Ceiling(tb.Height - FontClipT - FontClipB),
                    ColorChannel = (short)(IsRedChars ? 0x200 : 0x100),
                    XOffset = FntXOffset,
                    YOffset = FntYOffset,
                };
                c.NextCharOffset = (short)(c.Width + FntNextCharExOffset);
                TempFntData.Add(c);
            }
            ExportTempFntCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanExportTempFnt))]
        private void ExportTempFnt()
        {
            var temp = TempFntData.ToList();
            temp.Sort((x, y) => x.Code.CompareTo(y.Code));
            var file = System.IO.Path.Combine(OutDir, System.IO.Path.GetFileName(FntPath));
            if (File.Exists(file))
                File.Delete(file);
            using FileStream fs = new(file, FileMode.Create);
            fs.Write(Fnt!.Head);
            foreach (FntChar c in temp)
            {
                fs.WriteInt(c.Code);
                fs.WriteInt(c.Type);
                fs.WriteShort(c.X);
                fs.WriteShort(c.Y);
                fs.WriteShort(c.Width);
                fs.WriteShort(c.Height);
                fs.WriteShort(c.ColorChannel);
                fs.WriteShort(c.XOffset);
                fs.WriteShort(c.YOffset);
                fs.WriteShort(c.NextCharOffset);
            }
        }
        [RelayCommand]
        private void OpenOutDir()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = OutDir,
                UseShellExecute = true
            });
        }

        private void GenerateChars(bool isRed)
        {
            if (string.IsNullOrEmpty(FontName)) return;
            if (isRed) TempFntData.Clear();
            InitReplaceGroup();
            IsRedChars = isRed;
            var chars = Fnt!.Chars.Where(x => x.ColorChannel == (isRed ? 0x200 : 0x100)).ToList();
            var color = isRed ? Brushes.Red : Brushes.Lime;
            var font = new FontFamily(FontName + ",Segoe UI Symbol");
            var wp = new WrapPanel();

            foreach (var c in chars)
            {
                var t = GenerateCharBlock(c.Code, font, color);
                wp.Children.Add(t);
            }
            CharBorder.Child = wp;
        }

        private void InitReplaceGroup()
        {
            ReplaceGroup.Clear();
            var regex = new Regex(@"\[(.*?)\]\s*=\s*\[(.*?)\]");
            var matches = regex.Matches(ReplaceText);
            foreach (Match match in matches)
            {
                var group = new ReplaceItem(match.Groups[1].Value, match.Groups[2].Value);
                ReplaceGroup.Add(group);
            }
        }

        private TextBlock GenerateCharBlock(int code, FontFamily font, IBrush foreground, IBrush? background = null)
        {
            var text = Encoding.Unicode.GetString(BitConverter.GetBytes(code)).TrimEnd('\0');
            var match = ReplaceGroup.FirstOrDefault(x => x.Old == text);
            if (match != null) text = match.New;
            if (IsSimplifiedChinese) text = ChineseConverter.ToSimplified(text);
            var t = new TextBlock
            {
                Text = text,
                Foreground = foreground,
                FontFamily = font,
                FontSize = Convert.ToDouble(FontSize),
                FontWeight = Enum.Parse<FontWeight>(FontWeight),
                FontStyle = Enum.Parse<FontStyle>(FontStyle),
                Tag = code,
                Background = background
            };
            t.Loaded += CharJustify_Loaded;
            return t;
        }

        private void CharJustify_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var t = (TextBlock)sender!;
            t.Width = Math.Ceiling(t.Bounds.Width);
            t.Height = Math.Ceiling(t.Bounds.Height);
            t.Clip = new RectangleGeometry(
                new Rect(
                    FontClipL,
                    FontClipT,
                    t.Width - FontClipL - FontClipR,
                    t.Height - FontClipT - FontClipB
                ));
            t.Margin = new Thickness(-FontClipL, -FontClipT, -FontClipR, -FontClipB);
        }
    }
}