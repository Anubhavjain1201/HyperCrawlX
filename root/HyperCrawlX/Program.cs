using HyperCrawlX.BackgroundWorkers;
using HyperCrawlX.DAL;
using HyperCrawlX.Middlewares;
using HyperCrawlX.Services;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.RegisterServices();
builder.Services.RegisterDbDependencies();
builder.Services.AddHostedService<AsyncCrawler>();

// Use Gzip compression
builder.Services.Configure<GzipCompressionProviderOptions>
    (options => options.Level = CompressionLevel.Optimal);
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.EnableForHttps = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();

app.AddCustomMiddlewares();
app.UseCors(options =>
{
    options.AllowAnyMethod();
    options.AllowAnyHeader();
    options.AllowCredentials();
    options.SetIsOriginAllowed(origin => true);
});

app.UseCors(options => options.AllowAnyOrigin());

//app.UseHttpsRedirection();
app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();

app.Run();
