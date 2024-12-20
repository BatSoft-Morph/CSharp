using System;

namespace Morph
{
    public class EMorph : Exception
    {
        public EMorph(int errorCode)
          : base(errorCode.ToString())
        {
            _errorCode = errorCode;
        }

        public EMorph(string message)
          : base(message)
        {
        }

        public EMorph(int errorCode, string message)
          : base(message)
        {
            _errorCode = errorCode;
        }

        private EMorph(int errorCode, string message, string morphTrace)
          : base(message)
        {
            _errorCode = errorCode;
            _stackTrace = morphTrace;
        }

        private readonly int _errorCode = Any;
        public int ErrorCode
        {
            get => _errorCode;
        }

        public const int None = 0;
        public const int Any = -1;

        private readonly string _stackTrace = null;
        public override string StackTrace
        {
            get
            {
                if (_stackTrace != null)
                    return _stackTrace;
                return base.StackTrace;
            }
        }

        static public void Throw(int errorCode, string message)
            =>Throw(errorCode, message, null);

        static public void Throw(int errorCode, string message, string morphTrace)
        {
            if (message == null)
                message = "Morph error: " + errorCode.ToString();
            throw new EMorph(errorCode, message, morphTrace);
        }
    }

    public class EMorphImplementation : EMorph
    {
        public EMorphImplementation()
          : base("Morph implementation error")
        {
        }
    }

    public class EMorphUsage : EMorph
    {
        public EMorphUsage(string message)
          : base(message)
        {
        }
    }
}