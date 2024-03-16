using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CustomEd.Classroom.Service.DTOs;
using CustomEd.Classroom.Service.DTOs.Validation;
using CustomEd.Classroom.Service.Search.Service;
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
        private readonly IGenericRepository<Model.Teacher> _teacherRepository;
        private readonly IGenericRepository<Model.Student> _studentRepository;
        private readonly IMapper _mapper;
        public ClassroomController(IGenericRepository<Model.Classroom> classroomRepository, IMapper mapper, IGenericRepository<Model.Teacher> teacherRepository, IGenericRepository<Model.Student> studentRepository)
        {
            _classroomRepository = classroomRepository;
            _mapper = mapper;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
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
        public async Task<ActionResult<SharedResponse<ClassroomDto>>> UpdateClassroom(UpdateClassroomDto updateClassroomDto)
        {
            var updateClassroomDtoValidator = new UpdateClassroomDtoValidator(_teacherRepository, _classroomRepository);
            var validationResult = await updateClassroomDtoValidator.ValidateAsync(updateClassroomDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<ClassroomDto>.Fail("Invalid input", validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
            }
            
            var room = await _classroomRepository.GetAsync(updateClassroomDto.Id);
            if (room == null)
            {
                return NotFound(SharedResponse<Model.Classroom>.Fail("No classroom with such id", null));
            }

            var classroom = _mapper.Map<Model.Classroom>(updateClassroomDto);
            await _classroomRepository.UpdateAsync(classroom);
            return Ok(SharedResponse<Model.Classroom>.Success(classroom, null));
        }

        [HttpPost]
        public async Task<ActionResult<ClassroomDto>> CreateClassroom(CreateClassroomDto createClassroomDto)
        {
            var createClassroomDtoValidator = new CreateClassroomDtoValidator(_teacherRepository);
            var validationResult = await createClassroomDtoValidator.ValidateAsync(createClassroomDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<ClassroomDto>.Fail("Invalid input", validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
            }
            var classroom = _mapper.Map<Model.Classroom>(createClassroomDto); 
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

        [HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<SharedResponse<IEnumerable<ClassroomDto>>>> GetRoomsByTeacherId(Guid teacherId)
        {
            var classrooms = await _classroomRepository.GetAllAsync(x => x.Creator.Id == teacherId);
            var classroomDtos = _mapper.Map<IEnumerable<ClassroomDto>>(classrooms);
            return Ok(SharedResponse<IEnumerable<ClassroomDto>>.Success(classroomDtos, null));
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<SharedResponse<IEnumerable<ClassroomDto>>>> GetRoomsByStudentId(Guid studentId)
        {
            var classrooms = await _classroomRepository.GetAllAsync(x => x.Members.Any(s => s.Id == studentId));
            var classroomDtos = _mapper.Map<IEnumerable<ClassroomDto>>(classrooms);
            return Ok(SharedResponse<IEnumerable<ClassroomDto>>.Success(classroomDtos, null));
        }

        [HttpPost("add-batch")]
        public async Task<ActionResult<SharedResponse<ClassroomDto>>> AddBatch([FromBody] AddBatchDto batchDto)
        {
            var addBatchDtoValidator = new AddBatchDtoValidator(_classroomRepository);
            var validationResult = await addBatchDtoValidator.ValidateAsync(batchDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<ClassroomDto>.Fail("Invalid input", validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
            }
            var students = await _studentRepository.GetAllAsync(x => x.Section == batchDto.Section && x.Year == batchDto.Year && x.Department == batchDto.Department);

            var classroom = await _classroomRepository.GetAsync(batchDto.ClassRoomId);
            
            foreach (var student in students)
            {
                classroom.Members.Add(student);

            }
            await _classroomRepository.UpdateAsync(classroom);
            return Ok(SharedResponse<ClassroomDto>.Success(_mapper.Map<ClassroomDto>(classroom), null));
        }

        [HttpGet("search")]
        public async Task<ActionResult<SharedResponse<SearchResult<ClassroomDto>>>> SearchClassrooms([FromQuery] string query, int pageNumber = 1, int pageSize = 10)
        {
            var classrooms = await _classroomRepository.GetAllAsync();
            var _searchService = new SearchService(classrooms.ToList());
            var matches = _searchService.FindClosestMatchs(query);
            var totalClassrooms = matches.Count;
            var res = _classroomRepository.GetAllAsync(x => matches.Contains(x.Id)).Result.ToList();

            var searchResult = new SearchResult<ClassroomDto>
            {
                TotalCount = totalClassrooms,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Results = _mapper.Map<IEnumerable<ClassroomDto>>(res).ToList()
            };

            return Ok(SharedResponse<SearchResult<ClassroomDto>>.Success(searchResult, null));
        }
    
        
    }
}