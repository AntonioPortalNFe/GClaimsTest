using System;
using System.Collections.Generic;
using System.Text;

namespace TestAntonio.Contracts.Marvel
{
    public class ComicDataWrapper : ResponseContainer
    {       
        public ComicDataContainer data { get; set; }
    }
}
