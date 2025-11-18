using Swashbuckle.AspNetCore.Filters;
using CAPApi.APIModels;

namespace CAPApi.APIExamples
{
    public class CreateSurveyRequestExample : IExamplesProvider<CreateSurveyRequest>
    {
        public CreateSurveyRequest GetExamples()
        {
            return new CreateSurveyRequest
            {
                Title = "Customer Satisfaction Survey",
                Description = "Collects feedback about the customer service experience."
            };
        }
    }
}
