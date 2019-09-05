using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Json;
using Newtonsoft.Json;

namespace WindowsExplorer
{
    [JsonObject]
    public class Files
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        public Files(string name, string size, string path)
        {
            Name = name;
            Size = size;
            Path = path;
        }

    }
}
