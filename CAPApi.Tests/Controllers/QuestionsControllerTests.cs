using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CAPApi.Controllers;
using CAPApi.Data;
using CAPApi.DataModels;
using CAPApi.APIModels;

namespace CAPApi.Tests.Controllers
{
    public class QuestionsControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddQuestion_ShouldAddQuestion_WhenSurveyExists()
        {
            var db = GetDbContext();
            var survey = new Survey { Title = "Customer Survey", Description = "Desc" };
            db.Surveys.Add(survey);
            await db.SaveChangesAsync();

            var controller = new QuestionsController(db);
            var req = new CreateQuestionRequest
            {
                SurveyId = survey.Id,
                Text = "How satisfied are you?",
                Type = QuestionType.SingleChoice,
                Answers =
                [
                    new AnswerObject { Text = "Good", Weight = 5 },
                    new AnswerObject { Text = "Bad", Weight = 1 }
                ]
            };

            var result = await controller.AddQuestion(req) as CreatedAtActionResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeOfType<Question>();
            var q = (Question)result.Value!;
            q.Text.Should().Be("How satisfied are you?");
            q.Answers.Should().HaveCount(2);

            db.Questions.Count().Should().Be(1);
            db.Answers.Count().Should().Be(2);
        }

        [Fact]
        public async Task AddQuestion_ShouldReturnNotFound_WhenSurveyDoesNotExist()
        {
            var db = GetDbContext();
            var controller = new QuestionsController(db);

            var req = new CreateQuestionRequest
            {
                SurveyId = 999,
                Text = "Missing survey test",
                Type = QuestionType.SingleChoice
            };

            var result = await controller.AddQuestion(req);
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task DeleteQuestion_ShouldRemoveQuestion_WhenExists()
        {
            var db = GetDbContext();
            var survey = new Survey { Title = "Delete Q Test", Description = "Desc" };
            var question = new Question
            {
                Text = "Will it delete?",
                Type = QuestionType.SingleChoice,
                Survey = survey,
                Answers =
                [
                    new() { Text = "Yes", Weight = 1 },
                    new() { Text = "No", Weight = 0 }
                ]
            };
            db.Surveys.Add(survey);
            db.Questions.Add(question);
            await db.SaveChangesAsync();

            var controller = new QuestionsController(db);

            var result = await controller.DeleteQuestion(question.Id);
            result.Should().BeOfType<NoContentResult>();

            db.Questions.Should().BeEmpty();
            db.Answers.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteQuestion_ShouldReturnNotFound_WhenMissing()
        {
            var db = GetDbContext();
            var controller = new QuestionsController(db);

            var result = await controller.DeleteQuestion(12345);
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
