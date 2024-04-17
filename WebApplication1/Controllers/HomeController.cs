using Microsoft.AspNetCore.Mvc;
using Jint;
using WebApplication1.Program;

namespace WebApplication1.Controllers
{
    [Route("/{endpoint}")]
    public class HomeController : ProgramMain
    {
        public void UseJint(List<string> options, string endpoint)
        {
            int i = 0;
            for (; i < _endpoints.Length; i++)
            {
                if (_endpoints[i] == endpoint)
                {
                    Console.WriteLine("Endpoint: " + endpoint);
                    break;
                }

                if (i == _endpoints.Length - 1)
                {
                    return;
                }
            }

            if (options.Count > 0)
            {
                Engine engine = new Engine();

                engine.SetValue("log", new Action<object>(Console.WriteLine));
                engine.Execute(_scripts[i]);
                engine.Invoke("GetArr", options);
            }
        }

        [HttpGet]
        public async Task Get(List<string> options)
        {
            string endpoint = "/" + RouteData.Values["endpoint"].ToString();
            UseJint(options, endpoint);
            StreamReader reader = new StreamReader("html/index.html");
            string content = await reader.ReadToEndAsync();
            Response.ContentType = "text/html;charset=utf-8";

            await Response.WriteAsync(content);
        }

        [HttpPost]
        public List<string> Post()
        {
            string[] optArray = Request.Form["options[]"];
            List<string> options = new List<string>();
            for (int i = 0; i < optArray.Length; i++)
                options.Add(optArray[i]);

            string endpoint = "/" + RouteData.Values["endpoint"].ToString();
            UseJint(options, endpoint);

            return options;
        }
    }
}
