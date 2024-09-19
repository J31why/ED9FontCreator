using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace ED9FontCreator.ViewModels
{
    public partial class Fnt : ViewModelBase
    {
        [ObservableProperty] public int _totalChars;
        [ObservableProperty] public int _dataLength;
        public byte[] Head = [];
        public List<FntChar> Chars = [];
    }
}