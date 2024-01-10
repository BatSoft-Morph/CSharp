namespace Morph.Lib
{
  /**
   * As a Morph rule, any ID=0 is the same as saying ID=No_ID.
   * Also, some platforms might not deal with unsigned integers.
   * Therefore, valid ID's range from {1..0x7FFFFFFF}.
   **/
  public interface IIDFactory
  {
    int Generate();
    void Release(int id);
  }

  public class IDSeed : IIDFactory
  {
    public IDSeed()
    {
      _seed = 1;
    }

    public IDSeed(int startID)
    {
      _seed = startID;
    }

    private int _seed;

    #region IIDFactory Members

    public int Generate()
    {
      lock (this)
      {
        if (_seed == int.MaxValue)
          _seed = 1;
        return _seed++;
      }
    }

    public void Release(int id)
    {
    }

    #endregion
  }
}