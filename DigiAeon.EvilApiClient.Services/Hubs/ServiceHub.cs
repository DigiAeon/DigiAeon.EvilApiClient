using Microsoft.AspNet.SignalR;

namespace DigiAeon.EvilApiClient.Services.Hubs
{
    public class ServiceHub : Hub
    {
        public void EstablishConnection()
        {
            // Reason: It's experienced that calling websocket method establish a connection.
            // TODO: Find a better way for establishing a connection or why broadcast receiving method not working
        }
    }
}