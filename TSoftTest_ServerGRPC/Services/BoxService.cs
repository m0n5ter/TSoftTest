using Grpc.Core;
using TSoftTest.Shared;

namespace TSoftTest_ServerGRPC.Services;

public class BoxService : TSoftTest.Shared.BoxService.BoxServiceBase
{
    private readonly BoxRepository _boxRepository;
    private readonly ILogger<BoxService> _logger;

    public BoxService(ILogger<BoxService> logger, BoxRepository boxRepository)
    {
        _logger = logger;
        _boxRepository = boxRepository;
    }

    public override async Task BoxStream(EmptyRequest request, IServerStreamWriter<BoxReply> responseStream, ServerCallContext context)
    {
        while (true)
        {
            foreach (var box in _boxRepository.Boxes)
            {
                try
                {
                    if (context.CancellationToken.IsCancellationRequested) return;
                    await responseStream.WriteAsync(new BoxReply {Box = box.Clone()});
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "BoxStream");
                }
            }
        }
    }
}