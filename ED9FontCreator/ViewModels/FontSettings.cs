﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace ED9FontCreator.ViewModels
{
    public partial class FontSettings : ViewModelBase
    {
        [ObservableProperty] private string _fontName = "Source Han Sans SC VF";
        [ObservableProperty] private short _fontSize = 42;
        [ObservableProperty] private string _fontWeight = nameof(Avalonia.Media.FontWeight.Medium);
        [ObservableProperty] private string _fontStyle = nameof(Avalonia.Media.FontStyle.Normal);

    }
}