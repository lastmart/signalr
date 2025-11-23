using System;
using System.Collections.Generic;

namespace BadNews.Models.Comments
{
    public class CommentsDto
    {
        public Guid NewsId { get; set; }

        public IReadOnlyCollection<CommentDto> Comments { get; set; }
    }
}