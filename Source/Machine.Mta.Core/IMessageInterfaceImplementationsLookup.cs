using System;

namespace Machine.Mta
{
  public interface IMessageInterfaceImplementationsLookup
  {
    Type GetClassFor(Type type);
    Type GetInterfaceFor(Type type);
    Type GetClassOrInterfaceFor(Type type);
    bool IsClassOrInterface(Type type);
  }
}