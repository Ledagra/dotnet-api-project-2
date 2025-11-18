using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAPApi.DataModels
{
    public class Response
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Survey")]
        public int SurveyId { get; set; }
        public Survey Survey { get; set; } = null!;

        public List<int> SelectedAnswerIds { get; set; } = [];

        public double TotalScore { get; set; }
        public string? FreeTextResponsesJson { get; set; }
        public string? SurveyTitle { get; set; }
    }
}
