using System.IO.Pipes;
using System.Text;

namespace hacker_buddy_console.Pipe
{
    internal class PipedPiper
    {
        private const string PIPE_NAME = "vibes-pipeline";

        private static readonly Lazy<PipedPiper> _instance = new Lazy<PipedPiper>(() => new PipedPiper());

        private NamedPipeServerStream _pipeServer;

        public static PipedPiper Instance => _instance.Value;

        private PipedPiper()
        {
            InitNamedPipeAsync();
        }

        private async Task InitNamedPipeAsync()
        {
            _pipeServer = new NamedPipeServerStream(PIPE_NAME, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

            // Wait for a client to connect
            await _pipeServer.WaitForConnectionAsync();
        }

        public void SpreadVibe(string vibe)
        {
            string messageJson = System.Text.Json.JsonSerializer.Serialize(new VibePackage() { Vibe = vibe });
            byte[] message = Encoding.UTF8.GetBytes(messageJson);
            if (_pipeServer.IsConnected)
            {
                _pipeServer.Write(message, 0, message.Length);
            }
            else
            {
                Console.WriteLine("No one clippy connected yet :(");
            }
        }
    }
}
