using HorsesForCourses.Core.Models;
using HorsesForCourses.Core.ValueObjects;
using System;
using Xunit;

namespace HorsesForCourses.Tests.Models;

public class CourseConfirmTests
{
    [Fact]
    public void ConfirmCourse_WithValidData_ShouldSetIsConfirmedTrue()
    {
        var course = new Course("Fullstack", new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31));
        course.AddTimeSlot(new TimeSlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0)));

        course.Confirm();

        Assert.True(course.IsConfirmed);
    }

    [Fact]
    public void ConfirmCourse_WithoutTimeSlots_ShouldThrow()
    {
        var course = new Course("No TimeSlots", new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31));

        Assert.Throws<InvalidOperationException>(() => course.Confirm());
    }

    [Fact]
    public void ConfirmCourse_Twice_ShouldThrow()
    {
        var course = new Course("Twice", new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31));
        course.AddTimeSlot(new TimeSlot(DayOfWeek.Friday, new TimeOnly(10, 0), new TimeOnly(12, 0)));
        course.Confirm();

        Assert.Throws<InvalidOperationException>(() => course.Confirm());
    }
}
