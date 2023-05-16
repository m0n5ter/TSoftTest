using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using TSoftTest.Shared;

namespace TSoftTest_Client.ViewModels;

public class BoxViewModel: ObservableObject
{
    private readonly Color? _background;

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

    public Color? Background
    {
        get => _background;
        private init => SetProperty(ref _background, value);
    }

    public BoxViewModel(BoxClass box)
    {
        Box = box;
        Background = Color.FromUInt32(box.Color);
    }
}