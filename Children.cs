using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Json;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace WindowsExplorer
{
    [JsonObject]
    public class Children
    {
        [JsonProperty ("name")]
        public string Name { get; set; }

        //[JsonProperty("dateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty("Files", NullValueHandling = NullValueHandling.Ignore)]
        public List<Files> Files { get; set; }

        [JsonProperty("Childrens", NullValueHandling = NullValueHandling.Ignore)]
        public List<Children> Childrens { get; set; }

        public Children(string name)
        {
            Name = name;            
        }

        public Children(string name, string datecreated)
        {
            Name = name;
            DateCreated = datecreated;
        }
        public Children(string name, string datecreated, List<Files> files)
        {
           Name = name;
           DateCreated = datecreated;
           if (files != null)
            {
                Files = files;
            }
                
        }

        public Children(string name, string datecreated, List<Files> files, List<Children> children)
        {
            Name = name;
            DateCreated = datecreated;
            if (files != null)
            {
                Files = files;
            }

            if (children != null)
            {
                Childrens = children;
            }

        }
        //public Children(string name, DateTime datecreated, List<Files> files = null, Files files1 = null) : this(name, datecreated, files)
        //{
        //    this.files1 = files1;
        //}
    }
}
