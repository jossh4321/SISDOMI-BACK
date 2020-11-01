using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace SISDOMI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class ResidenteController : ControllerBase
    {
        // GET: api/<ResidenteController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ResidenteController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ResidenteController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ResidenteController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ResidenteController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
