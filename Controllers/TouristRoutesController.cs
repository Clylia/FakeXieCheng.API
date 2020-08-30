using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeXieCheng.API.Dtos;
using FakeXieCheng.API.Models;
using FakeXieCheng.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Text.RegularExpressions;
using FakeXieCheng.API.ResourceParameters;
using Microsoft.AspNetCore.JsonPatch;

namespace FakeXieCheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;

        public readonly IMapper _mapper;
        public TouristRoutesController(ITouristRouteRepository touristRouteRepository,IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [HttpHead]
        public IActionResult GetTouristRoutes(
            [FromQuery] TouristRouteResourceParameters parameters
            //[FromQuery]string keyword,string rating
            )
        {
            var touristRoutesFromRepo = _touristRouteRepository.GetTouristRoutes(parameters.Keyword, parameters.OperatorType, parameters.RatingValue);
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("没有旅游路线");
            }
            var touristRouteDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            return Ok(touristRouteDto);
        }

        [HttpGet("{touristRouteId}",Name = "GetTouristRouteById")]
        public IActionResult GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            if (touristRouteFromRepo==null)
            {
                return NotFound($"未找到路线");
            }
            //decimal price = 0;
            //if (touristRouteFromRepo.DiscountPresent != null)
            //{

            //    price = touristRouteFromRepo.OriginalPrice * (decimal)touristRouteFromRepo.DiscountPresent;
            //}
            //else
            //{
            //    price = touristRouteFromRepo.OriginalPrice * 1;
            //}
            //ToutistRouteDto toutistRouteDto = new ToutistRouteDto()
            //{ 
            //    Id=touristRouteFromRepo.Id,
            //    Title=touristRouteFromRepo.Title,
            //    Description=touristRouteFromRepo.Description,
            //    Price = price,
            //    CreateTime=touristRouteFromRepo.CreateTime,
            //    UpdateTime=touristRouteFromRepo.UpdateTime,
            //    Features=touristRouteFromRepo.Features,
            //    Fees=touristRouteFromRepo.Fees,
            //    Notes=touristRouteFromRepo.Notes,
            //    Rating=touristRouteFromRepo.Rating,
            //    TravelDays=touristRouteFromRepo.TravelDays.ToString(),
            //    TripType=touristRouteFromRepo.TripType.ToString(),
            //    DepartureCity=touristRouteFromRepo.DepartureCity.ToString()

            //};
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            return Ok(touristRouteDto);
        }

        //*/api/TouristRoutes
        [HttpPost]
        public IActionResult CreateTouristRoute([FromBody]TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            _touristRouteRepository.Save();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute(
                "GetTouristRouteById",
                new { touristRouteId=touristRouteToReturn.Id},
                touristRouteToReturn
            );
        }

        [HttpPut("{touristRouteId}")]
        public IActionResult UpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto
        )
        {
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("旅游路线找不到");
            }

            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            // 1. 映射dto
            // 2. 更新dto
            // 3. 映射model
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);

            _touristRouteRepository.Save();

            return NoContent();
        }

        [HttpPatch("{touristRouteId}")]
        public IActionResult PartiallyUpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument
            )
        {
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId
                ))
            {
                return NotFound("旅游路线找不到");
            }

            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPatch,ModelState);

            if (!TryValidateModel(touristRouteToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(touristRouteToPatch,touristRouteFromRepo);
            _touristRouteRepository.Save();

            return NoContent();
        }
    }
}
