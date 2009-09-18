using System;
using System.Collections.Generic;

using Machine.Container;
using Machine.Mta.Dispatching;
using Machine.Mta.Endpoints;
using Machine.Utility.ThreadPool;

namespace Machine.Mta
{
  public class MessageBusFactory : IMessageBusFactory
  {
    readonly IEndpointResolver _endpointResolver;
    readonly IMessageDestinations _messageDestinations;
    readonly TransportMessageBodySerializer _transportMessageBodySerializer;
    readonly ITransactionManager _transactionManager;
    readonly IMachineContainer _container;
    readonly IMessageAspectsProvider _messageAspectsProvider;
    readonly IEndpointHandlerRules _handlerRules;
    readonly IMessageFailureManager _messageFailureManager;

    public MessageBusFactory(IEndpointResolver endpointResolver, IMessageDestinations messageDestinations, TransportMessageBodySerializer transportMessageBodySerializer, ITransactionManager transactionManager, IMachineContainer container, IMessageAspectsProvider messageAspectsProvider, IEndpointHandlerRules handlerRules, IMessageFailureManager messageFailureManager)
    {
      _endpointResolver = endpointResolver;
      _messageFailureManager = messageFailureManager;
      _handlerRules = handlerRules;
      _messageAspectsProvider = messageAspectsProvider;
      _container = container;
      _transactionManager = transactionManager;
      _messageDestinations = messageDestinations;
      _transportMessageBodySerializer = transportMessageBodySerializer;
    }

    public IMessageBus CreateMessageBus(EndpointAddress listeningOnEndpointAddress, EndpointAddress poisonEndpointAddress, IProvideHandlerTypes handlerTypes, ThreadPoolConfiguration threadPoolConfiguration)
    {
      MessageDispatcher dispatcher = new MessageDispatcher(_container, _messageAspectsProvider, new EndpointHandlerFilter(_handlerRules, listeningOnEndpointAddress, handlerTypes));
      return new MessageBus(_endpointResolver, _messageDestinations, _transportMessageBodySerializer, dispatcher, listeningOnEndpointAddress, poisonEndpointAddress, _transactionManager, _messageFailureManager, threadPoolConfiguration);
    }
  }

  public class EndpointHandlerFilter : IProvideHandlerTypes
  {
    readonly IEndpointHandlerRules _handlerRules;
    readonly EndpointAddress _address;
    readonly IProvideHandlerTypes _target;

    public EndpointHandlerFilter(IEndpointHandlerRules handlerRules, EndpointAddress address, IProvideHandlerTypes target)
    {
      _handlerRules = handlerRules;
      _target = target;
      _address = address;
    }

    public IEnumerable<Type> HandlerTypes()
    {
      foreach (Type type in _target.HandlerTypes())
      {
        if (_handlerRules.ApplyRules(_address, type))
        {
          yield return type;
        }
      }
    }
  }
}