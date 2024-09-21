using Avalonia;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using ED9FontCreator.Views;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ED9FontCreator.ViewModels
{
    public partial class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
#if DEBUG
            _fntPath = @"E:\Games\ED9A\asset\common\font\font_0.fnt";
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
            if (!FntHelper.GetFnt(FntPath, out var fnt))
            {
                ShowInfo("解析失败", InfoBarState.Error);
                return;
            }
            this.Fnt = fnt;
            TempFntData.Clear();
            DrawChars = null;
            ShowInfo("解析成功", InfoBarState.Success);
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
            FntHelper.InitReplaceGroup(ReplaceText);
            var charArr = text.ToCharArray();
            var temp = new List<FntChar>();
            foreach (var c in charArr)
            {
                temp.Add(new FntChar
                {
                    Code = FntHelper.GetCode(c.ToString()),
                    Type = 1,
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
            try
            {
                if (DrawChars == null || DrawChars.Count == 0)
                {
                    ShowInfo("先生成字符", InfoBarState.Alert);
                    return;
                }
                var pSize = new PixelSize((int)DrawCanvas.Bounds.Width, (int)DrawCanvas.Bounds.Height);
                var size = new Size(pSize.Width, pSize.Height);
                using RenderTargetBitmap bitmap = new(pSize, new Vector(96, 96));
                DrawCanvas.Measure(size);
                DrawCanvas.Arrange(new Rect(size));
                DrawCanvas.UpdateLayout();
                bitmap.Render(DrawCanvas);
                var file = Path.Combine(OutDir, IsRedChars ? "r.png" : "g.png");
                bitmap.Save(file);
                if (IsRedChars)
                    TempFntData.Clear();
                SaveTempFnt();
                ShowInfo($"导出{(IsRedChars ? "红色" : "绿色")}字符图片成功", InfoBarState.Success);
            }
            catch
            {
                ShowInfo($"导出{(IsRedChars ? "红色" : "绿色")}字符图片失败", InfoBarState.Error);
            }
        }

        [RelayCommand]
        private void GenerateChars(bool isRed)
        {
            if (isRed)
            {
                FntHelper.InitReplaceGroup(ReplaceText);
                TempFntData.Clear();
            }
            IsRedChars = isRed;
            var chars = Fnt!.Chars.Where(x => x.ColorChannel == (isRed ? 0x200 : 0x100)).ToList();
            chars = chars.OrderBy(x => x.Y).ThenBy(x => x.X).ToList(); //匹配原版排序
            var temp = new List<FntChar>();
            foreach (var c in chars)
            {
                var @char = (char)c.Code;
                temp.Add(new()
                {
                    Code = c.Code,
                    ColorChannel = c.ColorChannel,
                    Offset = c.Offset,
                    Type = @char is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or
                        >= '0' and <= '9' or ' ' or '(' or ')' or '.' ? 1 : 0,
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
            try
            {
                var temp = TempFntData.ToList();
                temp.Sort((x, y) => x.Code.CompareTo(y.Code));
                var file = Path.Combine(OutDir, Path.GetFileName(FntPath));
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
                ShowInfo("导出Fnt成功", InfoBarState.Success);
            }
            catch
            {
                ShowInfo("导出Fnt失败", InfoBarState.Error);
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

        [RelayCommand]
        private void BlendChars()
        {
            try
            {
                var r = Path.Combine(OutDir, "r.png");
                var g = Path.Combine(OutDir, "g.png");
                var file = Path.Combine(OutDir, Path.GetFileNameWithoutExtension(FntPath) + ".png");
                if (!File.Exists(r) || !File.Exists(g))
                {
                    ShowInfo("先生成红绿字符图片", InfoBarState.Alert);
                    return;
                }
                BlendImage(r, g, file);
                ShowInfo("图片生成成功", InfoBarState.Success);
            }
            catch
            {
                ShowInfo("图片生成失败", InfoBarState.Error);
            }
        }

        public void BlendImage(string imagePath1, string imagePath2, string outputPath)
        {
            using var img1 = SKBitmap.Decode(imagePath1);
            using var img2 = SKBitmap.Decode(imagePath2);
            var combinedWidth = Math.Max(img1.Width, img2.Width);
            var combinedHeight = Math.Max(img1.Height, img2.Height);

            using var combinedImage = new SKBitmap(combinedWidth, combinedHeight);
            using var canvas = new SKCanvas(combinedImage);
            canvas.DrawBitmap(img1, new SKPoint(0, 0));
            var paint = new SKPaint
            {
                BlendMode = SKBlendMode.Screen // 滤色模式
            };
            canvas.DrawBitmap(img2, new SKPoint(0, 0), paint);

            using var image = SKImage.FromBitmap(combinedImage);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using var stream = System.IO.File.OpenWrite(outputPath);
            data.SaveTo(stream);
        }

        public void ShowInfo(string text, InfoBarState state = InfoBarState.None)
        {
            InfoText = text;
            InfoState = state;
        }
    }
}