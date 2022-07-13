using System.Threading.Tasks;
using TestAntonio.Contracts.Marvel;

namespace TestAntonio.Services.Interfaces
{
    public  interface IMarvelServices
    {
        Task<CharacterDataWrapper> GetCharacters(string orderBy, string name, string nameStartsWith, int pageNumber, int limit);
        void FavoriteCharacters(int id);

        void NotFavoriteCharacters(int id);

        void DeleteCharacters(int id);

    }
}
