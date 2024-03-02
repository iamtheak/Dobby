using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dobby.Utils
{
    public static class JsonReader
    {
        public static async Task<JsonObject> GetJsonFromFilePath(string path){
            string input = await System.IO.File.ReadAllTextAsync(path);
            JsonObject obj = JsonConvert.DeserializeObject<JsonObject>(input) ?? new JsonObject();
            return obj;
        }

    }
}
