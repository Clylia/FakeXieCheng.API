using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeXieCheng.API.Models;
using FakeXieCheng.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeXieCheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;

        public TouristRoutesController(ITouristRouteRepository touristRouteRepository)
        {
            _touristRouteRepository = touristRouteRepository;
        }
        [HttpGet]
        public IActionResult GetTouristRoutes()
        {
            var touristRoutesFromRepo = _touristRouteRepository.GetTouristRoutes();
            //if (touristRoutesFromRepo==null||touristRoutesFromRepo.Count()<=0)
            //{
            //    return NotFound("没有旅游路线");
            //}
            return Ok(touristRoutesFromRepo);
        }

        [HttpGet("{touristRouteId}")]
        public IActionResult GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            if (touristRouteFromRepo==null)
            {
                return NotFound($"未找到{touristRouteFromRepo}路线");
            }
            return Ok(touristRouteFromRepo);
        }
    }
}
