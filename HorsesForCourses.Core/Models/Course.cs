using HorsesForCourses.Core.ValueObjects;
namespace HorsesForCourses.Core.Models;

public class Course
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public List<TimeSlot> TimeSlots { get; private set; } = new();
    public List<Skill> RequiredSkills { get; private set; } = new();
    public bool IsConfirmed { get; private set; }

    // Coach komt later
    // public Coach? AssignedCoach { get; private set; }

    public Course(string name, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Course name cannot be empty.");

        if (startDate > endDate)
            throw new ArgumentException("Start date must be before end date.");

        Id = Guid.NewGuid();
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void AddRequiredSkill(Skill skill)
    {
        if (IsConfirmed)
            throw new InvalidOperationException("Cannot modify required skills after course is confirmed.");

        if (!RequiredSkills.Contains(skill))
        {
            RequiredSkills.Add(skill);
        }
    }

    public void RemoveRequiredSkill(Skill skill)
    {
        if (IsConfirmed)
            throw new InvalidOperationException("Cannot modify required skills after course is confirmed.");

        RequiredSkills.Remove(skill);
    }

    public void AddTimeSlot(TimeSlot slot)
    {
        if (IsConfirmed)
            throw new InvalidOperationException("Cannot add time slots after course is confirmed.");

        if (TimeSlots.Any(existing =>
            existing.Day == slot.Day &&
            existing.Start == slot.Start &&
            existing.End == slot.End))
        {
            throw new InvalidOperationException("This time slot already exists.");
        }

        TimeSlots.Add(slot);
    }

    public void Confirm()
    {
        if (IsConfirmed)
            throw new InvalidOperationException("Course is already confirmed.");

        if (TimeSlots.Count == 0)
            throw new InvalidOperationException("Course must have at least one time slot before confirmation.");

        if (StartDate > EndDate)
            throw new InvalidOperationException("Start date must be before end date.");

        if (string.IsNullOrWhiteSpace(Name))
            throw new InvalidOperationException("Course must have a name.");

        IsConfirmed = true;
    }
}

