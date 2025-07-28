using HorsesForCourses.Core.Models;
using HorsesForCourses.WebApi.DTOs;
using HorsesForCourses.WebApi.Storage;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("coaches")]
public class CoachesController : ControllerBase
{
    private readonly InMemoryCoachRepository _repository;

    public CoachesController(InMemoryCoachRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public ActionResult RegisterCoach(CreateCoachRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
        {
            return Problem(
                title: "Ongeldige aanvraag",
                detail: "Naam en e-mail mogen niet leeg zijn.",
                statusCode: 400
            );
        }

        var coach = new Coach(request.Name, request.Email);
        _repository.Add(coach);

        var dto = new CoachDto
        {
            Id = coach.Id,
            Name = coach.Name,
            Email = coach.Email,
            Skills = coach.Skills.ToList(),
            AssignedCourseIds = coach.AssignedCourses.Select(c => c.Id).ToList()
        };
        return CreatedAtAction(nameof(GetById), new { id = coach.Id }, dto);
    }

    [HttpGet("{id}")]
    public ActionResult<CoachDto> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        if (coach is null) return NotFound();

        var dto = new CoachDto
        {
            Id = coach.Id,
            Name = coach.Name,
            Email = coach.Email,
            Skills = coach.Skills.ToList(),
            AssignedCourseIds = coach.AssignedCourses.Select(c => c.Id).ToList()
        };

        return Ok(dto);
    }

    [HttpPost("{id}/skills")]
    public ActionResult UpdateCoachSkills(Guid id, CoachSkillsRequest request)
    {
        var coach = _repository.GetById(id);
        if (coach is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Coach niet gevonden",
                Status = 404,
                Detail = $"Coach met ID {id} bestaat niet."
            });
        }

        foreach (var skill in request.Add.Distinct())
            coach.AddSkill(skill);

        foreach (var skill in request.Remove.Distinct())
            coach.RemoveSkill(skill);

        return Ok();
    }
}
