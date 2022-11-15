using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Mwan.Licensing.MEWA.API.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mwan.Licensing.MEWA.API.Controllers
{
    [Authorize(Roles = "MEWA")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class LicensesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LicensesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public  ActionResult<ResponseResult> Get(string CRNumber="", string IdNumber ="", string licenseNumber = "")
        {
            ResponseResult outputResponse = new ResponseResult();
            if (CRNumber == "" && IdNumber =="" && licenseNumber == "")
            {
                outputResponse.ErrorMessage = "Please, either one of the three inputs should be provided";
                outputResponse.StatusCode = 400;
                outputResponse.Result = null;
                outputResponse.Success = false;

                return outputResponse;
            }
                
            List<LicenseResponseVM> lstLicenses = new List<LicenseResponseVM>();
            
            try
            {
                string sql = @"select a.LicenseNumber,
                                      b.CommercialRegistrationNumber,
	                                  b.IdNumber,
	                                  c.TradeName,
	                                  c.City,
	                                  c.Address,
	                                  c.CompanyEmail,
	                                  c.Activity,
	                                  c.SubActivity,
	                                  c.MobileNumber,
	                                  a.LastUpdatedOn,
	                                  c.ExpirationDate
                                from
                                      licenses a inner join
                                      Accounts b on a.AccountId = b.Id inner join
                                      BeneficiaryCompanies c on a.Id = c.RequestId
                                where
                                      (b.CommercialRegistrationNumber = @CRNumber Or
                                       b.IdNumber = @IdNumber Or 
                                       a.LicenseNumber = @LicenseNumber)
                                 and   a.WasteActivity = N'جمع ونقل النفايات غير الخطرة'
                                 and   a.Status = 3";

                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.Add(new SqlParameter("@CRNumber", CRNumber));
                    command.Parameters.Add(new SqlParameter("@IdNumber", IdNumber));
                    command.Parameters.Add(new SqlParameter("@LicenseNumber", licenseNumber));

                    SqlDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        LicenseResponseVM licenseObj = new LicenseResponseVM()
                        {
                            Activity = dr["Activity"].ToString(),
                            Address = dr["Address"].ToString(),
                            City = dr["City"].ToString(),
                            CompanyEmail = dr["CompanyEmail"].ToString(),
                            CompanyName = dr["TradeName"].ToString(),
                            CRNumber = dr["CommercialRegistrationNumber"].ToString(),
                            IdNumber = dr["IdNumber"].ToString(),
                            LicenseExpiryDate = dr["ExpirationDate"].ToString(),
                            LicenseGeneratedDate = dr["LastUpdatedOn"].ToString(),
                            LicenseNumber = dr["LicenseNumber"].ToString(),
                            MobileNumber = dr["MobileNumber"].ToString(),
                            SubActivity = dr["SubActivity"].ToString()
                        };
                        lstLicenses.Add(licenseObj);
                    }
                    if (lstLicenses.Count > 0)
                    {
                        outputResponse.ErrorMessage = "";
                        outputResponse.StatusCode = 200;
                        outputResponse.Result = lstLicenses;
                        outputResponse.Success = true;

                        return outputResponse;
                      
                    }
                    else
                    {
                        outputResponse.ErrorMessage = "No valid data found";
                        outputResponse.StatusCode = 406;
                        outputResponse.Result = null;
                        outputResponse.Success = false;

                        return outputResponse;
                    }
                }
            }
            catch (Exception ex) {
                outputResponse.ErrorMessage = "Unhandled Error Occured";
                outputResponse.StatusCode = 500;
                outputResponse.Result = null;
                outputResponse.Success = false;
                return outputResponse;
            }
        
           
        }

    }
}
