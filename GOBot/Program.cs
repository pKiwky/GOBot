using GOBot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program {
    private static List<CServer> _servers = new();

    public static void Main(string[] args) {
        JObject serversJson = JObject.Parse(File.ReadAllText("server.json"));
        _servers = JsonConvert.DeserializeObject<List<CServer>>(serversJson["servers"].ToString());

        foreach (var server in _servers) {
            CServer temp = server;

            Task.Factory.StartNew(async () => {
                var bot = new Bot(temp);
                await bot.Run();
            });
        }

        while (true) {
            Console.ReadKey();
        }
    }
}