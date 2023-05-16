using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Grpc.Net.Client;
using TSoftTest.Shared;

namespace TSoftTest_Client.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    private readonly Dictionary<int, BoxViewModel> _boxesById = new();
    private Exception? _connectionError;
    private readonly Dispatcher _dispatcher = Dispatcher.UIThread;

    public AvaloniaList<BoxViewModel> Boxes { get; } = new();
    
    public MainWindowViewModel()
    {
        new Thread(ReadBoxStream) {IsBackground = true}.Start();
    }

    private async void ReadBoxStream()
    {
        while (true)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:7133");
                var client = new BoxService.BoxServiceClient(channel);
                var call = client.BoxStream(new EmptyRequest());
                ConnectionError = null;

                while (await call.ResponseStream.MoveNext(CancellationToken.None))
                {
                    var box = call.ResponseStream.Current.Box;
                    BoxViewModel? boxViewModel;
                    var added = false;

                    lock (_boxesById)
                    {
                        if (!_boxesById.TryGetValue(box.Id, out boxViewModel))
                        {
                            _boxesById.Add(box.Id, boxViewModel = new BoxViewModel(box));
                            added = true;
                        }
                    }

                    _dispatcher.Invoke(() =>
                    {
                        if (added)
                            Boxes.Add(boxViewModel);
                        else
                            boxViewModel.Box = box;
                    }, DispatcherPriority.Input);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                ConnectionError = exception;
                await Task.Delay(5000);
            }
        }
    }

    public Exception? ConnectionError
    {
        get => _connectionError;
        set => SetProperty(ref _connectionError, value);
    }
}