using System;
using System.Collections;
using System.IO;
using System.Threading;

namespace Morph.Core
{
  public class MorphStream : Stream, IDisposable
  {
    #region IDisposable

    void IDisposable.Dispose()
    {
      lock (_Queue)
        if (_TotalRemaining >= 0)
        {
          _TotalRemaining = -1;
          _Queue.Clear();
          _Gate.Set();
        }
    }

    #endregion

    #region Internal

    private Queue _Queue = new Queue();
    private long _TotalRemaining = 0;
    private int _WaitingFor = 0;

    private void WaitFor(int count)
    {
      //  If not enough data is available, then we wait
      lock (_Queue)
        if (count <= _TotalRemaining)
          return;
        else
          _WaitingFor = count;
      _Gate.WaitOne();
      //  Stream may no longer be valid
      if (_Queue == null)
        throw new ObjectDisposedException("MorphStream is disposed");
    }

    private int _SegmentPos = 0;
    private byte[] _Segment = new byte[0];

    private ManualResetEvent _Gate = new ManualResetEvent(false);

    private byte[] Segment()
    {
      lock (_Gate)
      {
        if (_SegmentPos == _Segment.Length)
        {
          _Segment = (byte[])_Queue.Dequeue();
          _SegmentPos = 0;
        }
        return _Segment;
      }
    }

    #endregion

    public long Remaining
    {
      get { return _TotalRemaining; }
    }

    public byte Peek()
    {
      if (_Queue == null)
        throw new ObjectDisposedException("MorphStream is disposed");
      WaitFor(1);
      lock (_Segment)
        return Segment()[_SegmentPos];
    }

    public byte[] Read(int count)
    {
      WaitFor(count);
      //  Read the data
      byte[] result = new byte[count];
      if (Read(result, 0, result.Length) < count)
        throw new EMorphImplementation();
      return result;
    }

    #region Stream implementation

    public override bool CanRead
    {
      get { return true; }
    }

    public override bool CanSeek
    {
      get { return false; }
    }

    public override bool CanWrite
    {
      get { return true; }
    }

    public override void Flush()
    {
      _Queue.Clear();
    }

    public override long Length
    {
      get { throw new NotSupportedException(); }
    }

    public override long Position
    {
      get { throw new NotSupportedException(); }
      set { throw new NotSupportedException(); }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (buffer.Length < offset + count)
        throw new ArgumentException();
      if (buffer == null)
        throw new ArgumentNullException();
      if ((offset < 0) || (count < 0))
        throw new ArgumentOutOfRangeException();
      if (_Queue == null)
        throw new ObjectDisposedException("MorphStream is disposed");
      //  If no data is available, then we must wait
      _Gate.WaitOne();
      //  Can't copy more than we have
      if (_TotalRemaining < count)
        count = (int)_TotalRemaining;
      int result = count;
      //  Might have to copy from several segments
      while (count > 0)
        lock (_Segment)
        {
          //  Might have to "page" to next segment
          Segment();
          //  Determine copy count for this segment
          int CopyCount = _Segment.Length - _SegmentPos;
          if (CopyCount > count)
            CopyCount = count;
          //  Copy from segment
          lock (_Gate)
          {
            Array.Copy(_Segment, _SegmentPos, buffer, offset, CopyCount);
            offset += CopyCount;
            _TotalRemaining -= CopyCount;
            _SegmentPos += CopyCount;
            if (_TotalRemaining == 0)
              _Gate.Reset();
          }
          count -= CopyCount;
        }
      return result;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (buffer.Length < offset + count)
        throw new ArgumentException();
      if (count == 0)
        return;
      //  Copy data
      byte[] NewSegment = new byte[count];
      Array.Copy(buffer, offset, NewSegment, 0, NewSegment.Length);
      //  Add data to queue
      lock (_Gate)
      {
        _Queue.Enqueue(NewSegment);
        _TotalRemaining += NewSegment.Length;
        _Gate.Set();
      }
    }

    #endregion
  }
}