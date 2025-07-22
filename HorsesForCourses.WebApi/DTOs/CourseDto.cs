using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.WebApi.DTOs;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsConfirmed { get; set; }
    public List<Skill> RequiredSkills { get; set; } = new();
    public List<TimeSlot> TimeSlots { get; set; } = new();
    public Guid? AssignedCoachId { get; set; }
}
