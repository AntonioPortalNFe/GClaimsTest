using System;
using System.Collections.Generic;
using System.Text;

namespace TestAntonio.Contracts.Marvel
{
    public class CharacterDataWrapper : ResponseContainer
    {       
        public CharacterDataContainer data { get; set; }
    }
}
