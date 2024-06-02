using BlossomServer.Datas;
using BlossomServer.Entities;
using BlossomServer.Services.AuthenticationServices;
using BlossomServer.Services.BookingServices;
using BlossomServer.Services.CategoryServices;
using BlossomServer.Services.ContactServices;
using BlossomServer.Services.FileServices;
using BlossomServer.Services.HubServices;
using BlossomServer.Services.NotificationServices;
using BlossomServer.Services.ProductServices;
using BlossomServer.Services.RoleServices;
using BlossomServer.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using BlossomServer.Services.EmailService;
using BlossomServer.Services.ProductImageServices;
using BlossomServer.Services.WhatsappServices;
using BlossomServer.Services.PaymentServices;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BlossomServer.Datas.Chat;
using BlossomServer.Services.ChatServices;

var builder = WebApplication.CreateBuilder(args);

// Set environment
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"F:/Workspace/BlossomNails/BlossomServer/wwwroot/config/blossom-nails-firebase-sdk.json");

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JWT_Configuration:Issuer"],
                        ValidAudience = builder.Configuration["JWT_Configuration:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT_Configuration:SecretKey"]!))
                    };
                }).AddJwtBearer("Firebase", options =>
                {
                    options.Authority = builder.Configuration["Firebase_Configuration:Issuer"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Firebase_Configuration:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Firebase_Configuration:Audience"],
                        ValidateLifetime = true
                    };
                });

builder.Services.AddDbContext<BlossomNailsContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.Configure<MongoDBSetting>(builder.Configuration.GetSection("MongoDBConfiguration"));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(x => x.WithOrigins(
        "http://localhost:3000",
        "http://localhost:3001",
        "https://blossom-nails-client.vercel.app",
        "https://blossom-nails-admin.vercel.app",
        "https://localhost:7176"
        ).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
});

builder.Services.AddSignalR(e => e.MaximumReceiveMessageSize = 102400000);

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IWhatsappService, WhatsappService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IChatService, ChatService>();
/*builder.Services.AddScoped<IEmailService, EmailService>();*/

builder.Services.AddSingleton<IDictionary<string, UserConnection>>(opts => new Dictionary<string, UserConnection>());
builder.Services.AddSingleton<IDictionary<string, ChatConnection>>(opts => new Dictionary<string, ChatConnection>());
builder.Services.AddSingleton(builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Blossom Nails API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}*/
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapHub<HubService>("/notify");
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
//app.Run("http://0.0.0.0:80"); Change port here to EXPOSE in docker
