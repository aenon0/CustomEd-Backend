using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CustomEd.Classroom.Service.DTOs;
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
        private readonly IMapper _mapper;

        public ClassroomController(IGenericRepository<Model.Classroom> classroomRepository, IMapper mapper)
        {
           _classroomRepository = classroomRepository;
           _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<SharedResponse<IEnumerable<ClassroomDto>>>> GetAllRooms()
        {
            var classrooms = await _classroomRepository.GetAllAsync();
            var classroomDtos = _mapper.Map<IEnumerable<ClassroomDto>>(classrooms);
            return Ok(SharedResponse<IEnumerable<ClassroomDto>>.Success(classroomDtos, null));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SharedResponse<ClassroomDto>>> GetRoomById(Guid id)
        {
            var classroom = await _classroomRepository.GetAsync(id);

            if (classroom == null)
            {
                return NotFound(SharedResponse<Model.Classroom>.Fail("No classroom with such id", null));
            }

            var dto = _mapper.Map<ClassroomDto>(classroom);

            return Ok(SharedResponse<ClassroomDto>.Success(dto, null));
        }

        [HttpPut]
        public async Task<ActionResult<SharedResponse<ClassroomDto>>> UpdateClassroom(Model.Classroom classroom)
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
        public async Task<ActionResult<ClassroomDto>> CreateClassroom(Model.Classroom classroom)
        {
            await _classroomRepository.CreateAsync(classroom);
            return CreatedAtAction("GetRoomById", new { id = classroom.Id }, classroom);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveRoom(Guid id)
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