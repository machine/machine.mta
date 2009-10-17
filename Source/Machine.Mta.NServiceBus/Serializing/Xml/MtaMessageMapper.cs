using System;
using System.Collections.Generic;
using Machine.Mta.MessageInterfaces;
using NServiceBus.MessageInterfaces;

namespace Machine.Mta.Serializing.Xml
{
  public class MtaMessageMapper : IMessageMapper
  {
    readonly static log4net.ILog _log = log4net.LogManager.GetLogger(typeof(MtaMessageMapper));
    readonly IMessageRegisterer _registerer;
    readonly IMessageInterfaceImplementationsLookup _lookup;
    readonly OpaqueMessageFactory _opaqueMessageFactory;

    public MtaMessageMapper(IMessageInterfaceImplementationsLookup lookup, IMessageRegisterer registerer, MessageDefinitionFactory messageDefiniionFactory, MessageInterfaceImplementations messageInterfaceImplementations)
    {
      _lookup = lookup;
      _registerer = registerer;
      _opaqueMessageFactory = new OpaqueMessageFactory(messageInterfaceImplementations, messageDefiniionFactory);
    }

    public T CreateInstance<T>() where T : NServiceBus.IMessage
    {
      return (T)_opaqueMessageFactory.Create(typeof(T));
    }

    public T CreateInstance<T>(Action<T> action) where T : NServiceBus.IMessage
    {
      T message = (T)_opaqueMessageFactory.Create(typeof(T));
      action(message);
      return message;
    }

    public object CreateInstance(Type messageType)
    {
      return _opaqueMessageFactory.Create(messageType);
    }

    public void Initialize(IEnumerable<Type> types)
    {
    }

    public Type GetMappedTypeFor(Type type)
    {
      if (type.IsClass)
      {
        if (_lookup.IsClassOrInterface(type))
        {
          return _lookup.GetClassOrInterfaceFor(type);
        }
        return type;
      }
      if (_lookup.IsClassOrInterface(type))
      {
        return _lookup.GetClassOrInterfaceFor(type);
      }
      return null;
    }

    public Type GetMappedTypeFor(string typeName)
    {
      foreach (var type in _registerer.MessageTypes)
      {
        if (type.FullName == typeName)
          return type;
      }
      var permutations = new[] {typeName};
      if (!typeName.Contains(","))
      {
        permutations = new[] { typeName, typeName + ", NServiceBus.Core" };
      }
      foreach (var permutation in permutations)
      {
        try
        {
          var found = Type.GetType(permutation);
          if (found != null)
          {
            return found;
          }
        }
        catch (Exception)
        {
          _log.Error("Error resolving: " + permutation);
          return null;
        }
      }
      return null;
    }
  }
}