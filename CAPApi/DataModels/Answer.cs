using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAPApi.DataModels
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = null!;

        public double Weight { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }
}
