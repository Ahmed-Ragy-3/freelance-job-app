using backend;
using backend.FileUpload;
using backend.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<AttachmentOptions>(builder.Configuration.GetSection("Attachment"));
builder.Services.Configure<CloudinaryOptions>(builder.Configuration.GetSection("Cloudinary"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFileUploadService, CloudinaryService>();

builder.Services.AddDbContext<AppDbContext>(cfg => cfg.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(options => {
        options.RoutePrefix = string.Empty;
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Freelance Job API v1");
    });
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
