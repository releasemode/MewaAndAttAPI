using Microsoft.EntityFrameworkCore;
using Mwan.Licensing.MEWA.API.Data.Models;

namespace Mwan.Licensing.MEWA.API.Context
{
    public class AttendanceContext
       : DbContext
    {
        public AttendanceContext(DbContextOptions options)
           : base(options)
        {

        }
        public DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }
    }
}
