using System;
using System.Collections.Generic;

namespace CustomEd.Discussion.Service.DTOs
{
    public class CreateMessageDto
    {
        public required Guid ClassroomId { get; set; }
        public required Guid SenderId { get; set; }
        public required string Content { get; set; }
    }
}