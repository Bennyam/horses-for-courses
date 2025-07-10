using HorsesForCourses.Core.Models;

namespace HorsesForCourses.Tests.Models;

public class CourseTests
{
    [Fact]
    public void CreateCourse_WithValidData_ShouldSucceed()
    {
        var course = new Course("C# Bootcamp", new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));

        Assert.Equal("C# Bootcamp", course.Name);
        Assert.Equal(new DateOnly(2025, 7, 1), course.StartDate);
        Assert.Equal(new DateOnly(2025, 7, 31), course.EndDate);
    }

    [Fact]
    public void CreateCourse_WithEmptyName_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            new Course("", new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31))
        );
    }

    [Fact]
    public void CreateCourse_WithEndDateBeforeStartDate_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            new Course("Agile Training", new DateOnly(2025, 7, 31), new DateOnly(2025, 7, 1))
        );
    }
}
