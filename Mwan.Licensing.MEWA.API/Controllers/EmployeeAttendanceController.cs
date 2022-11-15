using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mwan.Licensing.MEWA.API.Context;
using Mwan.Licensing.MEWA.API.Data.Models;
using Mwan.Licensing.MEWA.API.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Mwan.Licensing.MEWA.API.Controllers
{
    [Authorize(Roles ="MWAN")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    public class EmployeeAttendanceController : ControllerBase
    {
        private AttendanceContext _db;
        public EmployeeAttendanceController(AttendanceContext db)
        {
            _db = db;
        }
        // GET: api/<EmployeeAttendanceController>
        [HttpGet]
        public IEnumerable<EntryExitModel> Get(string createDate, string createToDate)
        {
            List<EntryExitModel> attendanceList = new List<EntryExitModel>();
            try
            {
                DateTime cdate = DateTime.ParseExact(createDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime cdateTo = DateTime.ParseExact(createToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
         
                var list = _db.EmployeeAttendances
                              .Where(a => a.CreateDateTime >= cdate)
                              .Where(a => a.CreateDateTime <= cdateTo.AddDays(1)).AsEnumerable()
                              .GroupBy(x => new { x.Name, x.CreateDateTime.Date })
                     .Select(x => new
                     {
                         Name = x.Key.Name,

                         Group1 = x.Where(y => y.RegistrationType == "لدخول").FirstOrDefault(),
                         Group2 = x.Where(y => y.RegistrationType == "الخروج").LastOrDefault(),
                         //  Group3 = x.Where(y => y.RegistrationType == "EarlyEntry").FirstOrDefault(),
                         Group4 = x.Where(y => y.RegistrationType == "إستئذان").LastOrDefault(),

                     })
                    .Select(x => new EntryExitModel
                    {
                        Name = x.Name,
                        // Department = x.Group1 == null ? x.Group2.Department : x.Group1.Department,
                        Department = x.Group1 == null ? x.Group2 == null ? x.Group4.Department : x.Group2.Department : x.Group1.Department,
                        EntryTime = x.Group1 == null ? null : x.Group1.CreateDateTime,
                        ExitTime = x.Group2 == null ? null : x.Group2.CreateDateTime,
                        // EarlyEntryTime = x.Group3 == null ? null : x.Group3.CreateDateTime,
                        EarlyExitTime = x.Group4 == null ? null : x.Group4.CreateDateTime,
                    }).ToList();

                foreach (var attendanceEntry in list)
                {
                    EntryExitModel objList = new EntryExitModel();
                    objList.Name = attendanceEntry.Name;
                    objList.Department = attendanceEntry.Department;
                    objList.EntryTime = attendanceEntry.EntryTime;
                    objList.ExitTime = attendanceEntry.ExitTime;
                    objList.EarlyEntryTime = attendanceEntry.EarlyEntryTime;
                    objList.EarlyExitTime = attendanceEntry.EarlyExitTime;
                    attendanceList.Add(objList);
                }

                return attendanceList;


            }
            catch (Exception ex)
            {
                return attendanceList;
            }

        }

        [HttpGet]
        [Route("Employee")]
        public IEnumerable<EntryExitModel> GetForTheEmployee(string name, string createDate)
        {
            List<EntryExitModel> attendanceList = new List<EntryExitModel>();
            try
            {
                DateTime cdate = DateTime.ParseExact(createDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var list = _db.EmployeeAttendances
                              .Where(a => a.Name == name)
                              .Where(a => a.CreateDateTime >= cdate)
                              .Where(a => a.CreateDateTime <= cdate.AddDays(1)).AsEnumerable()
                              .GroupBy(x => new { x.Name, x.CreateDateTime.Date })
                     .Select(x => new
                     {
                         Name = x.Key.Name,

                         Group1 = x.Where(y => y.RegistrationType == "لدخول").FirstOrDefault(),
                         Group2 = x.Where(y => y.RegistrationType == "الخروج").LastOrDefault(),
                         // Group3 = x.Where(y => y.RegistrationType == "EarlyEntry").FirstOrDefault(),
                         Group4 = x.Where(y => y.RegistrationType == "إستئذان").LastOrDefault(),

                     })
                    .Select(x => new EntryExitModel
                    {
                        Name = x.Name,
                        Department = x.Group1 == null ? x.Group2 == null ? x.Group4.Department : x.Group2.Department : x.Group1.Department,
                        EntryTime = x.Group1 == null ? null : x.Group1.CreateDateTime,
                        ExitTime = x.Group2 == null ? null : x.Group2.CreateDateTime,
                        //EarlyEntryTime = x.Group3 == null ? null : x.Group3.CreateDateTime,
                        EarlyExitTime = x.Group4 == null ? null : x.Group4.CreateDateTime,
                    }).ToList();

                foreach (var attendanceEntry in list)
                {
                    EntryExitModel objList = new EntryExitModel();
                    objList.Name = attendanceEntry.Name;
                    objList.Department = attendanceEntry.Department;
                    objList.EntryTime = attendanceEntry.EntryTime;
                    objList.ExitTime = attendanceEntry.ExitTime;
                    //objList.EarlyEntryTime = attendanceEntry.EarlyEntryTime;
                    objList.EarlyExitTime = attendanceEntry.EarlyExitTime;
                    attendanceList.Add(objList);
                }

                return attendanceList;


            }
            catch (Exception ex)
            {
                return attendanceList;
            }

        }

        [HttpGet]
        [Route("AttendanceStatus")]
        public ActionResult<List<EntryExitModel>> AttendanceStatus(string name, string createDate)
        {
            List<EntryExitModel> attendanceStatusList = new List<EntryExitModel>();
            try
            {
                DateTime cdate = DateTime.ParseExact(createDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var list = _db.EmployeeAttendances
                              .Where(a => a.CreateDateTime >= cdate)
                              .Where(a => a.CreateDateTime <= cdate.AddDays(1))
                              .Where(a => a.Name == name)
                              .AsEnumerable().GroupBy(x => new { x.Name, x.CreateDateTime.Date })
                     .Select(x => new
                     {
                         Name = x.Key.Name,
                         Group1 = x.Where(y => y.RegistrationType == "لدخول").FirstOrDefault(),
                         Group2 = x.Where(y => y.RegistrationType == "الخروج").LastOrDefault(),
                         // Group3 = x.Where(y => y.RegistrationType == "EarlyEntry").LastOrDefault(),
                         Group4 = x.Where(y => y.RegistrationType == "إستئذان").LastOrDefault(),

                     })
                    .Select(x => new EntryExitModel
                    {
                        Name = x.Name,
                        EntryTime = x.Group1 == null ? null : x.Group1.CreateDateTime,
                        ExitTime = x.Group2 == null ? null : x.Group2.CreateDateTime,
                        // EarlyEntryTime = x.Group3 == null ? null : x.Group3.CreateDateTime,
                        EarlyExitTime = x.Group4 == null ? null : x.Group4.CreateDateTime,
                    }).ToList();

                foreach (var attendanceEntry in list)
                {
                    EntryExitModel objList = new EntryExitModel();
                    objList.Name = attendanceEntry.Name;
                    objList.EntryTime = attendanceEntry.EntryTime;
                    objList.ExitTime = attendanceEntry.ExitTime;
                    // objList.EarlyEntryTime = attendanceEntry.EarlyEntryTime;
                    objList.EarlyExitTime = attendanceEntry.EarlyExitTime;
                    attendanceStatusList.Add(objList);
                }
                if (attendanceStatusList.Count > 0)
                {
                    return Ok(attendanceStatusList);
                }
                else
                    return NoContent();



            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error occured..");
            }
        }



        // POST api/<EmployeeAttendanceController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmployeeAttendance value)
        {
            //if(value.RegistrationType == "EarlyEntry")
            //{
            //    bool isRequested = isEarlyExitRequested(value.Name,value.CreateDateTime.Date.ToShortDateString());
            //    if (!isRequested)
            //        return BadRequest("No Early exit requested");
            //}
            value.CreateDateTime = DateTime.Now;
            _db.EmployeeAttendances.Add(value);
            _db.SaveChanges();
            if (value.RegistrationType == "لدخول" || value.RegistrationType == "الخروج")
                return Ok(new { str = "تم تسجيل الحضور بنجاح" });
            else
            {
                return Ok(new { str = "تم تسجيل استئذان خروج مبكر" });
            }

        }

        bool isEarlyExitRequested(string name, string createDate)
        {
            DateTime cdate = DateTime.ParseExact(createDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            int numberOfEarlyExit = _db.EmployeeAttendances
                                    .Where(a => a.CreateDateTime >= cdate)
                                    .Where(a => a.CreateDateTime <= cdate.AddDays(1))
                                    .Where(n => n.Name == name)
                                    .Where(t => t.RegistrationType == "EarlyExit").Count();
            return numberOfEarlyExit > 0;
        }

        //[HttpPost]
        //[Route("PostCountry")]
        //[AllowAnonymous]
        //public postedCountry PostCountry([FromBody] postedCountry value)
        //{
        //    return value;
        //}



    }
}
