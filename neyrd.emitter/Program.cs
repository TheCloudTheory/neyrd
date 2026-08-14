using System.Net;
using neyrd.core;
using neyrd.core.Benchmark;
using neyrd.core.Benchmark.Handlers;
using neyrd.core.Environment;
using neyrd.core.Events;
using neyrd.emitter.Capturing;
using neyrd.emitter.Capturing.CoreGraphics;
using neyrd.emitter.Capturing.ScreenCaptureKit;
using neyrd.emitter.Capturing.X11;
using neyrd.emitter.Environment;
using neyrd.emitter.Handlers;
using neyrd.emitter.Networking;
using Spectre.Console;

var adapterOption = args.SkipWhile(a => a != "--adapter").Skip(1).FirstOrDefault();
if (string.IsNullOrWhiteSpace(adapterOption))
{
    AnsiConsole.Markup("[red]No adapter specified. Please provide an adapter using the --adapter option.[/]");
    Environment.Exit(1);
}

var receiverIpAddressOption = args.SkipWhile(a => a != "--receiver-ip").Skip(1).FirstOrDefault();
if (string.IsNullOrWhiteSpace(receiverIpAddressOption))
{
    AnsiConsole.Markup("[red]No receiver IP address specified. Please provide an IP address using the --receiver-ip option.[/]");
    Environment.Exit(1);
}

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
table.AddRow("[blue]Network[/]", string.Join(", ", NetworkInfoCollector.NetworkInterfaces));
table.AddRow("[blue]Threads[/]", dotnet.AvailableThreads.ToString());
table.AddRow("[blue]GC Mode[/]", dotnet.GarbageCollectorMode);

AnsiConsole.Write(table);
AnsiConsole.WriteLine();

AnsiConsole.WriteLine("Performing self-check if data can be collected...");

var adapters = new ICaptureAdapter[]
{
    new X11Capture(),
    new CoreGraphicsCapture(),
    new ScreenCaptureKitCapture()
};

var atLeastOneAdapterSupported = false;
foreach(var adapter in adapters)
{
    if(!adapter.IsSupported)
    {
        AnsiConsole.Markup($"[yellow]Adapter {adapter.Name} is not supported.[/]");
        AnsiConsole.WriteLine();
    }
    else
    {
        AnsiConsole.Markup($"[green]Adapter {adapter.Name} is supported.[/]");
        AnsiConsole.WriteLine();
        
        atLeastOneAdapterSupported = true;
    }
}

if(!atLeastOneAdapterSupported)
{
    AnsiConsole.Markup("[red]No supported adapters found.[/]");
    return;
}

AnsiConsole.WriteLine("Registering handlers...");

var sender = new NeyrdSender(IPAddress.Parse(NetworkInfoCollector.NetworkInterfaces.First()), IPAddress.Parse(receiverIpAddressOption));

EventPipeline.Subscribe(new TestStartedEventHandler());
EventPipeline.Subscribe(new TestReceivedHandler());
EventPipeline.Subscribe(new TestCompletedHandler());
EventPipeline.Subscribe(new AcknowledgementReceivedHandler(sender));

AnsiConsole.WriteLine("Initializing listener...");

var cts = new CancellationTokenSource();
var listener = new NeyrdListener(IPAddress.Parse(NetworkInfoCollector.NetworkInterfaces.First()));
_ = listener.BeginListeningAsync(cts.Token);

AnsiConsole.WriteLine("Connecting with receiver...");

var test = await sender.TestConnectionAsync();

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

AnsiConsole.WriteLine("Waiting for test to complete...");
var i = 0;

while(!TestSuite.HasCompleted && i < 100)
{
    i++;
    await Task.Delay(100);
}

if(!TestSuite.HasCompleted)
{
    AnsiConsole.WriteLine("Test timed out.");
    Environment.Exit(1);
}
else
{
    AnsiConsole.WriteLine("Test completed. Results:");
    AnsiConsole.Write(TestSuite.DisplayResults());
}

AnsiConsole.WriteLine();
AnsiConsole.WriteLine("Synchronizing emitter and receiver...");


AnsiConsole.WriteLine();
AnsiConsole.WriteLine("Capturing. You can minimize the window.");

var capture = new CapturePipeline(adapters.First(adapter => adapter.IsSupported && adapter.Name == adapterOption), sender, cts.Token);
await capture.Begin();

Console.ReadKey();