using Api.Middleware;
using Application.Abstractions;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddScoped<IChunkStorage, FileSystemChunkStorage>();
builder.Services.AddScoped<IUploadManager, UploadManager>();
builder.Services.AddScoped<UserContextMiddleware>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<UserContextMiddleware>();

app.MapControllers();

app.Run();
