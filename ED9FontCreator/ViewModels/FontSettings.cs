using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED9FontCreator.ViewModels
{
    public partial class FontSettings : ViewModelBase
    {
        [ObservableProperty] private string _fontName = "Source Han Serif CN";
        [ObservableProperty] private short _fontSize = 42;
        [ObservableProperty] private string _fontWeight = nameof(Avalonia.Media.FontWeight.Bold);
        [ObservableProperty] private string _fontStyle = nameof(Avalonia.Media.FontStyle.Normal);
        [ObservableProperty] private short _fontMarginL = 2;
        [ObservableProperty] private short _fontMarginT = -12;
        [ObservableProperty] private short _fontMarginR = 2;
        [ObservableProperty] private short _fontMarginB = 0;

        public void Restore()
        {
            FontName = "Source Han Serif CN";
            FontSize = 42;
            FontWeight = nameof(Avalonia.Media.FontWeight.Bold);
            FontStyle = nameof(Avalonia.Media.FontStyle.Normal);
            FontMarginL = 2;
            FontMarginT = -12;
            FontMarginR = 2;
            FontMarginB = 0;
        }
    }
}