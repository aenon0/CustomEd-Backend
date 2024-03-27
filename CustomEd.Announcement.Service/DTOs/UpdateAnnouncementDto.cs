using System;
using System.Collections.Generic;

namespace CustomEd.Announcement.Service.DTOs
{
    public class UpdateAnnouncementDto
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required List<string> Attachments { get; set; }
        public Guid ClassRoomId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}