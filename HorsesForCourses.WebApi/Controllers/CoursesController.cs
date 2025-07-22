using HorsesForCourses.Core.Models;
using HorsesForCourses.Core.ValueObjects;
using HorsesForCourses.WebApi.DTOs;
using HorsesForCourses.WebApi.Storage;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("courses")]
public class CoursesController : ControllerBase
{
    private readonly InMemoryCourseRepository _courseRepository;
    private readonly InMemoryCoachRepository _coachRepository;

    public CoursesController(InMemoryCourseRepository courseRepository, InMemoryCoachRepository coachRepository)
    {
        _courseRepository = courseRepository;
        _coachRepository = coachRepository;
    }

    [HttpPost]
    public ActionResult CreateCourse(CreateCourseRequest request)
    {
        try
        {
            var course = new Course(request.Name, request.StartDate, request.EndDate);
            _courseRepository.Add(course);

            return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Ongeldige input",
                Detail = ex.Message,
                Status = 400
            });
        }
    }

    [HttpGet("{id}")]
    public ActionResult<CourseDto> GetById(Guid id)
    {
        var course = _courseRepository.GetById(id);
        if (course is null) return NotFound();

        var dto = new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            StartDate = course.StartDate,
            EndDate = course.EndDate,
            IsConfirmed = course.IsConfirmed,
            RequiredSkills = course.RequiredSkills.ToList(),
            TimeSlots = course.TimeSlots.ToList(),
            AssignedCoachId = course.AssignedCoach?.Id
        };

        return Ok(dto);
    }

    [HttpPost("{id}/skills")]
    public ActionResult UpdateCourseSkills(Guid id, CourseSkillsRequest request)
    {
        var course = _courseRepository.GetById(id);
        if (course is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Cursus niet gevonden",
                Status = 404,
                Detail = $"Cursus met ID {id} bestaat niet."
            });
        }

        try
        {
            foreach (var skill in request.Add.Distinct())
                course.AddRequiredSkill(skill);

            foreach (var skill in request.Remove.Distinct())
                course.RemoveRequiredSkill(skill);

            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Bewerking niet toegestaan",
                Detail = ex.Message,
                Status = 400
            });
        }
    }

    [HttpPost("{id}/timeslots")]
    public ActionResult UpdateTimeSlots(Guid id, CourseTimeSlotsRequest request)
    {
        var course = _courseRepository.GetById(id);
        if (course is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Cursus niet gevonden",
                Status = 404,
                Detail = $"Cursus met ID {id} bestaat niet."
            });
        }

        try
        {
            foreach (var dto in request.Add.DistinctBy(t => (t.Day, t.Start, t.End)))
            {
                var slot = new TimeSlot(dto.Day, dto.Start, dto.End);
                course.AddTimeSlot(slot);
            }

            foreach (var dto in request.Remove.DistinctBy(t => (t.Day, t.Start, t.End)))
            {
                var slot = new TimeSlot(dto.Day, dto.Start, dto.End);
                course.RemoveTimeSlot(slot);
            }

            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Ongeldig tijdslot",
                Detail = ex.Message,
                Status = 400
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Bewerking niet toegestaan",
                Detail = ex.Message,
                Status = 400
            });
        }
    }

    [HttpPost("{id}/confirm")]
    public ActionResult ConfirmCourse(Guid id)
    {
        var course = _courseRepository.GetById(id);
        if (course is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Cursus niet gevonden",
                Status = 404,
                Detail = $"Cursus met ID {id} bestaat niet."
            });
        }

        try
        {
            course.Confirm();
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Kan cursus niet bevestigen",
                Detail = ex.Message,
                Status = 400
            });
        }
    }

    [HttpPost("{id}/assign-coach")]
    public ActionResult AssignCoachToCourse(Guid id, AssignCoachRequest request)
    {
        var course = _courseRepository.GetById(id);
        if (course is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Cursus niet gevonden",
                Status = 404,
                Detail = $"Cursus met ID {id} bestaat niet."
            });
        }

        var coach = _coachRepository.GetById(request.CoachId);
        if (coach is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Coach niet gevonden",
                Status = 404,
                Detail = $"Coach met ID {request.CoachId} bestaat niet."
            });
        }

        try
        {
            course.AssignCoach(coach);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Kan coach niet toewijzen",
                Detail = ex.Message,
                Status = 400
            });
        }
    }
}
