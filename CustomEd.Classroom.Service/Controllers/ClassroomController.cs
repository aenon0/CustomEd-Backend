using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CustomEd.Classroom.Service.DTOs;
using CustomEd.Classroom.Service.DTOs.Validation;
using CustomEd.Classroom.Service.Model;
using CustomEd.Classroom.Service.Search.Service;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Response;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public ClassroomController(IGenericRepository<Model.Classroom> classroomRepository, IGenericRepository<Teacher> teacherRepository, IGenericRepository<Student> studentRepository, IHttpContextAccessor httpContextAccessor, IJwtService jwtService, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _classroomRepository = classroomRepository;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<SharedResponse<IEnumerable<ClassroomDto>>>> GetAllRooms()
        {
            var classrooms = await _classroomRepository.GetAllAsync();
            var classroomDtos = _mapper.Map<IEnumerable<ClassroomDto>>(classrooms);
            return Ok(SharedResponse<IEnumerable<ClassroomDto>>.Success(classroomDtos, null));
        }

        [Authorize]
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

        [Authorize]
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
    
            var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            if(room.Creator.Id != currentUserId)
            {
                return Unauthorized(SharedResponse<Model.Classroom>.Fail("You are not authorized to update this classroom", null));
            }

            var classroom = _mapper.Map<Model.Classroom>(updateClassroomDto);
            classroom.Creator = room.Creator;
            classroom.Members = room.Members;

            await _classroomRepository.UpdateAsync(classroom);

            var classroomUpdatedEvent = _mapper.Map<ClassroomUpdatedEvent>(classroom);
            await _publishEndpoint.Publish(classroomUpdatedEvent);

            return Ok(SharedResponse<Model.Classroom>.Success(classroom, null));
        }

        [Authorize(Policy = "TeacherOnlyPolicy")]
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
            classroom.Creator = await _teacherRepository.GetAsync(createClassroomDto.CreatorId);

            await _classroomRepository.CreateAsync(classroom);
            var classroomCreatedEvent = _mapper.Map<ClassroomCreatedEvent>(classroom);
            await _publishEndpoint.Publish(classroomCreatedEvent);

            return CreatedAtAction("GetRoomById", new { id = classroom.Id }, classroom);
        }

        [Authorize(Policy = "TeacherOnlyPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveRoom(Guid id)
        {
            var room = await _classroomRepository.GetAsync(id);
            if (room == null)
            {
                return NotFound(SharedResponse<Model.Classroom>.Fail("No classroom with such id", null));
            }
            var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            if(room.Creator.Id != currentUserId)
            {
                return Unauthorized(SharedResponse<Model.Classroom>.Fail("You are not authorized to delete this classroom", null));
            }
            await _classroomRepository.RemoveAsync(id);
            var classroomDeletedEvent =  new ClassroomDeletedEvent { Id = id };
            await _publishEndpoint.Publish(classroomDeletedEvent);
            return NoContent();
        }

        [Authorize]
        [HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<SharedResponse<IEnumerable<ClassroomDto>>>> GetRoomsByTeacherId(Guid teacherId)
        {
            var classrooms = await _classroomRepository.GetAllAsync(x => x.Creator.Id == teacherId);
            var classroomDtos = _mapper.Map<IEnumerable<ClassroomDto>>(classrooms);
            return Ok(SharedResponse<IEnumerable<ClassroomDto>>.Success(classroomDtos, null));
        }

        [Authorize]
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<SharedResponse<IEnumerable<ClassroomDto>>>> GetRoomsByStudentId(Guid studentId)
        {
            var classrooms = await _classroomRepository.GetAllAsync(x => x.Members.Any(s => s.Id == studentId));
            var classroomDtos = _mapper.Map<IEnumerable<ClassroomDto>>(classrooms);
            return Ok(SharedResponse<IEnumerable<ClassroomDto>>.Success(classroomDtos, null));
        }

        [Authorize(Policy = "TeacherOnlyPolicy")]
        [Authorize]
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
            var classroomUpdatedEvent = _mapper.Map<ClassroomUpdatedEvent>(classroom);
            await _publishEndpoint.Publish(classroomUpdatedEvent);
            return Ok(SharedResponse<ClassroomDto>.Success(_mapper.Map<ClassroomDto>(classroom), null));
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<ActionResult<SharedResponse<SearchResult<ClassroomDto>>>> SearchClassrooms([FromQuery] string query)
        {
            var classrooms = await _classroomRepository.GetAllAsync();
            var _searchService = new SearchService(classrooms.ToList());
            var matches = _searchService.FindClosestMatchs(query);
            var totalClassrooms = matches.Count;
            var res = _classroomRepository.GetAllAsync(x => matches.Contains(x.Id)).Result.ToList();

            var searchResult = new SearchResult<ClassroomDto>
            {
                TotalCount = 10,
                Results = _mapper.Map<IEnumerable<ClassroomDto>>(res).ToList()
            };

            return Ok(SharedResponse<SearchResult<ClassroomDto>>.Success(searchResult, null));
        }

        [Authorize(Policy = "TeacherOnlyPolicy")]
        [HttpPost("add-student")]
        public async Task<ActionResult<SharedResponse<ClassroomDto>>> AddStudent(Guid classroomId, Guid studentId)
        {
            var addStudentDtoValidatior = new AddStudentDtoValidator(_studentRepository, _classroomRepository);
            var validationResult = await addStudentDtoValidatior.ValidateAsync(new AddStudentDto { ClassroomId = classroomId, StudentId = studentId });
            if (!validationResult.IsValid)
            {
                return BadRequest(SharedResponse<ClassroomDto>.Fail("Invalid input", validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
            }
            var classroom = await _classroomRepository.GetAsync(classroomId);
            var student = await _studentRepository.GetAsync(studentId);

            classroom.Members.Add(student);
            await _classroomRepository.UpdateAsync(classroom);
            var memberJoinedEvent = new MemberJoinedEvent { ClassroomId = classroomId, StudentId = studentId };
            await _publishEndpoint.Publish(memberJoinedEvent);

            return Ok(SharedResponse<ClassroomDto>.Success(_mapper.Map<ClassroomDto>(classroom), null));
        }
        
    
        
    }
}