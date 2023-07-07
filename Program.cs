using CustomerCore.Authentication;
using CustomerCore.Contract;
using CustomerCore.Data;
using CustomerCore.Entities;
using CustomerCore.Enum;
using CustomerCore.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CustomerWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.

           
            builder.Services.AddDbContext<ApplicationDBContext>(
                options => options.UseSqlServer(
                    builder.Configuration["ConnectionStrings:DefaultSQLConnection"]
                    ));

            builder.Services.Configure<CacheConfiguration>(
               builder.Configuration.GetSection("CacheConfiguration")
               );
            
           // var jwtConfig = builder.Configuration.GetSection("JwtAuthConfig").Get<JwtAuthConfig>();
            builder.Services.Configure<JwtAuthConfig>(
               builder.Configuration.GetSection("JwtAuthConfig")
               );
            builder.Services.AddMemoryCache();
            builder.Services.AddTransient<MemoryCacheService>();
            builder.Services.AddTransient<Func<CacheType, ICacheService>>(
                serviceProvider => key =>
                {
                    switch (key)
                    {
                        case CacheType.InMemory:
                            return serviceProvider.GetService<MemoryCacheService>();
                        default:
                            return serviceProvider.GetService<MemoryCacheService>();
                    }
                });

            builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IJwtAuthManager, JwtAuthManager>();
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                config.SaveToken = true;
                config.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey=new SymmetricSecurityKey(Encoding.ASCII.GetBytes("123456789123456789")),
                    ValidateAudience=false,
                    ValidateIssuer = false,
                    ValidateLifetime = false,
                    RequireExpirationTime = false,
                    ClockSkew = TimeSpan.Zero,
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}