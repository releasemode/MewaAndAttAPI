using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mwan.Licensing.MEWA.API.Data.Models
{
    public class EmployeeAttendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string RegistrationType { get; set; }
        public string ContractType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime CreateDateTime { get; set; }

    }
}
