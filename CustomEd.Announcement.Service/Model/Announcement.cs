using CustomEd.Shared.Model;

namespace CustomEd.Announcement.Service.Model
{
    public class Announcement : BaseEntity
    {
        
        public string? Title { get; set; }
        public string? Content { get; set; }
        public List<string>? attachements { get; set; }
        public ClassRoom  ClassRoom { get; set; } = null!;
        public DateTime TimeStamp { get; set; }


    }
}
