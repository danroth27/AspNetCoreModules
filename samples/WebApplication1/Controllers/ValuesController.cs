using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Value Get(int id)
        {
            return new Value { Id = id, Text = $"Value {id}" };
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Value value)
        {
        }
    }

    public class Value
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}
