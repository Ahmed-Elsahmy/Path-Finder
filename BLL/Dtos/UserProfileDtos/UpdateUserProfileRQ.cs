using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BLL.Dtos.UserProfileDtos
{
    public class UpdateUserProfileRQ
    {
        public string ?UserName { get; set; }
        public string ?FirstName { get; set; }
        public string ?LastName { get; set; }
        public string ?PhoneNumber { get; set; }
        public string ?Bio { get; set; }
        public string ?Location { get; set; }
        public IFormFile ?ProfilePictureUrl { get; set; }
    }
}
