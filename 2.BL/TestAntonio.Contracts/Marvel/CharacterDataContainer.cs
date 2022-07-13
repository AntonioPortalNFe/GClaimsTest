using System;
using System.Collections.Generic;
using System.Text;

namespace TestAntonio.Contracts.Marvel
{
    public class CharacterDataContainer
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
        public int count { get; set; }
        public List<Character> results { get; set; }
    }
}
