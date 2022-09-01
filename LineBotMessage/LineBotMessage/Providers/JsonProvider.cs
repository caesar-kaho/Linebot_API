using System.Text.Json;
using System.Text.Json.Serialization;

namespace LineBotMessage.Providers
{
    public class JsonProvider
    {
        private JsonSerializerOptions serializeOption = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public string Serialize<T>(T str)
        {
            return JsonSerializer.Serialize(str, serializeOption);
        }

        public string SerializeObject<T>(T str)
        {
            return JsonSerializer.Serialize<Object>(str, serializeOption);
        }
    }
}

