using Swashbuckle.AspNetCore.Filters;
using CAPApi.DataModels;

namespace CAPApi.Examples
{
    public class CreateSurveyResponseExample : IExamplesProvider<Survey>
    {
        public Survey GetExamples()
        {
            return new Survey
            {
                Id = 1,
                Title = "Employee Satisfaction Survey",
                Description = "Gathers feedback from employees about workplace culture and satisfaction.",
                Questions =
                [
                    new Question
                    {
                        Id = 1,
                        Text = "How would you rate your overall job satisfaction?",
                        Type = QuestionType.SingleChoice,
                        Answers =
                        [
                            new Answer { Id = 1, Text = "Very Satisfied", Weight = 5 },
                            new Answer { Id = 2, Text = "Somewhat Satisfied", Weight = 3 },
                            new Answer { Id = 3, Text = "Not Satisfied", Weight = 1 }
                        ]
                    }
                ]
            };
        }
    }
}
