using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using TSoftTest.Shared;

namespace TSoftTest_ClientWPF.ViewModels;

public class BoxViewModel: ObservableObject
{
    private readonly SolidColorBrush? _background;

    public BoxClass Box
    {
        set
        {
            ID = value.Id;
            X = value.X;
            Y = value.Y;
            Width = value.Width;
            Height = value.Height;

            OnPropertyChanged((string?) null);
        }
    }

    public int ID { get; private set; }

    public double X { get; private set; }

    public double Y { get; private set; }

    public double Width { get; private set; }

    public double Height { get; private set; }

    public SolidColorBrush? Background
    {
        get => _background;
        private init => SetProperty(ref _background, value);
    }

    public BoxViewModel(BoxClass box)
    {
        Box = box;
        Background = new SolidColorBrush(Color.FromArgb((byte) ((box.Color >> 24) & 0xff), (byte) ((box.Color >> 16) & 0xff), (byte) ((box.Color >> 8) & 0xff), (byte) (box.Color & 0xff)));
        Background.Freeze();
    }
}