using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CustomEd.LearningEngine.Service;
using CustomEd.LearningEngine.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Response;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomEd.LearningEngine.Service.Controllers
{
    [Route("api/learningEngine")]
    [ApiController]
    public class LearningPathController : ControllerBase
    {
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IGenericRepository<LearningPath> _learningPathRepository;
        private readonly IGenericRepository<ChatbotMessage> _chatbotMessageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public LearningPathController(IGenericRepository<Student> studentRepository, IGenericRepository<LearningPath> learningPathRepository, IGenericRepository<ChatbotMessage> chatbotMessageRepository, IHttpContextAccessor httpContextAccessor, IJwtService jwtService, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _chatbotMessageRepository = chatbotMessageRepository;
            _learningPathRepository = learningPathRepository;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        [Authorize(Policy = "StudentOnlyPolicy")]
        [HttpGet("learningPaths")]
        public async Task<ActionResult<SharedResponse<IEnumerable<LearningPath>>>> GetLearningPaths()
        {
            var studentId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            if(studentId == Guid.Empty)
            {
                return Unauthorized(SharedResponse<IEnumerable<LearningPath>>.Fail("You're not authorized", null));
            }
            var learningPaths = await _learningPathRepository.GetAllAsync(x => x.StudentId == studentId);
            return Ok(SharedResponse<IEnumerable<LearningPath>>.Success(learningPaths, null));
        }

         
        [Authorize(Policy = "StudentOnlyPolicy")]
        [HttpGet("learningPaths/{id}")]
        public async Task<ActionResult<SharedResponse<LearningPath>>> GetLearningPath(Guid id)
        {
            var learningPath = await _learningPathRepository.GetAsync(id);

            if (learningPath == null)
            {
                return NotFound(SharedResponse<LearningPath>.Fail("No learning path with such id", null));
            }
            return Ok(SharedResponse<LearningPath>.Success(learningPath, null));
        }

        [Authorize(Policy = "StudentOnlyPolicy")]
        [HttpPost("learningPaths")]
        public async Task<ActionResult<SharedResponse<LearningPath>>> CreateLearningPath(LearningPath learningPath)
        {
            var studentId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            if(studentId == Guid.Empty)
            {
                return Unauthorized(SharedResponse<IEnumerable<LearningPath>>.Fail("You're not authorized", null));
            }
            learningPath.StudentId = studentId;
            learningPath.Id = Guid.NewGuid();
            await _learningPathRepository.CreateAsync(learningPath);
            return Ok(SharedResponse<LearningPath>.Success(learningPath, null));
        }

        [Authorize(Policy = "StudentOnlyPolicy")]
        [HttpPut("learningPaths")]
        public async Task<ActionResult<SharedResponse<LearningPath>>> UpdateLearningPathStatus(Guid learningPathId, LearningPathStatus status)
        {
            var learningPath = await _learningPathRepository.GetAsync(learningPathId);
            learningPath.Status = status;
            await _learningPathRepository.UpdateAsync(learningPath);
            return Ok(SharedResponse<LearningPath>.Success(learningPath, null));
        }


        [Authorize(Policy = "StudentOnlyPolicy")]
        [HttpDelete("learningPaths/{id}")]
        public async Task<IActionResult> RemoveLearningPath(Guid id)
        {
            var learningPath = await _learningPathRepository.GetAsync(id);
            if (learningPath == null)
            {
                return NotFound(SharedResponse<LearningPath>.Fail("No learning path with such id", null));
            }
            var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            if(learningPath.StudentId != currentUserId)
            {
                return Unauthorized(SharedResponse<LearningPath>.Fail("You are not authorized to delete this learning path", null));
            }
            await _learningPathRepository.RemoveAsync(id);
            return NoContent();
        }
        
        [Authorize(Policy = "StudentOnlyPolicy")]
        [HttpGet("chatbotMessages")]
        public async Task<ActionResult<SharedResponse<IEnumerable<ChatbotMessage>>>> GetChatbotMessages()
        {
            var studentId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            if(studentId == Guid.Empty)
            {
                return Unauthorized(SharedResponse<IEnumerable<LearningPath>>.Fail("You're not authorized", null));
            }
            
            var chatbotMessages = await _chatbotMessageRepository.GetAllAsync(x => x.StudentId == studentId);
            var sortedChatbotMessages = chatbotMessages.OrderBy(x => x.CreatedAt).ToList();

            return Ok(SharedResponse<IEnumerable<ChatbotMessage>>.Success(sortedChatbotMessages, null));
        }


        [Authorize(Policy = "StudentOnlyPolicy")]
        [HttpPost("chatbotMessages")]
        public async Task<ActionResult<SharedResponse<IEnumerable<ChatbotMessage>>>> CreateChatbotMessage(List<ChatbotMessage> chatbotMessages)
        {
            var studentId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            if(studentId == Guid.Empty)
            {
                return Unauthorized(SharedResponse<IEnumerable<LearningPath>>.Fail("You're not authorized", null));
            }
            
            foreach(var chatbotMessage in chatbotMessages)
            {
                chatbotMessage.StudentId = studentId;
                await _chatbotMessageRepository.CreateAsync(chatbotMessage);
            }
            return Ok(SharedResponse<IEnumerable<ChatbotMessage>>.Success(chatbotMessages, null));
        }
        
        
    }
}