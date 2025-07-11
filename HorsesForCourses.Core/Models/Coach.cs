using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.Core.Models;

public class Coach
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    public List<Skill> Skills { get; private set; } = new();

    public Coach(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Coach name cannot be empty.");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Coach email cannot be empty.");

        Id = Guid.NewGuid();
        Name = name;
        Email = email;
    }

    public void AddSkill(Skill skill)
    {
        if (!Skills.Contains(skill)) Skills.Add(skill);
    }

    public void RemoveSkill(Skill skill)
    {
        Skills.Remove(skill);
    }
}
