using CommunityToolkit.Mvvm.ComponentModel;

namespace ED9FontCreator.ViewModels
{
    public partial class FntSettings : ViewModelBase
    {
        [ObservableProperty] private short _fntXOffset = 0;
        [ObservableProperty] private short _fntYOffset = 0;
    }
}
