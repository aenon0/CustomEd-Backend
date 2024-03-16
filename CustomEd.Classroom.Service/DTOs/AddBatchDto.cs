using CustomEd.Shared.Model;

namespace CustomEd.Classroom.Service.DTOs
{
    public class AddBatchDto
    {
        public string Section { get; set; } = null!;
        public int Year { get; set; }
        public Department Department { get; set; }
        public Guid ClassRoomId { get; set; }

    }
}