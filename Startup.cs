using InvenSmartApi.Infrastructure.Database;
using InvenSmartApi.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using System.Data;

namespace InvenSmartApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "InvenSmartApi",
                    Version = "v1",
                    Description = "API principal del sistema InvenSmart"
                });
            });

            services.AddLogging();

            // Dapper: conexión por request (Scoped)
            services.AddScoped<IDbConnection>(_ =>
                new SqlConnection(Configuration.GetConnectionString("ConnectionDb")));

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyPolicy", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });

            // ---- Dependencias propias ----
            services.AddSingleton<InvenSmartApi.Utils.ErrorLogger>();

            // ---- Database Initializer (SOLO UNO) ----
            services.Configure<DatabaseInitializerOptions>(
                Configuration.GetSection("DatabaseInitializer"));

            services.AddHostedService<DbInitializerHostedService>();

            // Repos/Services
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IPermisoRepository, PermisoRepository>();
            services.AddScoped<IUsuarioService, UsuarioService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "InvenSmartApi v1");
                c.RoutePrefix = "swagger";
            });

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("AllowAnyPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
