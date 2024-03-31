using AutoMapper;
using CustomEd.Assessment.Service.DTOs;
using CustomEd.Assessment.Service.DTOs.Validation;
using CustomEd.Assessment.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Response;
using Microsoft.AspNetCore.Mvc;

namespace CustomEd.Assessment.Service.Controllers;

[ApiController]
[Route("/api/assessment")]
public class AssessmentController: ControllerBase
{

    private readonly IGenericRepository<Model.Assessment> _assessmentRepository;
    private readonly IGenericRepository<Question> _questionRepository;
    private readonly IGenericRepository<Answer> _answerRepository;
    private readonly IGenericRepository<Classroom> _classroomRepository;
    private readonly IGenericRepository<Submission> _submissionRepository;
    private readonly IMapper _mapper;


    public AssessmentController(IGenericRepository<Model.Assessment> assessmentRepository, IGenericRepository<Question> questionRepository, IGenericRepository<Answer> answerRepository, IMapper mapper, IGenericRepository<Classroom> classroomRepository, IGenericRepository<Submission> submissionRepository)
    {
        _assessmentRepository = assessmentRepository;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
        _mapper = mapper;
        _classroomRepository = classroomRepository;
        _submissionRepository = submissionRepository;
    }

    [HttpPost]
    public async Task<ActionResult<SharedResponse<AssessmentDto>>> CreateAssessment([FromBody] CreateAssessmentDto createAssessmentDto)
    {
        var createAssessementDtoValidator = new CreateAssessmentDtoValidator(_classroomRepository);
        var validationResult = await createAssessementDtoValidator.ValidateAsync(createAssessmentDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(SharedResponse<AssessmentDto>.Fail("Assessment could not be created", errors));
        }
        
        var assessment = _mapper.Map<Model.Assessment>(createAssessmentDto);
        await _assessmentRepository.CreateAsync(assessment);
        var assessmentDto  = _mapper.Map<AssessmentDto>(assessment);
        return Ok(SharedResponse<AssessmentDto>.Success(assessmentDto, "Assessment created successfully."));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SharedResponse<AssessmentDto>>> GetAssessment(Guid id)
    {
        var assessment = await _assessmentRepository.GetAsync(id);
        if (assessment == null)
        {
            return NotFound(SharedResponse<AssessmentDto>.Fail("Assessment not found", null));
        }
        var assessmentDto = _mapper.Map<AssessmentDto>(assessment);
        return Ok(SharedResponse<AssessmentDto>.Success(assessmentDto, "Assessment retrived successfully."));
    }

    [HttpGet("classroom/{classroomId}")]
    public async Task<ActionResult<SharedResponse<List<AssessmentDto>>>> GetAssessmentsByClassroom(Guid classroomId)
    {
        var assessments = await _assessmentRepository.GetAllAsync(a => a.Classroom.Id == classroomId);
        var assessmentDtos = _mapper.Map<List<AssessmentDto>>(assessments);
        return Ok(SharedResponse<List<AssessmentDto>>.Success(assessmentDtos, "Assessments retrived successfully."));
    }

    [HttpPut]
    public async Task<ActionResult<SharedResponse<AssessmentDto>>> UpdateAssessment([FromBody] UpdateAssessmentDto updateAssessmentDto)
    {
        var updateAssessmentDtoValidator = new UpdateAssessmentDtoValidator(_assessmentRepository, _classroomRepository);
        var validationResult = await updateAssessmentDtoValidator.ValidateAsync(updateAssessmentDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(SharedResponse<AssessmentDto>.Fail("Assessment could not be updated", errors));
        }
        var assessment = _mapper.Map<Model.Assessment>(updateAssessmentDto);
        await _assessmentRepository.UpdateAsync(assessment);
        var assessmentDto = _mapper.Map<AssessmentDto>(assessment);
        return Ok(SharedResponse<AssessmentDto>.Success(assessmentDto, "Assessment updated successfully."));
    
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<SharedResponse<AssessmentDto>>> DeleteAssessment(Guid id)
    {
        var assessment = await _assessmentRepository.GetAsync(id);
        if (assessment == null)
        {
            return NotFound(SharedResponse<AssessmentDto>.Fail("Assessment not found", null));
        }
        await _assessmentRepository.RemoveAsync(assessment);
        var assessmentDto = _mapper.Map<AssessmentDto>(assessment);
        return Ok(SharedResponse<AssessmentDto>.Success(assessmentDto, "Assessment deleted successfully."));
    
    }

    [HttpPost("add-question")]
    public async Task<ActionResult<SharedResponse<QuestionDto>>> CreateQuestion([FromBody] CreateQuestionDto createQuestionDto)
    {
        var createQuestionDtoValidator = new CreateQuestionDtoValidator(_assessmentRepository, _answerRepository);
        var validationResult = await createQuestionDtoValidator.ValidateAsync(createQuestionDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(SharedResponse<QuestionDto>.Fail("Question could not be created", errors));
        }
        var question = _mapper.Map<Question>(createQuestionDto);
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
        await _questionRepository.CreateAsync(question);

        var assessment = await _assessmentRepository.GetAsync(createQuestionDto.AssessmentId);
        assessment.Questions.Add(question);
        await _assessmentRepository.UpdateAsync(assessment);

        var questionDto = _mapper.Map<QuestionDto>(question);
        return Ok(SharedResponse<QuestionDto>.Success(questionDto, "Question created successfully."));
    }

    [HttpGet("question/{id}")]
    public async Task<ActionResult<SharedResponse<QuestionDto>>> GetQuestion(Guid id)
    {
        var question = await _questionRepository.GetAsync(id);
        if (question == null)
        {
            return NotFound(SharedResponse<QuestionDto>.Fail("Question not found", null));
        }
        var questionDto = _mapper.Map<QuestionDto>(question);
        return Ok(SharedResponse<QuestionDto>.Success(questionDto, "Question retrived successfully."));
    }

    [HttpPut("edit-question")]
    public async Task<ActionResult<SharedResponse<QuestionDto>>> UpdateQuestion([FromBody] UpdateQuestionDto updateQuestionDto)
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
        return Ok(SharedResponse<QuestionDto>.Success(questionDto, "Question deleted successfully."));
    }

    [HttpPost("add-submission")]
    public async Task<ActionResult<SharedResponse<SubmissionDto>>> CreateSubmission([FromBody] CreateSubmissionDto createSubmissionDto)
    {
        var createSubmissionDtoValidator = new CreateSubmissionDtoValidator(_assessmentRepository, _answerRepository);
        var validationResult = await createSubmissionDtoValidator.ValidateAsync(createSubmissionDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(SharedResponse<SubmissionDto>.Fail("Submission could not be created", errors));
        }
        var assessment = await _assessmentRepository.GetAsync(createSubmissionDto.AssessmentId);
        if (DateTime.UtcNow > assessment.Deadline)
        {
            return BadRequest(SharedResponse<SubmissionDto>.Fail("Submission could not be created", new List<string> { "Assessment is expired." }));
        }
        var submission = _mapper.Map<Submission>(createSubmissionDto);
        await _submissionRepository.CreateAsync(submission);
        var submissionDto = _mapper.Map<SubmissionDto>(submission);
        return Ok(SharedResponse<SubmissionDto>.Success(submissionDto, "Submission created successfully."));
    }

    [HttpGet("submission/{id}")]
    public async Task<ActionResult<SharedResponse<SubmissionDto>>> GetSubmission(Guid id)
    {
        var submission = await _submissionRepository.GetAsync(id);
        if (submission == null)
        {
            return NotFound(SharedResponse<SubmissionDto>.Fail("Submission not found", null));
        }
        var submissionDto = _mapper.Map<SubmissionDto>(submission);
        return Ok(SharedResponse<SubmissionDto>.Success(submissionDto, "Submission retrived successfully."));
    }

    [HttpPut("edit-submission")]
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
        var submission = _mapper.Map<Submission>(updateSubmissionDto);
        await _submissionRepository.UpdateAsync(submission);
        var submissionDto = _mapper.Map<SubmissionDto>(submission);
        return Ok(SharedResponse<SubmissionDto>.Success(submissionDto, "Submission updated successfully."));
    }





}