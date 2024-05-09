using AutoMapper;
using CustomEd.Announcement.Service.DTOs;
using CustomEd.Announcement.Service.DTOs.Validtion;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomEd.Announcement.Service.Controllers
{
    [ApiController]
    [Route("/api/classroom/{classRoomId:Guid}/announcements")]
    public class AnnouncementController : ControllerBase
    {
        private readonly IGenericRepository<Model.Announcement> _announcementRepository;
        private readonly IGenericRepository<Model.ClassRoom> _classRoomRepository;
        private readonly IGenericRepository<Model.Teacher> _teacherRepository;
        private readonly IMapper _mapper; 
        public AnnouncementController(IGenericRepository<Model.Announcement> announcementRepository, IGenericRepository<Model.ClassRoom> classRoomRepository, IGenericRepository<Model.Teacher> teacherRepository, IMapper mapper)
        {
            _announcementRepository = announcementRepository;
            _classRoomRepository = classRoomRepository;
            _teacherRepository = teacherRepository;
            _mapper = mapper;
            
        }

        
        [HttpGet]
        [Authorize(policy:"MemberOnlyPolicy")]
        public async Task<ActionResult<SharedResponse<List<AnnouncementDto>>>> GetAll(Guid classRoomId)
        {
            var posts =  await  _announcementRepository.GetAllAsync(x => x.ClassRoom.Id == classRoomId);
            var dtos = _mapper.Map<List<AnnouncementDto>>(posts);
            return SharedResponse<List<AnnouncementDto>>.Success(dtos, "Announcements Retrived");
        }

        [HttpGet("{id}")]
        [Authorize(policy:"MemberOnlyPolicy")]
        public async Task<ActionResult<SharedResponse<SharedResponse<AnnouncementDto>>>>Get(Guid id)
        {
            var post = await _announcementRepository.GetAsync(id);
            if (post == null)
            {
                return NotFound(SharedResponse<AnnouncementDto>.Fail("Announcement not found", null));
            }

            var dto = _mapper.Map<AnnouncementDto>(post);
            return Ok(SharedResponse<AnnouncementDto>.Success(dto, "Announcement Retrived"));

        }

        [HttpPost]
        [Authorize(policy:"CreatorOnlyPolicy")]
        public async Task<ActionResult<SharedResponse<AnnouncementDto>>> Create(CreateAnnouncementDto dto)
        {
            var createAnnouncementDtoValidator = new CreateAnnouncementDtoValidator(_classRoomRepository);
            var result = await createAnnouncementDtoValidator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
                BadRequest(SharedResponse<AnnouncementDto>.Fail("Validation Error", errors));

            }
            var announcement = _mapper.Map<Model.Announcement>(dto);
            await _announcementRepository.CreateAsync(announcement);
            var announcementDto = _mapper.Map<AnnouncementDto>(announcement);
            return Ok(SharedResponse<AnnouncementDto>.Success(announcementDto, "Announcement Created"));
            
        }

        [HttpPut]
        [Authorize(policy:"CreatorOnlyPolicy")]
        public async Task<ActionResult<SharedResponse<AnnouncementDto>>> Update(UpdateAnnouncementDto dto)
        {
            var updateAnnouncementDtoValidator = new UpdateAnnouncementDtoValidator(_classRoomRepository, _announcementRepository);
            var result = await updateAnnouncementDtoValidator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
                return BadRequest(SharedResponse<AnnouncementDto>.Fail("Validation Error", errors));

            }
            var announcement = _mapper.Map<Model.Announcement>(dto);
            await _announcementRepository.UpdateAsync(announcement);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        [Authorize(policy:"CreatorOnlyPolicy")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var announcement =  await _announcementRepository.GetAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }
            await _announcementRepository.RemoveAsync(announcement);
            return NoContent();

            
        }
    }
}