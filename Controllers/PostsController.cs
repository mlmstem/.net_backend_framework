using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _context.Posts
                .Include(p => p.Comments)
                .ToListAsync();

            return Ok(posts);
        }

        // GET: api/posts/empty-comments
        [HttpGet("empty")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsWithNoComments()
        {
            var postsWithNoComments = await _context.Posts
                .Include(p => p.Comments) // Include Comments to filter
                .Where(p => !p.Comments.Any()) // Check if Comments collection is empty
                .ToListAsync();

            return Ok(postsWithNoComments);
        }

        // GET: api/posts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound("Post not found.");
            }

            return Ok(post);
        }

        // POST: api/posts/create
        [HttpPost("create")]
        public async Task<ActionResult<Post>> CreatePost([FromBody] CreatePostDto createPostDto)
        {
            var post = new Post
            {
                Title = createPostDto.Title,
                Author = createPostDto.Author,
                Date = DateTime.UtcNow.ToString("g"),
                Category = createPostDto.Category,
                Content = createPostDto.Content,
                Comments = new List<Comment>(),
                ShowComments = true
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        // PUT: api/posts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto updatePostDto)
        {
            var post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound("Post not found.");
            }

            post.Comments = updatePostDto.Comments.Select(c => new Comment
            {
                Content = c.content,
                Date = DateTime.UtcNow.ToString("g")
            }).ToList();

            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/posts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound("Post not found.");
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok("success");
        }
    }
}
