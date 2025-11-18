using Swashbuckle.AspNetCore.Filters;
using CAPApi.APIModels;
using CAPApi.DataModels;

namespace CAPApi.APIExamples
{
    public class CreateQuestionRequestExample : IExamplesProvider<CreateQuestionRequest>
    {
        public CreateQuestionRequest GetExamples()
        {
            return new CreateQuestionRequest
            {
                SurveyId = 1,
                Text = "How satisfied are you with our support team?",
                Type = QuestionType.SingleChoice,
                Answers =
                [
                    new() { Text = "Very satisfied", Weight = 5 },
                    new() { Text = "Somewhat satisfied", Weight = 3 },
                    new() { Text = "Not satisfied", Weight = 1 }
                ]
            };
        }
    }
}
