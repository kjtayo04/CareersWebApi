# CareersWebApi

This repository contains a small Careers web API (ASP.NET Core, .NET 8) and a React + TypeScript frontend (Vite). The backend exposes a jobs listing and detail API and fetches live job data from the Greenhouse public boards API. The frontend is a Vite app that consumes the backend and provides search, pagination and job detail pages.

Contents
- CareersWebApi/       — ASP.NET Core Web API (C# .NET 8)
- frontend/            — Vite + React + TypeScript frontend

Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm (for the frontend)
- Optional: curl for quick API checks

Quickstart (development)

1. Backend

   - From solution root run:

	 dotnet build
	 dotnet run --project CareersWebApi

   - By default the API will listen on the URLs printed by Kestrel (HTTPS and/or HTTP). The API uses the Greenhouse public API as the jobs source by default.

   - Configuration
	 - Greenhouse base URL can be configured via configuration key `Greenhouse:BaseUrl` (appsettings, environment variable `Greenhouse__BaseUrl`, etc.). Default: `https://boards-api.greenhouse.io`.
	 - The API registers a typed HttpClient for the Greenhouse repository, caches results for 5 minutes, and maps the external JSON into the local JobDetail/JobSummary models.

   - Migrations
	 - A DbContext (SQLite) and DesignTimeDbContextFactory are present. You can create EF Core migrations if you want persistent storage:

	   dotnet tool install --global dotnet-ef --version 8.*
	   dotnet ef migrations add InitialCreate --project CareersWebApi --startup-project CareersWebApi
	   dotnet ef database update --project CareersWebApi --startup-project CareersWebApi

	 - Note: the app seeds a small sample dataset if the SQLite DB is missing.

2. Frontend

   - Install and run the frontend from the `frontend` folder:

	 cd frontend
	 npm install
	 # Copy .env.example -> .env and set VITE_API_BASE_URL if desired
	 npm run dev

   - The frontend expects the API at `/api/v1/jobs` (it uses a dev proxy when running under Vite). Set `VITE_API_BASE_URL` in `frontend/.env` to your backend origin (e.g. `https://localhost:7284`) if you want axios to call the backend directly. The Vite dev server will proxy `/api` to the configured backend and includes a rewrite to normalize path casing.

API contract (public)

- GET {API_BASE_URL}/api/v1/jobs?search={term}&page={n}&pageSize={5-10}

  Returns JSON:

  {
	"items": [ { id, title, location, department, publishedAt, absoluteUrl } ],
	"page": 1,
	"pageSize": 10,
	"totalCount": 42,
	"totalPages": 5,
	"hasPreviousPage": false,
	"hasNextPage": true
  }

- GET {API_BASE_URL}/api/v1/jobs/{id}

  Returns JobDetail (same fields plus `content` with HTML description).

Errors
- Errors are returned as RFC7807 ProblemDetails (title, status, detail). Validate parameters: `page >= 1`, `pageSize` between 5 and 10, `search` length limit.

Notes and implementation details
- The backend includes multiple repository implementations:
  - GreenhouseJobRepository — fetches live data from Greenhouse and maps it to JobDetail (default)
  - EfJobRepository — EF Core backed repository (SQLite) if you prefer persisted data
  - InMemoryJobRepository — simple seeded in-memory store (useful for tests or fallback)

- Greenhouse mapping
  - The Greenhouse JSON is mapped using `GreenhouseJobMapper` to extract title, location (location.name), department (from metadata such as 'Sector'), published dates and description preview fields.
  - The mapper handles numeric and large IDs, falling back to a stable integer when required.

- Frontend
  - React + TypeScript (strict), React Router, TanStack Query (react-query), Axios
  - Debounced search, page-size selector limited to 5–10, pagination controls and job detail page render sanitized HTML (DOMPurify)
  - Vite dev server includes a /api proxy that rewrites `/api/v1/jobs` → `/api/v1/Jobs` to normalize casing.

Troubleshooting
- If frontend requests return 404 from the dev server:
  - Confirm Vite shows the proxy mapping on startup: `[vite] proxy /api -> https://localhost:7284`.
  - Ensure the backend is running and listening on the same host/port/protocol. The Vite proxy attempts an HTTPS→HTTP fallback if the HTTPS target is unreachable.
  - Check browser DevTools Network tab to see the proxied request and response.

- If the Greenhouse fetch fails:
  - Check backend logs — the Greenhouse repository logs request URL and response body for non-success statuses.
  - Test the Greenhouse endpoint directly from the machine: `curl -v https://boards-api.greenhouse.io/v1/boards/baringa/jobs`.

Contributing
- Fork and open a pull request. Keep changes small and focused: mapping fixes, tests, and interface improvements are welcome.

License
- MIT (add your license file as needed)
