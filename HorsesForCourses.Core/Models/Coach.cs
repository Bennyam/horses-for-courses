using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.Core.Models;

public class Coach
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    public List<Skill> Skills { get; private set; } = new();

    private readonly List<Course> _assignedCourses = new();
    public IReadOnlyCollection<Course> AssignedCourses => _assignedCourses.AsReadOnly();

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

    public bool IsSuitableFor(Course course)
    {
        return course.RequiredSkills.All(skill => Skills.Contains(skill));
    }

    public void AssignCourse(Course course)
    {
        _assignedCourses.Add(course);
    }

    public void UnassignCourse(Course course)
    {
        _assignedCourses.Remove(course);
    }

    public bool IsAvailableFor(Course course)
    {
        foreach (var assigned in _assignedCourses)
        {
            if (DatesOverlap(course, assigned))
            {
                foreach (var newSlot in course.TimeSlots)
                {
                    foreach (var existingSlot in assigned.TimeSlots)
                    {
                        if (newSlot.Day == existingSlot.Day &&
                            newSlot.Start < existingSlot.End &&
                            newSlot.End > existingSlot.Start)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    private bool DatesOverlap(Course a, Course b)
    {
        return a.StartDate <= b.EndDate && a.EndDate >= b.StartDate;
    }
}
