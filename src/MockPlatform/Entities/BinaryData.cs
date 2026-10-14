using Sungero.CoreEntities;
using Sungero.Domain.Shared;

namespace MockPlatform.Entities;

public class BinaryData : IBinaryData
{
  private byte[] data = [];
  public long Size { get; set; }
  public string Hash { get; set; }
  public Guid? Id { get; set; }

  public IStorage Storage => throw new NotImplementedException();

  public IEntity RootEntity { get; set; }

  public event Action OnRead;
  public event Action OnWrite;
  public event Action OnDelete;

  public object Clone() => null;

  public void Delete() { }

  public Stream Read() => new MemoryStream(data);

  public void SetStorage(IStorage storage) { }

  public void Write(Stream stream)
  {
    data = new byte[stream.Length];
    stream.Read(data);
  }
}