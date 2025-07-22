using HorsesForCourses.Core.Models;

namespace HorsesForCourses.WebApi.Storage;

public class InMemoryCoachRepository
{
    private readonly Dictionary<Guid, Coach> _coaches = new();

    public void Add(Coach coach) => _coaches[coach.Id] = coach;

    public Coach? GetById(Guid id) =>
        _coaches.TryGetValue(id, out var coach) ? coach : null;

    public IEnumerable<Coach> GetAll() => _coaches.Values;
}
