using System.ComponentModel.DataAnnotations;

namespace HangFireSample.Models
{
    public class EmailModel
    {
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime SendDateTime { get; set; }

        [Required]
        [EmailAddress]
        public string RecipientEmail { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
