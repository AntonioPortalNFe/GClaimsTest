using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAntonio.Contracts.Marvel;
using TestAntonio.Services.Interfaces;

namespace TestAntoino.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MarvelController : ControllerBase
    {
        
        private readonly ILogger<MarvelController> _logger;
        private readonly IMarvelServices _marvelServices;

        public MarvelController(ILogger<MarvelController> logger, IMarvelServices marvelServices)
        {
            _logger = logger;
            _marvelServices = marvelServices;
        }
        
        [HttpGet("GetCharacters")]
        public async Task<CharacterDataWrapper> GetCharacters(string orderBy = "name",
                                                    string nameFilter = "",
                                                    string nameStartsWith = "",
                                                    int? pageNumber = 1,
                                                    int? limit = 15)
        {
            
            return  await _marvelServices.GetCharacters(orderBy, nameFilter, nameStartsWith, (int)pageNumber, (int)limit);
        }

        [HttpGet("FavoriteCharacters")]
        public async Task<ResponseContainer> FavoriteCharacters(int id)
        {            
              return await _marvelServices.FavoriteCharacters(id);            
        }

        [HttpGet("NotFavoriteCharacters")]
        public ResponseContainer NotFavoriteCharacters(int id)
        {
            return _marvelServices.NotFavoriteCharacters(id);
        }

        [HttpGet("ListFavoritesCharacters")]
        public CharacterDataWrapper ListFavoritesCharacters()
        {
            return _marvelServices.ListFavoritesCharacters();
        }

        [HttpGet("DeleteCharacters")]
        public ResponseContainer DeleteCharacters(int id)
        {            
            return _marvelServices.DeleteCharacters(id);
        }
    }
}
