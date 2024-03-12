using Microsoft.AspNetCore.Mvc;
using Jint;
using WebApplication1.Program;
using Microsoft.Extensions.Options;

namespace WebApplication1.Controllers
{
    [Route("/{endpoint}")]
    public class HomeController : ProgramMain
    {
        Engine engine = new Engine();

        [HttpGet]
        public void Get(List<int> options)
        {
            string endpoint = "/" + RouteData.Values["endpoint"].ToString();

            int i = 0;
            for (; i < _endpointsGet.Length; i++)
            {
                if (_endpointsGet[i] == endpoint)
                {
                    Console.WriteLine("Endpoint: " + endpoint);
                    break;
                }

                if (i == _endpointsGet.Length - 1)
                {
                    return;
                }
            }

            var fromValue = engine.Execute("function " + _scriptsGet[i] + "(option) " +
                "{ return option; }").GetValue(_scriptsGet[i]);

            foreach (var option in options)
                Console.Write(fromValue.Call(option) + "\t");

            Console.WriteLine();
        }

        [HttpPost]
        public IActionResult Post(HttpContext context)
        {
            context.Response.ContentType = "text/html; charset=utf-8";

            if (context.Request.Path == "/test-post")
            {
                IFormCollection form = context.Request.Form;
                int first = Int32.Parse(form["first"]);
                int second = Int32.Parse(form["second"]);
                int sum = first + second;

                int i = 0;
                for (; i < _endpointsPost.Length; i++)
                {
                    if (_endpointsPost[i] == "/test-post")
                    {
                        Console.WriteLine("Endpoint: /test-post");
                        break;
                    }
                }

                //var fromValue = engine.Execute("function " + _scriptsGet[i] + "(option) " +
                //    "{ return option; }").GetValue(_scriptsGet[i]);

                //foreach (var option in options)
                //    Console.Write(fromValue.Call(option) + "\t");

                //Console.WriteLine();

                return CreatedAtAction("", context.Response.WriteAsync($"<div><p>Result: {sum}</p></div>"));
            }
            else
            {
                return CreatedAtAction("", context.Response.SendFileAsync("html/index.html"));
            }
        }
    }
}
