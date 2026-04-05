using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.AiDtos
{
    public class ChatRQ
    {
        [StringLength(5000, ErrorMessage = "Message cannot exceed 5000 characters.")]
        public string? Message { get; set; }
        public IFormFile? Attachment { get; set; }
        public IFormFile? HistoryFile { get; set; }
    }
}