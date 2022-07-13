using System;
using System.Collections.Generic;
using System.Text;

namespace TestAntonio.Commom.Interfaces
{
    public  interface IMarvelSettings
    {
        /// <summary>
        /// The PublicKey for Authentication.
        /// </summary>
        string PublicKey { get; set; }

        /// <summary>
        /// The PrivateKey for Authentication.
        /// </summary>
        string PrivateKey { get; set; }

        /// <summary>
        /// /// Endpoints to Marvel API.
        /// </summary>
        string Endpoint { get; set; }
    }
}
