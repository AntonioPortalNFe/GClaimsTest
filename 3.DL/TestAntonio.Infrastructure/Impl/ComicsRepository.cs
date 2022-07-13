using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestAntonio.Contracts.Marvel;

namespace TestAntonio.Infrastructure.Interfaces
{
    public class ComicsRepository : IComicsRepository
    {
        private readonly IHttpRepository _httpReposository;

        public ComicsRepository(IHttpRepository httpReposository)
        {
            _httpReposository = httpReposository;
        }

        public async Task<ComicDataWrapper> Get()
        {
            return await _httpReposository.GetAsync<ComicDataWrapper>("comics");
        }

        public async Task<ComicDataWrapper> GetByID(string comicId)
        {
            return  await _httpReposository.GetAsync<ComicDataWrapper>($"comics/{comicId}");
        }
        public async void GetCharacters(string comicId)
        {

        }
        public async void GetCreators(string comicId)
        {

        }
    }
}
