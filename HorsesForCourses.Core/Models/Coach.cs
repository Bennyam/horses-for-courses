using HorsesForCourses.Core.ValueObjects;

namespace HorsesForCourses.Core.Models;

public class Coach
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    public List<Skill> Skills { get; private set; } = new();
    private readonly List<TimeSlot> _assignedTimeSlots = new();
    public IReadOnlyCollection<TimeSlot> AssignedTimeSlots => _assignedTimeSlots.AsReadOnly();

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

    public void AssignCourseTimeSlots(IEnumerable<TimeSlot> timeSlots)
    {
        _assignedTimeSlots.AddRange(timeSlots);
    }

    public void RemoveTimeSlot(TimeSlot slot)
    {
        _assignedTimeSlots.RemoveAll(t => t.Day == slot.Day && t.Start == slot.Start && t.End == slot.End);
    }

    public bool IsAvailableFor(Course course)
    {
        foreach (var newSlot in course.TimeSlots)
        {
            foreach (var existingSlot in _assignedTimeSlots)
            {
                if (newSlot.Day == existingSlot.Day && newSlot.Start < existingSlot.End && newSlot.End > existingSlot.Start)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
