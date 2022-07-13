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

namespace TestAntonio.Services.Impl
{
    public class MarvelServices : IMarvelServices
    {        
        private readonly ICharacterRepository _characterRepository;
        private readonly HttpContext _context;
        private readonly ILogger<MarvelServices> _logger;
        public MarvelServices(ILogger<MarvelServices> logger, ICharacterRepository characterRepository, IHttpContextAccessor context)
        {            
            _characterRepository = characterRepository;
            _context = context.HttpContext;
            _logger = logger;
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
            var timeStopWatch = new Stopwatch();
            timeStopWatch.Start();

            _logger.Log(LogLevel.Information, "Started FavoriteCharacters");

            try
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
            catch (System.Exception ex)
            {

                _logger.LogError($" Message: {ex.Message} \tFavoriteCharactersId: {id}");
            }
            finally
            {
                _logger.Log(LogLevel.Information, "Finished FavoriteCharacters \tTime: {1}ms", timeStopWatch.ElapsedMilliseconds);
            }
        }

        public void NotFavoriteCharacters(int id)
        {

            var timeStopWatch = new Stopwatch();
            timeStopWatch.Start();

            _logger.Log(LogLevel.Information, "Started NotFavoriteCharacters");

            try
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
            catch (System.Exception ex)
            {

                _logger.LogError($" Message: {ex.Message} \tNotFavoriteCharactersId: {id}");
            }
            finally
            {
                _logger.Log(LogLevel.Information, "Finished NotFavoriteCharacters \tTime: {1}ms", timeStopWatch.ElapsedMilliseconds);
            }

        }

        public void DeleteCharacters(int id)
        {
            var timeStopWatch = new Stopwatch();
            timeStopWatch.Start();

            _logger.Log(LogLevel.Information, "Started DeleteCharacters");

            try
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
            catch (System.Exception ex)
            {

                _logger.LogError($" Message: {ex.Message} \tDeleteCharactersId: {id}");
            }
            finally
            {
                _logger.Log(LogLevel.Information, "Finished DeleteCharacters \tTime: {1}ms", timeStopWatch.ElapsedMilliseconds);
            }

        }
    }
}
