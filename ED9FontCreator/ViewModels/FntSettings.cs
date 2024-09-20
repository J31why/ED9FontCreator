using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ED9FontCreator.ViewModels
{
    public partial class FntSettings : ViewModelBase
    {
        [ObservableProperty] private short _fntXOffset = 0;
        [ObservableProperty] private short _fntYOffset = 0xC;
        [ObservableProperty] private short _fntNextCharExOffset = 2;
    }
}
