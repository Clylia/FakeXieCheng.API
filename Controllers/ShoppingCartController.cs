using AutoMapper;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.API.Helper;
using FakeXieCheng.API.Models;
using FakeXieCheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FakeXieCheng.API.Controllers
{
    [ApiController]
    [Route("api/shoppingCart")]
    public class ShoppingCartController:ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        public ShoppingCartController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository,
                                        IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }

        /*
         
        测试的话先注册一个用户
        {
"email":"cuilishen1234@163.com",
"password":"Fake123$",
"confirmPassword":"Fake123$"
}

        注册地址：https://localhost:5001/auth/register
        POST Body raw json

        注册完成后就登录这个用户，获取jwt Token
        {
"email":"cuilishen1234@163.com",
"password":"Fake123$"
}
        登录地址：https://localhost:5001/auth/login
        POST body raw json
         
        获取jwt之后，获取用户购物车
        地址：https://localhost:5001/api/shoppingCart
        Get （在headers里添加jwt 的token，key：Authorization,value :Bearer ""）

        发送之后获取结果
        结果示例如下{
    "id": "9b54a305-f8db-47e2-92ca-2aa87c4b8ffd",
    "userId": "3520333c-2cd3-4bd5-8d4e-7fd05113cf77",
    "shoppingCartItems": []
}

         */
        [HttpGet]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> GetShoppingCart()
        {
            //1. 获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //2. 使用userId 获得购物车
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));

        }

        [HttpPost("items")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> AddShoppingCartItem(
            [FromBody] AddShoppingCartItemDto addShoppingCartItemDto)
        {
            //1. 获取当前用户
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //2. 使用userId 获得购物车
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserId(userId);

            // 3 创建LineItem 但是创建之前需要查询一下旅游路线是否存在
            var touristRoute = await _touristRouteRepository
                .GetTouristRouteAsync(addShoppingCartItemDto.TouristRouteId);
            //添加之前检查一下要添加的旅游路线是否存在
            if (touristRoute == null)
            {
                return NotFound("旅游路线不存在");
            }

            //路线存在，继续进行添加工作
            var lineItem = new LineItem()
            {
                TouristRouteId = addShoppingCartItemDto.TouristRouteId,
                ShoppingCartId = shoppingCart.Id,
                OriginalPrice = touristRoute.OriginalPrice,
                DiscountPresent = touristRoute.DiscountPresent
            };

            // 4 添加Lineitem 并保存数据库

            await _touristRouteRepository.AddShoppingCartItem(lineItem);
            await _touristRouteRepository.SaveAsync();

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }

        [HttpDelete("items/{itemId}")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] int itemId)
        {
            var lineItem = await _touristRouteRepository
                .GetShoppingCartItemByItemId(itemId);

            if (lineItem == null)
            {
                return NotFound("旅游路线找不到");
            }

            _touristRouteRepository.DeleteShoppingCartItem(lineItem);

            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("items/({itemIDs})")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> RemoveShoppingCartItems(
            [ModelBinder(BinderType =typeof(ArrayModelBinder))]
            [FromRoute]IEnumerable<int> itemIDs
            )
        {
            var lineitems = await _touristRouteRepository
                .GetShoppingCartsByIdListAsync(itemIDs);

            _touristRouteRepository.DeleteShoppingCartItems(lineitems);

            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
    }
}
