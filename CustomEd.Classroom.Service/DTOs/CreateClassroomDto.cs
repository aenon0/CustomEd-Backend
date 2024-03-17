namespace CustomEd.Classroom.Service.DTOs
{
    public class CreateClassroomDto
    {
        public string Name { get; set; } = null!;
        public string CourseNo { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid CreatorId { get; set; }
    }
}