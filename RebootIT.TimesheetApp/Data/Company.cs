using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RebootIT.TimesheetApp.Data
{
    public class Company
    {
        public int CompanyId { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
