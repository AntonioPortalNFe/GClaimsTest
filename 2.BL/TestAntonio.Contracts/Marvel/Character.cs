using System;
using System.Collections.Generic;
using System.Text;

namespace TestAntonio.Contracts.Marvel
{
    public class Character
    {
        public bool favorite { get; set; }
        public bool deleted { get; set; }
        public int id { get; set; }        
        public string name { get; set; }                
        public string description { get; set; }
        public string modified { get; set; }
        public string resourceURI { get; set; }                
        public List<Url> urls { get; set; }
        public Image thumbnail { get; set; }        
        public ComicList comics { get; set; }
        public StoryList stories { get; set; }
        public EventList events { get; set; }
        public SeriesSummary series { get; set; }

    }
}
