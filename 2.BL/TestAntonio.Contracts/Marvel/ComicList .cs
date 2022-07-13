using System;
using System.Collections.Generic;
using System.Text;

namespace TestAntonio.Contracts.Marvel
{
    public class ComicList
    {
        public int available { get; set; }
        public string collectionURI { get; set; }
        public List<ComicSummary> items { get; set; }
        public int returned { get; set; }
    }
}
