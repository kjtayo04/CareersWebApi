using CareersWebApi.Models;
using System.Threading.Tasks;

namespace CareersWebApi.Repositories.JobRepository;

public interface IJobRepository
{
    Task<PagedResult<JobSummary>> GetJobsAsync(string? search, int page, int pageSize);
   
}
