using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpeechVS
{
    public class SpeechListener
    {
        public event Action<string> OnSpeechRecognized;

        private ClientWebSocket ws;
        private CancellationTokenSource cts;

        public void StartListening()
        {
            Task.Run(async () =>
            {
                ws = new ClientWebSocket();
                cts = new CancellationTokenSource();

                try
                {
                    await ws.ConnectAsync(new Uri("ws://localhost:8765"), cts.Token);
                    SpeechVS.Logger.LogInfo("Connected to WebSocket");

                    var buffer = new byte[1024];

                    while (ws.State == WebSocketState.Open)
                    {
                        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
                            break;
                        }

                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        SpeechVS.Logger.LogInfo("Got: " + message);

                        string text = ParseRecognizedText(message);
                        SpeechVS.Logger.LogInfo("Parsed: " + text);
                        SpeechVS.RunOnMainThread(() =>
                        {
                            OnSpeechRecognized?.Invoke(text);
                        });
                    }
                }
                catch (Exception ex)
                {
                    SpeechVS.Logger.LogError("WebSocket error: " + ex.Message + ex.StackTrace);
                }
            });
        }

        private string ParseRecognizedText(string json)
        {
            if(json != null) { 
                var match = System.Text.RegularExpressions.Regex.Match(json, @"""text""\s*:\s*""([^""]*)""");
                return match.Success ? match.Groups[1].Value : "";
            }
            return "";
        }
    }
}
