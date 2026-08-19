using CareersWebApi.Data;
using CareersWebApi.Entities;
using CareersWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CareersWebApi.Repositories;

public class EfJobRepository : IJobRepository
{
    private readonly AppDbContext _db;

    public EfJobRepository(AppDbContext db)
    {
        _db = db;
    }
    public async Task<PagedResult<JobSummary>> GetJobsAsync(string? search, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 5) pageSize = 5;
        if (pageSize > 10) pageSize = 10;

        var query = _db.Jobs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(j => j.Title.ToLower().Contains(s) || j.Department.ToLower().Contains(s) || j.Location.ToLower().Contains(s));
        }
        var totalCount = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var items = await query.OrderByDescending(j => j.PublishedAt).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(j => new JobSummary
            {
                Id = j.Id,
                Title = j.Title,
                Location = j.Location,
                Department = j.Department,
                PublishedAt = j.PublishedAt,
                AbsoluteUrl = j.AbsoluteUrl
            }).ToListAsync();

        return new PagedResult<JobSummary>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = page > 1,
            HasNextPage = page < totalPages
        };
    }

    public async Task<JobDetail?> GetJobByIdAsync(int id)
    {
        var j = await _db.Jobs.FirstOrDefaultAsync(x => x.Id == id);
        if (j == null) return null;
        return new JobDetail
        {
            Id = j.Id,
            Title = j.Title,
            Location = j.Location,
            Department = j.Department,
            PublishedAt = j.PublishedAt,
            AbsoluteUrl = j.AbsoluteUrl,
            Content = j.Content
        };
    }
}
