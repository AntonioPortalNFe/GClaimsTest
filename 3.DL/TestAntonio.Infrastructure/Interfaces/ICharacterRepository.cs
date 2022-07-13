using System.Threading.Tasks;
using TestAntonio.Contracts.Marvel;

namespace TestAntonio.Infrastructure.Interfaces
{
    public interface ICharacterRepository
    {
        Task<CharacterDataWrapper> Get(string parameters);
        Task<CharacterDataWrapper> GetByID(string characterId);        
    }
}
