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
            var timeStopWatch = new Stopwatch();
            timeStopWatch.Start();

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

                timeStopWatch.Stop();
                
            }
            catch (System.Exception ex)
            {

                response.code = (int)HttpStatusCode.InternalServerError;
                response.message = ex.Message;

                _logger.LogCritical($"Error code: {response.code} - message: {response.message}");

            }
            finally
            {
                _logger.Log(LogLevel.Information, "Finished GetCharacters \tTime: {1}ms \tparameters url: {2}", timeStopWatch.ElapsedMilliseconds, parameters);
            }

            return OrganizeCharacters(response);

        }      

        private CharacterDataWrapper OrganizeCharacters(CharacterDataWrapper characterDataWrapper)
        {

            string favorites = string.Empty;
            string deleteds = string.Empty;

            _cache.TryGetValue("favorite", out favorites);    
            _cache.TryGetValue("deleted", out deleteds);

            if (!string.IsNullOrEmpty(favorites))
            {
                characterDataWrapper.data?.results?.Where(w => favorites.Contains(w.id.ToString())).ToList()
                                                    .ForEach(w => w.favorite = true);             
            }

            if (!string.IsNullOrEmpty(deleteds))
            {
                characterDataWrapper.data?.results?.Where(w => deleteds.Contains(w.id.ToString())).ToList()
                                                 .ForEach(w => w.deleted = true);
            }

            var  results = characterDataWrapper.data?.results.Where(w => w.deleted == false)
                                                    .OrderByDescending(o => o.favorite);

                if(characterDataWrapper.data != null)
                    characterDataWrapper.data.results = results.ToList();
            

            return characterDataWrapper;
        }

        public ResponseContainer FavoriteCharacters(int id)
        {
            var response = new ResponseContainer();
            var timeStopWatch = new Stopwatch();
            timeStopWatch.Start();

            _logger.Log(LogLevel.Information, "Started FavoriteCharacters");

            try
            {                

                var listFavorites = new List<string>();

                string favorites = string.Empty;

                _cache.TryGetValue("favorite", out favorites);
                

                listFavorites = (favorites ?? "").Split(',').ToList();

                if (listFavorites.Count >= 5)
                    listFavorites.Remove(listFavorites.First());

                listFavorites.Add(id.ToString());

                favorites = string.Empty;

                foreach (var item in listFavorites.Where(x => !string.IsNullOrEmpty(x)))
                    favorites += ',' + item;
                
                _cache.Set("favorite", favorites.Substring(1));

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
                _logger.Log(LogLevel.Information, "Finished FavoriteCharacters \tTime: {1}ms", timeStopWatch.ElapsedMilliseconds);
            }

            return response;
        }

        public ResponseContainer NotFavoriteCharacters(int id)
        {

            var response = new ResponseContainer();
            var timeStopWatch = new Stopwatch();
            timeStopWatch.Start();

            _logger.Log(LogLevel.Information, "Started NotFavoriteCharacters");

            try
            {

                var listFavorites = new List<string>();

                string favorites = string.Empty;
                _cache.TryGetValue("favorite", out favorites); 

                listFavorites = (favorites ?? "").Split(',').ToList();

                listFavorites.Remove(id.ToString());

                favorites = string.Empty;

                foreach (var item in listFavorites.Where(x => !string.IsNullOrEmpty(x)))
                    favorites += ',' + item;
                
                _cache.Set("favorite", favorites.Substring(1));

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
                _logger.Log(LogLevel.Information, "Finished NotFavoriteCharacters \tTime: {1}ms", timeStopWatch.ElapsedMilliseconds);
            }

            return response;
        }

        public ResponseContainer DeleteCharacters(int id)
        {
            var response = new ResponseContainer();
            var timeStopWatch = new Stopwatch();
            timeStopWatch.Start();

            _logger.Log(LogLevel.Information, "Started DeleteCharacters");

            try
            {
                var listDeleted = new List<string>();

                string deleteds = string.Empty;
                
                _cache.TryGetValue("deleted", out deleteds);

                listDeleted = (deleteds ?? "").Split(',').ToList();

                listDeleted.Add(id.ToString());

                deleteds = string.Empty;

                foreach (var item in listDeleted.Where(x => !string.IsNullOrEmpty(x)))
                    deleteds += ',' + item;

                
                _cache.Set("deleted", deleteds.Substring(1));

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
                _logger.Log(LogLevel.Information, "Finished DeleteCharacters \tTime: {1}ms", timeStopWatch.ElapsedMilliseconds);
            }

            return response;
        }
    }
}
