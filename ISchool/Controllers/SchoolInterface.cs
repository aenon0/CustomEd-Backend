using System.Linq;
using ISchool.Model;
using ISchool.Repositories;
using ISchool.Response;
using Microsoft.AspNetCore.Mvc;

namespace ISchool.Controllers
{
    [ApiController]
    [Route("/schooldb/")]
    public class SchoolInterface : ControllerBase
    {
        private readonly StudentRepository _studentRepository; 
        private readonly TeacherRepository _teacherRepository;

        public SchoolInterface(StudentRepository studentRepository, TeacherRepository teacherRepository)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
        }

        //     new Student
        //     {
        //         Id = Guid.NewGuid(),
        //         Name = "Student1",
        //         Email = "student1@example.com",
        //         Phone = "1234567890",
        //         Address = "Address1",
        //         City = "City1",
        //         State = "State1",
        //         Country = "Country1",
        //         Zip = "Zip1",
        //         Section = "Section1",
        //         Year = 1,
        //         Department = Department.ComputerScience
        //     },

        //     new Teacher
        //     {
        //         Id = Guid.NewGuid(),
        //         Name = "Teacher1",
        //         Email = "teacher1@example.com",
        //         Phone = "1234567891",
        //         Address = "Address1",
        //         City = "City1",
        //         State = "State1",
        //         Country = "Country1",
        //         Zip = "Zip1",
        //         Department = Department.ComputerScience
        //     },
      

        [HttpGet("isStudent")]
        public async Task<ActionResult<SchoolResponse<bool>>> IsStudent([FromQuery] string email)
        {
            var student = await _studentRepository.Exists(email);
            if (!student)
            {
                return NotFound(new SchoolResponse<bool> { Data = false });
            }

            return Ok(new SchoolResponse<bool> { Data = true });
        }


        [HttpGet("isTeacher")]
        public async Task<ActionResult<SchoolResponse<bool>>> IsTeacher([FromQuery] string email)
        {
            var teacher = await _teacherRepository.Exists(email);
            if (!teacher)
            {
                return NotFound(new SchoolResponse<bool> { Data = false });
            }

            return Ok(new SchoolResponse<bool> { Data = true });
        }

        [HttpGet("getStudents")]
        public async Task<ActionResult<SchoolResponse<List<Student>>>> GetStudents(Department deb, int year, string section)
        {
            var students = await _studentRepository.GetPreferredStudents(deb, year, section);

            return Ok(new SchoolResponse<List<Student>> { Data = students });
        }
    }
}
