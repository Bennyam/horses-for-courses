namespace HorsesForCourses.WebApi.DTOs;

public class CourseTimeSlotsRequest
{
    public List<TimeSlotDto> Add { get; set; } = new();
    public List<TimeSlotDto> Remove { get; set; } = new();
}
