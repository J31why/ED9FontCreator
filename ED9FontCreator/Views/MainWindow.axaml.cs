using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ED9FontCreator.ViewModels;
using System.Linq;

namespace ED9FontCreator.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            fntTextBox.AddHandler(DragDrop.DropEvent, FntFileDrop);
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            if (Design.IsDesignMode) return;
            base.OnLoaded(e);
            var vm = DataContext as MainWindowViewModel;
            vm!.DrawCanvas = drawCanvas;
        }

        private void FntFileDrop(object? sender, DragEventArgs e)
        {
            var file = e.Data.GetFiles()?.FirstOrDefault();
            if (file != null)
                fntTextBox.SetValue(TextBox.TextProperty, file.Path.LocalPath);
        }
    }
}