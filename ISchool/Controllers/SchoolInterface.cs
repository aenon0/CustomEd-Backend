using System.Linq;
using ISchool.Model;
using ISchool.Response;
using Microsoft.AspNetCore.Mvc;

namespace ISchool.Controllers
{
    [ApiController]
    [Route("/schooldb/")]
    public class SchoolInterface : ControllerBase
    {
        private static List<Student> studentdb = new List<Student>
        {
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Student1",
                Email = "student1@example.com",
                Phone = "1234567890",
                Address = "Address1",
                City = "City1",
                State = "State1",
                Country = "Country1",
                Zip = "Zip1",
                Section = "Section1",
                Year = 1,
                Department = Department.ComputerScience
            },
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Student2",
                Email = "student2@example.com",
                Phone = "1234567891",
                Address = "Address2",
                City = "City2",
                State = "State2",
                Country = "Country2",
                Zip = "Zip2",
                Section = "Section2",
                Year = 2,
                Department = Department.ComputerScience
            },
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Student3",
                Email = "student3@example.com",
                Phone = "1234567892",
                Address = "Address3",
                City = "City3",
                State = "State3",
                Country = "Country3",
                Zip = "Zip3",
                Section = "Section3",
                Year = 3,
                Department = Department.ComputerScience
            },
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Student4",
                Email = "student4@example.com",
                Phone = "1234567893",
                Address = "Address4",
                City = "City4",
                State = "State4",
                Country = "Country4",
                Zip = "Zip4",
                Section = "Section4",
                Year = 4,
                Department = Department.ComputerScience
            },
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Student5",
                Email = "student5@example.com",
                Phone = "1234567894",
                Address = "Address5",
                City = "City5",
                State = "State5",
                Country = "Country5",
                Zip = "Zip5",
                Section = "Section5",
                Year = 5,
                Department = Department.ComputerScience
            },
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Student6",
                Email = "student6@example.com",
                Phone = "1234567895",
                Address = "Address6",
                City = "City6",
                State = "State6",
                Country = "Country6",
                Zip = "Zip6",
                Section = "Section6",
                Year = 6,
                Department = Department.ComputerScience
            },
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Student7",
                Email = "student7@example.com",
                Phone = "1234567896",
                Address = "Address7",
                City = "City7",
                State = "State7",
                Country = "Country7",
                Zip = "Zip7",
                Section = "Section7",
                Year = 7,
                Department = Department.ComputerScience
            },
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Student8",
                Email = "student8@example.com",
                Phone = "1234567897",
                Address = "Address8",
                City = "City8",


                State = "State8",
                Country = "Country8",
                Zip = "Zip8",
                Section = "Section8",
                Year = 8,
                Department = Department.ComputerScience
            },
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Student9",
                Email = "student9@example.com",
                Phone = "1234567898",
                Address = "Address9",
                City = "City9",
                State = "State9",
                Country = "Country9",
                Zip = "Zip9",
                Section = "Section9",
                Year = 9,
                Department = Department.ComputerScience
            },
            new Student
            {
                Id = Guid.NewGuid(),
                Name = "Student10",
                Email = "student10@example.com",
                Phone = "1234567899",
                Address = "Address10",
                City = "City10",
                State = "State10",
                Country = "Country10",
                Zip = "Zip10",
                Section = "Section10",
                Year = 10,
                Department = Department.ComputerScience
            }
        };


        private static List<Teacher> teacherdb = new List<Teacher>
        {
            new Teacher
            {
                Id = Guid.NewGuid(),
                Name = "Teacher1",
                Email = "teacher1@example.com",
                Phone = "1234567891",
                Address = "Address1",
                City = "City1",
                State = "State1",
                Country = "Country1",
                Zip = "Zip1",
                Department = Department.ComputerScience
            },
            new Teacher
            {
                Id = Guid.NewGuid(),
                Name = "Teacher2",
                Email = "teacher2@example.com",
                Phone = "1234567892",
                Address = "Address2",
                City = "City2",
                State = "State2",
                Country = "Country2",
                Zip = "Zip2",
                Department = Department.ComputerScience
            },
            // ... Add more teachers as needed
        };

        [HttpGet("isStudent")]
        public ActionResult<SchoolResponse<bool>> IsStudent([FromQuery] string email)
        {
            var student = studentdb.FirstOrDefault(s => s.Email == email);
            if (student == null)
            {
                return NotFound(new SchoolResponse<bool> { Data = false });
            }

            return Ok(new SchoolResponse<bool> { Data = true });
        }


        [HttpGet("isTeacher")]
        public ActionResult<SchoolResponse<bool>> IsTeacher([FromQuery] string email)
        {
            var teacher = teacherdb.FirstOrDefault(s => s.Email == email);
            if (teacher == null)
            {
                return NotFound(new SchoolResponse<bool> { Data = false });
            }

            return Ok(new SchoolResponse<bool> { Data = true });
        }

        [HttpGet("getStudents")]
        public ActionResult<SchoolResponse<List<Student>>> GetStudents(Request.Request req)
        {
            var students = studentdb.Where(s => s.Department == req.Deb && s.Year == req.Year && s.Section == req.Sec).ToList();
            if (students.Count == 0)
            {
                return NotFound(new SchoolResponse<List<Student>> { Data = null });
            }

            return Ok(new SchoolResponse<List<Student>> { Data = students });
        }
    }
}
