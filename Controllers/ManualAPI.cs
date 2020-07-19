using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXieCheng.API.Controllers
{
    [Route("api/manualapi")]
    [Controller]
    public class ManualAPI
    {
        
        [HttpGet]
        public IEnumerable<string> Post()
        {
            return new  string[]{ "v","post"};
        }
    }
}
