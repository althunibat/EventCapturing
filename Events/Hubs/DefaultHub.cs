using System;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Events.Hubs
{
    public class DefaultHub:Hub
    {
        private readonly IEventStoreConnection _connection;

        public DefaultHub(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        public async Task SendMessage(Message message)
        {
            await _connection.AppendToStreamAsync("web-stream", ExpectedVersion.Any,
                new EventData(NewId.NextGuid(), "web-event", true,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)),
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