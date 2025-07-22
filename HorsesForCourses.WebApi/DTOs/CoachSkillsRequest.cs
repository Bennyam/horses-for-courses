using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.WebApi.DTOs;

public class CoachSkillsRequest
{
    public List<Skill> Add { get; set; } = new();
    public List<Skill> Remove { get; set; } = new();
}
