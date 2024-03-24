using CustomEd.Shared.Model;

namespace CustomEd.Announcement.Service.Model
{
    public class Teacher : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Role Role { get; set; }
    }
}