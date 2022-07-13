using System;
using System.Collections.Generic;
using System.Text;

namespace TestAntonio.Contracts.Marvel
{
    public class EventList
    {
        public int available { get; set; }
        public string collectionURI { get; set; }
        public List<EventSummary> items { get; set; }
        public int returned { get; set; }
    }
}
