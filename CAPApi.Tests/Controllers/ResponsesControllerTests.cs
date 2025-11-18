using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CAPApi.Controllers;
using CAPApi.Data;
using CAPApi.DataModels;
using CAPApi.APIModels;

namespace CAPApi.Tests.Controllers
{
    public class ResponsesControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task SubmitResponse_ShouldCalculateScore_ForSingleChoice()
        {
            var db = GetDbContext();

            var survey = new Survey { Title = "Score Test", Description = "Desc" };
            var question = new Question
            {
                Text = "Pick one",
                Type = QuestionType.SingleChoice,
                Answers =
                [
                    new Answer { Text = "A", Weight = 2 },
                    new Answer { Text = "B", Weight = 5 }
                ]
            };
            survey.Questions.Add(question);
            db.Surveys.Add(survey);
            await db.SaveChangesAsync();

            var controller = new ResponsesController(db);
            var request = new SubmitResponseRequest
            {
                SurveyId = survey.Id,
                SelectedAnswerIds = [question.Answers.Last().Id]
            };

            var result = await controller.SubmitResponse(request) as OkObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().NotBeNull();
            result.Value!.ToString().Should().Contain("5");
        }

        [Fact]
        public async Task SubmitResponse_ShouldReturnSelectedAnswerWeight_ForMultipleChoice()
        {
            var db = GetDbContext();

            var survey = new Survey { Title = "Multi", Description = "Test" };
            var question = new Question
            {
                Text = "Pick one",
                Type = QuestionType.MultipleChoice,
                Answers =
                [
                    new Answer { Text = "Low", Weight = 1 },
                    new Answer { Text = "High", Weight = 5 }
                ]
            };
            survey.Questions.Add(question);
            db.Surveys.Add(survey);
            await db.SaveChangesAsync();

            var controller = new ResponsesController(db);
            var request = new SubmitResponseRequest
            {
                SurveyId = survey.Id,
                SelectedAnswerIds = [question.Answers.Last().Id]
            };

            var result = await controller.SubmitResponse(request) as OkObjectResult;
            result.Should().NotBeNull();

            var json = result!.Value!.ToString();
            json.Should().Contain("5");
        }

        [Fact]
        public async Task SubmitResponse_ShouldUseWeightForFreeText_WhenAnswered()
        {
            var db = GetDbContext();

            var survey = new Survey { Title = "Free Text", Description = "T" };
            var question = new Question
            {
                Text = "Describe your experience",
                Type = QuestionType.FreeText,
                Answers =
                [
                    new Answer { Text = "placeholder", Weight = 7 }
                ]
            };
            survey.Questions.Add(question);
            db.Surveys.Add(survey);
            await db.SaveChangesAsync();

            var controller = new ResponsesController(db);
            var request = new SubmitResponseRequest
            {
                SurveyId = survey.Id,
                FreeTextAnswers = new Dictionary<int, string>
                {
                    { question.Id, "It was great!" }
                }
            };

            var result = await controller.SubmitResponse(request) as OkObjectResult;

            result.Should().NotBeNull();
            result!.Value!.ToString().Should().Contain("7");
        }

        [Fact]
        public async Task SubmitResponse_ShouldReturnZero_ForEmptyFreeText()
        {
            var db = GetDbContext();

            var survey = new Survey { Title = "Free Text Empty", Description = "T" };
            var question = new Question
            {
                Text = "Leave blank?",
                Type = QuestionType.FreeText,
                Answers =
                [
                    new Answer { Text = "placeholder", Weight = 5 }
                ]
            };
            survey.Questions.Add(question);
            db.Surveys.Add(survey);
            await db.SaveChangesAsync();

            var controller = new ResponsesController(db);
            var request = new SubmitResponseRequest
            {
                SurveyId = survey.Id,
                FreeTextAnswers = new Dictionary<int, string>
                {
                    { question.Id, "" }
                }
            };

            var result = await controller.SubmitResponse(request) as OkObjectResult;
            result.Should().NotBeNull();

            result!.Value!.ToString().Should().Contain("0");
        }

        [Fact]
        public async Task SubmitResponse_ShouldReturnNotFound_WhenSurveyMissing()
        {
            var db = GetDbContext();
            var controller = new ResponsesController(db);

            var req = new SubmitResponseRequest
            {
                SurveyId = 999,
                SelectedAnswerIds = [1]
            };

            var result = await controller.SubmitResponse(req);
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
