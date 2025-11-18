using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CAPApi.Controllers;
using CAPApi.Data;
using CAPApi.DataModels;
using CAPApi.APIModels;

namespace CAPApi.Tests.Controllers
{
    public class SurveysControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateSurvey_ShouldReturnCreatedSurvey_WhenValidRequest()
        {
            var db = GetDbContext();
            var controller = new SurveysController(db);

            var req = new CreateSurveyRequest
            {
                Title = "Customer Feedback",
                Description = "Q4 satisfaction survey"
            };

            var result = await controller.CreateSurvey(req) as CreatedAtActionResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeOfType<Survey>();
            var survey = (Survey)result.Value!;
            survey.Title.Should().Be("Customer Feedback");
            survey.Description.Should().Be("Q4 satisfaction survey");

            db.Surveys.Count().Should().Be(1);
        }

        [Fact]
        public async Task GetAllSurveys_ShouldReturnAllSurveys()
        {
            var db = GetDbContext();
            db.Surveys.AddRange(
                new Survey { Title = "S1", Description = "D1" },
                new Survey { Title = "S2", Description = "D2" }
            );
            await db.SaveChangesAsync();

            var controller = new SurveysController(db);

            var result = await controller.GetAllSurveys() as OkObjectResult;
            result.Should().NotBeNull();

            var surveys = result!.Value as List<Survey>;
            surveys.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetSurveyById_ShouldReturnSurvey_WhenExists()
        {
            var db = GetDbContext();
            var survey = new Survey { Title = "Demo", Description = "Testing" };
            db.Surveys.Add(survey);
            await db.SaveChangesAsync();

            var controller = new SurveysController(db);

            var result = await controller.GetSurveyById(survey.Id) as OkObjectResult;
            result.Should().NotBeNull();

            var returned = result!.Value as Survey;
            returned.Should().NotBeNull();
            returned!.Title.Should().Be("Demo");
        }

        [Fact]
        public async Task GetSurveyById_ShouldReturnNotFound_WhenSurveyDoesNotExist()
        {
            var db = GetDbContext();
            var controller = new SurveysController(db);

            var result = await controller.GetSurveyById(999);
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateSurvey_ShouldModifySurvey_WhenExists()
        {
            var db = GetDbContext();
            var survey = new Survey { Title = "Old", Description = "OldDesc" };
            db.Surveys.Add(survey);
            await db.SaveChangesAsync();

            var controller = new SurveysController(db);
            var req = new CreateSurveyRequest
            {
                Title = "New Title",
                Description = "New Description"
            };

            var result = await controller.UpdateSurvey(survey.Id, req);
            result.Should().BeOfType<NoContentResult>();

            var updated = await db.Surveys.FindAsync(survey.Id);
            updated!.Title.Should().Be("New Title");
            updated.Description.Should().Be("New Description");
        }

        [Fact]
        public async Task DeleteSurvey_ShouldRemoveSurvey_WhenExists()
        {
            var db = GetDbContext();
            var survey = new Survey { Title = "ToDelete", Description = "Desc" };
            db.Surveys.Add(survey);
            await db.SaveChangesAsync();

            var controller = new SurveysController(db);

            var result = await controller.DeleteSurvey(survey.Id);
            result.Should().BeOfType<NoContentResult>();

            db.Surveys.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteSurvey_ShouldReturnNotFound_WhenMissing()
        {
            var db = GetDbContext();
            var controller = new SurveysController(db);

            var result = await controller.DeleteSurvey(999);
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}