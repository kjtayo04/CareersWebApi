using System.Text.Json;
using CareersWebApi.Mappers;
using CareersWebApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CareersWebApi.Repositories.JobRepository;

/// <summary>
/// Fetches jobs from the Greenhouse public boards API and exposes them via IJobRepository.
/// Caches results for a short duration to avoid excessive external calls.
/// </summary>
public class GreenhouseJobRepository : IJobRepository
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GreenhouseJobRepository> _logger;
    private const string JobsCacheKey = "greenhouse_jobs";

    public GreenhouseJobRepository(HttpClient client, IMemoryCache cache, ILogger<GreenhouseJobRepository> logger)
    {
        _client = client;
        _cache = cache;
        _logger = logger;
        // Helpful defaults for calling the public Greenhouse API
        try
        {
            _client.DefaultRequestHeaders.UserAgent.TryParseAdd("CareersWebApi/1.0 (+https://example.com)");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
        }
        catch
        {
            // ignore header parse errors
        }
    }

    private async Task<List<JobDetail>> FetchAllJobsAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(JobsCacheKey, out List<JobDetail>? cached) && cached != null)
        {
            return cached;
        }

        try
        {
            var requestPath = "/v1/boards/baringa/jobs";
            _logger.LogDebug("Fetching greenhouse jobs from {Base}{Path}", _client.BaseAddress, requestPath);
            using var resp = await _client.GetAsync(requestPath, ct);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("Greenhouse API returned non-success status {Status} for {Url}: {Body}", (int)resp.StatusCode, resp.RequestMessage?.RequestUri, body);
                return new List<JobDetail>();
            }
            var stream = await resp.Content.ReadAsStreamAsync(ct);
            var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            var list = new List<JobDetail>();

            // Greenhouse may return either a root array or an object with a 'jobs' array property
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                var idx = 0;
                foreach (var el in doc.RootElement.EnumerateArray())
                {
                    try
                    {
                        idx++;
                        var job = GreenhouseJobMapper.Map(el, idx);
                        list.Add(job);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to map greenhouse job element");
                    }
                }
            }
            else if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                // try to find a 'jobs' property (case-insensitive)
                if (doc.RootElement.TryGetProperty("jobs", out var jobsEl) && jobsEl.ValueKind == JsonValueKind.Array)
                {
                    var idx = 0;
                    foreach (var el in jobsEl.EnumerateArray())
                    {
                        try
                        {
                            idx++;
                            var job = GreenhouseJobMapper.Map(el, idx);
                            list.Add(job);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to map greenhouse job element");
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Greenhouse response root was object but did not contain a 'jobs' array");
                }
            }

            // cache for short duration
            _cache.Set(JobsCacheKey, list, TimeSpan.FromMinutes(5));
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching greenhouse jobs");
            return new List<JobDetail>();
        }
    }

    public async Task<PagedResult<JobSummary>> GetJobsAsync(string? search, int page, int pageSize)
    {
        var list = await FetchAllJobsAsync();

        var query = list.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(j => (!string.IsNullOrEmpty(j.Title) && j.Title.ToLowerInvariant().Contains(s)) || (!string.IsNullOrEmpty(j.Department) && j.Department.ToLowerInvariant().Contains(s)) || (!string.IsNullOrEmpty(j.Location) && j.Location.ToLowerInvariant().Contains(s)));
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


}
