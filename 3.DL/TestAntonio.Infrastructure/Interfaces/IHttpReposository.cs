using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestAntonio.Contracts.Marvel;

namespace TestAntonio.Infrastructure.Interfaces
{
    public interface IHttpRepository
    {
        Task<T> PostAsync<T>(string action, object request) where T : ResponseContainer, new();
        Task<T> GetAsync<T>(string action, string parameters = "") where T : ResponseContainer, new();

    }
}
