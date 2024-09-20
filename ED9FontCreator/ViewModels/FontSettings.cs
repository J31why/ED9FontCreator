using CommunityToolkit.Mvvm.ComponentModel;

namespace ED9FontCreator.ViewModels
{
    public partial class FontSettings : ViewModelBase
    {
        [ObservableProperty] private string _fontName = "Source Han Serif CN";
        [ObservableProperty] private short _fontSize = 41;
        [ObservableProperty] private string _fontWeight = nameof(Avalonia.Media.FontWeight.Bold);
        [ObservableProperty] private string _fontStyle = nameof(Avalonia.Media.FontStyle.Normal);
        [ObservableProperty] private short _fontMarginL = 0;
        [ObservableProperty] private short _fontMarginT = -4;
        [ObservableProperty] private short _fontMarginR = 0;
        [ObservableProperty] private short _fontMarginB = 0;

    }
}