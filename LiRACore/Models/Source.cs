using LiRACore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models
{
    public class Source
    {

        [JsonProperty("vid")]
        public int @vid { get; set; }


        public string event.system.time.old { get; set; }
        public string @tag { get; set; }
        public string @t { get; set; }
        public string event.system.time.source { get; set; }
        public DateTime @ts { get; set; }
        public string @uid { get; set; }
        public DateTime @rec { get; set; }
        public string event.system.time.new { get; set; }
        public string event.system.power.trigger { get; set; }
    }

    public class Hit
    {
        public Source _source { get; set; }
    }

    public class Hits
    {
        public List<Hit> hits { get; set; }
    }

    public class RootObject
    {
        public Hits hits { get; set; }
    }



}
