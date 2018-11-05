using System;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Utf8Json;

namespace Events.Hubs
{
    public class DefaultHub:Hub
    {
        private readonly IEventStoreConnection _connection;

        public DefaultHub(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        public async Task SendEvent(Event evt)
        {
           // evt.ServerTimeStamp = DateTime.Now;
            await _connection.AppendToStreamAsync("game-stream", ExpectedVersion.Any,
                new EventData(NewId.NextGuid(), "tic-tac-toe-move", true,
                    JsonSerializer.Serialize(evt),
                    Encoding.UTF8.GetBytes("signalr-generated")));
        }

        public async Task SendMessage(Message msg)
        {
            msg.ServerTimeStamp = DateTime.Now;
            await _connection.AppendToStreamAsync("web-stream", ExpectedVersion.Any,
                new EventData(NewId.NextGuid(), msg.Event, true,
                    JsonSerializer.Serialize(msg),
                    Encoding.UTF8.GetBytes("signalr-generated")));
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