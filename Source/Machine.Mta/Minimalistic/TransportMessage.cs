using System;

namespace Machine.Mta.Minimalistic
{
  [Serializable]
  public class TransportMessage
  {
    private readonly EndpointName _returnAddress;
    private readonly Guid _id;
    private readonly Guid _returnCorrelationId;
    private readonly Guid _correlationId;
    private readonly byte[] _body;

    public Guid Id
    {
      get { return _id; }
    }

    public Guid ReturnCorrelationId
    {
      get { return _returnCorrelationId; }
    }

    public Guid CorrelationId
    {
      get { return _correlationId; }
    }

    public EndpointName ReturnAddress
    {
      get { return _returnAddress; }
    }

    public byte[] Body
    {
      get { return _body; }
    }

    protected TransportMessage()
    {
    }

    protected TransportMessage(Guid id, EndpointName returnAddress, Guid correlationId, Guid returnCorrelationId, byte[] body)
    {
      _id = id;
      _returnAddress = returnAddress;
      _correlationId = correlationId;
      _returnCorrelationId = returnCorrelationId;
      _body = body;
    }

    public override string ToString()
    {
      return "TransportMessage from " + _returnAddress + " with " + _body.Length + "bytes";
    }

    public static TransportMessage For(EndpointName returnAddress, Guid correlationId, Guid returnCorrelationId, byte[] body)
    {
      Guid id = Guid.NewGuid();
      if (returnCorrelationId == Guid.Empty)
      {
        returnCorrelationId = id;
      }
      return new TransportMessage(id, returnAddress, correlationId, returnCorrelationId, body);
    }
  }
}