namespace HorsesForCourses.WebApi.DTOs;

public class CreateCoachRequest
{
  public string Name { get; set; } = default!;
  public string Email { get; set; } = default!;
}