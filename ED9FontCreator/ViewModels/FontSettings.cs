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
        [ObservableProperty] private short _fontSize = 38;
        [ObservableProperty] private string _fontWeight = nameof(Avalonia.Media.FontWeight.Bold);
        [ObservableProperty] private string _fontStyle = nameof(Avalonia.Media.FontStyle.Normal);
        [ObservableProperty] private short _fontMarginL = 4;
        [ObservableProperty] private short _fontMarginT = -8;
        [ObservableProperty] private short _fontMarginR = 4;
        [ObservableProperty] private short _fontMarginB = 0;

    }
}