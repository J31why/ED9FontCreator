using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Chinese;
using CommunityToolkit.Mvvm.Input;
using ED9FontCreator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ED9FontCreator.ViewModels
{
    public partial class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
#if DEBUG
            _fntPath = @"E:\Games\ED9A\asset\common\font\font_0.fnt.bak";
            _fontSettings.FontName = "Source Han Serif CN";
#endif
            OutDir = Path.Combine(Environment.CurrentDirectory, "out");
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
            var temp = new List<FntChar>();
            foreach (var c in charArr)
            {
                temp.Add(new FntChar()
                {
                    Code = FntHelper.GetCode(c.ToString()),
                    XOffset = FntSettings.FntXOffset,
                    YOffset = FntSettings.FntYOffset
                });
            }
            PreviewChars = temp;
        }

        [RelayCommand]
        private void DefaultUserSetting()
        {
            FontSettings = new();
            FntSettings = new();
        }

        [RelayCommand]
        private void ExportCharPng()
        {
            if (DrawChars?.Count == 0) return;
            var size = new PixelSize((int)DrawCanvas.Bounds.Width, (int)DrawCanvas.Bounds.Height);
            DrawCanvas.UpdateLayout();
            using RenderTargetBitmap bitmap = new(size, new Vector(96, 96));
            bitmap.Render(DrawCanvas);
            var file = Path.Combine(OutDir, IsRedChars ? "r.png" : "g.png");
            bitmap.Save(file);
            if (IsRedChars)
                TempFntData.Clear();

            SaveTempFnt();
        }

        [RelayCommand]
        private void GenerateChars(bool isRed)
        {
            if (isRed)
            {
                InitReplaceGroup();
                TempFntData.Clear();
            }
            IsRedChars = isRed;
            var chars = Fnt!.Chars.Where(x => x.ColorChannel == (isRed ? 0x200 : 0x100)).ToList();
            var temp = new List<FntChar>();
            foreach (var c in chars)
            {
                temp.Add(new()
                {
                    Code = c.Code,
                    ColorChannel = c.ColorChannel,
                    Offset = c.Offset,
                    Type = 0,
                    XOffset = FntSettings.FntXOffset,
                    YOffset = FntSettings.FntYOffset
                });
            }
            DrawChars = temp;
        }

        private void SaveTempFnt()
        {
            if (DrawChars == null) return;
            foreach (var fntChar in DrawChars)
            {
                TempFntData.Add(fntChar);
            }
            this.ExportTempFntCommand.NotifyCanExecuteChanged();
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

        private void InitReplaceGroup()
        {
            FntHelper.ReplaceGroup.Clear();
            var regex = new Regex(@"\[(.*?)\]\s*=\s*\[(.*?)\]");
            var matches = regex.Matches(ReplaceText);
            foreach (Match match in matches)
            {
                var group = new ReplaceItem(match.Groups[1].Value, match.Groups[2].Value);
                FntHelper.ReplaceGroup.Add(group);
            }
        }
    }
}