using System;
using System.Linq;
using System.Threading.Tasks;
using BadNews.Models.Comments;
using BadNews.Repositories.Comments;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Controllers
{
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly CommentsRepository commentsRepository;

        public CommentsController(CommentsRepository commentsRepository)
        {
            this.commentsRepository = commentsRepository;
        }

        [HttpGet("api/news/{id}/comments")]
        public async Task<ActionResult<CommentsDto>> GetCommentsForNews(Guid newsId)
        {
           var comments = commentsRepository
                .GetComments(newsId)
                .Select(c => new CommentDto { User = c.User, Value = c.Value })
                .ToList();
            return new CommentsDto { NewsId = newsId, Comments = comments};
        }
    }
}