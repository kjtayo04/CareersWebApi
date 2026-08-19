using CareersWebApi.Models;
using CareersWebApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CareersWebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobRepository _repo;

    public JobsController(IJobRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Returns a paged list of job summaries. Supports search, paging and pageSize (5-10).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<JobSummary>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [SwaggerOperation(Summary = "Search and page job listings")]
    public async Task<ActionResult<PagedResult<JobSummary>>> Get([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        // validate page input
        if (page < 1)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid page", Detail = "page must be >= 1", Status = 400 });
        }

        // enforce allowed pageSize range (5-10)
        if (pageSize < 5 || pageSize > 10)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid pageSize", Detail = "pageSize must be between 5 and 10", Status = 400 });
        }

        // limit search length to avoid abuse
        if (!string.IsNullOrWhiteSpace(search) && search.Length > 200)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid search", Detail = "search must be 200 characters or fewer", Status = 400 });
        }

        var result = await _repo.GetJobsAsync(search, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Returns full job detail including HTML content for the given id.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(JobDetail), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [SwaggerOperation(Summary = "Get job detail by id")]
    public async Task<ActionResult<JobDetail>> GetById(int id)
    {
        if (id <= 0) return BadRequest(new ProblemDetails { Title = "Invalid id", Detail = "id must be a positive integer", Status = 400 });
        var job = await _repo.GetJobByIdAsync(id);
        if (job == null)
        {
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Job with id {id} not found", Status = 404 });
        }

        return Ok(job);
    }
}
