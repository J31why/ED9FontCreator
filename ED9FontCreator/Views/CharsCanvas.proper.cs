using Avalonia;
using Avalonia.Controls;
using ED9FontCreator.ViewModels;
using System.Collections.Generic;

namespace ED9FontCreator.Views
{
    public partial class CharsCanvas : Control
    {


        public static readonly StyledProperty<FontSettings> FontProperty =
            AvaloniaProperty.Register<CharsCanvas, FontSettings>(nameof(Font));

        public static readonly StyledProperty<List<FntChar>?> CharsProperty =
            AvaloniaProperty.Register<CharsCanvas, List<FntChar>?>(nameof(Chars));

        public static readonly StyledProperty<bool> IsPreviewProperty =
            AvaloniaProperty.Register<CharsCanvas, bool>(nameof(IsPreview));

        public static readonly StyledProperty<bool> IsShowCharBackgroundProperty =
            AvaloniaProperty.Register<CharsCanvas, bool>(nameof(IsShowCharBackground));

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

        public List<FntChar>? Chars
        {
            get => this.GetValue(CharsProperty);
            set => SetValue(CharsProperty, value);
        }

        public FontSettings Font
        {
            get => this.GetValue(FontProperty);
            set => SetValue(FontProperty, value);
        }


    }
}