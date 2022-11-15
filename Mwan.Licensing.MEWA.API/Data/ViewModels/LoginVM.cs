using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mwan.Licensing.MEWA.API.Data.ViewModels
{
    public class LoginVM
    {
      
        public string EmailAddress { get; set; }

    
        public string Password { get; set; }
    }
}
