using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAntonio.Contracts.Marvel;
using TestAntonio.Infrastructure.Interfaces;
using TestAntonio.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace TestAntonio.Services.Impl
{
    public class MarvelServices : IMarvelServices
    {        
        private readonly ICharacterRepository _characterRepository;        
        private readonly ILogger<MarvelServices> _logger;
        private IMemoryCache _cache;

        public MarvelServices(ILogger<MarvelServices> logger, ICharacterRepository characterRepository, IMemoryCache memoryCache)
        {            
            _characterRepository = characterRepository;            
            _logger = logger;
            _cache = memoryCache;
        }

        public async Task<CharacterDataWrapper> GetCharacters(string orderBy, string name, string nameStartsWith, int pageNumber, int limit)
        {
            var response = new CharacterDataWrapper();
            string parameters = string.Empty;
            var watch = new Stopwatch();
            watch.Start();

            _logger.Log(LogLevel.Information, "Started GetCharacters");

            try
            {                

                int offSet = (pageNumber - 1) * limit;
                parameters = $"limit={limit}&offset={offSet}&orderBy={orderBy}";

                if (!string.IsNullOrEmpty(name))
                    parameters += $"&name={name}";

                if (!string.IsNullOrEmpty(nameStartsWith))
                    parameters += $"&nameStartsWith={nameStartsWith}";

                response = await _characterRepository.Get(parameters);

                watch.Stop();
                
            }
            catch (System.Exception ex)
            {

                response.code = (int)HttpStatusCode.InternalServerError;
                response.message = ex.Message;

                _logger.LogCritical($"Error code: {response.code} - message: {response.message}");

            }
            finally
            {
                _logger.Log(LogLevel.Information, "Finished GetCharacters \tTime: {1}ms \tparameters url: {2}", watch.ElapsedMilliseconds, parameters);
            }

            return OrganizeCharacters(response);

        }      

        private CharacterDataWrapper OrganizeCharacters(CharacterDataWrapper characterDataWrapper)
        {

            List<Character> listFavorites;
            List<int> listDeleteds;

            if(!_cache.TryGetValue("favorite", out listFavorites))
                listFavorites = new List<Character>();

            if(!_cache.TryGetValue("deleted", out listDeleteds))
                listDeleteds = new List<int>();

            if (listFavorites.Any())
            {
                List<int> idsFavorites = listFavorites.Select(s => s.id).ToList();

                characterDataWrapper.data?.results?.Where(x => idsFavorites.Contains(x.id)).ToList()
                                                    .ForEach(w => w.favorite = true);             
            }

            if (listDeleteds.Any())
            {
                List<int> idsDeleteds = listFavorites.Select(s => s.id).ToList();

                characterDataWrapper.data?.results?.Where(w => idsDeleteds.Contains(w.id)).ToList()
                                                 .ForEach(w => w.deleted = true);
            }                     

            return characterDataWrapper;
        }

        public CharacterDataWrapper ListFavoritesCharacters()
        {
            List<Character> listFavorites;
            var response = new CharacterDataWrapper() { 
                            data = new CharacterDataContainer() { 
                                results = new List<Character>()
                            }  
            };
            var watch = new Stopwatch();
            watch.Start();

            _logger.Log(LogLevel.Information, "Started ListFavoritesCharacters");

            try
            {
                if (!_cache.TryGetValue("favorite", out listFavorites))
                    listFavorites = new List<Character>();

                if (listFavorites.Any())
                    response.data.results = listFavorites;

                response.code = (int)HttpStatusCode.OK;
                response.status = HttpStatusCode.OK.ToString();

            }
            catch (System.Exception ex)
            {

                _logger.LogError($"Error\tMessage: {ex.Message} \tListFavoritesCharacters");

                response.code = (int)HttpStatusCode.InternalServerError;
                response.status = HttpStatusCode.InternalServerError.ToString();
                response.message = ex.Message;
            }
            finally
            {
                _logger.Log(LogLevel.Information, "Finished ListFavoritesCharacters \tTime: {1}ms", watch.ElapsedMilliseconds);
                watch.Stop();
            }

            return OrganizeCharacters(response);
        }

        public async Task<ResponseContainer> FavoriteCharacters(int id)
        {
            var response = new ResponseContainer();
            List<Character> listFavorites;
            var watch = new Stopwatch();
            watch.Start();

            _logger.Log(LogLevel.Information, "Started FavoriteCharacters");

            try
            {                

                if (!_cache.TryGetValue("favorite", out listFavorites))
                    listFavorites = new List<Character>();

                if (!listFavorites.Any(a => a.id == id))
                {
                    var character = await _characterRepository.GetByID(id);

                    if (character?.data.results?.Any() == true)
                    {
                        if (listFavorites.Count >= 5)
                            listFavorites.Remove(listFavorites.First());

                        listFavorites.Add(character?.data.results.FirstOrDefault());

                        _cache.Set("favorite", listFavorites);
                    }
                }
                
                response.code = (int)HttpStatusCode.OK;
                response.status = HttpStatusCode.OK.ToString();

            }
            catch (System.Exception ex)
            {

                _logger.LogError($"Error\tMessage: {ex.Message} \tFavoriteCharactersId: {id}");

                response.code = (int)HttpStatusCode.InternalServerError;
                response.status = HttpStatusCode.InternalServerError.ToString();
                response.message = ex.Message;
            }
            finally
            {                
                _logger.Log(LogLevel.Information, "Finished FavoriteCharacters \tTime: {1}ms", watch.ElapsedMilliseconds);
                watch.Stop();
            }

            return response;
        }

        public ResponseContainer NotFavoriteCharacters(int id)
        {

            var response = new ResponseContainer();
            List<Character> listFavorites;
            var watch = new Stopwatch();
            watch.Start();

            _logger.Log(LogLevel.Information, "Started NotFavoriteCharacters");

            try
            {

                if (!_cache.TryGetValue("favorite", out listFavorites))
                    listFavorites = new List<Character>();

                if (listFavorites.Any(a => a.id == id))
                {
                    listFavorites.Remove(listFavorites.Where(w => w.id == id).First());

                    _cache.Set("favorite", listFavorites);
                }

                response.code = (int)HttpStatusCode.OK;
                response.status = HttpStatusCode.OK.ToString();

            }
            catch (System.Exception ex)
            {

                _logger.LogError($"Error\tMessage: {ex.Message} \tNotFavoriteCharactersId: {id}");
                response.code = (int)HttpStatusCode.InternalServerError;
                response.status = HttpStatusCode.InternalServerError.ToString();
                response.message = ex.Message;
            }
            finally
            {
                _logger.Log(LogLevel.Information, "Finished NotFavoriteCharacters \tTime: {1}ms", watch.ElapsedMilliseconds);
                watch.Stop();
            }

            return response;
        }

        public ResponseContainer DeleteCharacters(int id)
        {
            var response = new ResponseContainer();
            List<int> listDeleted;
            var watch = new Stopwatch();
            watch.Start();

            _logger.Log(LogLevel.Information, "Started DeleteCharacters");

            try
            {

                if (!_cache.TryGetValue("deleted", out listDeleted))
                    listDeleted = new List<int>();
                
                listDeleted.Add(id);                
                
                _cache.Set("deleted", listDeleted);

                NotFavoriteCharacters(id);

                response.code = (int)HttpStatusCode.OK;
                response.status = HttpStatusCode.OK.ToString();

            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error\tMessage: {ex.Message} \tDeleteCharactersId: {id}");

                response.code = (int)HttpStatusCode.InternalServerError;
                response.status = HttpStatusCode.InternalServerError.ToString();
                response.message = ex.Message;
            }
            finally
            {
                _logger.Log(LogLevel.Information, "Finished DeleteCharacters \tTime: {1}ms", watch.ElapsedMilliseconds);
                watch.Stop();
            }

            return response;
        }
    }
}
