using System;
using System.Collections.Generic;

namespace CustomEd.Announcement.Service.DTOs
{
    public class CreateAnnouncementDto
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required List<string> Attachments { get; set; }
        public Guid ClassRoomId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}