using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ED9FontCreator.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ED9FontCreator.Views
{
    public class CharCanvas : Control
    {
        #region Control Properties

        public static readonly StyledProperty<FontSettings> FontProperty =
            AvaloniaProperty.Register<CharCanvas, FontSettings>(nameof(Font));

        public static readonly StyledProperty<FntSettings> FntProperty =
            AvaloniaProperty.Register<CharCanvas, FntSettings>(nameof(Fnt));

        public static readonly StyledProperty<List<FntChar>?> CharsProperty =
            AvaloniaProperty.Register<CharCanvas, List<FntChar>?>(nameof(Chars));

        public static readonly StyledProperty<IBrush?> BackgroundProperty =
            AvaloniaProperty.Register<CharCanvas, IBrush?>(nameof(Background), Brushes.Black);

        public static readonly StyledProperty<bool> IsSimplifyProperty =
            AvaloniaProperty.Register<CharCanvas, bool>(nameof(IsSimplify));

        public static readonly StyledProperty<IBrush?> ForegroundProperty =
            AvaloniaProperty.Register<CharCanvas, IBrush?>(nameof(Foreground), Brushes.White);

        public static readonly StyledProperty<IBrush?> CharBackgroundProperty =
            AvaloniaProperty.Register<CharCanvas, IBrush?>(nameof(CharBackground));

        public static readonly StyledProperty<bool> IsPreviewProperty =
            AvaloniaProperty.Register<CharCanvas, bool>(nameof(IsPreview));

        public static readonly StyledProperty<bool> IsShowCharBackgroundProperty =
            AvaloniaProperty.Register<CharCanvas, bool>(nameof(IsShowCharBackground));


        public bool IsShowCharBackground
        {
            get => this.GetValue(IsShowCharBackgroundProperty);
            set => SetValue(IsShowCharBackgroundProperty, value);
        }


        public bool IsPreview
        {
            get => this.GetValue(IsPreviewProperty);
            set => SetValue(IsPreviewProperty, value);
        }

        public IBrush? CharBackground
        {
            get => this.GetValue(CharBackgroundProperty);
            set => SetValue(CharBackgroundProperty, value);
        }

        public IBrush? Foreground
        {
            get => this.GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public bool IsSimplify
        {
            get => this.GetValue(IsSimplifyProperty);
            set => SetValue(IsSimplifyProperty, value);
        }

        public IBrush? Background
        {
            get => this.GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public List<FntChar>? Chars
        {
            get => this.GetValue(CharsProperty);
            set => SetValue(CharsProperty, value);
        }

        public FntSettings Fnt
        {
            get => this.GetValue(FntProperty);
            set => SetValue(FntProperty, value);
        }

        public FontSettings Font
        {
            get => this.GetValue(FontProperty);
            set => SetValue(FontProperty, value);
        }

        #endregion Control Properties

        private Size _size;
        private readonly List<FormattedText> _formattedTexts = [];
        private int _charsHeight;

        static CharCanvas()
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
            _formattedTexts.Clear();
            _charsHeight = 0;
            if (Chars == null)
            {
                InvalidateMeasure();
                return;
            }
            var typeface = new Typeface(new FontFamily(Font.FontName + ",Segoe UI Symbol"), Enum.Parse<FontStyle>(Font.FontStyle), Enum.Parse<FontWeight>(Font.FontWeight));
            short x = 0, y = 0, highest = 0;

            if (IsPreview)
                y += Fnt.FntYOffset;

            foreach (var fntChar in Chars)
            {
                var @char = FntHelper.GetString(fntChar.Code, IsSimplify);
                var text = new FormattedText(
                    @char,
                    CultureInfo.CurrentCulture,
                    FlowDirection,
                    typeface,
                    Font.FontSize,
                    Foreground);
                _formattedTexts.Add(text);

                fntChar.Width = (short)Math.Ceiling(text.Width + Font.FontMarginL + Font.FontMarginR);
                fntChar.Height = (short)Math.Ceiling(text.Height + Font.FontMarginT + Font.FontMarginB);
                fntChar.NextCharOffset = (short)(fntChar.Width + Fnt.FntNextCharExOffset);

                if (x + fntChar.Width + (IsPreview ? Fnt.FntXOffset : 0) > Bounds.Width)
                {
                    x = 0;
                    y += highest;
                    highest = 0;
                    if (IsPreview)
                        y += Fnt.FntYOffset;
                }

                if (highest < fntChar.Height)
                    highest = fntChar.Height;

                if (IsPreview)
                {
                    x += Fnt.FntXOffset;
                }

                fntChar.X = x;
                fntChar.Y = y;

                x += fntChar.Width;
            }

            _charsHeight = y + highest;
            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_charsHeight > 0 && double.IsInfinity(availableSize.Height))
            {
                _size = new Size(availableSize.Width, _charsHeight);
                return _size;
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _size = _charsHeight > 0 && double.IsNaN(Height) ? new Size(finalSize.Width, _charsHeight) : finalSize;
            return base.ArrangeOverride(finalSize);
        }

        public override void Render(DrawingContext context)
        {
            if (Background != null)
                context.DrawRectangle(Background, null, new Rect(Bounds.Size));
            if (Chars == null) return;

            if (IsShowCharBackground)
            {
                Chars.ForEach(fntChar =>
                {
                    context.DrawRectangle(CharBackground, null, new(fntChar.X, fntChar.Y, fntChar.Width, fntChar.Height));
                });
            }

            for (var i = 0; i < _formattedTexts.Count; i++)
            {
                var text = _formattedTexts[i];
                var fntChar = Chars[i];

                //context.DrawRectangle(Brushes.Bisque ,null, new(x, y, text.Width, text.Height));
                var x = fntChar.X + Font.FontMarginL;
                var y = fntChar.Y + Font.FontMarginT;

                context.DrawText(text, new Point(x, y));
            }
        }
    }
}