using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using ED9FontCreator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ED9FontCreator.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        //GAME FNT

        [ObservableProperty][NotifyPropertyChangedFor(nameof(Analysed))] private Fnt? _fnt;
        [ObservableProperty] private string _fntPath = "";
        [ObservableProperty] private FntChar? _searchedFntChar;
        public bool Analysed => this.Fnt != null;
        //font settings

        [ObservableProperty] private string _fontName = "SimSun";
        [ObservableProperty] private short _fontSize = 42;
        [ObservableProperty] private string _fontWeight = nameof(Avalonia.Media.FontWeight.Normal);
        [ObservableProperty] private string _fontStyle = nameof(Avalonia.Media.FontStyle.Normal);
        [ObservableProperty] private short _fontClipL = 0;
        [ObservableProperty] private short _fontClipT = 0;
        [ObservableProperty] private short _fontClipR = 0;
        [ObservableProperty] private short _fontClipB = 0;
        public string[] FontWeights => Enum.GetNames(typeof(FontWeight));
        public string[] FontStyles => Enum.GetNames(typeof(FontStyle));
        //FNT settings

        [ObservableProperty] private short _fntXOffset = 0;
        [ObservableProperty] private short _fntYOffset = 0xC;
        [ObservableProperty] private short _fntNextCharExOffset = 2;

        //char settings
        [ObservableProperty] private bool _isSimplifiedChinese = true;

        [ObservableProperty] private string _replaceText = "";
        private readonly List<ReplaceItem> ReplaceGroup = [];
        private string ReplaceTxtFile => System.IO.Path.Combine(Environment.CurrentDirectory, "replace.txt");
        //display canvas

        [ObservableProperty] private ObservableCollection<FntChar> _tempFntData = [];
        public bool CanExportTempFnt => Fnt?.TotalChars == TempFntData.Count;
        private string OutDir;
        private bool IsRedChars;
        public Border CharBorder = new();
        public Border PreviewBorder = new();
    }
}