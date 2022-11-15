using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mwan.Licensing.MEWA.API.Data.ViewModels
{
    public class LicenseResponseVM
    {
        public string LicenseNumber { get; set; }
        public string CRNumber { get; set; }
        public string IdNumber { get; set; }
        public string CompanyName { get; set; }
        public string MobileNumber { get; set; }
        public string CompanyEmail { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
      
        public string Activity { get; set; }
        public string SubActivity { get; set; }

        public string LicenseGeneratedDate { get; set; }
        public string LicenseExpiryDate { get; set; }


    }


    public class ResponseResult
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<LicenseResponseVM> Result { get; set; }

        public bool Success { get; set; }
    }
}
