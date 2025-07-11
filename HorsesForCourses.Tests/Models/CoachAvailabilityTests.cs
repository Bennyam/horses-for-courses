using HorsesForCourses.Core.Models;
using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.Tests.Models;

public class CoachAvailabilityTests
{
    private static Course CreateCourse(string name, DateOnly start, DateOnly end, params TimeSlot[] slots)
    {
        var course = new Course(name, start, end);
        foreach (var slot in slots)
            course.AddTimeSlot(slot);
        course.Confirm();
        return course;
    }

    [Fact]
    public void IsAvailableFor_ShouldReturnTrue_WhenNoOverlap()
    {
        var coach = new Coach("Ben", "ben@example.com");

        var existingCourse = CreateCourse("Reeds gepland",
            new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 31),
            new TimeSlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(10, 0)));

        coach.AssignCourse(existingCourse);

        var newCourse = CreateCourse("Nieuwe cursus",
            new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 31),
            new TimeSlot(DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(11, 0)));

        Assert.True(coach.IsAvailableFor(newCourse));
    }

    [Fact]
    public void IsAvailableFor_ShouldReturnFalse_WhenOverlap()
    {
        var coach = new Coach("Ben", "ben@example.com");

        var existingCourse = CreateCourse("Reeds gepland",
            new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 31),
            new TimeSlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0)));

        coach.AssignCourse(existingCourse);

        var overlappingCourse = CreateCourse("Overlappend",
            new DateOnly(2025, 8, 15), new DateOnly(2025, 8, 31),
            new TimeSlot(DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(12, 0)));

        Assert.False(coach.IsAvailableFor(overlappingCourse));
    }

    [Fact]
    public void IsAvailableFor_ShouldReturnTrue_WhenDifferentDays()
    {
        var coach = new Coach("Ben", "ben@example.com");

        var existingCourse = CreateCourse("Maandagcursus",
            new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 31),
            new TimeSlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0)));

        coach.AssignCourse(existingCourse);

        var tuesdayCourse = CreateCourse("Dinsdagcursus",
            new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 31),
            new TimeSlot(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(11, 0)));

        Assert.True(coach.IsAvailableFor(tuesdayCourse));
    }

    [Fact]
    public void IsAvailableFor_ShouldReturnFalse_WhenExactSameSlotAndOverlap()
    {
        var coach = new Coach("Ben", "ben@example.com");

        var existingSlot = new TimeSlot(DayOfWeek.Wednesday, new TimeOnly(13, 0), new TimeOnly(15, 0));

        var course1 = CreateCourse("Cursus A",
            new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 31), existingSlot);

        var course2 = CreateCourse("Cursus B",
            new DateOnly(2025, 8, 10), new DateOnly(2025, 8, 20), existingSlot);

        coach.AssignCourse(course1);

        Assert.False(coach.IsAvailableFor(course2));
    }

    [Fact]
    public void IsAvailableFor_ShouldReturnTrue_WhenSameSlotButDifferentPeriod()
    {
        var coach = new Coach("Ben", "ben@example.com");

        var existingSlot = new TimeSlot(DayOfWeek.Friday, new TimeOnly(10, 0), new TimeOnly(12, 0));

        var course1 = CreateCourse("Voorjaar",
            new DateOnly(2025, 1, 1), new DateOnly(2025, 3, 31), existingSlot);

        var course2 = CreateCourse("Najaar",
            new DateOnly(2025, 4, 1), new DateOnly(2025, 6, 30), existingSlot);

        coach.AssignCourse(course1);

        Assert.True(coach.IsAvailableFor(course2));
    }
}
