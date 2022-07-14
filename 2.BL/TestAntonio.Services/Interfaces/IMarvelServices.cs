using System.Threading.Tasks;
using TestAntonio.Contracts.Marvel;

namespace TestAntonio.Services.Interfaces
{
    public  interface IMarvelServices
    {
        Task<CharacterDataWrapper> GetCharacters(string orderBy, string name, string nameStartsWith, int pageNumber, int limit);
        ResponseContainer FavoriteCharacters(int id);
        ResponseContainer NotFavoriteCharacters(int id);
        ResponseContainer DeleteCharacters(int id);

    }
}
