using Swashbuckle.AspNetCore.Filters;

namespace CAPApi.APIExamples
{
    public class SubmitResponseResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                Message = "Response submitted successfully",
                SurveyId = 1,
                TotalScore = 13.0
            };
        }
    }
}