using System.ComponentModel.DataAnnotations;

namespace CAPApi.APIModels
{
    public class CreateSurveyRequest
    {
        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;
    }
}
