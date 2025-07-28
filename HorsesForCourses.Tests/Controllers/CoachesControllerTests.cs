using HorsesForCourses.WebApi.Controllers;
using HorsesForCourses.WebApi.DTOs;
using HorsesForCourses.WebApi.Storage;
using HorsesForCourses.Core.Models;
using HorsesForCourses.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.Tests.Controllers;

public class CoachesControllerTests
{
  [Fact]
  public void CreateCoach_ReturnsCreatedResult_WhenRequestIsValid()
  {
    var repo = new InMemoryCoachRepository();
    var controller = new CoachesController(repo);

    var request = new CreateCoachRequest
    {
      Name = "Ben Ameryckx",
      Email = "ben@example.com"
    };

    var result = controller.RegisterCoach(request);

    var created = Assert.IsType<CreatedAtActionResult>(result);
    var coachDto = Assert.IsType<CoachDto>(created.Value);

    Assert.Equal(request.Name, coachDto.Name);
    Assert.Equal(request.Email, coachDto.Email);
    Assert.NotEqual(Guid.Empty, coachDto.Id);
  }

  [Fact]
  public void UpdateCoachSkills_AddsSkillsToCoach_WhenValid()
  {
    var repo = new InMemoryCoachRepository();
    var controller = new CoachesController(repo);

    var coach = new Coach("Ben", "ben@example.com");
    repo.Add(coach);

    var request = new CoachSkillsRequest
    {
      Add = new List<Skill> { Skill.Agile, Skill.DevOps },
      Remove = new List<Skill>()
    };

    var result = controller.UpdateCoachSkills(coach.Id, request);

    var okResult = Assert.IsType<OkResult>(result);
    var updatedCoach = repo.GetById(coach.Id);
    Assert.Contains(Skill.Agile, updatedCoach!.Skills);

    Assert.Contains(Skill.Agile, updatedCoach.Skills);
    Assert.Contains(Skill.DevOps, updatedCoach.Skills);
    Assert.Equal(2, updatedCoach.Skills.Count);
  }

  [Fact]
  public void UpdateCoachSkills_RemovesSkillsFromCoach_WhenValid()
  {
    var repo = new InMemoryCoachRepository();
    var controller = new CoachesController(repo);

    var coach = new Coach("Ben", "ben@example.com");
    coach.AddSkill(Skill.Agile);
    coach.AddSkill(Skill.DevOps);
    repo.Add(coach);

    var request = new CoachSkillsRequest
    {
      Add = new List<Skill>(),
      Remove = new List<Skill> { Skill.DevOps }
    };

    var result = controller.UpdateCoachSkills(coach.Id, request);

    var okResult = Assert.IsType<OkResult>(result);
    var updatedCoach = repo.GetById(coach.Id);

    Assert.DoesNotContain(Skill.DevOps, updatedCoach!.Skills);
    Assert.Contains(Skill.Agile, updatedCoach.Skills);
    Assert.Single(updatedCoach.Skills);
  }

  [Fact]
  public void UpdateCoachSkills_ReplacesSkillsCorrectly_WhenAddAndRemoveAreUsed()
  {
    var repo = new InMemoryCoachRepository();
    var controller = new CoachesController(repo);

    var coach = new Coach("Ben", "ben@example.com");
    coach.AddSkill(Skill.Frontend);
    coach.AddSkill(Skill.Backend);
    repo.Add(coach);

    var request = new CoachSkillsRequest
    {
      Add = new List<Skill> { Skill.DevOps, Skill.Security },
      Remove = new List<Skill> { Skill.Frontend }
    };

    var result = controller.UpdateCoachSkills(coach.Id, request);

    var okResult = Assert.IsType<OkResult>(result);
    var updatedCoach = repo.GetById(coach.Id)!;

    Assert.DoesNotContain(Skill.Frontend, updatedCoach.Skills);
    Assert.Contains(Skill.Backend, updatedCoach.Skills);
    Assert.Contains(Skill.DevOps, updatedCoach.Skills);
    Assert.Contains(Skill.Security, updatedCoach.Skills);
    Assert.Equal(3, updatedCoach.Skills.Count);
  }

  [Fact]
  public void UpdateCoachSkills_ReturnsNotFound_WhenCoachDoesNotExist()
  {
    var repo = new InMemoryCoachRepository();
    var controller = new CoachesController(repo);

    var nonExistentCoachId = Guid.NewGuid();

    var request = new CoachSkillsRequest
    {
      Add = new List<Skill> { Skill.DevOps },
      Remove = new List<Skill>()
    };

    var result = controller.UpdateCoachSkills(nonExistentCoachId, request);

    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    var problem = Assert.IsType<ProblemDetails>(notFoundResult.Value);
    
    Assert.Equal(404, problem.Status);
    Assert.Equal("Coach niet gevonden", problem.Title);
    Assert.StartsWith("Coach met ID", problem.Detail);
  }
}
