using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Response;
using Microsoft.AspNetCore.Mvc;

namespace CustomEd.Classroom.Service.Controllers
{
    [Route("api/classroom")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;

        public ClassroomController(IGenericRepository<Model.Classroom> classroomRepository)
        {
           _classroomRepository = classroomRepository;
        }

        [HttpGet]
        public async Task<ActionResult<SharedResponse<IEnumerable<Model.Classroom>>>> GetClassrooms()
        {
            var classrooms = await _classroomRepository.GetAllAsync();
            return Ok(SharedResponse<IEnumerable<Model.Classroom>>.Success(classrooms, null));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SharedResponse<Model.Classroom>>> GetClassroom(Guid id)
        {
            var classroom = await _classroomRepository.GetAsync(id);

            if (classroom == null)
            {
                return NotFound(SharedResponse<Model.Classroom>.Fail("No classroom with such id", null));
            }

            return Ok(SharedResponse<Model.Classroom>.Success(classroom, null));
        }

        [HttpPut]
        public async Task<ActionResult<SharedResponse<Model.Classroom>>> UpdateClassroom(Model.Classroom classroom)
        {
            var room = await _classroomRepository.GetAsync(classroom.Id);
            if (room == null)
            {
                return NotFound(SharedResponse<Model.Classroom>.Fail("No classroom with such id", null));
            }

            await _classroomRepository.UpdateAsync(classroom);
            return Ok(SharedResponse<Model.Classroom>.Success(classroom, null));
        }

        [HttpPost]
        public async Task<ActionResult<Model.Classroom>> CreateClassroom(Model.Classroom classroom)
        {
            await _classroomRepository.CreateAsync(classroom);
            return CreatedAtAction("GetClassroom", new { id = classroom.Id }, classroom);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassroom(Guid id)
        {
            var room = await _classroomRepository.GetAsync(id);
            if (room == null)
            {
                return NotFound(SharedResponse<Model.Classroom>.Fail("No classroom with such id", null));
            }
            await _classroomRepository.RemoveAsync(id);
            return NoContent();
        }
    }
}