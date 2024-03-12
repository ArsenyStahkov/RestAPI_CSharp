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
            builder.Services.AddControllers();  // добавляем поддержку контроллеров
            WebApplication app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // устанавливаем сопоставление маршрутов с контроллерами
            app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{endpoint?}");

            //app.Run(async (context) =>
            //{
            //    context.Response.ContentType = "text/html; charset=utf-8";

            //    if (context.Request.Path == "/test-post")
            //    {
            //        var form = context.Request.Form;
            //        int first = Int32.Parse(form["first"]);
            //        int second = Int32.Parse(form["second"]);
            //        int sum = first + second;

            //        await context.Response.WriteAsync($"<div><p>Result: {sum}</p></div>");
            //    }
            //    else
            //    {
            //        await context.Response.SendFileAsync("html/index.html");
            //    }
            //});

            app.Run();
        }
    }
}
