using HorsesForCourses.Core.Models;
using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.Tests.Models;

public class CoachAvailabilityTests
{
    [Fact]
    public void IsAvailableFor_ShouldReturnTrue_WhenNoOverlap()
    {
        var coach = new Coach("Ben", "ben@example.com");
        coach.AssignCourseTimeSlots(new[]
        {
            new TimeSlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(10, 0))
        });

        var course = new Course("Nieuwe cursus", new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 31));
        course.AddTimeSlot(new TimeSlot(DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(11, 0)));

        Assert.True(coach.IsAvailableFor(course));
    }

    [Fact]
    public void IsAvailableFor_ShouldReturnFalse_WhenOverlap()
    {
        var coach = new Coach("Ben", "ben@example.com");
        coach.AssignCourseTimeSlots(new[]
        {
            new TimeSlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0))
        });

        var course = new Course("Overlappende cursus", new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 31));
        course.AddTimeSlot(new TimeSlot(DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(12, 0)));

        Assert.False(coach.IsAvailableFor(course));
    }

    [Fact]
    public void IsAvailableFor_ShouldReturnTrue_WhenDifferentDays()
    {
        var coach = new Coach("Ben", "ben@example.com");
        coach.AssignCourseTimeSlots(new[]
        {
            new TimeSlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0))
        });

        var course = new Course("Andere dag", new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 31));
        course.AddTimeSlot(new TimeSlot(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(11, 0)));

        Assert.True(coach.IsAvailableFor(course));
    }

    [Fact]
    public void IsAvailableFor_ShouldReturnFalse_WhenExactSameSlot()
    {
        var coach = new Coach("Ben", "ben@example.com");
        var existingSlot = new TimeSlot(DayOfWeek.Wednesday, new TimeOnly(13, 0), new TimeOnly(15, 0));
        coach.AssignCourseTimeSlots(new[] { existingSlot });

        var course = new Course("Exact conflict", new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 31));
        course.AddTimeSlot(existingSlot);

        Assert.False(coach.IsAvailableFor(course));
    }
}
