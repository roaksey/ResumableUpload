📦 ResumableUpload

A .NET 8 Web API project that supports resumable file uploads with chunking, SQL Server metadata storage, and reassembly.

🚀 Features

Chunked Uploads: Supports uploading large files in small parts (Content-Range, X-Upload-Id).

Resumable: Retry safe and allows out-of-order chunk uploads.

Temporary Storage: Chunks are written to local disk until final reassembly.

SQL Server Tracking: Upload sessions, chunks, and progress are stored in the database.

File Reassembly: Automatically merges chunks after upload completion.

Status API: Poll progress (uploading, uploaded, completed, failed).

Middleware: Requires X-User-Id header for per-user tracking.

Swagger UI: Test endpoints interactively.

🏗️ Project Structure
ResumableUpload.sln
 ├── Domain
 │    ├── Entities (UploadSession, UploadChunk)
 │    └── Enums (UploadState)
 ├── Application
 │    ├── Abstractions (IUploadManager, IChunkStorage)
 │    ├── Models (UploadStatusDto)
 ├── Infrastructure
 │    ├── Persistence (AppDbContext, DesignTimeDbContextFactory)
 │    ├── Services (UploadManager implementation)
 │    └── Storage (FileSystemChunkStorage)
 └── Api
      ├── Controllers (UploadController, StatusController)
      ├── Middleware (UserContextMiddleware)
      ├── Program.cs
      └── appsettings.json


Domain → Core entities, no dependencies

Application → Interfaces & DTOs

Infrastructure → EF Core + File storage + implementations

Api → ASP.NET Core Web API (controllers & startup)

🛠️ Prerequisites

Visual Studio 2022 (with .NET 8 SDK installed)

SQL Server (local) with Windows Authentication

Optional: SQL Server Management Studio (SSMS)

⚙️ Setup
1. Clone Repo
git clone https://github.com/your-username/ResumableUpload.git
cd ResumableUpload

2. Configure Database

Api/appsettings.json → set your SQL Server connection string:

"ConnectionStrings": {
  "SqlServer": "Server=localhost;Database=ResumableUpload;Trusted_Connection=True;TrustServerCertificate=True"
}

3. Run EF Core Migrations

Open Package Manager Console in VS2022:

Default Project: Infrastructure

Run:

Add-Migration Initial -StartupProject Api -Project Infrastructure -OutputDir Persistence\Migrations
Update-Database -StartupProject Api -Project Infrastructure

4. Run the API

Set Api as startup project

Press F5

Open Swagger:

https://localhost:5001/swagger

📡 API Endpoints
1. Init Upload
POST /api/upload/init
Headers:
  X-User-Id: test-user
  X-Upload-Id: abc123
  X-File-Name: test.csv
  X-File-Size: 10485760
  X-Chunk-Size: 5242880
  X-Total-Chunks: 2
  X-Content-Type: text/csv

2. Upload Chunk
PUT /api/upload/chunk
Headers:
  X-User-Id: test-user
  X-Upload-Id: abc123
  X-Chunk-Index: 0
Content-Range: bytes 0-5242879/10485760
Body: <binary data>

3. Finalize
POST /api/upload/finalize
Headers:
  X-User-Id: test-user
  X-Upload-Id: abc123

4. Status
GET /api/status/abc123


Response:

{
  "uploadId": "abc123",
  "userId": "test-user",
  "state": "Completed",
  "totalBytes": 10485760,
  "receivedBytes": 10485760,
  "totalChunks": 2,
  "receivedChunks": 2,
  "updatedAt": "2025-09-18T12:34:56Z"
}

🧪 Testing

Upload with Swagger UI

Or write a client (C#, JS, Python) that:

Splits file into chunks

Calls init → chunk → finalize

Polls status

🔧 Notes

Default storage path: C:\temp\resumable-upload (configurable in appsettings.json)

All endpoints require X-User-Id header

Files are reassembled & stored locally after completion
