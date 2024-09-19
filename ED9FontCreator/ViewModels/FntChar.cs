using CommunityToolkit.Mvvm.ComponentModel;

namespace ED9FontCreator.ViewModels
{
    public partial class FntChar : ViewModelBase
    {
        [ObservableProperty] private long _offset;
        [ObservableProperty] private int _code;
        [ObservableProperty] private int _type;
        [ObservableProperty] private short _x;
        [ObservableProperty] private short _y;
        [ObservableProperty] private short _width;
        [ObservableProperty] private short _height;
        [ObservableProperty] private short _colorChannel;
        [ObservableProperty] private short _xOffset;
        [ObservableProperty] private short _yOffset;
        [ObservableProperty] private short _nextCharOffset;
    }
}