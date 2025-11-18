using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAPApi.DataModels
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = null!;

        [Required]
        public QuestionType Type { get; set; }

        [ForeignKey("Survey")]
        public int SurveyId { get; set; }
        public Survey Survey { get; set; } = null!;

        public List<Answer> Answers { get; set; } = [];
    }

    public enum QuestionType
    {
        SingleChoice = 0,
        MultipleChoice = 1,
        FreeText = 2
    }
}
