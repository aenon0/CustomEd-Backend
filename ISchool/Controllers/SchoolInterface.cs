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
        private readonly DepartmentCoursesRepository _departmentCoursesRepository;

        public SchoolInterface(StudentRepository studentRepository, TeacherRepository teacherRepository, DepartmentCoursesRepository departmentCoursesRepository)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _departmentCoursesRepository = departmentCoursesRepository;
        }
      

        // [HttpGet("isStudent")]
        // public async Task<ActionResult<SchoolResponse<bool>>> IsStudent([FromQuery] string email)
        // {
        //     var student = await _studentRepository.Exists(email);
        //     if (!student)
        //     {
        //         return Ok(new SchoolResponse<bool> { Data = false });
        //     }

        //     return Ok(new SchoolResponse<bool> { Data = true });
        // }
        
        [HttpGet("getStudentInfo")]
        public async Task<ActionResult<SchoolResponse<Student>>> GetStudentInfo([FromQuery] string email)
        {
            email = email.ToLower(); 
            var student = await _studentRepository.Get(s => s.Email == email);
            return Ok(new SchoolResponse<Student> { Data = student });
        }

        // [HttpGet("isTeacher")]
        // public async Task<ActionResult<SchoolResponse<bool>>> IsTeacher([FromQuery] string email)
        // {
        //     var teacher = await _teacherRepository.Exists(email);
        //     if (!teacher)
        //     {
        //         return Ok(new SchoolResponse<bool> { Data = false });
        //     }

        //     return Ok(new SchoolResponse<bool> { Data = true });
        // }

        [HttpGet("getTeacherInfo")]
        public async Task<ActionResult<SchoolResponse<Teacher>>> GetTeacherInfo([FromQuery] string email)
        {
            email = email.ToLower(); 
            var teacher = await _teacherRepository.Get(s => s.Email == email);
            return Ok(new SchoolResponse<Teacher> { Data = teacher });
        }

        [HttpGet("getStudents")]
        public async Task<ActionResult<SchoolResponse<List<Student>>>> GetStudents(Department department, int year, string section)
        {
            section = section.ToLower();
            var students = await _studentRepository.GetPreferredStudents(department, year, section);
            return Ok(new SchoolResponse<List<Student>> { Data = students });
        }
        [HttpGet("getDepartmentCourses")]
        public async Task<ActionResult<SchoolResponse<List<String>>>> GetDepartmentCourses(Department department)
        {
            var departmentCourses = await _departmentCoursesRepository.Get(i => i.Department == department);
            Console.WriteLine($"{department}");
            
            return Ok(new SchoolResponse<List<String>> { Data = departmentCourses?.Courses });
        }
    
    }
}
