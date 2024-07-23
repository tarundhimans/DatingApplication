using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DatingApplication.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SenderId { get; set; }
        [ForeignKey("SenderId")]
        public ApplicationUser Sender { get; set; }
        [Required]
        public string ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public ApplicationUser Receiver { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
