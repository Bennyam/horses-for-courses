using HorsesForCourses.Core.Models;
using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.Tests.Models;

public class CourseTimeSlotTests
{
    [Fact]
    public void AddTimeSlot_ValidSlot_ShouldAddToList()
    {
        var course = new Course("TDD", new DateOnly(2025, 1, 1), new DateOnly(2025, 2, 1));
        var slot = new TimeSlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));

        course.AddTimeSlot(slot);

        Assert.Single(course.TimeSlots);
        Assert.Equal(slot, course.TimeSlots.First());
    }

    [Fact]
    public void AddTimeSlot_Duplicate_ShouldThrow()
    {
        var course = new Course("TDD", new DateOnly(2025, 1, 1), new DateOnly(2025, 2, 1));
        var slot = new TimeSlot(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(12, 0));
        course.AddTimeSlot(slot);

        Assert.Throws<InvalidOperationException>(() => course.AddTimeSlot(slot));
    }

    [Fact]
    public void AddTimeSlot_WhenCourseIsConfirmed_ShouldThrow()
    {
        var course = new Course("TDD", new DateOnly(2025, 1, 1), new DateOnly(2025, 2, 1));
        var slot = new TimeSlot(DayOfWeek.Wednesday, new TimeOnly(13, 0), new TimeOnly(15, 0));

        typeof(Course).GetProperty("IsConfirmed")!.SetValue(course, true);

        Assert.Throws<InvalidOperationException>(() => course.AddTimeSlot(slot));
    }
}
