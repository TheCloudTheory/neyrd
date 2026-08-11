using neyrd.core;
using neyrd.emitter.Environment;
using neyrd.emitter.Networking;
using Spectre.Console;

AnsiConsole.Write(new FigletText("neyrd emitter").Color(Color.DodgerBlue1));
AnsiConsole.Write(new Rule($"[grey]v{ThisAssembly.AssemblyInformationalVersion}[/]").LeftJustified());
AnsiConsole.WriteLine();

var env = new EnvironmentInfoCollector();
var net = new NetworkInfoCollector();
var dotnet = new DotnetInfoCollector();

var table = new Table()
    .Border(TableBorder.Rounded)
    .BorderColor(Color.Grey)
    .AddColumn(new TableColumn("[bold grey]Property[/]").Width(20))
    .AddColumn("[bold white]Value[/]");

table.AddRow("[blue]OS[/]", $"{env.OsName} {env.OsVersion}");
table.AddRow("[blue]CPU Cores[/]", env.Cpu);
table.AddRow("[blue]Network[/]", string.Join(", ", net.NetworkInterfaces));
table.AddRow("[blue]Threads[/]", dotnet.AvailableThreads.ToString());
table.AddRow("[blue]GC Mode[/]", dotnet.GarbageCollectorMode);

AnsiConsole.Write(table);
AnsiConsole.WriteLine();

AnsiConsole.WriteLine("Performing self-check if data can be collected...");

AnsiConsole.WriteLine("Initializing listener...");

var cts = new CancellationTokenSource();
var listener = new NeyrdListener(net.NetworkInterfaces.First());
_ = listener.BeginListeningAsync(cts.Token);

AnsiConsole.WriteLine("Connecting with receiver...");

var connectionManager = new NeyrdSender(net.NetworkInterfaces.First());
var test = await connectionManager.TestConnectionAsync();

if(test.IsSuccessful)
{
    AnsiConsole.WriteLine("Connection successful.");
}
else
{
    AnsiConsole.WriteLine("Connection failed.");
    AnsiConsole.WriteLine($"Error: {test.ErrorMessage}");
    AnsiConsole.WriteLine($"Exception: {test.Exception}");
}

Console.ReadKey();