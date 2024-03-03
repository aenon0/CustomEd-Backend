using ISchool.Model;
using MongoDB.Driver;

namespace ISchool.Repositories;

public class TeacherRepository : GenericRepository<Teacher>
{
    private readonly IMongoCollection<Teacher> _teacherCollection;

    public TeacherRepository(IMongoDatabase database) : base(database)
    {
        _teacherCollection = database.GetCollection<Teacher>("Teacher");
    }
}
