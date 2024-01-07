using System;
using Bat.Library.ADTs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Bat.Library.ADTs
{
  [TestClass]
  public class TestADTs
  {
    #region Utilities

    private void Error()
    {
      throw new Exception();
    }

    private void Error(string Message)
    {
      throw new Exception(Message);
    }

    private void CompareValues(object PoppedValue, object ExpectedValue)
    {
      if ((PoppedValue == null) && (ExpectedValue == null)) return;
      if ((PoppedValue == null) || (ExpectedValue == null)) Error();
      if (!PoppedValue.Equals(ExpectedValue)) Error();
    }

    #endregion

    #region Chain (plain)

    [TestMethod]
    public void TestChainPush()
    {
      Chain chain = new Chain();
      chain.Push(3);
      if (chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestChainPopQueue1()
    {
      Chain chain = new Chain();
      chain.Push(3);
      CompareValues(chain.PopQueue(), 3);
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestChainPopStack1()
    {
      Chain chain = new Chain();
      chain.Push(3);
      CompareValues(chain.PopStack(), 3);
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestChainPopQueue3()
    {
      Chain chain = new Chain();
      chain.Push("one");
      chain.Push("two");
      chain.Push("three");
      CompareValues(chain.PeekQueue(), "one");
      CompareValues(chain.PopQueue(), "one");
      CompareValues(chain.PeekQueue(), "two");
      CompareValues(chain.PopQueue(), "two");
      CompareValues(chain.PeekQueue(), "three");
      CompareValues(chain.PopQueue(), "three");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestChainPopStack3()
    {
      Chain chain = new Chain();
      chain.Push("one");
      chain.Push("two");
      chain.Push("three");
      CompareValues(chain.PeekStack(), "three");
      CompareValues(chain.PopStack(), "three");
      CompareValues(chain.PeekStack(), "two");
      CompareValues(chain.PopStack(), "two");
      CompareValues(chain.PeekStack(), "one");
      CompareValues(chain.PopStack(), "one");
      if (!chain.IsEmpty) throw new Exception();
    }

    #endregion

    #region Chain (indexed)

    [TestMethod]
    public void TestIndexedChainPush()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(3, "three");
      if (chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainPopQueue1()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(3, "three");
      CompareValues(chain.PopQueue(), "three");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainPopStack1()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(3, "three");
      CompareValues(chain.PopStack(), "three");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainPopQueue3()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.PeekQueue(), "one");
      CompareValues(chain.PopQueue(), "one");
      CompareValues(chain.PeekQueue(), "two");
      CompareValues(chain.PopQueue(), "two");
      CompareValues(chain.PeekQueue(), "three");
      CompareValues(chain.PopQueue(), "three");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainPopStack3()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.PeekStack(), "three");
      CompareValues(chain.PopStack(), "three");
      CompareValues(chain.PeekStack(), "two");
      CompareValues(chain.PopStack(), "two");
      CompareValues(chain.PeekStack(), "one");
      CompareValues(chain.PopStack(), "one");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainPullQueue1()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.Pull(1), "one");
      CompareValues(chain.PopQueue(), "two");
      CompareValues(chain.PopQueue(), "three");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainPullStack1()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.Pull(1), "one");
      CompareValues(chain.PopStack(), "three");
      CompareValues(chain.PopStack(), "two");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainPullQueue2()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.Pull(2), "two");
      CompareValues(chain.PopQueue(), "one");
      CompareValues(chain.PopQueue(), "three");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainPullStack2()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.Pull(2), "two");
      CompareValues(chain.PopStack(), "three");
      CompareValues(chain.PopStack(), "one");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainPullQueue3()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.Pull(3), "three");
      CompareValues(chain.PopQueue(), "one");
      CompareValues(chain.PopQueue(), "two");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainPullStack3()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.Pull(3), "three");
      CompareValues(chain.PopStack(), "two");
      CompareValues(chain.PopStack(), "one");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainRePushQueue1()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.RePush(1), "one");
      CompareValues(chain.PopQueue(), "two");
      CompareValues(chain.PopQueue(), "three");
      CompareValues(chain.PopQueue(), "one");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainRePushStack1()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.RePush(1), "one");
      CompareValues(chain.PopStack(), "one");
      CompareValues(chain.PopStack(), "three");
      CompareValues(chain.PopStack(), "two");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainRePushQueue2()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.RePush(2), "two");
      CompareValues(chain.PopQueue(), "one");
      CompareValues(chain.PopQueue(), "three");
      CompareValues(chain.PopQueue(), "two");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainRePushStack2()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.RePush(2), "two");
      CompareValues(chain.PopStack(), "two");
      CompareValues(chain.PopStack(), "three");
      CompareValues(chain.PopStack(), "one");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainRePushQueue3()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.RePush(3), "three");
      CompareValues(chain.PopQueue(), "one");
      CompareValues(chain.PopQueue(), "two");
      CompareValues(chain.PopQueue(), "three");
      if (!chain.IsEmpty) throw new Exception();
    }

    [TestMethod]
    public void TestIndexedChainRePushStack3()
    {
      IndexedChain chain = new IndexedChain();
      chain.Push(1, "one");
      chain.Push(2, "two");
      chain.Push(3, "three");
      CompareValues(chain.RePush(3), "three");
      CompareValues(chain.PopStack(), "three");
      CompareValues(chain.PopStack(), "two");
      CompareValues(chain.PopStack(), "one");
      if (!chain.IsEmpty) throw new Exception();
    }

    #endregion
  }
}
