using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using ED9FontCreator.ViewModels;

namespace ED9FontCreator.Views
{
    public partial class CharsCanvas : Control
    {
        public static readonly StyledProperty<FntSettings> FntProperty =
            AvaloniaProperty.Register<CharsCanvas, FntSettings>(nameof(Fnt));

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

        public FntSettings Fnt
        {
            get => this.GetValue(FntProperty);
            set => SetValue(FntProperty, value);
        }
    }
}