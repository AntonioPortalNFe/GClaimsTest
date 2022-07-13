using System;
using System.Collections.Generic;
using System.Text;

namespace TestAntonio.Contracts.Marvel
{
    public class ResponseContainer
    {
        public int code { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public string copyright { get; set; }
        public string attributionText { get; set; }
        public string attributionHTML { get; set; }
        public string etag { get; set; }
    }
}
