using Serilog;

namespace Api.Extensoes;

public static class LoggingSetup
{
	public static void ConfigurarLogs(this ConfigureHostBuilder host)
	{
		host.UseSerilog((context, loggerConfig) => { loggerConfig.ReadFrom.Configuration(context.Configuration); });
	}
}