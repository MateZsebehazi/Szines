using System.Collections.Concurrent;

namespace szines.API.Services
{
    public class ColorEventService
    {
        private readonly ConcurrentDictionary<string, StreamWriter> _clients = new();

        public string AddClient(StreamWriter writer)
        {
            var clientId = Guid.NewGuid().ToString();
            _clients.TryAdd(clientId, writer);
            return clientId;
        }

        public void RemoveClient(string clientId)
        {
            _clients.TryRemove(clientId, out _);
        }

        public async Task NotifyClientsAsync()
        {
            // Take a snapshot of clients to avoid concurrent modification issues
            var clientSnapshot = _clients.ToArray();
            var deadClients = new List<string>();

            foreach (var client in clientSnapshot)
            {
                try
                {
                    await client.Value.WriteAsync($"data: refresh\n\n");
                    await client.Value.FlushAsync();
                }
                catch
                {
                    deadClients.Add(client.Key);
                }
            }

            foreach (var clientId in deadClients)
            {
                RemoveClient(clientId);
            }
        }
    }
}
