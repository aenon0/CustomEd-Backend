using Amazon.Auth.AccessControlPolicy;
using AutoMapper;
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
    private readonly IGenericRepository<Model.Discussion> _discussionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;


    public DiscussionController(IGenericRepository<Model.Student> studentRepository, IGenericRepository<Model.Classroom> classroomRepository, IGenericRepository<Model.Discussion> discussionRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
    {
        _studentRepository = studentRepository;
        _classroomRepository = classroomRepository;
        _discussionRepository = discussionRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
    }



    [HttpPost]
    [Authorize(Policy = "MemberOnly")]
    public async Task<ActionResult<SharedResponse<Model.Discussion>>> CreateDiscussion([FromBody] Model.Discussion discussion, Guid classRoomId)
    {
        var studentId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
        if(studentId == Guid.Empty)
        {
            return Unauthorized(SharedResponse<Model.Discussion>.Fail("You're not authorized", null));
        }
        discussion.Id = Guid.NewGuid();
        var classroom = await _classroomRepository.GetAsync(classRoomId);
        var student = await _studentRepository.GetAsync(studentId);
        discussion.Classroom = classroom;
        discussion.Student = student;
        await _discussionRepository.CreateAsync(discussion);
        return Ok(SharedResponse<Model.Discussion>.Success(discussion, "Discussion created successfully."));
    }

    
    [HttpGet]
    [Authorize(Policy = "MemberOnly")]
    public async Task<ActionResult<SharedResponse<List<Model.Discussion>>>> GetPaginatedDiscussion(Guid classRoomId, int pageNumber, int pageSize)
    {
        var classroom = await _classroomRepository.GetAsync(classRoomId);
        var discussions = await _discussionRepository.GetAllAsync(d => d.Classroom.Id == classroom.Id);
        var paginatedDiscussions = discussions.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        //here fetch the student name and include that in the list
        var sortedDiscussions = paginatedDiscussions.OrderBy(d => d.CreatedAt);
        return Ok(SharedResponse<List<Model.Discussion>>.Success(sortedDiscussions.ToList(), "Discussion retrieved successfully."));
    }

}