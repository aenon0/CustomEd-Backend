using ISchool.Model;
using MongoDB.Driver;

namespace ISchool.Repositories;

public class StudentRepository : GenericRepository<Student>
{
    private readonly IMongoCollection<Student> _studentCollection;

    public StudentRepository(IMongoDatabase database) : base(database)
    {
        _studentCollection = database.GetCollection<Student>("Student");
    }
    public async Task<List<Student>> GetPreferredStudents(Department department, int year, string section)
    {
        var filter = Builders<Student>.Filter.Eq(s => s.Department, department) &
                     Builders<Student>.Filter.Eq(s => s.Year, year) &
                     Builders<Student>.Filter.Eq(s => s.Section, section);

        var students = await _studentCollection.Find(filter).ToListAsync();
        return students;
    }
}
