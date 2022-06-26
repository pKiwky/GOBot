using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Timers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using QueryMaster;
using QueryMaster.GameServer;

namespace GOBot {

    public class Bot {
        public const int MaxFails = 3;

        private DiscordClient _client;
        private CServer _server;

        public Bot(CServer server) {
            _server = server;

            var cfg = new DiscordConfiguration {
                Token = _server.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            _client = new DiscordClient(cfg);

            _client.Ready += OnReady;
        }

        public async Task Run() {
            _client.Logger.LogInformation("Start bot instance for {0}:{1}", _server.Ip, _server.Port);

            await _client.ConnectAsync();
            await Task.Delay(-1);
        }

        private async Task OnReady(DiscordClient client, ReadyEventArgs args) {
            uint fails = 0;

            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += async (sender, eventArgs) => {
                try {
                    using Server gameServer = ServerQuery.GetServerInstance((Game)EngineType.Source, _server.Ip, (ushort)_server.Port, false, 250, 250, 1, true);

                    if (gameServer != null) {
                        ServerInfo info = gameServer.GetInfo();

                        await _client.UpdateStatusAsync(new DiscordActivity(
                            FormatServerMessage(_server.Message, info)
                        ));

                        fails = 0;
                    }
                }
                catch (Exception e) {
                    if (++fails > 3) {
                        await _client.UpdateStatusAsync(new DiscordActivity("Server offline"));
                    }

                    _client.Logger.LogError("Steam query error {0}", e.Message);
                }
            };
            timer.Start();
        }

        private string FormatServerMessage(string message, ServerInfo info) {
            foreach (var prop in info.GetType().GetProperties()) {
                string name = prop.Name;
                object? value = prop.GetValue(info, null);

                message = Regex.Replace(message, $"{{{name}}}", value?.ToString() ?? "");
            }

            return message;
        }
    }

}