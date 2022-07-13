using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAntonio.Contracts.Marvel;
using TestAntonio.Infrastructure.Interfaces;
using TestAntonio.Services.Interfaces;

namespace TestAntonio.Services.Impl
{
    public class MarvelServices : IMarvelServices
    {        
        private readonly ICharacterRepository _characterRepository;
        private readonly HttpContext _context;
        
        public MarvelServices(ICharacterRepository characterRepository, IHttpContextAccessor context)
        {            
            _characterRepository = characterRepository;
            _context = context.HttpContext;
        }

        public async Task<CharacterDataWrapper> GetCharacters(string orderBy, string name, string nameStartsWith, int pageNumber, int limit)
        {
            int offSet = (pageNumber - 1) * limit;
            string parameters = $"limit={limit}&offset={offSet}&orderBy={orderBy}";
            
            if (!string.IsNullOrEmpty(name))
                parameters += $"&name={name}";

            if (!string.IsNullOrEmpty(nameStartsWith))
                parameters += $"&nameStartsWith={nameStartsWith}";            

            var response = await _characterRepository.Get(parameters);
            
            return OrganizeCharacters(response);
        }      

        public CharacterDataWrapper OrganizeCharacters(CharacterDataWrapper characterDataWrapper)
        {
            
            string favorites = _context.Request.Cookies["favorite"] ?? "";
            string deleteds = _context.Request.Cookies["deleted"] ?? "";

            if (!string.IsNullOrEmpty(favorites))
            {
                characterDataWrapper.data?.results?.Where(w => favorites.Contains(w.id.ToString())).ToList()
                                                    .ForEach(w => w.favorite = true);

                characterDataWrapper.data?.results?.Where(w => deleteds.Contains(w.id.ToString())).ToList()
                                                    .ForEach(w => w.deleted = true);

                var  results = characterDataWrapper.data?.results.Where(w => w.deleted == false)
                                                    .OrderByDescending(o => o.favorite);

                if(characterDataWrapper.data != null)
                    characterDataWrapper.data.results = results.ToList();
            }

            return characterDataWrapper;
        }

        public void FavoriteCharacters(int id)
        {            

            var listFavorites = new List<string>();

            string favorites = _context.Request.Cookies["favorite"] ?? "";

            listFavorites = favorites.Split(',').ToList();

            if (listFavorites.Count >= 5)
                listFavorites.Remove(listFavorites.First());

            listFavorites.Add(id.ToString());

            favorites = string.Empty;

            foreach (var item in listFavorites.Where(x => !string.IsNullOrEmpty(x)))
                favorites += ',' + item;

            _context.Response.Cookies.Append("favorite", favorites.Substring(1));
        }

        public void NotFavoriteCharacters(int id)
        {
                        
            var listFavorites = new List<string>();

            string favorites = _context.Request.Cookies["favorite"] ?? "";

            listFavorites = favorites.Split(',').ToList();            
            
            listFavorites.Remove(id.ToString());

            favorites = string.Empty;

            foreach (var item in listFavorites.Where(x => !string.IsNullOrEmpty(x)))
                favorites += ',' + item;

            _context.Response.Cookies.Append("favorite", favorites.Substring(1));
        }

        public void DeleteCharacters(int id)
        {

            var listDeleted = new List<string>();

            string deleteds = _context.Request.Cookies["deleted"] ?? "";

            listDeleted = deleteds.Split(',').ToList();

            listDeleted.Add(id.ToString());

            deleteds = string.Empty;

            foreach (var item in listDeleted.Where(x => !string.IsNullOrEmpty(x)))
                deleteds += ',' + item;

            _context.Response.Cookies.Append("deleted", deleteds.Substring(1));
        }
    }
}
