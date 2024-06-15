using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CustomEd.Assessment.Service.AnalyticsSevice;
using CustomEd.Assessment.Service.DTOs;
using CustomEd.Assessment.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;

namespace CustomEd.Assessment.Service.Controllers
{
    [Route("api/classroom/{classRoomId}/analytics")]
    
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IGenericRepository<Model.Assessment> _assessmentRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IGenericRepository<Answer> _answerRepository;
        private readonly IGenericRepository<Classroom> _classroomRepository;
        private readonly IGenericRepository<Submission> _submissionRepository;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly AnalysisService _analyticsService;

        public AnalyticsController(
            IGenericRepository<Model.Assessment> assessmentRepository,
            IGenericRepository<Question> questionRepository,
            IGenericRepository<Answer> answerRepository,
            IMapper mapper,
            IGenericRepository<Classroom> classroomRepository,
            IGenericRepository<Submission> submissionRepository,
            AnalysisService analyticsService,
            IJwtService jwtService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _assessmentRepository = assessmentRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _mapper = mapper;
            _classroomRepository = classroomRepository;
            _submissionRepository = submissionRepository;
            _analyticsService = analyticsService;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("cross-student/{studentId}")]
        [Authorize(Policy = "MemberOnly")]
        public async Task<ActionResult<SharedResponse<List<CrossStudent?>>>> GetCrossStudent(
            Guid studentId,
            Guid classRoomId
        )
        {
            try
            {
                if (studentId == Guid.Empty)
                {
                    return BadRequest(SharedResponse<List<CrossStudent?>>.Fail("Student id is required", null));
                }
                var currentUserId  = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
                if (currentUserId != studentId)
                {
                    return Unauthorized(SharedResponse<List<CrossStudent?>>.Fail("Unauthorized", null));
                }

                var crossStudent = await _analyticsService.PerformCrossStudent(
                    studentId,
                    classRoomId
                );
                if (crossStudent == null)
                {
                    return NotFound(
                        SharedResponse<List<CrossStudent?>>.Fail(
                            "No cross student data found",
                            null
                        )
                    );
                }

                return Ok(SharedResponse<List<CrossStudent?>>.Success(crossStudent, null));
            }
            catch (Exception e)
            {
                return BadRequest(SharedResponse<List<CrossStudent?>>.Fail(e.Message, null));
            }
        }

        [HttpGet("cross-assessment")]
        [Authorize(Policy = "CreatorOnly")]
        public async Task<ActionResult<SharedResponse<List<AnalyticsDto?>>>> GetCrossAssessment(
            Guid classRoomId
        )
        {
            try
            {
                var analytics = await _analyticsService.PerformCrossAssessment(classRoomId);
                if (analytics == null)
                {
                    return NotFound(
                        SharedResponse<List<AnalyticsDto?>>.Fail(
                            "No cross assessment data found",
                            null
                        )
                    );
                }
                var analyticsDto = _mapper.Map<List<AnalyticsDto?>>(analytics);
                return Ok(SharedResponse<List<AnalyticsDto?>>.Success(analyticsDto, null));
            }
            catch (Exception e)
            {
                return BadRequest(SharedResponse<List<AnalyticsDto?>>.Fail(e.Message, null));
            }
        }

        [HttpGet("assessment/{assessmentId}")]
        [Authorize(Policy = "CreatorOnly")]
        public async Task<ActionResult<SharedResponse<AnalyticsDto>>> GetAssessment(
            Guid assessmentId,
            Guid classRoomId
        )
        {
            var assessment = await _assessmentRepository.GetAsync(x =>
                x.Id == assessmentId && x.Classroom.Id == classRoomId
            );
            if (assessment == null)
            {
                return NotFound(SharedResponse<AnalyticsDto>.Fail("Assessment not found", null));
            }
            try
            {
                var assessmentAnalytics = _mapper.Map<AnalyticsDto>(
                    await _analyticsService.PerformClassAnalysis(assessmentId)
                );
                return Ok(SharedResponse<AnalyticsDto>.Success(assessmentAnalytics, null));
            }
            catch (Exception e)
            {
                return BadRequest(SharedResponse<AnalyticsDto>.Fail(e.Message, null));
            }
        }

        [HttpPost("assessment")]
        [Authorize(Policy = "CreatorOnly")]
        public async Task<ActionResult<SharedResponse<List<AnalyticsDto>>>> GetAssessmentByTag(
            [FromBody] List<string?> tags,
            Guid classRoomId
        )
        {
            if (tags == null || tags.Count == 0)
            {
                return BadRequest(SharedResponse<List<AnalyticsDto>>.Fail("Tags are required", null));
            }
            try
            {
                var assessmentAnalytics = _mapper.Map<List<AnalyticsDto>>(
                    await _analyticsService.PerformClassAnalysisByTag(tags!, classRoomId)
                );
                return Ok(SharedResponse<List<AnalyticsDto>>.Success(assessmentAnalytics, null));
            }
            catch (Exception e)
            {
                return BadRequest(SharedResponse<List<AnalyticsDto>>.Fail(e.Message, null));
            }
        }
    }
}
