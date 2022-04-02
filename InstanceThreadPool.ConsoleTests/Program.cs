
//System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());

var messages = Enumerable.Range(1, 1000).Select(i => $"Message-{i}");

var thread_pool = new InstanceThreadPool(10);

//thread_pool.Execute("123", obj => { });

foreach (var message in messages)
    thread_pool.Execute(message, obj =>
    {
        var msg = (string)obj!;
        Console.WriteLine(">> Обработка сообщения {0} начата...", msg);
        Thread.Sleep(100);
        Console.WriteLine(">> Обработка сообщения {0} выполнена", msg);
    });


Console.ReadLine();
