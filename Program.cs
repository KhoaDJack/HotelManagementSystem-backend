using Dapper;
using HotelDBFinal.InterfaceAndServiceSystem;
using HotelDBMiddle;
using HotelDBMiddle.Infrastructure.Models;
using HotelDBMiddle.Interfaces_And_Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using HotelDBFinal.Interfaces_And_Service;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDbConnection>(cnn => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
//jwt config
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
var appSettings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.AccessTokenSecret);

// Add DapperContext
builder.Services.AddSingleton<DapperContext>();
//old
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
//New System
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IGuestServiceService, GuestServiceService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthJWTService, AuthJWTService>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder
            .WithOrigins("http://localhost:5173") // your frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
//add security for Swagger
builder.Services.AddSwaggerGen((config) =>
{
    //AddSecurityDefinition
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer ey...')",
    });
    //AddSecurityRequirement
    config.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference{
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                    },
                    Array.Empty<string>()
                }
    });
});

//add authentication b7
builder.Services.AddAuthentication(ops =>
{
    ops.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    ops.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(ops =>
{
    ops.SaveToken = true; //lưu token
    ops.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = appSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = appSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
        options.AccessDeniedPath = "/api/auth/denied";
    });

builder.Services.AddAuthorization();
//add authentication b7

// Configure the HTTP request pipeline.
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//usemiddleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("../swagger/v1/swagger.json", "DemoAPI v1");
});
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowLocalhost");
app.MapGet("/", context =>
{
    context.Response.Redirect("/api/auth/login");
    return Task.CompletedTask;
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=Index}/{id?}"); // default to Login controller & Index action
});


app.MapControllers();
app.Run();

