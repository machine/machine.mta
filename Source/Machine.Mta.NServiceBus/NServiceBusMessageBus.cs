using System;
using System.Collections.Generic;

using NServiceBus;

namespace Machine.Mta
{
  public class NServiceBusMessageBus : IMessageBus
  {
    readonly NsbBus _bus;

    IBus CurrentBus()
    {
      return _bus.Bus;
    }

    public NServiceBusMessageBus(NsbBus bus)
    {
      _bus = bus;
    }

    public void Dispose()
    {
    }

    public EndpointAddress PoisonAddress
    {
      get { return _bus.PoisonAddress; }
    }

    public EndpointAddress Address
    {
      get { return _bus.ListenAddress; }
    }

    public void Start()
    {
    }

    public void Send<T>(params T[] messages) where T : IMessage
    {
      CurrentBus().Send(messages.ToNsbMessages());
    }

    public void Send<T>(string correlationId, params T[] messages) where T : IMessage
    {
      throw new NotImplementedException();
    }

    public void Send<T>(EndpointAddress destination, params T[] messages) where T : IMessage
    {
      CurrentBus().Send(destination.ToNsbAddress(), messages.ToNsbMessages());
    }

    public void Send(EndpointAddress destination, MessagePayload payload)
    {
      throw new NotImplementedException();
    }

    public void SendLocal<T>(params T[] messages) where T : IMessage
    {
      CurrentBus().SendLocal(messages.ToNsbMessages());
    }

    public void Stop()
    {
    }

    public IRequestReplyBuilder Request<T>(params T[] messages) where T : IMessage
    {
      return new NServiceBusRequestReplyBuilder(CurrentBus(), null, messages.ToNsbMessages());
    }

    public IRequestReplyBuilder Request<T>(string correlationId, params T[] messages) where T : IMessage
    {
      return new NServiceBusRequestReplyBuilder(CurrentBus(), correlationId, messages.ToNsbMessages());
    }

    public void Reply<T>(params T[] messages) where T : IMessage
    {
      CurrentBus().Reply(messages.ToNsbMessages());
    }

    public void Reply<T>(EndpointAddress destination, string correlationId, params T[] messages) where T : IMessage
    {
      CurrentBus().Send(destination.ToNsbAddress(), correlationId, messages.ToNsbMessages());
    }

    public void Reply<T>(string correlationId, params T[] messages) where T : IMessage
    {
      throw new NotImplementedException();
    }

    public void Publish<T>(params T[] messages) where T : IMessage
    {
      CurrentBus().Publish(messages.ToNsbMessages());
    }

    public void PublishAndReplyTo<T>(EndpointAddress destination, string correlationId, params T[] messages) where T : IMessage
    {
      throw new NotImplementedException();
    }

    public void PublishAndReply<T>(params T[] messages) where T : IMessage
    {
      throw new NotImplementedException();
    }

    public void PublishAndReply<T>(string correlationId, params T[] messages) where T : IMessage
    {
      throw new NotImplementedException();
    }
  }

  public static class NsbMessageHelpers
  {
    public static NServiceBus.IMessage[] ToNsbMessages<T>(this T[] messages) where T : IMessage
    {
      return new NServiceBus.IMessage[0];
    }

    public static string ToNsbAddress(this EndpointAddress address)
    {
      return address.Name + "@" + address.Host;
    }
  }

  public class NServiceBusRequestReplyBuilder : IRequestReplyBuilder
  {
    readonly string _correlationId;
    readonly NServiceBus.IMessage[] _messages;
    readonly IBus _bus;

    public NServiceBusRequestReplyBuilder(IBus bus, string correlationId, NServiceBus.IMessage[] messages)
    {
      _bus = bus;
      _correlationId = correlationId;
      _messages = messages;
    }

    public void OnReply(AsyncCallback callback, object state)
    {
      _bus.Send(_messages).Register(callback, state);
    }

    public void OnReply(AsyncCallback callback)
    {
      _bus.Send(_messages).Register(callback, null);
    }
  }
}