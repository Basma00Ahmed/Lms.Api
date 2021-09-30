using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lms.Core.Dtos
{
    public class CourseDto
    {
        [Required]
        [StringLength(20)]
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate  { get; set; }
        public ICollection<ModuleDto> Modules { get; set; }
    }
}
