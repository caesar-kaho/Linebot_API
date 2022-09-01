
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using LineBotMessage.Providers;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Add services to the container.
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    var assembly = Assembly.Load("LineBotMessage");

    builder.RegisterAssemblyTypes(assembly)
    .Where(a => a.Name.EndsWith("Service"))
    .AsImplementedInterfaces();
});

builder.Services.AddHttpClient();
builder.Services.AddControllers();

builder.Services.AddSingleton<JsonProvider>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// add swagger config
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

