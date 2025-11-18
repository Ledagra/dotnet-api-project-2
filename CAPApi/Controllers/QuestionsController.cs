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
    public class QuestionsController(AppDbContext db) : ControllerBase
    {
        private readonly AppDbContext _db = db;

        [HttpPost]
        [SwaggerOperation(
            Summary = "Add a new question to a survey",
            Description = "Creates a new question for a specific survey, including optional answer choices."
        )]
        [SwaggerRequestExample(typeof(CreateQuestionRequest), typeof(APIExamples.CreateQuestionRequestExample))]
        [SwaggerResponse(201, "Question created successfully", typeof(Question))]
        [SwaggerResponse(404, "Survey not found")]
        [SwaggerResponse(400, "Invalid request data")]
        public async Task<IActionResult> AddQuestion([FromBody] CreateQuestionRequest req)
        {
            var survey = await _db.Surveys.FindAsync(req.SurveyId);
            if (survey == null)
                return NotFound($"Survey {req.SurveyId} not found.");

            var question = new Question
            {
                Text = req.Text,
                Type = req.Type,
                SurveyId = req.SurveyId
            };

            if (req.Answers != null && req.Answers.Any())
            {
                foreach (var ans in req.Answers)
                {
                    question.Answers.Add(new Answer
                    {
                        Text = ans.Text,
                        Weight = ans.Weight
                    });
                }
            }

            _db.Questions.Add(question);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestionById), new { id = question.Id }, question);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Retrieve a question by ID",
            Description = "Gets a single question, including its possible answers."
        )]
        [SwaggerResponse(200, "Question retrieved successfully", typeof(Question))]
        [SwaggerResponse(404, "Question not found")]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var question = await _db.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
                return NotFound($"Question {id} not found.");

            return Ok(question);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update an existing question",
            Description = "Updates a question’s text, type, and answer choices (if applicable)."
        )]
        [SwaggerRequestExample(typeof(CreateQuestionRequest), typeof(APIExamples.CreateQuestionRequestExample))]
        [SwaggerResponse(204, "Question updated successfully (no content returned)")]
        [SwaggerResponse(404, "Question not found")]
        [SwaggerResponse(400, "Invalid input data")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] CreateQuestionRequest req)
        {
            var question = await _db.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
                return NotFound($"Question {id} not found.");

            question.Text = req.Text;
            question.Type = req.Type;

            if (req.Answers != null)
            {
                _db.Answers.RemoveRange(question.Answers);
                question.Answers.Clear();

                foreach (var ans in req.Answers)
                {
                    question.Answers.Add(new Answer
                    {
                        Text = ans.Text,
                        Weight = ans.Weight
                    });
                }
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a question",
            Description = "Deletes a question and all its associated answers."
        )]
        [SwaggerResponse(204, "Question deleted successfully (no content returned)")]
        [SwaggerResponse(404, "Question not found")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _db.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
                return NotFound($"Question {id} not found.");

            _db.Answers.RemoveRange(question.Answers);
            _db.Questions.Remove(question);

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
