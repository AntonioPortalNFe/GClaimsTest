using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestAntonio.Contracts.Marvel;
using TestAntonio.Services.Impl;
using TestAntonio.Services.Interfaces;
using TestAntonio.Site.Models;

namespace TestAntonio.Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMarvelServices _marvelServices;
        public HomeController(ILogger<HomeController> logger, IMarvelServices marvelServices)
        {
            _logger = logger;
            _marvelServices = marvelServices;
        }

        public async Task<IActionResult> Index(string orderBy = "name", 
                                               string nameFilter = "", 
                                               string nameStartsWith = "",
                                                int? pageNumber = 1,
                                                int? limit = 30)
        {
            ViewData["name"] = orderBy == "name" ? "-name" : "name";
            ViewData["modified"] = orderBy == "modified" ? "-modified" : "modified";
            ViewData["orderBy"] = orderBy;
            ViewData["nameStartsWith"] = nameStartsWith;
            ViewData["nameFilter"] = nameFilter;
            ViewData["pageNumber"] = pageNumber;
            ViewData["limit"] = limit;


            var characterDataWrapper = await _marvelServices.GetCharacters(orderBy, nameFilter, nameStartsWith, (int)pageNumber, (int)limit);

            return View(characterDataWrapper);

            
        }

        public void Favorite(int? id)
        {            

            if (id != null)            
                _marvelServices.FavoriteCharacters((int)id);            

            Response.Redirect("/");
        }
        public void NotFavorite(int? id)
        {

            if (id != null)
                _marvelServices.NotFavoriteCharacters((int)id);

            Response.Redirect("/");
        }

        public void Delete(int? id)
        {

            if (id != null)
            {
                _marvelServices.DeleteCharacters((int)id);
                _marvelServices.NotFavoriteCharacters((int)id);
            }

            Response.Redirect("/");
        }

    }
}
