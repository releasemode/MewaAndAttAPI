using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mwan.Licensing.MEWA.API.Data.ViewModels
{
    public class AuthResultVM
    {
        public string Token { get; set; }
      //  public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class AuthResultResponse
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public AuthResultVM Result { get; set; }

        public bool Success { get; set; }
    }
}
