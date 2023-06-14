
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using LineBotMessage.Models;
using LineBotMessage.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(option =>
{
    option.AddPolicy(name: "policy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

string sConnectionString =
    builder.Configuration.GetConnectionString("LinebotContextCS");
builder.Services.AddDbContext<LinebotAPIContext>(Options => Options.UseMySql(sConnectionString,
    MariaDbServerVersion.AutoDetect(sConnectionString))
    .EnableDetailedErrors());


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<JsonProvider>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// add swagger config
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("policy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "UploadFiles")),
    RequestPath = "/UploadFiles"
});

app.MapControllers();
app.Run();