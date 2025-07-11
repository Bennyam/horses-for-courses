using HorsesForCourses.Core.Models;
using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.Tests.Models;

public class CoachTests
{
    [Fact]
    public void CreateCoach_WithValidData_ShouldSucceed()
    {
        var coach = new Coach("Mark", "mark@example.com");

        Assert.Equal("Mark", coach.Name);
        Assert.Equal("mark@example.com", coach.Email);
        Assert.Empty(coach.Skills);
    }

    [Fact]
    public void CreateCoach_WithEmptyName_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            new Coach("", "mail@example.com"));
    }

    [Fact]
    public void CreateCoach_WithEmptyEmail_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            new Coach("Coachy", ""));
    }

    [Fact]
    public void AddSkill_ShouldAdd_WhenNotAlreadyPresent()
    {
        var coach = new Coach("Anna", "anna@example.com");

        coach.AddSkill(Skill.Agile);

        Assert.Contains(Skill.Agile, coach.Skills);
    }

    [Fact]
    public void AddSkill_ShouldNotAddDuplicate()
    {
        var coach = new Coach("Sam", "sam@example.com");

        coach.AddSkill(Skill.Backend);
        coach.AddSkill(Skill.Backend);

        Assert.Single(coach.Skills);
    }

    [Fact]
    public void RemoveSkill_ShouldRemoveIfPresent()
    {
        var coach = new Coach("Lina", "lina@example.com");

        coach.AddSkill(Skill.DotNet);
        coach.RemoveSkill(Skill.DotNet);

        Assert.DoesNotContain(Skill.DotNet, coach.Skills);
    }

    [Fact]
    public void IsSuitableFor_ShouldReturnTrue_WhenAllSkillsMatch()
    {
        var coach = new Coach("Anna", "anna@example.com");
        coach.AddSkill(Skill.Frontend);
        coach.AddSkill(Skill.Backend);

        var course = new Course("Fullstack", new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31));
        course.AddRequiredSkill(Skill.Frontend);
        course.AddRequiredSkill(Skill.Backend);

        Assert.True(coach.IsSuitableFor(course));
    }

    [Fact]
    public void IsSuitableFor_ShouldReturnFalse_WhenCoachLacksASkill()
    {
        var coach = new Coach("Tom", "tom@example.com");
        coach.AddSkill(Skill.Frontend);

        var course = new Course("Fullstack", new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31));
        course.AddRequiredSkill(Skill.Frontend);
        course.AddRequiredSkill(Skill.Backend);

        Assert.False(coach.IsSuitableFor(course));
    }
}
