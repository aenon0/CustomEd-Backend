using Amazon.Auth.AccessControlPolicy;
using AutoMapper;
using CustomEd.Discussion.Service.DTOs;
using CustomEd.Discussion.Service.DTOs.Validtion;
using CustomEd.Discussion.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Response;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace CustomEd.Discussion.Service.Controllers;



[ApiController]
[Route("/api/discussion")]
public class DiscussionController: ControllerBase
{

    private readonly IGenericRepository<Model.Student> _studentRepository;
    private readonly IGenericRepository<Model.Teacher> _teacherRepository;
    private readonly IGenericRepository<Model.Classroom> _classroomRepository;
    private readonly IGenericRepository<Model.Message> _messageRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;


    public DiscussionController(IGenericRepository<Model.Student> studentRepository, IGenericRepository<Model.Classroom> classroomRepository, IGenericRepository<Model.Message> messageRepository, IGenericRepository<Model.Teacher> teacherRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
    {
        _studentRepository = studentRepository;
        _classroomRepository = classroomRepository;
        _teacherRepository = teacherRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
    }


    [HttpPost("/classroom/{classroomId:Guid}")]
    [Authorize(Policy = "MemberOnlyPolicy")]
    public async Task<ActionResult<SharedResponse<Model.Message>>> CreateMessage([FromBody] CreateMessageDto messageDto)
    {
        var senderId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
        string classroomIdString = _httpContextAccessor.HttpContext!.Request.RouteValues["classroomId"]!.ToString();
        Guid classroomId; 
        Guid.TryParse(classroomIdString, out classroomId);
        messageDto.ClassroomId = classroomId;
        messageDto.SenderId = senderId;
        var createMessageDtoValidator = new CreateMessageDtoValidator(_classroomRepository, _studentRepository, _teacherRepository);
        var validationResult = await createMessageDtoValidator.ValidateAsync(messageDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(SharedResponse<Model.Message>.Fail("Invalid input", validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        var message = new Model.Message{
            Id = Guid.NewGuid(),
            Content = messageDto.Content,
            ClassroomId = messageDto.ClassroomId,
            SenderId = senderId
        };
        await _messageRepository.CreateAsync(message);
        return Ok(SharedResponse<Model.Message>.Success(message, "Message created successfully."));
    }

    [HttpPut("/messages/{messageId:Guid}")]
    [Authorize(Policy = "CreatorOnlyPolicy")]
    public async Task<ActionResult<SharedResponse<Model.Message>>> UpdateMessage([FromBody] UpdateMessageDto messageDto)
    {
        Console.WriteLine("hhhhhhhhhhh");
        string messageIdString = _httpContextAccessor.HttpContext!.Request.RouteValues["messageId"]!.ToString();
        Guid messageId; 
        Guid.TryParse(messageIdString, out messageId);
        messageDto.Id = messageId;
        var createMessageDtoValidator = new UpdateMessageDtoValidator(_messageRepository);
        var validationResult = await createMessageDtoValidator.ValidateAsync(messageDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(SharedResponse<Model.Message>.Fail("Invalid input", validationResult.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        var existingMessage = await _messageRepository.GetAsync(messageDto.Id);
        existingMessage.Content = messageDto.Content;
        existingMessage.UpdatedAt = DateTime.UtcNow;
        await _messageRepository.UpdateAsync(existingMessage);
        return Ok(SharedResponse<Model.Message>.Success(existingMessage, "Message updated successfully."));
    }

    [HttpGet("/classroom/{classroomId:Guid}")]
    [Authorize(Policy = "MemberOnlyPolicy")]
    public async Task<ActionResult<SharedResponse<List<GetMessageResponseDto>>>> GetPaginatedMessages(int pageNumber, int pageSize)
    {
        var senderId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
        
        string classroomIdString = _httpContextAccessor.HttpContext!.Request.RouteValues["classroomId"]!.ToString();
        Guid classroomId; 
        Guid.TryParse(classroomIdString, out classroomId);
        Console.WriteLine($"CHECK:  {classroomId}");
        if(senderId == Guid.Empty)
        {
            return Unauthorized(SharedResponse<Model.Message>.Fail("You're not authorized", null));
        }
        var classroom = await _classroomRepository.GetAsync(classroomId);
        var messages = await _messageRepository.GetAllAsync(d => d.ClassroomId == classroom.Id);
        var paginatedMessages = messages.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        var sortedMessages = paginatedMessages.OrderBy(d => d.CreatedAt);
        var modifiedMessages = new List<GetMessageResponseDto>();
        foreach(var message in sortedMessages)
        {
            var firstName = await _studentRepository.GetAsync(message.SenderId).Select(x => x.FirstName) ?? await _teacherRepository.GetAsync(message.SenderId).Select(x => x.FirstName);
            var lastName = await _studentRepository.GetAsync(message.SenderId).Select(x => x.LastName) ?? await _teacherRepository.GetAsync(message.SenderId).Select(x => x.LastName);
            modifiedMessages.Add(new GetMessageResponseDto{
                    Id = message.Id,
                    FirstName  = firstName,
                    LastName = lastName,
                    Content = message.Content,
                    SentAt = message.CreatedAt

            });
        }
        return Ok(SharedResponse<List<GetMessageResponseDto>>.Success(modifiedMessages, "Messages retrieved successfully."));
    }

}