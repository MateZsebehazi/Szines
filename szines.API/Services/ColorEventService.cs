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
            // Take a snapshot of client keys to avoid concurrent modification issues
            var clientKeys = _clients.Keys.ToArray();
            var deadClients = new List<string>();

            foreach (var clientId in clientKeys)
            {
                // Use TryGetValue for thread-safe access in case client was removed
                if (_clients.TryGetValue(clientId, out var writer))
                {
                    try
                    {
                        await writer.WriteAsync($"data: refresh\n\n");
                        await writer.FlushAsync();
                    }
                    catch
                    {
                        deadClients.Add(clientId);
                    }
                }
            }

            foreach (var clientId in deadClients)
            {
                RemoveClient(clientId);
            }
        }
    }
}
