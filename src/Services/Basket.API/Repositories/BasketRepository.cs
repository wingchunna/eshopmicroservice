using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializeService _serializeService;
        private readonly ILogger _logger;

        public BasketRepository(IDistributedCache redisCacheService, ISerializeService serializeService, ILogger logger )
        {
            _redisCacheService = redisCacheService;
            _serializeService = serializeService;
            _logger = logger;
        }


        public async Task<bool> DeleteBasketFromUserName(string userName)
        {
           try
            {
                _logger.Information($"BEGIN: DeleteBasketFromUserName {userName}");
              await  _redisCacheService.RemoveAsync(userName);
                _logger.Information($"END: DeleteBasketFromUserName {userName}");
                return true;
            }catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;

            }
        }

        public async Task<Cart?> GetBasketByUserName(string userName)
        {
            _logger.Information($"BEGIN: Get Basket for {userName}");
            var basket = await _redisCacheService.GetStringAsync(userName);
            _logger.Information($"END: Get Basket for {userName}");
            return string.IsNullOrEmpty(basket)? null : 
                _serializeService.Deserialize<Cart>(basket);
        }

        public async Task<Cart> UpdateBasket(Cart cart, DistributedCacheEntryOptions options = null)
        {
            if(options != null)
            
                await _redisCacheService.SetStringAsync(cart.Username, _serializeService.Serialize(cart),options);
           
            else
                _logger.Information($"BEGIN: UpdateBasket for {cart.Username}");
            await _redisCacheService.SetStringAsync(cart.Username, _serializeService.Serialize(cart));
            _logger.Information($"END: UpdateBasket for {cart.Username}");
            return await GetBasketByUserName(cart.Username);
        }
    }
}
