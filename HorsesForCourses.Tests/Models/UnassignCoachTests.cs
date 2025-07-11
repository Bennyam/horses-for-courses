using HorsesForCourses.Core.Models;
using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.Tests.Models;

public class UnassignCoachTests
{
    private static Course SetupCourseWithCoach(out Coach coach, out TimeSlot slot)
    {
        coach = new Coach("Ben", "ben@coach.com");
        coach.AddSkill(Skill.Agile);

        var course = new Course("Agile Course", new DateOnly(2025, 9, 1), new DateOnly(2025, 9, 30));
        slot = new TimeSlot(DayOfWeek.Thursday, new TimeOnly(10, 0), new TimeOnly(12, 0));
        course.AddRequiredSkill(Skill.Agile);
        course.AddTimeSlot(slot);
        course.Confirm();
        course.AssignCoach(coach);
        return course;
    }

    [Fact]
    public void UnassignCoach_ShouldRemoveCoachFromCourse_AndReleaseTimeSlots()
    {
        var course = SetupCourseWithCoach(out var coach, out var slot);

        course.UnassignCoach();

        Assert.Null(course.AssignedCoach);
        Assert.DoesNotContain(course, coach.AssignedCourses);
    }

    [Fact]
    public void UnassignCoach_WhenNoCoachAssigned_ShouldThrow()
    {
        var course = new Course("Unassigned Course", new DateOnly(2025, 10, 1), new DateOnly(2025, 10, 31));

        Assert.Throws<InvalidOperationException>(() => course.UnassignCoach());
    }
}
