using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dobby.Utils
{
    public class EnvironmentVariables
    {
        private string? discordToken;
        
        public async Task<string> GetDiscordToken()
        {
            EnvironmentVariables env = JsonConvert.DeserializeObject<EnvironmentVariables>(Dobby.Utils.JsonReader.GetJsonFromFilePath("/Env/token.env").ToString());
            return env.discordToken ?? throw new Exception("Discord token not found");
        }
    }
}
