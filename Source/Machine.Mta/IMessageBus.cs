using System;
using System.Collections.Generic;

namespace Machine.Mta
{
  public interface IMessageBus : IDisposable
  {
    EndpointAddress PoisonAddress { get; }
    EndpointAddress Address { get; }
    void Start();
    void Send<T>(params T[] messages) where T : class, IMessage;
    void Send<T>(EndpointAddress destination, params T[] messages) where T : class, IMessage;
    void Send(EndpointAddress destination, MessagePayload payload);
    void SendLocal<T>(params T[] messages) where T : class, IMessage;
    void Stop();
    IRequestReplyBuilder Request<T>(params T[] messages) where T : class, IMessage;
    IRequestReplyBuilder Request<T>(Guid correlationId, params T[] messages) where T : class, IMessage;
    void Reply<T>(params T[] messages) where T : class, IMessage;
    void Reply<T>(EndpointAddress destination, Guid correlationId, params T[] messages) where T : class, IMessage;
    void Reply<T>(Guid correlationId, params T[] messages) where T : class, IMessage;
    void Publish<T>(params T[] messages) where T : class, IMessage;
  }
  public interface IRequestReplyBuilder
  {
    void OnReply(AsyncCallback callback, object state);
    void OnReply(AsyncCallback callback);
  }
}
