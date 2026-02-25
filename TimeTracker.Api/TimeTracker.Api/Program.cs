using TimeTracker.Api.Logics;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.DatabaseConnection;
using TimeTracker.Api.Services;

namespace TimeTracker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<TimeTrackerContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<ActivityLogic>();
            builder.Services.AddScoped<Base64Resizer>();
            builder.Services.AddScoped<CheckFileType>();
            builder.Services.AddScoped<EmailSender>();
            builder.Services.AddScoped<FilePath>();
            builder.Services.AddScoped<RecordLogic>();
            builder.Services.AddScoped<RoleLogic>();
            builder.Services.AddScoped<StatusLogLogic>();
            builder.Services.AddScoped<UserAssignmentLogic>();
            builder.Services.AddScoped<UserLogic>();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.MapControllers();

            app.Run();
        }
    }
}
