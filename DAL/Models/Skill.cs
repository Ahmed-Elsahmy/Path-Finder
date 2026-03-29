using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Skill
    {
        [Key]
        public int SkillId { get; set; }

        [Required]
        [StringLength(200)]
        public string SkillName { get; set; }

        [StringLength(100)]
        public string? Category { get; set; } // Nullable

        [StringLength(500)]
        public string? Description { get; set; } // Nullable

        public bool IsTechnical { get; set; }

    }
}
