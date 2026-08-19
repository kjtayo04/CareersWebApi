using CareersWebApi.Models;

namespace CareersWebApi.Repositories;

public class InMemoryJobRepository : IJobRepository
{
    private readonly List<JobDetail> _jobs;

    public InMemoryJobRepository()
    {
        _jobs = Enumerable.Range(1, 42).Select(i => new JobDetail
        {
            Id = i,
            Title = $"Software Engineer {i}",
            Location = i % 3 == 0 ? "New York, NY" : "Remote",
            Department = i % 2 == 0 ? "Engineering" : "Product",
            PublishedAt = DateTime.UtcNow.AddDays(-i),
            AbsoluteUrl = $"https://boards.example.com/jobs/{i}",
            Content = $"<p>This is the job description for job {i}.</p>"
        }).ToList();
    }

    public Task<PagedResult<JobSummary>> GetJobsAsync(string? search, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 5) pageSize = 5;
        if (pageSize > 10) pageSize = 10;

        var query = _jobs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(j => j.Title.ToLowerInvariant().Contains(s) || j.Department.ToLowerInvariant().Contains(s) || j.Location.ToLowerInvariant().Contains(s));
        }

        var totalCount = query.Count();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).Select(j => new JobSummary
        {
            Id = j.Id,
            Title = j.Title,
            Location = j.Location,
            Department = j.Department,
            PublishedAt = j.PublishedAt,
            AbsoluteUrl = j.AbsoluteUrl
        }).ToList();

        var result = new PagedResult<JobSummary>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = page > 1,
            HasNextPage = page < totalPages
        };

        return Task.FromResult(result);
    }

    public Task<JobDetail?> GetJobByIdAsync(int id)
    {
        return Task.FromResult(_jobs.FirstOrDefault(j => j.Id == id));
    }
}
