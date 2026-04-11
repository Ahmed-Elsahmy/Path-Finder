using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class User: IdentityUser
    {
        public string  FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public  List<CV>? Cvs { get; set; }
        public List<UserSkill>? skills { get; set; }
        public List<UserEducation>? userEducations { get; set; }
        public virtual ICollection<CourseProgress> CourseProgresses { get; set; } = new List<CourseProgress>();
    }
}
