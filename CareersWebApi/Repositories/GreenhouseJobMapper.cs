using System;
using System.Text.Json;
using CareersWebApi.Models;

namespace CareersWebApi.Repositories;

internal static class GreenhouseJobMapper
{
    public static JobDetail Map(JsonElement el, int fallbackIndex)
    {
        var job = new JobDetail();

        // Id: prefer numeric id or internal_job_id; if too large for int, fall back to hash
        if (el.TryGetProperty("id", out var idProp))
        {
            if (idProp.ValueKind == JsonValueKind.Number && idProp.TryGetInt32(out var iid))
            {
                job.Id = iid;
            }
            else
            {
                // attempt to parse as long and bound to int via hash when necessary
                if (idProp.ValueKind == JsonValueKind.Number && idProp.TryGetInt64(out var lid))
                {
                    if (lid <= int.MaxValue && lid >= int.MinValue) job.Id = (int)lid;
                    else job.Id = GetStableHash(idProp.GetRawText());
                }
                else if (idProp.ValueKind == JsonValueKind.String && int.TryParse(idProp.GetString(), out var ps))
                {
                    job.Id = ps;
                }
            }
        }

        if (job.Id == 0 && el.TryGetProperty("internal_job_id", out var internalId))
        {
            if (internalId.ValueKind == JsonValueKind.Number && internalId.TryGetInt32(out var iid2)) job.Id = iid2;
            else if (internalId.ValueKind == JsonValueKind.Number && internalId.TryGetInt64(out var lid2))
            {
                if (lid2 <= int.MaxValue && lid2 >= int.MinValue) job.Id = (int)lid2;
                else job.Id = GetStableHash(internalId.GetRawText());
            }
        }

        // final fallback to the provided list index (1-based)
        if (job.Id == 0) job.Id = fallbackIndex;

        // Title
        if (el.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
            job.Title = title.GetString() ?? string.Empty;

        // Location: object with name or string
        if (el.TryGetProperty("location", out var loc))
        {
            if (loc.ValueKind == JsonValueKind.Object && loc.TryGetProperty("name", out var lname) && lname.ValueKind == JsonValueKind.String)
                job.Location = lname.GetString() ?? string.Empty;
            else if (loc.ValueKind == JsonValueKind.String)
                job.Location = loc.GetString() ?? string.Empty;
        }

        // Department: search metadata for 'Sector' or 'department'
        if (el.TryGetProperty("metadata", out var meta) && meta.ValueKind == JsonValueKind.Array)
        {
            foreach (var m in meta.EnumerateArray())
            {
                if (m.ValueKind != JsonValueKind.Object) continue;
                if (m.TryGetProperty("name", out var mname) && mname.ValueKind == JsonValueKind.String)
                {
                    var key = mname.GetString() ?? string.Empty;
                    if (string.Equals(key, "Sector", StringComparison.OrdinalIgnoreCase) || string.Equals(key, "Department", StringComparison.OrdinalIgnoreCase))
                    {
                        if (m.TryGetProperty("value", out var mval) && mval.ValueKind == JsonValueKind.String)
                        {
                            job.Department = mval.GetString() ?? string.Empty;
                            break;
                        }
                    }
                }
            }
        }

        // PublishedAt: prefer first_published then updated_at
        if (el.TryGetProperty("first_published", out var fp) && fp.ValueKind == JsonValueKind.String && DateTime.TryParse(fp.GetString(), out var dtp))
            job.PublishedAt = dtp.ToUniversalTime();
        else if (el.TryGetProperty("updated_at", out var up) && up.ValueKind == JsonValueKind.String && DateTime.TryParse(up.GetString(), out var dtu))
            job.PublishedAt = dtu.ToUniversalTime();
        else
            job.PublishedAt = DateTime.UtcNow;

        // AbsoluteUrl
        if (el.TryGetProperty("absolute_url", out var abs) && abs.ValueKind == JsonValueKind.String)
            job.AbsoluteUrl = abs.GetString() ?? string.Empty;

        // Content: try metadata 'Job Description preview' or 'Job Description'
        job.Content = string.Empty;
        if (el.TryGetProperty("metadata", out var metadata2) && metadata2.ValueKind == JsonValueKind.Array)
        {
            foreach (var m in metadata2.EnumerateArray())
            {
                if (m.ValueKind != JsonValueKind.Object) continue;
                if (m.TryGetProperty("name", out var nm) && nm.ValueKind == JsonValueKind.String)
                {
                    var name = nm.GetString() ?? string.Empty;
                    if (name.IndexOf("Job Description", StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf("Description preview", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (m.TryGetProperty("value", out var mv) && mv.ValueKind == JsonValueKind.String)
                        {
                            job.Content = mv.GetString() ?? string.Empty;
                            break;
                        }
                    }
                }
            }
        }

        return job;
    }

    private static int GetStableHash(string s)
    {
        if (string.IsNullOrEmpty(s)) return 0;
        unchecked
        {
            int hash = 23;
            foreach (var c in s)
            {
                hash = (hash * 31) + c;
            }
            return Math.Abs(hash);
        }
    }
}
