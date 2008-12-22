using System;
using System.Collections.Generic;

using Machine.Container.Services;

namespace Machine.Mta.Internal
{
  public class MessageBusManager : IMessageBusManager, IDisposable
  {
    private readonly IMachineContainer _container;
    private readonly IMessageBusFactory _messageBusFactory;
    private IMessageBus _bus;

    public MessageBusManager(IMessageBusFactory messageBusFactory, IMachineContainer container)
    {
      _messageBusFactory = messageBusFactory;
      _container = container;
    }

    #region IMessageBusManager Members
    public IMessageBus UseSingleBus(EndpointName listeningEndpoint, EndpointName poisonEndpoint)
    {
      _bus = _messageBusFactory.CreateMessageBus(listeningEndpoint, poisonEndpoint);
      _container.Register.Type<IMessageBus>().Is(_bus);
      return _bus;
    }
    #endregion

    #region IDisposable Members
    public void Dispose()
    {
      if (_bus != null)
      {
        _bus.Dispose();
      }
    }
    #endregion
  }
}