using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.WebApi.DTOs;

public class CoachDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public List<Skill> Skills { get; set; } = new();
    public List<Guid> AssignedCourseIds { get; set; } = new();
}
