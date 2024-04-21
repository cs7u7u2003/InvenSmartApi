using InvenSmartApi.Repositories;
using Microsoft.OpenApi.Models;
using System.Data.SqlClient;
using System.Data;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureCommonServices(services);
        ConfigureCustomServices(services);
    }

    private void ConfigureCommonServices(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "InvenSmartApi", Version = "v1" });
        });

        services.AddTransient<IDbConnection>(_ => new SqlConnection(Configuration.GetConnectionString("ConnectionDb")));

        services.AddControllersWithViews();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAnyPolicy",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });
    }

    private void ConfigureCustomServices(IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IUsuarioService, UsuarioService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InvenSmartApi V1"));

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseCors("AllowAnyPolicy");
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
