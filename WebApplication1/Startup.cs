using Jint;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; set; }

        public void Configure(IWebHostEnvironment env, Microsoft.Extensions.Hosting.IApplicationLifetime lt)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            builder.Services.AddControllers();  // Добавляем поддержку контроллеров
            WebApplication app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Устанавливаем сопоставление маршрутов с контроллерами
            app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{endpoint?}");

            app.Run();
        }
    }
}
