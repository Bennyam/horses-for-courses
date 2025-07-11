using HorsesForCourses.Core.Models;
using HorsesForCourses.Core.ValueObjects;
using System;
using Xunit;

namespace HorsesForCourses.Tests.Models;

public class CourseAssignCoachTests
{
    private static Course CreateConfirmedCourseWithSlotAndSkill(Skill skill, TimeSlot slot)
    {
        var course = new Course("Testcursus", new DateOnly(2025, 9, 1), new DateOnly(2025, 9, 30));
        course.AddRequiredSkill(skill);
        course.AddTimeSlot(slot);
        course.Confirm();
        return course;
    }

    [Fact]
    public void AssignCoach_WithAllConditionsMet_ShouldAssignAndRegisterTimeSlots()
    {
        var slot = new TimeSlot(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(12, 0));
        var course = CreateConfirmedCourseWithSlotAndSkill(Skill.Communication, slot);

        var coach = new Coach("Emma", "emma@coach.com");
        coach.AddSkill(Skill.Communication);

        course.AssignCoach(coach);

        Assert.Equal(coach, course.AssignedCoach);
        Assert.Single(coach.AssignedTimeSlots);
        Assert.Equal(slot, coach.AssignedTimeSlots.First());
    }

    [Fact]
    public void AssignCoach_ShouldThrow_WhenCoachHasOverlappingSlot()
    {
        var conflictingSlot = new TimeSlot(DayOfWeek.Wednesday, new TimeOnly(14, 0), new TimeOnly(16, 0));
        var coach = new Coach("Max", "max@coach.com");
        coach.AddSkill(Skill.DotNet);
        coach.AssignCourseTimeSlots(new[] { conflictingSlot });

        var newCourse = CreateConfirmedCourseWithSlotAndSkill(Skill.DotNet,
            new TimeSlot(DayOfWeek.Wednesday, new TimeOnly(15, 0), new TimeOnly(17, 0)));

        Assert.Throws<InvalidOperationException>(() => newCourse.AssignCoach(coach));
    }
}
