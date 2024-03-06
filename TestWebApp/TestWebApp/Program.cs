using Jint;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SQLite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

string source = "D:\\Programming\\C#\\TestWebApp\\TestWebApp\\Databases\\Devices.db";
SQLiteConnection connection = new SQLiteConnection("Data Source=" + source);
connection.Open();
Console.WriteLine("Connected\n");
SQLiteCommand command = new SQLiteCommand(connection);

static DataTable GetDataTable(SQLiteConnection connection, string query)
{
    SQLiteCommand command = new SQLiteCommand(connection);
    command.CommandText = query;

    DataTable dataTable = new DataTable();
    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
    adapter.Fill(dataTable);

    return dataTable;
}

// Endpoints GET
DataTable tableGet = GetDataTable(connection, "SELECT endpoint FROM 'test_get_post' WHERE type='get';");
Console.WriteLine($"Прочитано {tableGet.Rows.Count} endpoint-ов GET из таблицы БД\n");
string[] endpointsGet = new string[tableGet.Rows.Count];

// Endpoints POST
DataTable tablePost = GetDataTable(connection, "SELECT endpoint FROM 'test_get_post' WHERE type='post';");
Console.WriteLine($"Прочитано {tablePost.Rows.Count} endpoint-ов POST из таблицы БД\n");
string[] endpointsPost = new string[tablePost.Rows.Count];

// Scripts GET
DataTable tableScriptsGet = GetDataTable(connection, "SELECT script FROM 'test_get_post' WHERE type='get';");
Console.WriteLine($"Прочитано {tableScriptsGet.Rows.Count} скриптов GET из таблицы БД\n");
string[] scriptsGet = new string[tableScriptsGet.Rows.Count];

// Scripts POST
DataTable tableScriptsPost = GetDataTable(connection, "SELECT script FROM 'test_get_post' WHERE type='post';");
Console.WriteLine($"Прочитано {tableScriptsPost.Rows.Count} скриптов POST из таблицы БД\n");
string[] scriptsPost = new string[tableScriptsPost.Rows.Count];

foreach (DataRow row in tableGet.Rows)
{
    if (row.Field<string>("endpoint") != null)
    {
        endpointsGet[tableGet.Rows.IndexOf(row)] = row.Field<string>("endpoint");
    }
}

foreach (DataRow row in tablePost.Rows)
{
    if (row.Field<string>("endpoint") != null)
    {
        endpointsPost[tablePost.Rows.IndexOf(row)] = row.Field<string>("endpoint");
    }
}

foreach (DataRow row in tableScriptsGet.Rows)
{
    if (row.Field<string>("script") != null)
    {
        scriptsGet[tableScriptsGet.Rows.IndexOf(row)] = row.Field<string>("script");
    }
}

foreach (DataRow row in tableScriptsPost.Rows)
{
    if (row.Field<string>("script") != null)
    {
        scriptsPost[tableScriptsPost.Rows.IndexOf(row)] = row.Field<string>("script");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

[HttpGet("/test-get")]
void Get(int a, int b)
{
    int i = 0;
    for (; i < endpointsGet.Length; i++)
    {
        if (endpointsGet[i] == "/test-get")
        {
            Console.WriteLine("/test-get");
            break;
        }
    }

    Engine engine = new Engine();
    var fromValue = engine.Execute("function " + scriptsGet[i] + "(a, b) " +
        "{ return a + b; }").GetValue(scriptsGet[i]);
    Console.WriteLine(fromValue.Call(a, b));
}

//app.MapGet("/test-get", (int a, int b) =>
//{
//    int i = 0;
//    for (; i < endpointsGet.Length; i++)
//    {
//        if (endpointsGet[i] == "/test-get")
//        {
//            Console.WriteLine("/test-get");
//            break;
//        }
//    }

//    Engine engine = new Engine();
//    var fromValue = engine.Execute("function " + scriptsGet[i] + "(a, b) " +
//        "{ return a + b; }").GetValue(scriptsGet[i]);
//    Console.WriteLine(fromValue.Call(a, b));

//    //return engine.Evaluate((a + b).ToString()).ToObject();
//});

//app.MapGet("/test-get-2", (int a, int b) =>
//{
//    int i = 0;
//    for (; i < endpointsGet.Length; i++)
//    {
//        if (endpointsGet[i] == "/test-get-2")
//        {
//            Console.WriteLine("/test-get-2");
//            break;
//        }
//    }

//    Engine engine = new Engine();
//    var fromValue = engine.Execute("function " + scriptsGet[i] + "(a, b) " +
//        "{ return a + b; }").GetValue(scriptsGet[i]);
//    Console.WriteLine(fromValue.Call(a, b));
//});

app.Run(context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";

    if (context.Request.Path == "/test-post")
    {
        var form = context.Request.Form;
        int firstNum = Int32.Parse(form["firstNum"]);
        int secondNum = Int32.Parse(form["secondNum"]);

        int i = 0;
        for (; i < endpointsPost.Length; i++)
        {
            if (endpointsPost[i] == "/test-post")
            {
                Console.WriteLine("/test-post");
                break;
            }
        }

        Engine engine = new Engine();
        var fromValue = engine.Execute("function " + scriptsPost[i] + "(a, b) " +
            "{ return a + b; }").GetValue(scriptsPost[i]);
        string res = fromValue.Call(firstNum, secondNum).ToString();
        Console.WriteLine(res);

        return context.Response.WriteAsync($"<div><p>Result: {res}</p></div>");
    }
    else
    {
        return context.Response.SendFileAsync("html/index.html");
    }
});

app.Run();
