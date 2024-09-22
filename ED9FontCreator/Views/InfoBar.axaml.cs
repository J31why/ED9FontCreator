using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Threading;
using System.Threading;

namespace ED9FontCreator.Views;

public enum InfoBarState
{
    None,
    Success,
    Error,
    Alert
}

public class InfoBar : TemplatedControl
{
    private Timer? _timer;

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<InfoBar, string>(nameof(Text), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<InfoBarState> StateProperty =
        AvaloniaProperty.Register<InfoBar, InfoBarState>(nameof(State), defaultBindingMode: BindingMode.TwoWay);

    public InfoBarState State
    {
        get => this.GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public string Text
    {
        get => this.GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Close();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty)
            TextPropertyChanged();
    }

    private void TextPropertyChanged()
    {
        if (Text == "")
        {
            IsVisible = false;
            return;
        }
        IsVisible = true;
        _timer?.Dispose();
        _timer = new Timer(CloseTask, null, 5000, Timeout.Infinite);
    }

    private void Close()
    {
        SetValue(TextProperty, "");
    }
    private void CloseTask(object? state)
    {
        Dispatcher.UIThread.Invoke(Close);
    }
}