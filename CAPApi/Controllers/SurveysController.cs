using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CAPApi.Data;
using CAPApi.DataModels;
using CAPApi.APIModels;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace CAPApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveysController(AppDbContext db) : ControllerBase
    {
        private readonly AppDbContext _db = db;

        [HttpGet]
        [SwaggerOperation(
            Summary = "Retrieve all surveys",
            Description = "Gets a list of all surveys, including their questions and possible answers."
        )]
        [SwaggerResponse(200, "Successfully retrieved list of surveys", typeof(IEnumerable<Survey>))]
        public async Task<IActionResult> GetAllSurveys()
        {
            var surveys = await _db.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Answers)
                .ToListAsync();

            return Ok(surveys);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Retrieve a specific survey by ID",
            Description = "Fetches a single survey and its related questions and answers using the survey ID."
        )]
        [SwaggerResponse(200, "Survey retrieved successfully", typeof(Survey))]
        [SwaggerResponse(404, "Survey not found")]
        public async Task<IActionResult> GetSurveyById(int id)
        {
            var survey = await _db.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
                return NotFound($"Survey with ID {id} not found.");

            return Ok(survey);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new survey",
            Description = "Adds a new survey with a title and description."
        )]
        [SwaggerRequestExample(typeof(CreateSurveyRequest), typeof(APIExamples.CreateSurveyRequestExample))]
        [SwaggerResponseExample(201, typeof(CAPApi.Examples.CreateSurveyResponseExample))]
        [SwaggerResponse(201, "Survey created successfully", typeof(Survey))]
        [SwaggerResponse(400, "Invalid input data")]
        public async Task<IActionResult> CreateSurvey([FromBody] CreateSurveyRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var survey = new Survey
            {
                Title = req.Title,
                Description = req.Description
            };

            _db.Surveys.Add(survey);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSurveyById), new { id = survey.Id }, survey);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update an existing survey",
            Description = "Updates a survey’s title and description based on its ID."
        )]
        [SwaggerRequestExample(typeof(CreateSurveyRequest), typeof(APIExamples.CreateSurveyRequestExample))]
        [SwaggerResponse(204, "Survey updated successfully (no content returned)")]
        [SwaggerResponse(404, "Survey not found")]
        public async Task<IActionResult> UpdateSurvey(int id, [FromBody] CreateSurveyRequest req)
        {
            var survey = await _db.Surveys.FindAsync(id);
            if (survey == null)
                return NotFound($"Survey with ID {id} not found.");

            survey.Title = req.Title;
            survey.Description = req.Description;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a survey",
            Description = "Removes a survey and all of its associated questions and answers."
        )]
        [SwaggerResponse(204, "Survey deleted successfully (no content returned)")]
        [SwaggerResponse(404, "Survey not found")]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var survey = await _db.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
                return NotFound($"Survey with ID {id} not found.");

            _db.Answers.RemoveRange(survey.Questions.SelectMany(q => q.Answers));
            _db.Questions.RemoveRange(survey.Questions);
            _db.Surveys.Remove(survey);

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
