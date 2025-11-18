using System.ComponentModel.DataAnnotations;

namespace CAPApi.APIModels
{
    public class SubmitResponseRequest
    {
        [Required]
        public int SurveyId { get; set; }

        [Required]
        public List<int> SelectedAnswerIds { get; set; } = [];

        [Required]
        public Dictionary<int, string>? FreeTextAnswers { get; set; }
    }
}