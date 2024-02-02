using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class LoginDTO
    {
        public string user { get; set; }
        public string pass { get; set; }
        public bool logeado { get; set; }
    }
}
