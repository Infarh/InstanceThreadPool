namespace InstanceThreadPool;

public class InstanceThreadPool
{
    private readonly ThreadPriority _Prioroty;
    private readonly string? _Name;
    private readonly Thread[] _Threads;
    private readonly Queue<(Action<object?> Work, object? Parameter)> _Works = new();

    private readonly AutoResetEvent _WorkingEvent = new(false);
    private readonly AutoResetEvent _ExecuteEvent = new(true);

    public InstanceThreadPool(int MaxThreadsCount, ThreadPriority Prioroty = ThreadPriority.Normal, string? Name = null)
    {
        if (MaxThreadsCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(MaxThreadsCount), MaxThreadsCount, "Число потоков в пуле должно быть больше, либо равно 1");

        _Prioroty = Prioroty;
        _Name = Name;
        _Threads = new Thread[MaxThreadsCount];
    }

    private void Initialize()
    {
        for (var i = 0; i < _Threads.Length; i++)
        {
            var name = $"{nameof(InstanceThreadPool)}[{_Name ?? GetHashCode().ToString("x")}]-Thread[{i}]";
            var thread = new Thread(WorkingThread)
            {
                Name = name,
                IsBackground = true,
                Priority = _Prioroty
            };
            _Threads[i] = thread;
            thread.Start();
        }
    }

    public void Execute(Action Work) => Execute(null, _ => Work());

    public void Execute(object? Parameter, Action<object?> Work)
    {
        _ExecuteEvent.WaitOne(); // запрашиваем доступ к очереди
        _Works.Enqueue((Work, Parameter));
        _ExecuteEvent.Set();    // разрешаем доступ к очереди

        _WorkingEvent.Set();
    }

    private void WorkingThread()
    {
        while (true)
        {
            _WorkingEvent.WaitOne();

            _ExecuteEvent.WaitOne(); // запрашиваем доступ к очереди
            var (work, parameter) = _Works.Dequeue();
            _ExecuteEvent.Set();    // разрешаем доступ к очереди

            work(parameter);
        }

    }
}
