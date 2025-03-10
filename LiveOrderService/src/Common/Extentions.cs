
using System.Diagnostics;

namespace LiveOrderService.Common.Extensions
{
    public static class Extensions
    {
        public static IEndpointRouteBuilder MapGroup(
            this IEndpointRouteBuilder builder, 
            string path, 
            Action<IEndpointRouteBuilder> handle)
        {
            var route = builder.MapGroup(path);
            handle(route);
            return route;
        }

        public static IServiceCollection AddDockerConfiguration(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("configuration.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            var postgresIp = configuration["POSTGRES_IP"];
            var redisIp = configuration["REDIS_IP"];

            if (postgresIp != "127.0.0.1" || redisIp != "127.0.0.1")
            {
                var processInfo = new ProcessStartInfo("docker-compose", "up -d")
                {
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(processInfo);

                process!.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var error = process.StandardError.ReadToEnd();
                    throw new Exception($"docker-compose failed: {error}");
                }
            }

            return services;
        }
    } 
}