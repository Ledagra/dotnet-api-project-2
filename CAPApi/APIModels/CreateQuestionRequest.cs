using System.ComponentModel.DataAnnotations;
using CAPApi.DataModels;

namespace CAPApi.APIModels
{
    public class CreateQuestionRequest
    {
        [Required]
        public int SurveyId { get; set; }

        [Required]
        public string Text { get; set; } = null!;

        [Required]
        public QuestionType Type { get; set; }

        public List<AnswerObject>? Answers { get; set; }
    }
}
