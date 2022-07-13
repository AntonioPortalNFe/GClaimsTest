using System;
using System.Collections.Generic;
using System.Text;
using TestAntonio.Commom.Interfaces;

namespace TestAntonio.Commom.Impl
{
    public  class MarvelSettings : IMarvelSettings
    {
        /// <summary>
        /// The PublicKey for Authentication.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// The PrivateKey for Authentication.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// /// Endpoints to Marvel API.
        /// </summary>
        public string Endpoint { get; set; }
    }
}
