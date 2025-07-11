using HorsesForCourses.Core.Models;
using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.Tests.Models;

public class CourseTimeSlotRemovalTests
{
    [Fact]
    public void RemoveTimeSlot_BeforeConfirmation_ShouldRemove()
    {
        var course = new Course("RemoveTest", new DateOnly(2025, 9, 1), new DateOnly(2025, 9, 30));
        var slot = new TimeSlot(DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(12, 0));
        course.AddTimeSlot(slot);

        course.RemoveTimeSlot(slot);

        Assert.Empty(course.TimeSlots);
    }

    [Fact]
    public void RemoveTimeSlot_AfterConfirmation_ShouldThrow()
    {
        var course = new Course("RemoveTest", new DateOnly(2025, 9, 1), new DateOnly(2025, 9, 30));
        var slot = new TimeSlot(DayOfWeek.Tuesday, new TimeOnly(13, 0), new TimeOnly(15, 0));
        course.AddTimeSlot(slot);
        course.Confirm();

        Assert.Throws<InvalidOperationException>(() => course.RemoveTimeSlot(slot));
    }
}
