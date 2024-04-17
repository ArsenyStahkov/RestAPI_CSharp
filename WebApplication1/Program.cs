using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;
using System.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging.EventLog;

namespace WebApplication1.Program
{
    public class ServerParameters
    {
        public string Address { get; set; } = "localhost";
        public int Port { get; set; } = 8085;
        //public int TCPPort { get; set; }
        //public Configuration_Model Configuration { get; set; }
        public string Http { get; set; } = "http";
        public string CombinedAddress { get; set; }
        // Запущен ли веб-браузер
        public bool WebServerStarted { get; set; }
        // Задержка открытия браузера
        public UInt64 BrowserOpenDelay { get; set; }
    }

    public class ProgramMain : Controller
    {
        public static string[] _endpoints;
        public static string[] _scripts;

        static ServerParameters _svPm = new ServerParameters();

        public static void OnStartup()
        {
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

            // Endpoints
            DataTable tableEnpoints = GetDataTable(connection, "SELECT endpoint FROM 'test_get_post';");
            Console.WriteLine($"Прочитано {tableEnpoints.Rows.Count} endpoint-ов GET из таблицы БД\n");
            _endpoints = new string[tableEnpoints.Rows.Count];

            // Scripts
            DataTable tableScripts = GetDataTable(connection, "SELECT script FROM 'test_get_post';");
            Console.WriteLine($"Прочитано {tableScripts.Rows.Count} скриптов GET из таблицы БД\n");
            _scripts = new string[tableScripts.Rows.Count];

            foreach (DataRow row in tableEnpoints.Rows)
            {
                if (row.Field<string>("endpoint") != null)
                    _endpoints[tableEnpoints.Rows.IndexOf(row)] = row.Field<string>("endpoint");
            }

            foreach (DataRow row in tableScripts.Rows)
            {
                if (row.Field<string>("script") != null)
                    _scripts[tableScripts.Rows.IndexOf(row)] = row.Field<string>("script");
            }
        }

        // Инициализация сервера
        public static IWebHostBuilder CreateWebHostBuilder(string[] args, string ContentRoot)
        {
            // Корневой узел конфигурации
            var configuration = new ConfigurationBuilder()

            .AddCommandLine(args)
                    .Build();
            // Вызов конструктора Startup
            Startup.Configuration = configuration;

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.Configure<EventLogSettings>(config =>
                    {
                        config.LogName = "WCS API Service";
                        config.SourceName = "WCS API Service Source";
                    });
                    //services.AddHostedService<Worker>();
                })
                .UseStartup<Startup>()
                .UseContentRoot(ContentRoot)
                .UseKestrel()
                // "http" + "localhost" + ":" + "8085"
                .UseUrls(_svPm.Http + "://" + _svPm.Address + ":" + _svPm.Port)
                .SuppressStatusMessages(true)
                ;
        }

        public static void Main(string[] args)
        {
            // Подключение к БД и формирование массивов
            OnStartup();

            CreateWebHostBuilder(args, Directory.GetCurrentDirectory()).Build().Run();
        }
    }
}
