using Amazon.Auth.AccessControlPolicy;
using AutoMapper;
using CustomEd.Discussion.Service.DTOs;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace CustomEd.Discussion.Service.Controllers;



[ApiController]
[Route("/api/classroom/{classRoomId}/discussion")]
public class DiscussionController: ControllerBase
{

    private readonly IGenericRepository<Model.Student> _studentRepository;
    private readonly IGenericRepository<Model.Classroom> _classroomRepository;
    private readonly IGenericRepository<Model.Message> _messageRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;


    public DiscussionController(IGenericRepository<Model.Student> studentRepository, IGenericRepository<Model.Classroom> classroomRepository, IGenericRepository<Model.Message> messageRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
    {
        _studentRepository = studentRepository;
        _classroomRepository = classroomRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
    }


    [HttpPost]
    [Authorize(Policy = "MemberOnly")]
    public async Task<ActionResult<SharedResponse<Model.Message>>> CreateMessage([FromBody] CreateMessageDto messageDto)
    {
        var senderId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
        if(senderId == Guid.Empty)
        {
            return Unauthorized(SharedResponse<Model.Message>.Fail("You're not authorized", null));
        }
        var classroom = await _classroomRepository.GetAsync(messageDto.ClassroomId);
        var student = await _studentRepository.GetAsync(messageDto.SenderId);
        var message = new Model.Message{
            Id = Guid.NewGuid(),
            Content = messageDto.Content,
            ClassroomId = messageDto.ClassroomId,
            SenderId = messageDto.SenderId
        };
        await _messageRepository.CreateAsync(message);
        return Ok(SharedResponse<Model.Message>.Success(message, "Message created successfully."));
    }

    
    [HttpGet]
    [Authorize(Policy = "MemberOnly")]
    public async Task<ActionResult<SharedResponse<List<Model.Message>>>> GetPaginatedMessages(Guid classRoomId, int pageNumber, int pageSize)
    {
        var senderId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
        if(senderId == Guid.Empty)
        {
            return Unauthorized(SharedResponse<Model.Message>.Fail("You're not authorized", null));
        }
        var classroom = await _classroomRepository.GetAsync(classRoomId);
        var messages = await _messageRepository.GetAllAsync(d => d.ClassroomId == classroom.Id);
        var paginatedMessages = messages.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        
        var sortedMessages = paginatedMessages.OrderBy(d => d.CreatedAt);
        return Ok(SharedResponse<List<Model.Message>>.Success(sortedMessages.ToList(), "Messages retrieved successfully."));
    }

}