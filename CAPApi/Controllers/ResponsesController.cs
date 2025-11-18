using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CAPApi.Data;
using CAPApi.DataModels;
using CAPApi.APIModels;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;

namespace CAPApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponsesController(AppDbContext db) : ControllerBase
    {
        private readonly AppDbContext _db = db;

        [HttpPost]
        [SwaggerOperation(
            Summary = "Submit a survey response",
            Description = "Records user responses for a specific survey, including selected answers and free-text responses, and calculates a total score based on the selected answers' weights."
        )]
        [SwaggerRequestExample(typeof(SubmitResponseRequest), typeof(APIExamples.SubmitResponseRequestExample))]
        [SwaggerResponseExample(200, typeof(APIExamples.SubmitResponseResponseExample))]
        [SwaggerResponse(200, "Response submitted successfully and score calculated", typeof(object))]
        [SwaggerResponse(400, "Invalid or missing data in request")]
        [SwaggerResponse(404, "Survey or answers not found")]
        public async Task<IActionResult> SubmitResponse([FromBody] SubmitResponseRequest req)
        {
            var survey = await _db.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(s => s.Id == req.SurveyId);

            if (survey == null)
                return NotFound($"Survey {req.SurveyId} not found.");

            double totalScore = 0;

            foreach (var question in survey.Questions)
            {
                if (question.Type == QuestionType.FreeText)
                {
                    if (req.FreeTextAnswers != null &&
                        req.FreeTextAnswers.TryGetValue(question.Id, out var answerText))
                    {
                        if (!string.IsNullOrWhiteSpace(answerText))
                            totalScore += question.Answers.FirstOrDefault()?.Weight ?? 0;
                    }
                }
                else if (question.Type == QuestionType.MultipleChoice)
                {
                    var selectedAnswers = question.Answers
                        .Where(a => req.SelectedAnswerIds.Contains(a.Id))
                        .ToList();

                    if (selectedAnswers.Any())
                        totalScore += selectedAnswers.Max(a => a.Weight);
                }
                else
                {
                    var selectedAnswer = question.Answers
                        .FirstOrDefault(a => req.SelectedAnswerIds.Contains(a.Id));

                    if (selectedAnswer != null)
                        totalScore += selectedAnswer.Weight;
                }
            }

            var response = new Response
            {
                SurveyId = req.SurveyId,
                SurveyTitle = survey.Title, 
                SelectedAnswerIds = req.SelectedAnswerIds,
                TotalScore = totalScore
            };

            if (req.FreeTextAnswers != null && req.FreeTextAnswers.Count > 0)
            {
                var json = JsonSerializer.Serialize(req.FreeTextAnswers);
                typeof(Response).GetProperty("FreeTextResponsesJson")?.SetValue(response, json);
            }

            _db.Responses.Add(response);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "Response submitted successfully",
                req.SurveyId,
                TotalScore = totalScore
            });
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Retrieve all recorded responses with answer text",
            Description = "Returns all responses with the associated survey title and answer text for each selected answer."
        )]
        [SwaggerResponse(200, "Successfully retrieved all responses", typeof(IEnumerable<object>))]
        public async Task<IActionResult> GetAllResponses()
        {
            var responses = await _db.Responses
                .Include(r => r.Survey)
                .ToListAsync();

            var allAnswers = await _db.Answers.ToListAsync();

            var result = responses.Select(r => new
            {
                r.Id,
                r.SurveyTitle,
                r.SurveyId,
                r.TotalScore,
                SelectedAnswers = r.SelectedAnswerIds
                    .Select(id => allAnswers.FirstOrDefault(a => a.Id == id)?.Text ?? $"(ID {id})")
                    .ToList(),
                FreeText = r.FreeTextResponsesJson != null
                    ? string.Join("; ",
                        JsonSerializer
                            .Deserialize<Dictionary<int, string>>(r.FreeTextResponsesJson)?
                            .Values
                            .Where(v => !string.IsNullOrWhiteSpace(v))
                        ?? [])
                    : null
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Retrieve a specific survey response",
            Description = "Fetches a single survey response and its total score by response ID."
        )]
        [SwaggerResponse(200, "Response retrieved successfully", typeof(Response))]
        [SwaggerResponse(404, "Response not found")]
        public async Task<IActionResult> GetResponseById(int id)
        {
            var response = await _db.Responses.FindAsync(id);
            if (response == null)
                return NotFound($"Response {id} not found.");

            return Ok(response);
        }
    }
}