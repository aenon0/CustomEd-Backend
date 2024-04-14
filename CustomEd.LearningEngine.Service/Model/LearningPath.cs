using CustomEd.Shared.Model;
using System;
using System.Collections.Generic;

namespace CustomEd.LearningEngine.Service.Model
{
    public class LearningPath : BaseEntity
    {
        public Guid StudentId { set; get; }
        public string Content {set; get;}
        public LearningPathStatus Status {set; get;}
    }
}
