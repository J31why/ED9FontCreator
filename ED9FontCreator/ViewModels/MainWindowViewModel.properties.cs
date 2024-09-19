using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using ED9FontCreator.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ED9FontCreator.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly string OutDir;
        //GAME FNT
        [ObservableProperty][NotifyPropertyChangedFor(nameof(Analysed))] private Fnt? _fnt;
        [ObservableProperty] private string _fntPath = "";
        [ObservableProperty] private FntChar? _searchedFntChar;
        public bool Analysed => this.Fnt != null;
        //font settings
        [ObservableProperty] private FontSettings _fontSettings = new();
        public string[] FontWeights => Enum.GetNames(typeof(FontWeight));
        public string[] FontStyles => Enum.GetNames(typeof(FontStyle));
        //FNT settings
        [ObservableProperty] private FntSettings _fntSettings = new();

        //char settings
        [ObservableProperty] private bool _isSimplifiedChinese = true;

        [ObservableProperty] private string _replaceText = "";
        private string ReplaceTxtFile => System.IO.Path.Combine(Environment.CurrentDirectory, "replace.txt");
        //display canvas
        [ObservableProperty] private ObservableCollection<FntChar> _tempFntData = [];
        [ObservableProperty] private List<FntChar>? _drawChars;
        [ObservableProperty] private List<FntChar>? _previewChars;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(DrawCharForeground))] private bool _isRedChars;
        public IBrush DrawCharForeground => IsRedChars ? Brushes.Red : Brushes.Lime;
        public bool CanExportTempFnt => Fnt?.TotalChars == TempFntData.Count;

        public CharCanvas DrawCanvas;
    }
}