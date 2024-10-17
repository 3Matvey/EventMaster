using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EventMaster.Server.Data;
using Microsoft.EntityFrameworkCore;
using EventMaster.Server.Repositories.Implementation;
using EventMaster.Server.Repositories.Interfaces;
using EventMaster.Server.UnitOfWork;
using EventMaster.Server.Dto;
using EventMaster.Server.UseCases;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Регистрация Unit of Work и репозиториев
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();

// Регистрация use cases
builder.Services.AddScoped<CreateEventUseCase>();
builder.Services.AddScoped<DeleteEventUseCase>();
builder.Services.AddScoped<GetAllEventsUseCase>();
builder.Services.AddScoped<GetEventDetailsUseCase>();
builder.Services.AddScoped<GetFilteredEventsUseCase>();
builder.Services.AddScoped<GetRegisteredEventsUseCase>();
builder.Services.AddScoped<GetUserByIdUseCase>();
builder.Services.AddScoped<LoginUserUseCase>();
builder.Services.AddScoped<RegisterUserToEventUseCase>();
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<UnregisterUserFromEventUseCase>();
builder.Services.AddScoped<UpdateEventUseCase>();
builder.Services.AddScoped<UploadEventImageUseCase>();


// JWT настройки
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddSingleton(typeof(JwtSettings));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["ValidIssuer"],
            ValidAudience = jwtSettings["ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["IssuerSigningKey"]!))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"))
    .AddPolicy("UserPolicy", policy => policy.RequireRole("User"));

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// + ExceptionMiddleware
app.UseMiddleware<EventMaster.Server.Middlewares.ExceptionMiddleware>();

app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();