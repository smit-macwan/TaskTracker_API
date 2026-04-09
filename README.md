# TaskManager (Clean Architecture)

## Tech
- ASP.NET Core Web API (.NET 10 target)
- Clean Architecture: `Domain`, `Application`, `Infrastructure`, `WebApi`
- SQLite persistence via EF Core
- JWT auth + `user` / `admin` roles (ASP.NET Core Identity)
- React (Vite) minimal UI

## Run the backend

From `D:\Projects\TaskManager`:

```bash
dotnet run --project .\src\TaskManager.WebApi\TaskManager.WebApi.csproj
```

- The API seeds roles (`user`, `admin`) and **applies migrations on startup**.
- Swagger: `https://localhost:7021/swagger` (or `http://localhost:5134/swagger`)

### Default dev admin
- Email: `admin@local`
- Password: `Admin123!`

Configure in `src/TaskManager.WebApi/appsettings.json` under `SeedAdmin`.

## Run the frontend

You currently need Node.js + npm installed to run the React app.

From `D:\Projects\TaskManager\frontend`:

```bash
npm install
npm run dev
```

Optional: point the UI at a different API URL:

```bash
set VITE_API_BASE_URL=http://localhost:5134
npm run dev
```

