using ISchool.Model;
using MongoDB.Driver;

namespace ISchool.Repositories;

public class DepartmentCoursesRepository: GenericRepository<DepartmentCourses>
{
     private readonly IMongoCollection<DepartmentCourses> _departmentCourses;

    public DepartmentCoursesRepository(IMongoDatabase database) : base(database)
    {
        _departmentCourses = database.GetCollection<DepartmentCourses>("DepartmentCourses");
    }
}
