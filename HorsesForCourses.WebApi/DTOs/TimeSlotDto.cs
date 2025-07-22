namespace HorsesForCourses.WebApi.DTOs;

public class TimeSlotDto
{
    public DayOfWeek Day { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
}
