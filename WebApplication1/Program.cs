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
        // ������� �� ���-�������
        public bool WebServerStarted { get; set; }
        // �������� �������� ��������
        public UInt64 BrowserOpenDelay { get; set; }
    }

    public class ProgramMain : Controller
    {
        public static string[] _endpointsGet;
        public static string[] _endpointsPost;
        public static string[] _scriptsGet;
        public static string[] _scriptsPost;

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

            // Endpoints GET
            DataTable tableGet = GetDataTable(connection, "SELECT endpoint FROM 'test_get_post' WHERE type='get';");
            Console.WriteLine($"��������� {tableGet.Rows.Count} endpoint-�� GET �� ������� ��\n");
            _endpointsGet = new string[tableGet.Rows.Count];

            // Endpoints POST
            DataTable tablePost = GetDataTable(connection, "SELECT endpoint FROM 'test_get_post' WHERE type='post';");
            Console.WriteLine($"��������� {tablePost.Rows.Count} endpoint-�� POST �� ������� ��\n");
            _endpointsPost = new string[tablePost.Rows.Count];

            // Scripts GET
            DataTable tableScriptsGet = GetDataTable(connection, "SELECT script FROM 'test_get_post' WHERE type='get';");
            Console.WriteLine($"��������� {tableScriptsGet.Rows.Count} �������� GET �� ������� ��\n");
            _scriptsGet = new string[tableScriptsGet.Rows.Count];

            // Scripts POST
            DataTable tableScriptsPost = GetDataTable(connection, "SELECT script FROM 'test_get_post' WHERE type='post';");
            Console.WriteLine($"��������� {tableScriptsPost.Rows.Count} �������� POST �� ������� ��\n");
            _scriptsPost = new string[tableScriptsPost.Rows.Count];

            foreach (DataRow row in tableGet.Rows)
            {
                if (row.Field<string>("endpoint") != null)
                    _endpointsGet[tableGet.Rows.IndexOf(row)] = row.Field<string>("endpoint");
            }

            foreach (DataRow row in tablePost.Rows)
            {
                if (row.Field<string>("endpoint") != null)
                    _endpointsPost[tablePost.Rows.IndexOf(row)] = row.Field<string>("endpoint");
            }

            foreach (DataRow row in tableScriptsGet.Rows)
            {
                if (row.Field<string>("script") != null)
                    _scriptsGet[tableScriptsGet.Rows.IndexOf(row)] = row.Field<string>("script");
            }

            foreach (DataRow row in tableScriptsPost.Rows)
            {
                if (row.Field<string>("script") != null)
                    _scriptsPost[tableScriptsPost.Rows.IndexOf(row)] = row.Field<string>("script");
            }
        }

        // ������������� �������
        public static IWebHostBuilder CreateWebHostBuilder(string[] args, string ContentRoot)
        {
            // �������� ���� ������������
            var configuration = new ConfigurationBuilder()

            .AddCommandLine(args)
                    .Build();
            // ����� ������������ Startup
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
            // ����������� � �� � ������������ ��������
            OnStartup();

            CreateWebHostBuilder(args, Directory.GetCurrentDirectory()).Build().Run();
        }
    }
}
