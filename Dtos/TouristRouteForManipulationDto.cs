using FakeXieCheng.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXieCheng.API.Dtos
{
    [TouristRouteTitleMustBeDifferentFromDescriptionAttribute]
    public abstract class TouristRouteForManipulationDto
    {
        [Required(ErrorMessage = "Title  不可为空")]
        [MaxLength(1500)]
        public string Title { get; set; }
        [Required(ErrorMessage = "描述  不可为空")]
        [MaxLength(1500)]
        public virtual string Description { get; set; }
        //计算方式 原价 * 折扣
        public decimal Price { get; set; }
        //public decimal OriginalPrice { get; set; }
        //public double? DiscountPresent { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Features { get; set; }
        public string Fees { get; set; }
        public string Notes { get; set; }
        public ICollection<TouristRoutePictureForCreationDto> TouristRoutePictures { get; set; } = new List<TouristRoutePictureForCreationDto>();
        public double? Rating { get; set; }
        public string TravelDays { get; set; }
        public string TripType { get; set; }
        public string DepartureCity { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title==Description)
        //    {
        //        yield return new ValidationResult(
        //            "路线名称必须与路线描述不同",
        //            new[] { "TouristRouteForCreationDto" }
        //            );
        //    }
        //}
    }
}
