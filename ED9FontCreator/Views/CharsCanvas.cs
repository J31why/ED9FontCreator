using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using ED9FontCreator.ViewModels;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ED9FontCreator.Views;

public partial class CharsCanvas
{
    private class CharsDraw(Rect bounds, List<FntChar> chars, FntSettings fntSettings, FontSettings fontSettings, bool isPreview, bool isDrawBackground) : ICustomDrawOperation
    {
        public void Dispose()
        {
        }

        public bool Equals(ICustomDrawOperation? other) => false;

        public bool HitTest(Point p) => false;

        public Rect Bounds => bounds;

        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature == null) return;
            using var lease = leaseFeature.Lease();
            var canvas = lease.SkCanvas;
            canvas.Save();

            Enum.TryParse(fontSettings.FontWeight, out SKFontStyleWeight fontWeight);
            Enum.TryParse(fontSettings.FontStyle, out SKFontStyleSlant fontStyle);

            using var typeface = SKTypeface.FromFamilyName(fontSettings.FontName, fontWeight, SKFontStyleWidth.Normal, fontStyle);
            using var symbolTypeface = SKTypeface.FromFamilyName("Segoe UI Symbol", fontWeight, SKFontStyleWidth.Normal, fontStyle);
            
            using var paint = new SKPaint();
            paint.TextSize = fontSettings.FontSize;
            paint.IsAntialias = true;
            if (isPreview)
            {
                paint.Color = new SKColor(255,255,255);
                if (isDrawBackground)
                {
                    using var background = new SKPaint();
                    background.Color = new SKColor(244, 0, 161);
                    Draw(chars, canvas, paint, typeface, symbolTypeface, background);
                }
                else
                {
                    Draw(chars, canvas, paint, typeface, symbolTypeface);
                }
                canvas.Restore();
                return;
            }
            //red
            paint.Color = new SKColor(255, 0, 0);
            var filteredChars = chars.Where(c => c.ColorChannel == 0x200).ToList();
            Draw(filteredChars, canvas, paint, typeface, symbolTypeface);
            //green
            paint.BlendMode = SKBlendMode.Screen;
            paint.Color = new SKColor(0, 255, 0);
            filteredChars = chars.Where(c => c.ColorChannel == 0x100).ToList();
            Draw(filteredChars, canvas, paint, typeface, symbolTypeface);

            canvas.Restore();
        }

        private void Draw(List<FntChar> filteredChars, SKCanvas canvas, SKPaint paint, SKTypeface typeface, SKTypeface symbolTypeface ,SKPaint? background=null)
        {
            short x = 0, y = 0, highest = 0;
            var pixelRect = new SKRect();
            foreach (var c in filteredChars)
            {
                var text = c.ReplacedChar.ToString();
                paint.Typeface = typeface.ContainsGlyph(c.ReplacedChar) ? typeface : symbolTypeface;
                c.Width = (short)Math.Ceiling(paint.MeasureText(text, ref pixelRect));
                c.PixelWidth = (short)Math.Ceiling(pixelRect.Width);
                c.PixelHeight = pixelRect.IsEmpty ? c.Width : (short)Math.Ceiling(pixelRect.Height);
                c.XOffset= (short)Math.Ceiling(pixelRect.Left);
                c.YOffset = (short)Math.Ceiling(pixelRect.Top - paint.FontMetrics.Ascent);
                c.MaxWidth = c.Width > c.PixelWidth ? c.Width : c.PixelWidth;
                short drawXOffset = (short)(c.XOffset > 0 ? 0 : c.XOffset);
                if (x + c.MaxWidth > bounds.Width)
                {
                    x = 0;
                    y += (short)(highest + 1);  //+1防止过于拥挤
                    highest = 0;
                    if (isPreview)
                        y += fntSettings.FntYOffset;
                }

                if (highest < c.PixelHeight)
                    highest = c.PixelHeight;

                if (isPreview)
                {
                    //x += (short)(fntSettings.FntXOffset - c.XOffset);
                    x += fntSettings.FntXOffset;
                    y += (short)(fntSettings.FntYOffset + c.YOffset);
                }

                if (background != null)
                {
                    background.Color = new SKColor(255, 255, 255,150);
                    canvas.DrawRect(x, y, c.MaxWidth, c.PixelHeight, background);
                    background.Color = new SKColor(244, 0, 161);
                    canvas.DrawRect(x +c.XOffset - drawXOffset, y, c.PixelWidth, c.PixelHeight, background);
                }

                canvas.DrawText(text, new SKPoint(x- drawXOffset, y - pixelRect.Top), paint);

                if (isPreview)
                {
                    y -= (short)(fntSettings.FntYOffset + c.YOffset);
                }

                c.X = x;
                c.Y = y;

                x += (short)(c.MaxWidth+1); //+1防止过于拥挤
            }
        }
    }

    static CharsCanvas()
    {
        ClipToBoundsProperty.OverrideDefaultValue(typeof(bool), true);
        IsHitTestVisibleProperty.OverrideDefaultValue(typeof(bool), true);
        FocusableProperty.OverrideDefaultValue(typeof(bool), true);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CharsProperty)
            CharsPropertyChanged();
    }

    private void CharsPropertyChanged()
    {
        InvalidateMeasure();
    }

    public override void Render(DrawingContext context)
    {
        context.DrawRectangle(Brushes.Black, null, new Rect(Bounds.Size));

        if (Chars is not { Count: > 0 }) return;
        context.Custom(new CharsDraw(Bounds, Chars, Fnt, Font, IsPreview, IsShowCharBackground));


        var fontFamily = new FontFamily(Font.FontName + ",Segoe UI Symbol");
        var fontStyle = Enum.Parse<FontStyle>(Font.FontStyle);
        var fontWeight = Enum.Parse<FontWeight>(Font.FontWeight);
        var typeface = new Typeface(fontFamily, fontStyle, fontWeight);
        var x = 0d;
        foreach (var c in Chars)
        {
            var text = new FormattedText(
                c.ReplacedChar.ToString(),
                CultureInfo.CurrentCulture,
                FlowDirection,
                typeface,
                Font.FontSize,
                Brushes.Gray);
            //context.DrawText(text, new Point(x, 1));
            x += Math.Ceiling(text.Width);
        }
        var text1 = new FormattedText(
            "预览: ijfp",
            CultureInfo.CurrentCulture,
            FlowDirection,
            typeface,
            Font.FontSize,
            Brushes.Gray);
        //context.DrawText(text1, new Point(0, 1));
    }
}