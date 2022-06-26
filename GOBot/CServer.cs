using Newtonsoft.Json;

namespace GOBot {

    public class CServer {
        [JsonProperty("ip")]
        public string Ip;

        [JsonProperty("port")]
        public short Port;

        [JsonProperty("message")]
        public string Message;
        
        [JsonProperty("token")]
        public string Token;
    }

}