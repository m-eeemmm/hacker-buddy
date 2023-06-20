using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hacker_buddy_console.Pipe;

namespace hacker_buddy_app.Pipe
{
    internal class PiperPiped
    {
        private const string PIPE_NAME = "vibes-pipeline";

        private static readonly Lazy<PiperPiped> _instance = new Lazy<PiperPiped>(() => new PiperPiped());

        private NamedPipeClientStream _pipeClient;

        public event EventHandler<VibePackage> VibeRefreshed;
        
        public static PiperPiped Instance => _instance.Value;

        private PiperPiped()
        {
            InitNamedPipe();
        }

        private void InitNamedPipe()
        {
            _pipeClient = new NamedPipeClientStream(PIPE_NAME);
            _pipeClient.Connect();

            ListenForMessageAsync();
        }

        private async Task ListenForMessageAsync()
        {
            do
            {
                byte[] buffer = new byte[1024 * 1024];
                await _pipeClient.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer);
                VibePackage? package = System.Text.Json.JsonSerializer.Deserialize<VibePackage>(message.Replace("\0", ""));

                if (package != null)
                {
                    VibeRefreshed?.Invoke(this, package);
                }

            } while (true);
        }

        public void SpreadVibe(string vibe)
        {
            string messageJson = System.Text.Json.JsonSerializer.Serialize(new VibePackage() { Vibe = vibe });
            byte[] message = Encoding.UTF8.GetBytes(messageJson);
            _pipeClient.Write(message, 0, message.Length);
        }
    }
}
