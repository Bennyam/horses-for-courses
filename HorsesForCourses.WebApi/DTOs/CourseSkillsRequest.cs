using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.WebApi.DTOs;

public class CourseSkillsRequest
{
    public List<Skill> Add { get; set; } = new();
    public List<Skill> Remove { get; set; } = new();
}
