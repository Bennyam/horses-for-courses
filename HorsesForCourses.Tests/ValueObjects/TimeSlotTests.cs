using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.Tests.ValueObjects;

public class TimeSlotTests
{
    [Fact]
    public void CreateTimeSlot_ValidWeekdayWithinOfficeHours_ShouldSucceed()
    {
        var slot = new TimeSlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));

        Assert.Equal(DayOfWeek.Monday, slot.Day);
        Assert.Equal(new TimeOnly(9, 0), slot.Start);
        Assert.Equal(new TimeOnly(11, 0), slot.End);
    }

    [Theory]
    [InlineData(DayOfWeek.Saturday)]
    [InlineData(DayOfWeek.Sunday)]
    public void CreateTimeSlot_OnWeekend_ShouldThrow(DayOfWeek day)
    {
        Assert.Throws<ArgumentException>(() =>
            new TimeSlot(day, new TimeOnly(10, 0), new TimeOnly(12, 0))
        );
    }

    [Fact]
    public void CreateTimeSlot_OutsideOfficeHours_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            new TimeSlot(DayOfWeek.Tuesday, new TimeOnly(8, 0), new TimeOnly(10, 0))
        );

        Assert.Throws<ArgumentException>(() =>
            new TimeSlot(DayOfWeek.Tuesday, new TimeOnly(16, 30), new TimeOnly(17, 30))
        );
    }

    [Fact]
    public void CreateTimeSlot_LessThanOneHour_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            new TimeSlot(DayOfWeek.Wednesday, new TimeOnly(14, 0), new TimeOnly(14, 45))
        );
    }

    [Fact]
    public void CreateTimeSlot_EndBeforeStart_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            new TimeSlot(DayOfWeek.Thursday, new TimeOnly(15, 0), new TimeOnly(14, 0))
        );
    }
}
