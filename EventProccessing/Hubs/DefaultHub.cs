using EventStore.ClientAPI;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventProccessing.Hubs
{
    public class DefaultHub : Hub
    {
        private readonly IEventStoreConnection _connection;

        public DefaultHub(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        public async Task SendEvent(Event evt)
        {
            var context = Context.GetHttpContext();
            var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();
            var remoteIp = context.Connection.RemoteIpAddress.MapToIPv4().ToString();

            await _connection.AppendToStreamAsync("game-stream", ExpectedVersion.Any,
                new EventData(Guid.NewGuid(), "move", true,
                    Utf8Json.JsonSerializer.Serialize(evt),
                    Utf8Json.JsonSerializer.Serialize(new { UserAgent = userAgent, RemoteIp = remoteIp })));
        }

        public async Task SendMessage(Message msg)
        {
            var context = Context.GetHttpContext();
            var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();
            var remoteIp = context.Connection.RemoteIpAddress.MapToIPv4().ToString();

            await _connection.AppendToStreamAsync("web-stream", ExpectedVersion.Any,
                new EventData(Guid.NewGuid(), msg.Event, true,
                    Utf8Json.JsonSerializer.Serialize(msg),
                    Utf8Json.JsonSerializer.Serialize(new { UserAgent = userAgent, RemoteIp = remoteIp })));
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }

    }
}
