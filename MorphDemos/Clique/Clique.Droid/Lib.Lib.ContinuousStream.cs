using System.IO;

namespace Morph.Lib
{
  public class ContinuousStream : MemoryStream
  {
    public ContinuousStream()
    {
      ReadTimeout = int.MaxValue;
    }
  }
}