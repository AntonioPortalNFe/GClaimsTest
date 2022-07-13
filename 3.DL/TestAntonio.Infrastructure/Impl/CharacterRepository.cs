using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestAntonio.Contracts.Marvel;

namespace TestAntonio.Infrastructure.Interfaces
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly IHttpRepository _httpReposository;

        public CharacterRepository(IHttpRepository httpReposository)
        {
            _httpReposository = httpReposository;
        }

        public async Task<CharacterDataWrapper> Get(string parameters)
        {
            
            return await _httpReposository.GetAsync<CharacterDataWrapper>("characters", parameters);
        }

        public async Task<CharacterDataWrapper> GetByID(string characterId)
        {
            return  await _httpReposository.GetAsync<CharacterDataWrapper>($"characters/{characterId}");
        }
        
    }
}
