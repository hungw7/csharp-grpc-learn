using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcCodeLearn.Server.Services;

public class ChatService : Chat.ChatBase
{
    private readonly ILogger<ChatService> _logger;

    public ChatService(ILogger<ChatService> logger)
    {
        _logger = logger;
    }

    public override async Task SendMessage(IAsyncStreamReader<ClientToServerMessage> requestStream,
        IServerStreamWriter<ServerToClientMessage> responseStream, ServerCallContext context)
    {
        var c2sTask = ClientToServerHandleAsync(requestStream, context);
        var s2cTask = ServerToClientResponseAsync(responseStream, context);
        
        await Task.WhenAll(c2sTask, s2cTask);
    }

    private static async Task ServerToClientResponseAsync(IAsyncStreamWriter<ServerToClientMessage> responseStream, ServerCallContext context)
    {
        var pingCount = 0;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await responseStream.WriteAsync(new ServerToClientMessage
                { Message = $"Pong {pingCount}", Timestamp = Timestamp.FromDateTime(DateTime.UtcNow) });
            pingCount++;
            await Task.Delay(1000);
        }
    }

    private async Task ClientToServerHandleAsync(IAsyncStreamReader<ClientToServerMessage> requestStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
        {
            var message = requestStream.Current.Message;
            _logger.LogInformation("Client send message: {Message}", message);
        }
    }
}