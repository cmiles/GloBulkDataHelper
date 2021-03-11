using System;
using Serilog;
using Serilog.Formatting.Compact;

namespace GloDb
{
    public static class Logging
    {
        public static string ObjectDumpForLog(this object toDump)
        {
            return ObjectDumper.Dump(toDump, new DumpOptions {MaxLevel = 2, DumpStyle = DumpStyle.Console});
        }

        public static LoggerConfiguration StandardConfiguration(string fileNamePostFix)
        {
            return new LoggerConfiguration()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .WriteTo.Console()
                .WriteTo.File(new RenderedCompactJsonFormatter(),
                    $"GloLog-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}-{fileNamePostFix}.txt",
                    rollingInterval: RollingInterval.Day, shared: true);
            ;
        }
    }
}