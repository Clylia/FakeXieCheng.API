using AutoMapper;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FakeXieCheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper  _mapper;
        private readonly IHttpClientFactory   _httpClientFactory;
        public OrdersController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory)

        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> GetOrders()
        {
            //1 获取当前用户
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //2 使用用户id 来获取订单历史计录

            var orders = await _touristRouteRepository.GetOrdersByUserId(userId);
            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        [HttpGet("{orderId}")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            //1 获取当前用户
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var order = await _touristRouteRepository.GetOrderById(orderId);

            return Ok(_mapper.Map<OrderDto>(order));
        }

        [HttpPost("{orderId}/placeOrder")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> PlaceOrder([FromRoute] Guid orderId)
        {
            //1 获取当前用户
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //2 开始处理支付
            var order = await _touristRouteRepository.GetOrderById(orderId);
            order.PaymentProcessing();
            await _touristRouteRepository.SaveAsync();
            //3 向第三方提交支付请求，等待第三方相应
            var httpClient = _httpClientFactory.CreateClient();
            string url= @"http://123.56.149.216/api/FakeVanderPaymentProcess?icode={0}orderNumber={1}&returnFault={2}";
            var response = await httpClient.PostAsync(
                string.Format(url, "80421794A5349F19", order.Id,false),  //最后一个值设置的是支付的结果 ，决定了支付的结果
                null
                );
            //4 提取支付结果，以及支付信息
            bool isApproved = false;
            string transactionMetadata = "";
            if (response.IsSuccessStatusCode)
            {
                transactionMetadata = await response.Content.ReadAsStringAsync();
                var jsonObject = (JObject)JsonConvert.DeserializeObject(transactionMetadata);
                isApproved = jsonObject["isApproved"].Value<bool>();
            }
            //5 如果第三方支付成功，完成订单
            if (isApproved)
            {
                order.PaymentApprove();
            }
            else
            {
                order.PaymentReject();
            }

            order.TransactionMetadata = transactionMetadata;
            await _touristRouteRepository.SaveAsync();

            return Ok(_mapper.Map<OrderDto>(order));
        }
    }
}
