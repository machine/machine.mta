using System;
using System.Transactions;

using Machine.Core.Services;
using Machine.Utility.ThreadPool;

using MassTransit;

namespace Machine.Mta.Internal
{
  public class EndpointQueue : IQueue
  {
    readonly IEndpoint _listeningOn;
    readonly Action<object> _dispatcher;

    public EndpointQueue(IEndpoint listeningOn, Action<object> dispatcher)
    {
      _listeningOn = listeningOn;
      _dispatcher = dispatcher;
    }

    public void Enqueue(IRunnable runnable)
    {
      throw new NotSupportedException();
    }

    public IScope CreateScope()
    {
      return new EndpointScope(_listeningOn, _dispatcher);
    }

    public void Drainstop()
    {
    }
  }
  public class EndpointScope : IScope
  {
    static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(EndpointScope));
    readonly IEndpoint _listeningOn;
    readonly Action<object> _dispatcher;
    readonly TransactionScope _scope;

    public EndpointScope(IEndpoint listeningOn, Action<object> dispatcher)
    {
      _listeningOn = listeningOn;
      _dispatcher = dispatcher;
      _scope = new TransactionScope();
    }

    public IRunnable Dequeue()
    {
      try
      {
        object value = _listeningOn.Receive(TimeSpan.FromSeconds(3), x => x is TransportMessage);
        if (value == null)
        {
          return null;
        }
        return new DispatcherRunnable(_dispatcher, value);
      }
      catch (Exception error)
      {
        _log.Error(error);
        return null;
      }
    }

    public void Complete()
    {
      _scope.Complete();
    }

    public void Dispose()
    {
      _scope.Dispose();
    }
  }
  public class DispatcherRunnable : IRunnable
  {
    readonly Action<object> _dispatcher;
    readonly object _value;

    public DispatcherRunnable(Action<object> dispatcher, object value)
    {
      _dispatcher = dispatcher;
      _value = value;
    }

    public void Run()
    {
      _dispatcher(_value);
    }
  }
}