using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Module1.Controllers
{
    public class MessageController
    {
        [HttpGet("api/message")]
        public string GetMessage()
        {
            return "Hello from MVC in Module1!";
        }
    }
}
