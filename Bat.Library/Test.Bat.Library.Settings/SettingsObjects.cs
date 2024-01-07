using Bat.Library.Settings;

namespace Test.Bat.Library.Settings
{
  public class SettingsRoot : SettingsNode
  {
    public SettingsRoot()
      : base("Bat.Library.Test")
    {
      //  Ordinarily one would do this.  But for testing we want more flexibility.
      //_Branch = new SettingsBranch(this);
    }

    private bool _boolean = false;
    public bool Boolean
    {
      get { return _boolean; }
      set { _boolean = value; }
    }

    private byte _int8 = 0;
    public byte int8
    {
      get { return _int8; }
      set { _int8 = value; }
    }

    private short _int16 = 0;
    public short int16
    {
      get { return _int16; }
      set { _int16 = value; }
    }

    private int _int32 = 0;
    public int int32
    {
      get { return _int32; }
      set { _int32 = value; }
    }

    private long _int64 = 0;
    public long int64
    {
      get { return _int64; }
      set { _int64 = value; }
    }

    private string _str = null;
    public string Str
    {
      get { return _str; }
      set { _str = value; }
    }

    internal SettingsBranch _Branch;//  Ordinarily private.  But for testing we want more flexibility.
    public SettingsBranch Branch
    {
      get { return _Branch; }
    }

    public void Populate()
    {
      _boolean = true;
      _int8 = 0x01;
      _int16 = 0x0102;
      _int32 = 0x01020304;
      _int64 = 0x0102030405060708;
      _str = "Hello World!";
    }
  }

  public class SettingsBranch : SettingsNode
  {
    public SettingsBranch(SettingsNode Root)
      : base("Branch", Root)
    {
    }

    private int _UserID = 0;
    public int UserID
    {
      get { return _UserID; }
      set { _UserID = value; }
    }

    private string _UserName = null;
    public string UserName
    {
      get { return _UserName; }
      set { _UserName = value; }
    }

    public virtual void Populate()
    {
      _UserID = 123;
      _UserName = "John Doe";
    }
  }

  public class SettingsBranchFull : SettingsBranch
  {
    public SettingsBranchFull(SettingsNode Root)
      : base(Root)
    {
    }

    private string _Title = null;
    public string Title
    {
      get { return _Title; }
      set { _Title = value; }
    }

    public override void Populate()
    {
      base.Populate();
      _Title = "Mr";
    }
  }
}