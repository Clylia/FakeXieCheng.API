using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FakeXieCheng.API.ResourceParameters
{
    public class TouristRouteResourceParameters
    {

        public string Keyword { get; set; }
        public int? RatingValue { get; set; }
        public string OperatorType { get; set; }
        private string rating;
        public string Rating
        {
            get
            { return rating; }
            set
            {
                Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
                //string operatorType="";
                //int ratingValue=0;
                Match match = regex.Match(value);
                if (match.Success)
                {
                    OperatorType = match.Groups[1].Value;
                    RatingValue = Int32.Parse(match.Groups[2].Value);
                }
                rating = value;
            }
        }
    }
}
