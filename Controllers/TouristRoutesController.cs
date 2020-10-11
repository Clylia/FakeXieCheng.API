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
using FakeXieCheng.API.Helper;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> GetTouristRoutes(
            [FromQuery] TouristRouteResourceParameters parameters
            //[FromQuery]string keyword,string rating
            )
        {
            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesAsync(parameters.Keyword, parameters.OperatorType, parameters.RatingValue);
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("没有旅游路线");
            }
            var touristRouteDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
            return Ok(touristRouteDto);
        }

        [HttpGet("{touristRouteId}",Name = "GetTouristRouteById")]
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo =await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
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
        [Authorize(AuthenticationSchemes ="Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoute([FromBody]TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute(
                "GetTouristRouteById",
                new { touristRouteId=touristRouteToReturn.Id},
                touristRouteToReturn
            );
        }

        [HttpPut("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto
        )
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅游路线找不到");
            }

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            // 1. 映射dto
            // 2. 更新dto
            // 3. 映射model
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);

            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument
            )
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId
                )))
            {
                return NotFound("旅游路线找不到");
            }

            var touristRouteFromRepo =await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPatch,ModelState);

            if (!TryValidateModel(touristRouteToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(touristRouteToPatch,touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
        /*
         *
         *
         *delete
         *
         *
         https://localhost:5001/api/TouristRoutes/39996f34-013c-4fc6-b1b3-0c1036c47110


         [
    {
    "op": "replace",
    "path": "/title",
    "value": "AASDFERFAREF"
    },
    {
    "op": "replace",
    "path": "/description",
    "value": "AASDFERFAREF"
    }
]
         */
        [HttpDelete("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅游路线未找到");
            }

            var touristRoute =await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRoute);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        //   https://localhost:5001/api/TouristRoutes/(39996f34-013c-4fc6-b1b3-0c1036c47111,39996f34-013c-4fc6-b1b3-0c1036c47113)
        [HttpDelete("({touristIDs})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteByIDs(
            [ModelBinder(BinderType =typeof(ArrayModelBinder))][FromRoute] IEnumerable<Guid> touristIDs)
        {
            if (touristIDs==null)
            {
                return BadRequest();
            }

            var touristRouteFromRepo =await _touristRouteRepository.GetTouristRouteByIDListAsync(touristIDs);
            _touristRouteRepository.DeleteTouristRoutes(touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
    }
}
