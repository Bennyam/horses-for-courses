namespace HorsesForCourses.Core.ValueObjects;

public class TimeSlot
{
    public DayOfWeek Day { get; }
    public TimeOnly Start { get; }
    public TimeOnly End { get; }

    public TimeSlot(DayOfWeek day, TimeOnly start, TimeOnly end)
    {
        if (start >= end)
            throw new ArgumentException("Start time must be before end time.");

        var officeStart = new TimeOnly(9, 0);
        var officeEnd = new TimeOnly(17, 0);

        if (start < officeStart || end > officeEnd)
            throw new ArgumentException("Time slot must be within office hours (09:00 - 17:00).");

        if (day is DayOfWeek.Saturday or DayOfWeek.Sunday)
            throw new ArgumentException("Lessons must be scheduled on weekdays only.");

        if (end - start < TimeSpan.FromHours(1))
            throw new ArgumentException("Time slot must be at least 1 hour long.");

        Day = day;
        Start = start;
        End = end;
    }
}
