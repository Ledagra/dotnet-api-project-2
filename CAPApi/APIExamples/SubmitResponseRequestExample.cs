using Swashbuckle.AspNetCore.Filters;
using CAPApi.APIModels;
using System.Collections.Generic;

namespace CAPApi.APIExamples
{
    public class SubmitResponseRequestExample : IExamplesProvider<SubmitResponseRequest>
    {
        public SubmitResponseRequest GetExamples()
        {
            return new SubmitResponseRequest
            {
                SurveyId = 1,
                SelectedAnswerIds = [2, 5, 8]
            };
        }
    }
}
