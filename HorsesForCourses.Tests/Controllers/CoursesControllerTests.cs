using HorsesForCourses.WebApi.Controllers;
using HorsesForCourses.WebApi.DTOs;
using HorsesForCourses.WebApi.Storage;
using HorsesForCourses.Core.Models;
using HorsesForCourses.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System;

namespace HorsesForCourses.Tests.Controllers;

public class CoursesControllerTests
{
  private readonly CoursesController _controller;
  private readonly InMemoryCourseRepository _courseRepository;
  private readonly InMemoryCoachRepository _coachRepository;

  public CoursesControllerTests()
  {
    _courseRepository = new InMemoryCourseRepository();
    _coachRepository = new InMemoryCoachRepository();
    _controller = new CoursesController(_courseRepository, _coachRepository);
  }

  [Fact]
  public void CreateCourse_ShouldReturnCreated_WhenValidRequest()
  {
    var request = new CreateCourseRequest
    {
      Name = "C# Fundamentals",
      StartDate = new DateOnly(2025, 9, 1),
      EndDate = new DateOnly(2025, 9, 30)
    };

    var result = _controller.CreateCourse(request);

    var createdResult = Assert.IsType<CreatedAtActionResult>(result);
    Assert.Equal(nameof(CoursesController.GetById), createdResult.ActionName);
    Assert.NotNull(createdResult.RouteValues);
    Assert.NotNull(createdResult.Value);
  }

  [Fact]
  public void CreateCourse_ShouldReturnBadRequest_WhenEndDateBeforeStartDate()
  {
    var request = new CreateCourseRequest
    {
      Name = "Cursus met foute data",
      StartDate = new DateOnly(2025, 10, 1),
      EndDate = new DateOnly(2025, 9, 1)
    };

    var result = _controller.CreateCourse(request);

    var badRequest = Assert.IsType<BadRequestObjectResult>(result);
    var problemDetails = Assert.IsType<ProblemDetails>(badRequest.Value);
    Assert.Equal(400, problemDetails.Status);
    Assert.Equal("Ongeldige input", problemDetails.Title);
  }

  [Fact]
  public void GetById_ShouldReturnCourse_WhenIdExists()
  {
    var course = new Course("Test Cursus", new DateOnly(2025, 9, 1), new DateOnly(2025, 9, 30));
    _courseRepository.Add(course);

    var result = _controller.GetById(course.Id);

    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var dto = Assert.IsType<CourseDto>(okResult.Value);

    Assert.Equal(course.Id, dto.Id);
    Assert.Equal(course.Name, dto.Name);
    Assert.Equal(course.StartDate, dto.StartDate);
    Assert.Equal(course.EndDate, dto.EndDate);
    Assert.False(dto.IsConfirmed);
    Assert.Empty(dto.RequiredSkills);
    Assert.Empty(dto.TimeSlots);
    Assert.Null(dto.AssignedCoachId);
  }

  [Fact]
  public void GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
  {
    var nonExistentId = Guid.NewGuid();

    var result = _controller.GetById(nonExistentId);

    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public void UpdateCourseSkills_ShouldReturnOk_WhenSkillsUpdatedCorrectly()
  {
    var course = new Course("SkillTest", new DateOnly(2025, 9, 1), new DateOnly(2025, 9, 30));
    _courseRepository.Add(course);

    var request = new CourseSkillsRequest
    {
      Add = new List<Skill> { Skill.Agile, Skill.Frontend },
      Remove = new List<Skill>()
    };

    var result = _controller.UpdateCourseSkills(course.Id, request);

    Assert.IsType<OkResult>(result);
    Assert.Contains(Skill.Agile, course.RequiredSkills);
    Assert.Contains(Skill.Frontend, course.RequiredSkills);
  }

  [Fact]
  public void UpdateCourseSkills_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    var fakeId = Guid.NewGuid();
    var request = new CourseSkillsRequest
    {
      Add = new List<Skill> { Skill.DevOps },
      Remove = new List<Skill>()
    };

    var result = _controller.UpdateCourseSkills(fakeId, request);

    var notFound = Assert.IsType<NotFoundObjectResult>(result);
    var details = Assert.IsType<ProblemDetails>(notFound.Value);
    Assert.Equal(404, details.Status);
    Assert.Equal("Cursus niet gevonden", details.Title);
  }


  [Fact]
  public void UpdateCourseSkills_ShouldIgnoreInvalidRemoval_AndReturnOk()
  {
    var course = new Course("SkillTest", new DateOnly(2025, 9, 1), new DateOnly(2025, 9, 30));
    _courseRepository.Add(course);

    var request = new CourseSkillsRequest
    {
      Add = new List<Skill>(),
      Remove = new List<Skill> { Skill.DotNet } // niet aanwezig, maar API accepteert dat
    };

    var result = _controller.UpdateCourseSkills(course.Id, request);

    Assert.IsType<OkResult>(result);
    Assert.DoesNotContain(Skill.DotNet, course.RequiredSkills);
  }

  [Fact]
  public void UpdateTimeSlots_ShouldReturnOk_WhenValidRequest()
  {
    var course = new Course("TijdslotTest", new DateOnly(2025, 9, 1), new DateOnly(2025, 9, 30));
    _courseRepository.Add(course);

    var slotDto = new TimeSlotDto
    {
      Day = DayOfWeek.Monday,
      Start = new TimeOnly(9, 0),
      End = new TimeOnly(11, 0)
    };

    var request = new CourseTimeSlotsRequest
    {
      Add = new List<TimeSlotDto> { slotDto },
      Remove = new List<TimeSlotDto>()
    };

    var result = _controller.UpdateTimeSlots(course.Id, request);

    Assert.IsType<OkResult>(result);
    Assert.Single(course.TimeSlots);
    var slot = course.TimeSlots.First();
    Assert.Equal(DayOfWeek.Monday, slot.Day);
    Assert.Equal(new TimeOnly(9, 0), slot.Start);
    Assert.Equal(new TimeOnly(11, 0), slot.End);
  }

  [Fact]
  public void UpdateTimeSlots_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    var fakeId = Guid.NewGuid();

    var slotDto = new TimeSlotDto
    {
      Day = DayOfWeek.Monday,
      Start = new TimeOnly(9, 0),
      End = new TimeOnly(11, 0)
    };

    var request = new CourseTimeSlotsRequest
    {
      Add = new List<TimeSlotDto> { slotDto },
      Remove = new List<TimeSlotDto>()
    };

    var result = _controller.UpdateTimeSlots(fakeId, request);

    var notFound = Assert.IsType<NotFoundObjectResult>(result);
    var details = Assert.IsType<ProblemDetails>(notFound.Value);
    Assert.Equal(404, details.Status);
    Assert.Equal("Cursus niet gevonden", details.Title);
  }

  [Fact]
  public void UpdateTimeSlots_ShouldReturnBadRequest_WhenSlotIsInvalid()
  {
    var course = new Course("InvalidSlot", new DateOnly(2025, 9, 1), new DateOnly(2025, 9, 30));
    _courseRepository.Add(course);

    var invalidSlotDto = new TimeSlotDto
    {
      Day = DayOfWeek.Tuesday,
      Start = new TimeOnly(14, 0),
      End = new TimeOnly(13, 0)
    };

    var request = new CourseTimeSlotsRequest
    {
      Add = new List<TimeSlotDto> { invalidSlotDto },
      Remove = new List<TimeSlotDto>()
    };

    var result = _controller.UpdateTimeSlots(course.Id, request);

    var badRequest = Assert.IsType<BadRequestObjectResult>(result);
    var details = Assert.IsType<ProblemDetails>(badRequest.Value);
    Assert.Equal(400, details.Status);
    Assert.Equal("Ongeldig tijdslot", details.Title);
  }

  [Fact]
  public void UpdateTimeSlots_ShouldIgnoreInvalidRemoval_AndReturnOk()
  {
    var course = new Course("RemoveIgnoreTest", new DateOnly(2025, 9, 1), new DateOnly(2025, 9, 30));
    _courseRepository.Add(course);

    var nonExistingSlotDto = new TimeSlotDto
    {
      Day = DayOfWeek.Wednesday,
      Start = new TimeOnly(10, 0),
      End = new TimeOnly(12, 0)
    };

    var request = new CourseTimeSlotsRequest
    {
      Add = new List<TimeSlotDto>(),
      Remove = new List<TimeSlotDto> { nonExistingSlotDto }
    };

    var result = _controller.UpdateTimeSlots(course.Id, request);

    Assert.IsType<OkResult>(result);
    Assert.Empty(course.TimeSlots);
  }

  [Fact]
  public void ConfirmCourse_ShouldReturnOk_WhenConfirmationIsSuccessful()
  {
    var course = new Course("Bevestigbare cursus", new DateOnly(2025, 10, 1), new DateOnly(2025, 10, 31));

    course.AddTimeSlot(new TimeSlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0)));

    _courseRepository.Add(course);

    var result = _controller.ConfirmCourse(course.Id);

    Assert.IsType<OkResult>(result);
    Assert.True(course.IsConfirmed);
  }

  [Fact]
  public void ConfirmCourse_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    var fakeId = Guid.NewGuid();

    var result = _controller.ConfirmCourse(fakeId);

    var notFound = Assert.IsType<NotFoundObjectResult>(result);
    var details = Assert.IsType<ProblemDetails>(notFound.Value);
    Assert.Equal(404, details.Status);
    Assert.Equal("Cursus niet gevonden", details.Title);
  }

  [Fact]
  public void ConfirmCourse_ShouldReturnBadRequest_WhenAlreadyConfirmed()
  {
    var course = new Course("Dubbel bevestigen", new DateOnly(2025, 10, 1), new DateOnly(2025, 10, 31));
    course.AddTimeSlot(new TimeSlot(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(11, 0)));

    _courseRepository.Add(course);

    var firstResult = _controller.ConfirmCourse(course.Id);
    Assert.IsType<OkResult>(firstResult);

    var secondResult = _controller.ConfirmCourse(course.Id);

    var badRequest = Assert.IsType<BadRequestObjectResult>(secondResult);
    var details = Assert.IsType<ProblemDetails>(badRequest.Value);
    Assert.Equal(400, details.Status);
    Assert.Equal("Kan cursus niet bevestigen", details.Title);
  }

  [Fact]
  public void AssignCoachToCourse_ShouldReturnBadRequest_WhenCourseIsNotEligible()
  {
    var course = new Course("CoachLinkTest", new DateOnly(2025, 9, 1), new DateOnly(2025, 9, 30));
    course.AddTimeSlot(new TimeSlot(DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(12, 0)));
    course.AddRequiredSkill(Skill.DevOps);
    course.Confirm();
    _courseRepository.Add(course);

    var coach = new Coach("Ben Ameryckx", "ben@example.com");
    _coachRepository.Add(coach);

    var request = new AssignCoachRequest
    {
      CoachId = coach.Id
    };

    var result = _controller.AssignCoachToCourse(course.Id, request);

    var badRequest = Assert.IsType<BadRequestObjectResult>(result);
    var details = Assert.IsType<ProblemDetails>(badRequest.Value);
    Assert.Equal(400, details.Status);
    Assert.Equal("Kan coach niet toewijzen", details.Title);
  }

  [Fact]
  public void AssignCoachToCourse_ShouldReturnNotFound_WhenCourseDoesNotExist()
  {
    var nonExistentCourseId = Guid.NewGuid();

    var coach = new Coach("CoachX", "x@example.com");
    _coachRepository.Add(coach);

    var request = new AssignCoachRequest
    {
      CoachId = coach.Id
    };

    var result = _controller.AssignCoachToCourse(nonExistentCourseId, request);

    var notFound = Assert.IsType<NotFoundObjectResult>(result);
    var details = Assert.IsType<ProblemDetails>(notFound.Value);
    Assert.Equal(404, details.Status);
    Assert.Equal("Cursus niet gevonden", details.Title);
  }
  
  [Fact]
  public void AssignCoachToCourse_ShouldReturnNotFound_WhenCoachDoesNotExist()
  {
    var course = new Course("MissingCoachTest", new DateOnly(2025, 10, 1), new DateOnly(2025, 10, 31));
    course.AddTimeSlot(new TimeSlot(DayOfWeek.Tuesday, new TimeOnly(13, 0), new TimeOnly(15, 0)));
    course.AddRequiredSkill(Skill.Agile);
    course.Confirm();
    _courseRepository.Add(course);

    var fakeCoachId = Guid.NewGuid();

    var request = new AssignCoachRequest
    {
        CoachId = fakeCoachId
    };

    var result = _controller.AssignCoachToCourse(course.Id, request);

    var notFound = Assert.IsType<NotFoundObjectResult>(result);
    var details = Assert.IsType<ProblemDetails>(notFound.Value);
    Assert.Equal(404, details.Status);
    Assert.Equal("Coach niet gevonden", details.Title);
  }
}

