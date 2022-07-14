using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TestAntonio.Contracts.Marvel;

namespace TestAntonio.Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IActionResult> Index(string orderBy = "name", 
                                               string nameFilter = "", 
                                               string nameStartsWith = "",
                                                int? pageNumber = 1,
                                                int? limit = 15)
        {
            ViewData["name"] = orderBy == "name" ? "-name" : "name";
            ViewData["modified"] = orderBy == "modified" ? "-modified" : "modified";
            ViewData["orderBy"] = orderBy;
            ViewData["nameStartsWith"] = nameStartsWith;
            ViewData["nameFilter"] = nameFilter;
            ViewData["pageNumber"] = pageNumber;
            ViewData["limit"] = limit;


            string parameters = $"pageNumber={pageNumber}&limit={limit}&&orderBy={orderBy}&name={nameFilter}&nameStartsWith={nameStartsWith}";

            var characterDataWrapper = await GetAsync<CharacterDataWrapper>("GetCharacters", parameters); 

            return View(characterDataWrapper);
            
        }

        public async Task Favorite(int? id)
        {            

            if (id != null)
                await GetAsync<ResponseContainer>("FavoriteCharacters", "id=" + id);                  

            Response.Redirect("/" + Request.QueryString);
        }
        public async Task NotFavorite(int? id)
        {

            if (id != null)
                await GetAsync<ResponseContainer>("NotFavoriteCharacters", "id=" + id);            

            Response.Redirect("/" + Request.QueryString);
        }

        public async Task Delete(int? id)
        {

            if (id != null)
                await GetAsync<ResponseContainer>("DeleteCharacters", "id=" +id);                           

            Response.Redirect("/" + Request.QueryString);
        }


        private async Task<T> GetAsync<T>(string action, string parameters = "") where T : ResponseContainer, new()
        {
            T response = new T();
            string responseStr = string.Empty;

            var  httpClient = new HttpClient();
            var url =_config.GetValue<string>("ApiEndpoint");

            using (var httpResponse = await httpClient.GetAsync($"{url}{action}?{parameters}"))
            {
                    
                    responseStr = await httpResponse.Content.ReadAsStringAsync();

                    if (responseStr != null)
                        response = System.Text.Json.JsonSerializer.Deserialize<T>(responseStr) ?? new T();
                    
            }            

            return response;

        }

    }
}
