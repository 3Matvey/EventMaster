using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EventMaster.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EventMaster.Infrastructure.Repositories;
using EventMaster.Application.Interfaces.Repositories;
using EventMaster.Application.Interfaces.UnitOfWork;
using EventMaster.Infrastructure.UnitOfWork;
using EventMaster.Application.UseCases;
using EventMaster.Web.Middlewares;
using EventMaster.Application.Mapping;
using EventMaster.Application.Interfaces.JwtService;
using EventMaster.Infrastructure.JwtService;
using EventMaster.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// ��������� DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� ������������
builder.Services.AddControllers()
   .AddNewtonsoftJson(options =>
      options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// ��������� Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ��������� CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// ����������� Unit of Work � ������������
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// ����������� Use Cases
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

// ��������� JwtSettings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// ��������� JwtSettings �� ������������
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

// ��������� ��������������
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.ValidIssuer,
            ValidAudience = jwtSettings.ValidAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey))
        };
    });

// ��������� ������� �����������
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"))
    .AddPolicy("UserPolicy", policy => policy.RequireRole("User"));

// ��������� AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// ������������� Middleware ��� ��������� ����������
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

// ��������� HTTP-���������
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
