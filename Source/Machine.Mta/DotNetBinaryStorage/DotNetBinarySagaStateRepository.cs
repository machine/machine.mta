using System;
using System.Collections.Generic;
using System.IO;

using Machine.Mta.Sagas;

namespace Machine.Mta.DotNetBinaryStorage
{
  public abstract class DotNetBinarySagaStateRepository<T> : ISagaStateRepository<T> where T : class, ISagaState
  {
    static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(DotNetBinarySagaStateRepository<T>));
    readonly IFlatFileSystem _flatFileSystem;
    readonly IFlatBinaryFileConfiguration _configuration;

    protected DotNetBinarySagaStateRepository(IFlatBinaryFileConfiguration configuration, IFlatFileSystem flatFileSystem)
    {
      _configuration = configuration;
      _flatFileSystem = flatFileSystem;
    }

    public T FindSagaState(Guid sagaId)
    {
      string path = PathForState(sagaId);
      if (!_flatFileSystem.IsFile(path))
      {
        return default(T);
      }
      using (Stream stream = _flatFileSystem.Open(path))
      {
        return (T)Serializers.Binary.Deserialize(stream);
      }
    }

    public void Add(T sagaState)
    {
      string path = PathForState(sagaState.SagaId);
      using (Stream stream = _flatFileSystem.Create(path))
      {
        Serializers.Binary.Serialize(stream, sagaState);
      }
    }

    public void Save(T sagaState)
    {
      string path = PathForState(sagaState.SagaId);
      using (Stream stream = _flatFileSystem.Create(path))
      {
        Serializers.Binary.Serialize(stream, sagaState);
      }
    }

    public void Delete(T sagaState)
    {
      string path = PathForState(sagaState.SagaId);
      if (_flatFileSystem.IsFile(path))
      {
        _flatFileSystem.Delete(path);
      }
    }

    public IEnumerable<T> FindAll()
    {
      foreach (string path in _flatFileSystem.ListFiles(_configuration.SagaStatePath, Suffix))
      {
        using (Stream stream = _flatFileSystem.Open(path))
        {
          yield return (T)Serializers.Binary.Deserialize(stream);
        }
      }
    }

    public string PathForState(Guid id)
    {
      return Path.Combine(_configuration.SagaStatePath, id.ToString("D") + "." + Suffix);
    }

    public abstract string Suffix { get; }
  }
}
