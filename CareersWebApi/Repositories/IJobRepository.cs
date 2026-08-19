using CareersWebApi.Models;
using System.Threading.Tasks;

namespace CareersWebApi.Repositories;

public interface IJobRepository
{
    Task<PagedResult<JobSummary>> GetJobsAsync(string? search, int page, int pageSize);
    Task<JobDetail?> GetJobByIdAsync(int id);
}
