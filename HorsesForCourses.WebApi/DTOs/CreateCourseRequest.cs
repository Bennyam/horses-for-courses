namespace HorsesForCourses.WebApi.DTOs;

public class CreateCourseRequest
{
    public string Name { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
