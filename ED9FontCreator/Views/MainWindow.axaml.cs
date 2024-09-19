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
            var vm = (this.DataContext as MainWindowViewModel)!;
            vm.CharBorder = this.charBorder;
            vm.PreviewBorder = this.previewBorder;

        }
        private void FntFileDrop(object? sender, DragEventArgs e)
        {
            var file = e.Data.GetFiles()?.FirstOrDefault();
            if (file != null)
                fntTextBox.SetValue(TextBox.TextProperty, file.Path.LocalPath);
        }
    }
}