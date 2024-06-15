using Amazon.Auth.AccessControlPolicy;
using AutoMapper;
using CustomEd.Assessment.Service.DTOs;
using CustomEd.Assessment.Service.DTOs.Validation;
using CustomEd.Assessment.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Model;
using CustomEd.Shared.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomEd.Assessment.Service.Controllers;

[ApiController]
[Route("/api/classroom/{classRoomId}/assessment")]
public class AssessmentController: ControllerBase
{

    private readonly IGenericRepository<Model.Assessment> _assessmentRepository;
    private readonly IGenericRepository<Question> _questionRepository;
    private readonly IGenericRepository<Answer> _answerRepository;
    private readonly IGenericRepository<Classroom> _classroomRepository;
    private readonly IGenericRepository<Submission> _submissionRepository;
    private readonly IGenericRepository<Analytics> _analyticsRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AssessmentController(IGenericRepository<Model.Assessment> assessmentRepository, IGenericRepository<Question> questionRepository, IGenericRepository<Answer> answerRepository, IGenericRepository<Classroom> classroomRepository, IGenericRepository<Submission> submissionRepository, IGenericRepository<Analytics> analyticsRepository, IHttpContextAccessor httpContextAccessor, IJwtService jwtService, IMapper mapper)
    {
        _assessmentRepository = assessmentRepository;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
        _classroomRepository = classroomRepository;
        _submissionRepository = submissionRepository;
        _analyticsRepository = analyticsRepository;
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Policy = "CreatorOnly")]
    public async Task<ActionResult<SharedResponse<AssessmentDto>>> CreateAssessment([FromBody] CreateAssessmentDto createAssessmentDto, Guid classRoomId)
    {
        var createAssessementDtoValidator = new CreateAssessmentDtoValidator(_classroomRepository);
        var validationResult = await createAssessementDtoValidator.ValidateAsync(createAssessmentDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(SharedResponse<AssessmentDto>.Fail("Assessment could not be created", errors));
        }   

        if(classRoomId != createAssessmentDto.ClassroomId)
        {
            return BadRequest(SharedResponse<AssessmentDto>.Fail("Assessment could not be created", new List<string> { "Classroom id does not match." }));
        }

        var assessment = _mapper.Map<Model.Assessment>(createAssessmentDto);
        assessment.Classroom = await _classroomRepository.GetAsync(classRoomId);
        await _assessmentRepository.CreateAsync(assessment);
        var assessmentDto  = _mapper.Map<AssessmentDto>(assessment);
        return Ok(SharedResponse<AssessmentDto>.Success(assessmentDto, "Assessment created successfully."));
    }

    [HttpPut("publish/{id}")]
    [Authorize(Policy = "CreatorOnly")]
    public async Task<ActionResult<SharedResponse<AssessmentDto>>> PublishAssessment(Guid id, Guid classRoomId)
    {
        var assessment = await _assessmentRepository.GetAsync(x => x.Id == id && x.Classroom.Id == classRoomId);
        if (assessment == null)
        {
            return NotFound(SharedResponse<AssessmentDto>.Fail("Assessment not found", null));
        }

        if (assessment.IsPublished)
        {
            return BadRequest(SharedResponse<AssessmentDto>.Fail("Assessment could not be published", new List<string> { "Assessment is already published." }));
        }

        if(assessment.Deadline < DateTime.UtcNow)
        {
            return BadRequest(SharedResponse<AssessmentDto>.Fail("Assessment could not be published", new List<string> { "Assessment deadline is expired." }));
        }

        assessment.IsPublished = true;
        await _assessmentRepository.UpdateAsync(assessment);

        var assessmentDto = _mapper.Map<AssessmentDto>(assessment);
        return Ok(SharedResponse<AssessmentDto>.Success(assessmentDto, "Assessment published successfully."));
    }
    

    [HttpGet("{id}")]
    [Authorize(Policy = "MemberOnly")]
    public async Task<ActionResult<SharedResponse<AssessmentDto>>> GetAssessment(Guid id, Guid classRoomId)
    {
        var assessment = await _assessmentRepository.GetAsync(x => x.Id == id && x.Classroom.Id == classRoomId);
        if (assessment == null)
        {
            return NotFound(SharedResponse<AssessmentDto>.Fail("Assessment not found", null));
        }
        var userRole  = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserRole();
        if(userRole == Role.Student && assessment.IsPublished == false)
        {
            return BadRequest(SharedResponse<AssessmentDto>.Fail("Assessment could not be retrived", new List<string> { "Assessment is not published." }));
        }
        var assessmentDto = _mapper.Map<AssessmentDto>(assessment);
        return Ok(SharedResponse<AssessmentDto>.Success(assessmentDto, "Assessment retrived successfully."));
    }

    [HttpGet]
    [Authorize(Policy = "MemberOnly")]
    public async Task<ActionResult<SharedResponse<List<AssessmentDto>>>> GetAssessmentsByClassroom(Guid classRoomId)
    {
        var userRole  = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserRole();
        var allAssessments = await _assessmentRepository.GetAllAsync(a => a.Classroom.Id == classRoomId);
        var publishedAssessments = allAssessments.Where(a => a.IsPublished == true).ToList();
        if(userRole == Role.Student)
        {
            var dtos = _mapper.Map<List<AssessmentDto>>(publishedAssessments);
            return Ok(SharedResponse<List<AssessmentDto>>.Success(dtos, "Assessments retrived successfully."));
        }
        
        var assessmentDtos = _mapper.Map<List<AssessmentDto>>(allAssessments);
        return Ok(SharedResponse<List<AssessmentDto>>.Success(assessmentDtos, "Assessments retrived successfully."));
    }

    [HttpPut]
    [Authorize(Policy = "CreatorOnly")]
    public async Task<ActionResult<SharedResponse<AssessmentDto>>> UpdateAssessment(Guid classRoomId, [FromBody] UpdateAssessmentDto updateAssessmentDto)
    {
        var updateAssessmentDtoValidator = new UpdateAssessmentDtoValidator(_assessmentRepository, _classroomRepository);
        var validationResult = await updateAssessmentDtoValidator.ValidateAsync(updateAssessmentDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(SharedResponse<AssessmentDto>.Fail("Assessment could not be updated", errors));
        }
        if(classRoomId != updateAssessmentDto.ClassroomId)
        {
            return BadRequest(SharedResponse<AssessmentDto>.Fail("Assessment could not be updated", new List<string> { "Classroom id does not match." }));
        }
        var assessment = _mapper.Map<Model.Assessment>(updateAssessmentDto);
        assessment.Classroom = await _classroomRepository.GetAsync(classRoomId);
        await _assessmentRepository.UpdateAsync(assessment);
        var assessmentDto = _mapper.Map<AssessmentDto>(assessment);

        var analytics = await _analyticsRepository.GetAllAsync(a => a.Assessment.Id == assessment.Id);
        foreach (var analytic in analytics)
        {
            analytic.Assessment = assessment;
            await _analyticsRepository.UpdateAsync(analytic);
        }

        return Ok(SharedResponse<AssessmentDto>.Success(assessmentDto, "Assessment updated successfully."));
    
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "CreatorOnly")]
    public async Task<ActionResult<SharedResponse<AssessmentDto>>> DeleteAssessment(Guid classRoomId, Guid id)
    {
        var assessment = await _assessmentRepository.GetAsync(x => (x.Id == id )&& (x.Classroom.Id == classRoomId));
        if (assessment == null)
        {
            return NotFound(SharedResponse<AssessmentDto>.Fail("Assessment not found", null));
        }
        
        // Cascade delete related entities
        var questions = await _questionRepository.GetAllAsync(q => q.AssessmentId == assessment.Id);
        foreach (var question in questions)
        {
            var answers = await _answerRepository.GetAllAsync(a => a.QuestionId == question.Id);
            foreach (var answer in answers)
            {
                await _answerRepository.RemoveAsync(answer);
            }
            await _questionRepository.RemoveAsync(question);
        }

        var submissions = await _submissionRepository.GetAllAsync(s => s.AssessmentId == assessment.Id);
        foreach (var submission in submissions)
        {
            await _submissionRepository.RemoveAsync(submission);
        }

        var analytics = await _analyticsRepository.GetAllAsync(a => a.Assessment.Id == assessment.Id);
        foreach (var analytic in analytics)
        {
            await _analyticsRepository.RemoveAsync(analytic);
        }
        
        await _assessmentRepository.RemoveAsync(assessment);
        var assessmentDto = _mapper.Map<AssessmentDto>(assessment);
        return Ok(SharedResponse<AssessmentDto>.Success(assessmentDto, "Assessment deleted successfully."));
    
    }

    [HttpPost("add-question")]
    [Authorize(Policy = "CreatorOnly")]
    public async Task<ActionResult<SharedResponse<QuestionDto>>> CreateQuestion(Guid classRoomId, [FromBody] CreateQuestionDto createQuestionDto)
    {
        var createQuestionDtoValidator = new CreateQuestionDtoValidator(_assessmentRepository, _answerRepository);
        var validationResult = await createQuestionDtoValidator.ValidateAsync(createQuestionDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(SharedResponse<QuestionDto>.Fail("Question could not be created", errors));
        }
        
        var question = _mapper.Map<Question>(createQuestionDto);
        await _questionRepository.CreateAsync(question);
        for (int i = 0; i < question.Answers.Count; i++)
        {
            var answer = question.Answers[i];
            answer.QuestionId = question.Id;
            if (createQuestionDto.CorrectAnswerIndex == i)
            {
                answer.IsCorrect = true;
            }
            else
            {
                answer.IsCorrect = false;
            }
            await _answerRepository.CreateAsync(answer);
        }


        var assessment = await _assessmentRepository.GetAsync(x => x.Id == createQuestionDto.AssessmentId && x.Classroom.Id == classRoomId);
        if(assessment.Questions == null) assessment.Questions = new List<Question>();
        
        assessment.Questions.Add(question);
        await _assessmentRepository.UpdateAsync(assessment);

        var questionDto = _mapper.Map<QuestionDto>(question);
        return Ok(SharedResponse<QuestionDto>.Success(questionDto, "Question created successfully."));
    }

    [HttpGet("question/{id}")]
    [Authorize(Policy = "MemberOnly")]
    public async Task<ActionResult<SharedResponse<QuestionDto>>> GetQuestion(Guid classRoomId, Guid id)
    {
        var question = await _questionRepository.GetAsync(id);
        var userRole = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserRole();
        if (userRole == Role.Student)
        {
            var assessment = await _assessmentRepository.GetAsync(question.AssessmentId);
            if (assessment == null || assessment.IsPublished == false)
            {
                return BadRequest(SharedResponse<QuestionDto>.Fail("Question could not be retrived", null ));
            }
        }
        if (question == null)
        {
            return NotFound(SharedResponse<QuestionDto>.Fail("Question not found", null));
        }
        var questionDto = _mapper.Map<QuestionDto>(question);
        var answers = await _answerRepository.GetAllAsync(a => a.QuestionId == question.Id);
        questionDto.Answers = _mapper.Map<List<AnswerDto>>(answers);
        return Ok(SharedResponse<QuestionDto>.Success(questionDto, "Question retrived successfully."));
    }

    [HttpPut("edit-question")]
    [Authorize(Policy = "CreatorOnly")]
    public async Task<ActionResult<SharedResponse<QuestionDto>>> UpdateQuestion(Guid classRoomId,[FromBody] UpdateQuestionDto updateQuestionDto)
    {
        var updateQuestionDtoValidator = new UpdateQuestionDtoValidator(_assessmentRepository, _answerRepository, _questionRepository);
        var validationResult = await updateQuestionDtoValidator.ValidateAsync(updateQuestionDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(SharedResponse<QuestionDto>.Fail("Question could not be updated", errors));
        }
        var question = _mapper.Map<Question>(updateQuestionDto);
        var answers = await _answerRepository.GetAllAsync(a => a.QuestionId == question.Id);
        foreach (var answer in answers)
        {
            await _answerRepository.RemoveAsync(answer);
        }
        for (int i = 0; i < question.Answers.Count; i++)
        {
            var answer = question.Answers[i];
            answer.QuestionId = question.Id;
            if (updateQuestionDto.CorrectAnswerIndex == i)
            {
                answer.IsCorrect = true;
            }
            else
            {
                answer.IsCorrect = false;
            }
            await _answerRepository.CreateAsync(answer);
        }
        await _questionRepository.UpdateAsync(question);
        var questionDto = _mapper.Map<QuestionDto>(question);
        return Ok(SharedResponse<QuestionDto>.Success(questionDto, "Question updated successfully."));
    }

    [HttpDelete("question/{id}")]
    [Authorize(Policy = "CreatorOnly")]
    public async Task<ActionResult<SharedResponse<QuestionDto>>> DeleteQuestion(Guid id)
    {
        var question = await _questionRepository.GetAsync(id);
        if (question == null)
        {
            return NotFound(SharedResponse<QuestionDto>.Fail("Question not found", null));
        }
        var answers = await _answerRepository.GetAllAsync(a => a.QuestionId == question.Id);
        foreach (var answer in answers)
        {
            await _answerRepository.RemoveAsync(answer);
        }
        await _questionRepository.RemoveAsync(question);
        var questionDto = _mapper.Map<QuestionDto>(question);
        var assessment = await _assessmentRepository.GetAsync(question.AssessmentId);
        assessment.Questions = assessment.Questions.Where(q => q.Id != question.Id).ToList();
        await _assessmentRepository.UpdateAsync(assessment);

        var oldAnswers = await _answerRepository.GetAllAsync(a => a.QuestionId == question.Id);
        foreach (var answer in oldAnswers)
        {
            await _answerRepository.RemoveAsync(answer);
        }
        return Ok(SharedResponse<QuestionDto>.Success(questionDto, "Question deleted successfully."));
    }

    [HttpPost("add-submission")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult<SharedResponse<SubmissionDto>>> CreateSubmission(Guid classRoomId, [FromBody] CreateSubmissionDto createSubmissionDto)
    {
        var createSubmissionDtoValidator = new CreateSubmissionDtoValidator(_assessmentRepository, _answerRepository, _submissionRepository);
        var validationResult = await createSubmissionDtoValidator.ValidateAsync(createSubmissionDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(SharedResponse<SubmissionDto>.Fail("Submission could not be created", errors));
        }

        var assessment = await _assessmentRepository.GetAsync(createSubmissionDto.AssessmentId);
        var userRole = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserRole();
        if (userRole == Role.Student)
        {
            if (assessment == null || assessment.IsPublished == false)
            {
                return BadRequest(SharedResponse<SubmissionDto>.Fail("Submission could not be created", new List<string>{"No such assessment is published"}));
            }
        }

        
        var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
        if (DateTime.UtcNow > assessment.Deadline)
        {
            return BadRequest(SharedResponse<SubmissionDto>.Fail("Submission could not be created", new List<string> { "Assessment is expired." }));
        }
        if (currentUserId != createSubmissionDto.StudentId)
        {
            return BadRequest(SharedResponse<SubmissionDto>.Fail("Submission could not be created", new List<string> { "You are not authorized to create this submission." }));
        }
        var submission = _mapper.Map<Submission>(createSubmissionDto);
        await _submissionRepository.CreateAsync(submission);
        var submissionDto = _mapper.Map<SubmissionDto>(submission);
        return Ok(SharedResponse<SubmissionDto>.Success(submissionDto, "Submission created successfully."));
    }

    [HttpGet("submission/{id}")]
    [Authorize(Policy = "MemberOnly")]
    public async Task<ActionResult<SharedResponse<SubmissionDto>>> GetSubmission(Guid classRoomId, Guid id)
    {
        var submission = await _submissionRepository.GetAsync(id);
        if (submission == null)
        {
            return NotFound(SharedResponse<SubmissionDto>.Fail("Submission not found", null));
        }
        var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
        var creatorId = (await _classroomRepository.GetAsync(classRoomId)).CreatorId;
        if (submission.StudentId != currentUserId && currentUserId != creatorId)
        {
            return BadRequest(SharedResponse<SubmissionDto>.Fail("Submission could not be retrived", new List<string> { "You are not authorized to view this submission." }));
        }
        var submissionDto = _mapper.Map<SubmissionDto>(submission);
        return Ok(SharedResponse<SubmissionDto>.Success(submissionDto, "Submission retrived successfully."));
    }

    [HttpPut("edit-submission")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult<SharedResponse<SubmissionDto>>> UpdateSubmission([FromBody] UpdateSubmissionDto updateSubmissionDto)
    {
        var updateSubmissionDtoValidator = new UpdateSubmissionDtoValidator(_assessmentRepository, _answerRepository, _submissionRepository);
        var validationResult = await updateSubmissionDtoValidator.ValidateAsync(updateSubmissionDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(SharedResponse<SubmissionDto>.Fail("Submission could not be updated", errors));
        }

        var assessment = await _assessmentRepository.GetAsync(updateSubmissionDto.AssessmentId);
        if (DateTime.UtcNow > assessment.Deadline)
        {
            return BadRequest(SharedResponse<SubmissionDto>.Fail("Submission could not be created", new List<string> { "Assessment is expired." }));
        }
        var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
        var creatorId = (await _classroomRepository.GetAsync(assessment.Classroom.Id)).CreatorId;

        if(currentUserId != updateSubmissionDto.StudentId || currentUserId != creatorId)
        {
            return BadRequest(SharedResponse<SubmissionDto>.Fail("Submission could not be updated", new List<string> { "You are not authorized to update this submission." }));
        }
        
        var submission = _mapper.Map<Submission>(updateSubmissionDto);
        await _submissionRepository.UpdateAsync(submission);
        var submissionDto = _mapper.Map<SubmissionDto>(submission);
        return Ok(SharedResponse<SubmissionDto>.Success(submissionDto, "Submission updated successfully."));
    }





}