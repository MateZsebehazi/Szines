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
            if (_clients.TryRemove(clientId, out var writer))
            {
                try
                {
                    writer?.Dispose();
                }
                catch (Exception)
                {
                    // Suppress exceptions during disposal to ensure client removal completes
                    // even if the writer is already disposed or in an invalid state
                }
            }
        }

        public async Task NotifyClientsAsync()
        {
            var deadClients = new List<string>();

            foreach (var client in _clients)
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
