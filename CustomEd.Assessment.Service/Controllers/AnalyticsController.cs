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
using Microsoft.AspNetCore.Mvc;

namespace CustomEd.Assessment.Service.Controllers
{
    [Route("api/analytics")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IGenericRepository<Model.Assessment> _assessmentRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IGenericRepository<Answer> _answerRepository;
        private readonly IGenericRepository<Classroom> _classroomRepository;
        private readonly IGenericRepository<Submission> _submissionRepository;
        private readonly IMapper _mapper;
        private readonly AnalysisService _analyticsService;

        public AnalyticsController(
            IGenericRepository<Model.Assessment> assessmentRepository,
            IGenericRepository<Question> questionRepository,
            IGenericRepository<Answer> answerRepository,
            IMapper mapper,
            IGenericRepository<Classroom> classroomRepository,
            IGenericRepository<Submission> submissionRepository,
            AnalysisService analyticsService
        )
        {
            _assessmentRepository = assessmentRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _mapper = mapper;
            _classroomRepository = classroomRepository;
            _submissionRepository = submissionRepository;
            _analyticsService = analyticsService;
        }

        [HttpGet("cross-student/{studentId}")]
        public async Task<ActionResult<SharedResponse<List<CrossStudent?>>>> GetCrossStudent(
            Guid studentId,
            [FromQuery] Guid classroomId
        )
        {
            var crossStudent = await _analyticsService.PerformCrossStudent(studentId, classroomId);
            if (crossStudent == null)
            {
                return NotFound(
                    SharedResponse<List<CrossStudent?>>.Fail("No cross student data found", null)
                );
            }

            return Ok(SharedResponse<List<CrossStudent?>>.Success(crossStudent, null));
        }

        [HttpGet("cross-assessment/{classroomId}")]
        public async Task<ActionResult<SharedResponse<List<AnalyticsDto?>>>> GetCrossAssessment(
            Guid classroomId
        )
        {
            var analytics = await _analyticsService.PerformCrossAssessment(classroomId);
            if (analytics == null)
            {
                return NotFound(
                    SharedResponse<List<AnalyticsDto?>>.Fail("No cross assessment data found", null)
                );
            }
            var analyticsDto = _mapper.Map<List<AnalyticsDto?>>(analytics);
            return Ok(SharedResponse<List<AnalyticsDto?>>.Success(analyticsDto, null));
        }

        [HttpGet("assessment/{assessmentId}")]
        public async Task<ActionResult<SharedResponse<AnalyticsDto>>> GetAssessment(
            Guid assessmentId
        )
        {
            var assessment = await _assessmentRepository.GetAsync(assessmentId);
            if (assessment == null)
            {
                return NotFound(SharedResponse<AnalyticsDto>.Fail("Assessment not found", null));
            }

            var assessmentAnalytics = _mapper.Map<AnalyticsDto>(
                await _analyticsService.PerformClassAnalysis(assessmentId)
            );
            return Ok(SharedResponse<AnalyticsDto>.Success(assessmentAnalytics, null));
        }

        [HttpPost("assessment")]
        public async Task<ActionResult<SharedResponse<AnalyticsDto>>> GetAssessmentByTag(
            [FromBody] List<string?> tags
        )
        {
            if(tags == null || tags.Count == 0)
            {
                return BadRequest(SharedResponse<AnalyticsDto>.Fail("Tags are required", null));
            }
            var assessmentAnalytics = _mapper.Map<AnalyticsDto>(
                await _analyticsService.PerformClassAnalysisByTag(tags!)
            );
            return Ok(SharedResponse<AnalyticsDto>.Success(assessmentAnalytics, null));
        }

    }
}
