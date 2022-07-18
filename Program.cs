using EmailSenderService;
using EmailSenderService.Models;
using EmailSenderService.Services;
using EmailSenderService.SmtpConfiguration;
using EmailSenderService.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddTransient<IValidator<EmailModel>, EmailModelValidator>();
var connecionString = builder.Configuration.GetConnectionString("ConString");
builder.Services.AddDbContext<EmailAppDbContext>(option =>
{
    option.UseSqlServer(connecionString);
});

builder.Configuration.AddJsonFile("smtpConfiguration.json");
builder.Services.Configure<From>(builder.Configuration.GetSection("from"));
builder.Services.Configure<AuthenticatedData>(builder.Configuration.GetSection("authenticatedData"));
builder.Services.Configure<SmtpServer>(builder.Configuration.GetSection("smtpServer"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
