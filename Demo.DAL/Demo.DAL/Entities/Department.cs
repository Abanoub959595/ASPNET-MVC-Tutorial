using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Entities
{
    public class Department :  BaseEntity
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Department Name is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Department Code is Required")]
        public string Code { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
