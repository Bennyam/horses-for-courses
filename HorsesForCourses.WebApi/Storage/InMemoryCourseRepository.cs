using HorsesForCourses.Core.Models;

namespace HorsesForCourses.WebApi.Storage;

public class InMemoryCourseRepository
{
    private readonly Dictionary<Guid, Course> _courses = new();

    public void Add(Course course) => _courses[course.Id] = course;

    public Course? GetById(Guid id) =>
        _courses.TryGetValue(id, out var course) ? course : null;

    public IEnumerable<Course> GetAll() => _courses.Values;
}
