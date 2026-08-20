using System.Net;
using neyrd.core;
using neyrd.core.Benchmark;
using neyrd.core.Benchmark.Handlers;
using neyrd.core.Environment;
using neyrd.core.Events;
using neyrd.core.Models.Messages;
using neyrd.emitter.Capturing;
using neyrd.emitter.Capturing.CoreGraphics;
using neyrd.emitter.Capturing.ScreenCaptureKit;
using neyrd.emitter.Capturing.X11;
using neyrd.emitter.Environment;
using neyrd.emitter.Handlers;
using neyrd.emitter.Networking;
using neyrd.emitter.Puppeting;
using neyrd.emitter.Puppeting.X11;
using Spectre.Console;

IPuppeter? puppeter = null;
NeyrdSender? sender = null;
ICaptureAdapter? adapter =  null;

try
{
    var adapterOption = args.SkipWhile(a => a != "--adapter").Skip(1).FirstOrDefault();
    if (string.IsNullOrWhiteSpace(adapterOption))
    {
        AnsiConsole.Markup("[red]No adapter specified. Please provide an adapter using the --adapter option.[/]");
        Environment.Exit(1);
    }

    var receiverIpAddressOption = args.SkipWhile(a => a != "--receiver-ip").Skip(1).FirstOrDefault();
    if (string.IsNullOrWhiteSpace(receiverIpAddressOption))
    {
        AnsiConsole.Markup(
            "[red]No receiver IP address specified. Please provide an IP address using the --receiver-ip option.[/]");
        Environment.Exit(1);
    }

    AnsiConsole.Write(new FigletText("neyrd emitter").Color(Color.DodgerBlue1));
    AnsiConsole.Write(new Rule($"[grey]v{ThisAssembly.AssemblyInformationalVersion}[/]").LeftJustified());
    AnsiConsole.WriteLine();

    var env = new EnvironmentInfoCollector();
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
    foreach (var a in adapters)
    {
        if (!a.IsSupported)
        {
            AnsiConsole.Markup($"[yellow]Adapter {a.Name} is not supported.[/]");
            AnsiConsole.WriteLine();
        }
        else
        {
            AnsiConsole.Markup($"[green]Adapter {a.Name} is supported.[/]");
            AnsiConsole.WriteLine();

            atLeastOneAdapterSupported = true;
        }
    }

    if (!atLeastOneAdapterSupported)
    {
        AnsiConsole.Markup("[red]No supported adapters found.[/]");
        return;
    }

    var puppeters = new IPuppeter[]
    {
        new X11Puppeter()
    };

    var atLeastOnePuppeterSupported = false;
    foreach (var p in puppeters)
    {
        if (!p.IsSupported)
        {
            AnsiConsole.Markup($"[yellow]Puppeter {p.Name} is not supported.[/]");
            AnsiConsole.WriteLine();
        }
        else
        {
            AnsiConsole.Markup($"[green]Puppeter {p.Name} is supported.[/]");
            AnsiConsole.WriteLine();

            atLeastOnePuppeterSupported = true;
        }
    }

    if (!atLeastOnePuppeterSupported)
    {
        AnsiConsole.Markup("[red]No supported puppeters found.[/]");
        return;
    }

    puppeter = puppeters.First(p => p.IsSupported && p.Name == adapterOption);
    AnsiConsole.WriteLine($"Initializing puppeter `{puppeter.Name}`...");
    puppeter.Initialize();

    AnsiConsole.WriteLine("Registering handlers...");

    sender = new NeyrdSender(IPAddress.Parse(NetworkInfoCollector.NetworkInterfaces.First()),
        IPAddress.Parse(receiverIpAddressOption));
    EventPipeline.Subscribe(new TestStartedEventHandler());
    EventPipeline.Subscribe(new TestReceivedHandler());
    EventPipeline.Subscribe(new TestCompletedHandler());
    EventPipeline.Subscribe(new AcknowledgementReceivedEventHandler(sender));
    EventPipeline.Subscribe(new PointerMovedEventHandler(puppeter));
    EventPipeline.Subscribe(new PointerPressedEventHandler(puppeter));
    EventPipeline.Subscribe(new PointerWheelChangedEventHandler(puppeter));
    EventPipeline.Subscribe(new KeyPressedDownEventHandler(puppeter));
    EventPipeline.Subscribe(new KeyPressedUpEventHandler(puppeter));

    AnsiConsole.WriteLine("Initializing listener...");

    var cts = new CancellationTokenSource();
    var listener = new NeyrdListener(IPAddress.Parse(NetworkInfoCollector.NetworkInterfaces.First()));
    _ = listener.BeginListeningAsync(cts.Token);

    AnsiConsole.WriteLine("Connecting with receiver...");

    var test = await sender.TestConnectionAsync();

    if (test.IsSuccessful)
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

    while (!TestSuite.HasCompleted && i < 100)
    {
        i++;
        await Task.Delay(100);
    }

    if (!TestSuite.HasCompleted)
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
    AnsiConsole.WriteLine("Checking resolution...");
    var resolution = puppeter.GetScreenSize();
    AnsiConsole.WriteLine($"Width: {resolution.width}, Height: {resolution.height}");
    await sender.Send(ScreenResolutionMessage.ToMessage(resolution.width, resolution.height));

    AnsiConsole.WriteLine();
    AnsiConsole.WriteLine("Initializing adapter...");
    adapter = adapters.First(a => a.IsSupported && a.Name == adapterOption);
    adapter.Initialize();

    AnsiConsole.WriteLine("Capturing. You can minimize the window.");

    var capture = new CapturePipeline(adapter, sender, cts.Token);
    await capture.Begin();

    Console.ReadKey();

    AnsiConsole.WriteLine("Cleanup...");
}
finally
{
    puppeter?.Dispose();
    sender?.Dispose();
    adapter?.Dispose();
}