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
}

