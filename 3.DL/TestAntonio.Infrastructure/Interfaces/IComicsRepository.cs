using System.Threading.Tasks;
using TestAntonio.Contracts.Marvel;

namespace TestAntonio.Infrastructure.Interfaces
{
    public interface IComicsRepository
    {
        Task<ComicDataWrapper> Get();
        Task<ComicDataWrapper> GetByID(string comicId);        
    }
}
