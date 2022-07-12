using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class BasketController :ControllerBase
 {
    private readonly IBasketRepository _repository;
    public BasketController(IBasketRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{username}", Name = "GetBasket")]
    [ProducesResponseType(typeof(Cart),(int)HttpStatusCode.OK)]

    public async Task<ActionResult<Cart>> GetBasketByUsername([Required] string username)
    {
        var result = await _repository.GetBasketByUserName(username);
        return Ok(result ?? new Cart());
        // ?? : Nếu result có kq thì lấy result, nếu không thì tạo 1 card rỗng
    }

    [HttpPost(Name = "UpdateBasket")]
    [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Cart>> UpdateBasket([FromBody]Cart cart)
    {
        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.UtcNow.AddDays(1)) // set thời gian lưu thông tin sp trong basket là 1 ngày
            .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // set thời gian kiểm tra là 10p
        var result = await _repository.UpdateBasket(cart, options);
        return Ok(result);
    }


    [HttpDelete("{username}",Name = "Delete Basket")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteBasket([Required] string username) { 
        var result = await _repository.DeleteBasketFromUserName(username);
        return Ok(result);
    }
}

