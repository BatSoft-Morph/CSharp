using System;
using System.IO;
using System.Net.Sockets;

namespace Bat.Library.Logging
{
  public class LogTypeException : ILogType
  {
    public string ToString(object obj)
    {
      if (!(obj is Exception))
        return null;
      Exception x = (Exception)obj;
      if (x is SocketException exception)
        return SocketErrorMessage(exception);
      else
        return nl +
          "Class: " + x.GetType().Name + nl +
          "Message: " + x.Message + nl +
          "StackTrace: " + nl + x.StackTrace + nl;
    }

    private string SocketErrorMessage(SocketException x)
    {
      string errorCode;
      string errorMessage;
      switch (x.ErrorCode)
      {
        case 6:
          errorCode = "6 - WSA_INVALID_HANDLE";
          errorMessage = "Specified event object handle is invalid.";
          break;

        case 8:
          errorCode = "8 - WSA_NOT_ENOUGH_MEMORY";
          errorMessage = "Insufficient memory available.";
          break;

        case 87:
          errorCode = "87 - WSA_INVALID_PARAMETER";
          errorMessage = "One or more parameters are invalid.";
          break;

        case 995:
          errorCode = "995 - WSA_OPERATION_ABORTED";
          errorMessage = "Overlapped operation aborted.";
          break;

        case 996:
          errorCode = "996 - WSA_IO_INCOMPLETE";
          errorMessage = "Overlapped I/O event object not in signaled state.";
          break;

        case 997:
          errorCode = "997 - WSA_IO_PENDING";
          errorMessage = "Overlapped operations will complete later.";
          break;

        case 10004:
          errorCode = "10004 - WSAEINTR";
          errorMessage = "Interrupted function call.";
          break;

        case 10009:
          errorCode = "10009 - WSAEBADF";
          errorMessage = "File handle is not valid.";
          break;

        case 10013:
          errorCode = "10013 - WSAEACCES";
          errorMessage = "Permission denied.";
          break;

        case 10014:
          errorCode = "10014 - WSAEFAULT";
          errorMessage = "Bad address.";
          break;

        case 10022:
          errorCode = "10022 - WSAEINVAL";
          errorMessage = "Invalid argument.";
          break;

        case 10024:
          errorCode = "10024 - WSAEMFILE";
          errorMessage = "Too many open files.";
          break;

        case 10035:
          errorCode = "10035 - WSAEWOULDBLOCK";
          errorMessage = "Resource temporarily unavailable.";
          break;

        case 10036:
          errorCode = "10036 - WSAEINPROGRESS";
          errorMessage = "Operation now in progress.";
          break;

        case 10037:
          errorCode = "10037 - WSAEALREADY";
          errorMessage = "Operation already in progress.";
          break;

        case 10038:
          errorCode = "10038 - WSAENOTSOCK";
          errorMessage = "Socket operation on nonsocket.";
          break;

        case 10039:
          errorCode = "10039 - WSAEDESTADDRREQ";
          errorMessage = "Destination address required.";
          break;

        case 10040:
          errorCode = "10040 - WSAEMSGSIZE";
          errorMessage = "Message too long.";
          break;

        case 10041:
          errorCode = "10041 - WSAEPROTOTYPE";
          errorMessage = "Protocol wrong type for socket.";
          break;

        case 10042:
          errorCode = "10042 - WSAENOPROTOOPT";
          errorMessage = "Bad protocol option.";
          break;

        case 10043:
          errorCode = "10043 - WSAEPROTONOSUPPORT";
          errorMessage = "Protocol not supported.";
          break;

        case 10044:
          errorCode = "10044 - WSAESOCKTNOSUPPORT";
          errorMessage = "Socket type not supported.";
          break;

        case 10045:
          errorCode = "10045 - WSAEOPNOTSUPP";
          errorMessage = "Operation not supported.";
          break;

        case 10046:
          errorCode = "10046 - WSAEPFNOSUPPORT";
          errorMessage = "Protocol family not supported.";
          break;

        case 10047:
          errorCode = "10047 - WSAEAFNOSUPPORT";
          errorMessage = "Address family not supported by protocol family.";
          break;

        case 10048:
          errorCode = "10048 - WSAEADDRINUSE";
          errorMessage = "Address already in use.";
          break;

        case 10049:
          errorCode = "10049 - WSAEADDRNOTAVAIL";
          errorMessage = "Cannot assign requested address.";
          break;

        case 10050:
          errorCode = "10050 - WSAENETDOWN";
          errorMessage = "Network is down.";
          break;

        case 10051:
          errorCode = "10051 - WSAENETUNREACH";
          errorMessage = "Network is unreachable.";
          break;

        case 10052:
          errorCode = "10052 - WSAENETRESET";
          errorMessage = "Network dropped connection on reset.";
          break;

        case 10053:
          errorCode = "10053 - WSAECONNABORTED";
          errorMessage = "Software caused connection abort.";
          break;

        case 10054:
          errorCode = "10054 - WSAECONNRESET";
          errorMessage = "Connection reset by peer.";
          break;

        case 10055:
          errorCode = "10055 - WSAENOBUFS";
          errorMessage = "No buffer space available.";
          break;

        case 10056:
          errorCode = "10056 - WSAEISCONN";
          errorMessage = "Socket is already connected.";
          break;

        case 10057:
          errorCode = "10057 - WSAENOTCONN";
          errorMessage = "Socket is not connected.";
          break;

        case 10058:
          errorCode = "10058 - WSAESHUTDOWN";
          errorMessage = "Cannot send after socket shutdown.";
          break;

        case 10059:
          errorCode = "10059 - WSAETOOMANYREFS";
          errorMessage = "Too many references.";
          break;

        case 10060:
          errorCode = "10060 - WSAETIMEDOUT";
          errorMessage = "Connection timed out.";
          break;

        case 10061:
          errorCode = "10061 - WSAECONNREFUSED";
          errorMessage = "Connection refused.";
          break;

        case 10062:
          errorCode = "10062 - WSAELOOP";
          errorMessage = "Cannot translate name.";
          break;

        case 10063:
          errorCode = "10063 - WSAENAMETOOLONG";
          errorMessage = "Name too long.";
          break;

        case 10064:
          errorCode = "10064 - WSAEHOSTDOWN";
          errorMessage = "Host is down.";
          break;

        case 10065:
          errorCode = "10065 - WSAEHOSTUNREACH";
          errorMessage = "No route to host.";
          break;

        case 10066:
          errorCode = "10066 - WSAENOTEMPTY";
          errorMessage = "Directory not empty.";
          break;

        case 10067:
          errorCode = "10067 - WSAEPROCLIM";
          errorMessage = "Too many processes.";
          break;

        case 10068:
          errorCode = "10068 - WSAEUSERS";
          errorMessage = "User quota exceeded.";
          break;

        case 10069:
          errorCode = "10069 - WSAEDQUOT";
          errorMessage = "Disk quota exceeded.";
          break;

        case 10070:
          errorCode = "10070 - WSAESTALE";
          errorMessage = "Stale file handle reference.";
          break;

        case 10071:
          errorCode = "10071 - WSAEREMOTE";
          errorMessage = "Item is remote.";
          break;

        case 10091:
          errorCode = "10091 - WSASYSNOTREADY";
          errorMessage = "Network subsystem is unavailable.";
          break;

        case 10092:
          errorCode = "10092 - WSAVERNOTSUPPORTED";
          errorMessage = "Winsock.dll version out of range.";
          break;

        case 10093:
          errorCode = "10093 - WSANOTINITIALISED";
          errorMessage = "Successful WSAStartup not yet performed.";
          break;

        case 10101:
          errorCode = "10101 - WSAEDISCON";
          errorMessage = "Graceful shutdown in progress.";
          break;

        case 10102:
          errorCode = "10102 - WSAENOMORE";
          errorMessage = "No more results.";
          break;

        case 10103:
          errorCode = "10103 - WSAECANCELLED";
          errorMessage = "Call has been canceled.";
          break;

        case 10104:
          errorCode = "10104 - WSAEINVALIDPROCTABLE";
          errorMessage = "Procedure call table is invalid.";
          break;

        case 10105:
          errorCode = "10105 - WSAEINVALIDPROVIDER";
          errorMessage = "Service provider is invalid.";
          break;

        case 10106:
          errorCode = "10106 - WSAEPROVIDERFAILEDINIT";
          errorMessage = "Service provider failed to initialize.";
          break;

        case 10107:
          errorCode = "10107 - WSASYSCALLFAILURE";
          errorMessage = "System call failure.";
          break;

        case 10108:
          errorCode = "10108 - WSASERVICE_NOT_FOUND";
          errorMessage = "Service not found.";
          break;

        case 10109:
          errorCode = "10109 - WSATYPE_NOT_FOUND";
          errorMessage = "Class type not found.";
          break;

        case 10110:
          errorCode = "10110 - WSA_E_NO_MORE";
          errorMessage = "No more results.";
          break;

        case 10111:
          errorCode = "10111 - WSA_E_CANCELLED";
          errorMessage = "Call was canceled.";
          break;

        case 10112:
          errorCode = "10112 - WSAEREFUSED";
          errorMessage = "Database query was refused.";
          break;

        case 11001:
          errorCode = "11001 - WSAHOST_NOT_FOUND";
          errorMessage = "Host not found.";
          break;

        case 11002:
          errorCode = "11002 - WSATRY_AGAIN";
          errorMessage = "Nonauthoritative host not found.";
          break;

        case 11003:
          errorCode = "11003 - WSANO_RECOVERY";
          errorMessage = "This is a nonrecoverable error.";
          break;

        case 11004:
          errorCode = "11004 - WSANO_DATA";
          errorMessage = "Valid name, no data record of requested type.";
          break;

        case 11005:
          errorCode = "11005 - WSA_QOS_RECEIVERS";
          errorMessage = "QOS receivers.";
          break;

        case 11006:
          errorCode = "11006 - WSA_QOS_SENDERS";
          errorMessage = "QOS senders.";
          break;

        case 11007:
          errorCode = "11007 - WSA_QOS_NO_SENDERS";
          errorMessage = "No QOS senders.";
          break;

        case 11008:
          errorCode = "11008 - WSA_QOS_NO_RECEIVERS";
          errorMessage = "QOS no receivers.";
          break;

        case 11009:
          errorCode = "11009 - WSA_QOS_REQUEST_CONFIRMED";
          errorMessage = "QOS request confirmed.";
          break;

        case 11010:
          errorCode = "11010 - WSA_QOS_ADMISSION_FAILURE";
          errorMessage = "QOS admission error.";
          break;

        case 11011:
          errorCode = "11011 - WSA_QOS_POLICY_FAILURE";
          errorMessage = "QOS policy failure.";
          break;

        case 11012:
          errorCode = "11012 - WSA_QOS_BAD_STYLE";
          errorMessage = "QOS bad style.";
          break;

        case 11013:
          errorCode = "11013 - WSA_QOS_BAD_OBJECT";
          errorMessage = "QOS bad object.";
          break;

        case 11014:
          errorCode = "11014 - WSA_QOS_TRAFFIC_CTRL_ERROR";
          errorMessage = "QOS traffic control error.";
          break;

        case 11015:
          errorCode = "11015 - WSA_QOS_GENERIC_ERROR";
          errorMessage = "QOS generic error.";
          break;

        case 11016:
          errorCode = "11016 - WSA_QOS_ESERVICETYPE";
          errorMessage = "QOS service type error.";
          break;

        case 11017:
          errorCode = "11017 - WSA_QOS_EFLOWSPEC";
          errorMessage = "QOS flowspec error.";
          break;

        case 11018:
          errorCode = "11018 - WSA_QOS_EPROVSPECBUF";
          errorMessage = "Invalid QOS provider buffer.";
          break;

        case 11019:
          errorCode = "11019 - WSA_QOS_EFILTERSTYLE";
          errorMessage = "Invalid QOS filter style.";
          break;

        case 11020:
          errorCode = "11020 - WSA_QOS_EFILTERTYPE";
          errorMessage = "Invalid QOS filter type.";
          break;

        case 11021:
          errorCode = "11021 - WSA_QOS_EFILTERCOUNT";
          errorMessage = "Incorrect QOS filter count.";
          break;

        case 11022:
          errorCode = "11022 - WSA_QOS_EOBJLENGTH";
          errorMessage = "Invalid QOS object length.";
          break;

        case 11023:
          errorCode = "11023 - WSA_QOS_EFLOWCOUNT";
          errorMessage = "Incorrect QOS flow count.";
          break;

        case 11024:
          errorCode = "11024 - WSA_QOS_EUNKOWNPSOBJ";
          errorMessage = "Unrecognized QOS object.";
          break;

        case 11025:
          errorCode = "11025 - WSA_QOS_EPOLICYOBJ";
          errorMessage = "Invalid QOS policy object.";
          break;

        case 11026:
          errorCode = "11026 - WSA_QOS_EFLOWDESC";
          errorMessage = "Invalid QOS flow descriptor.";
          break;

        case 11027:
          errorCode = "11027 - WSA_QOS_EPSFLOWSPEC";
          errorMessage = "Invalid QOS provider-specific flowspec.";
          break;

        case 11028:
          errorCode = "11028 - WSA_QOS_EPSFILTERSPEC";
          errorMessage = "Invalid QOS provider-specific filterspec.";
          break;

        case 11029:
          errorCode = "11029 - WSA_QOS_ESDMODEOBJ";
          errorMessage = "Invalid QOS shape discard mode object.";
          break;

        case 11030:
          errorCode = "11030 - WSA_QOS_ESHAPERATEOBJ";
          errorMessage = "Invalid QOS shaping rate object.";
          break;

        case 11031:
          errorCode = "11031 - WSA_QOS_RESERVED_PETYPE";
          errorMessage = "Reserved policy QOS element type.";
          break;

        default:
          errorCode = x.ErrorCode.ToString();
          errorMessage = x.Message;
          break;
      }
      return nl +
        "Class: " + x.GetType().Name + nl +
        "Code: " + errorCode + nl +
        "Message: " + errorMessage + nl +
        "StackTrace: " + nl + x.StackTrace + nl;
    }

    private const string nl = "\u000D\u000A";
  }

  public class LogTypeBytes : ILogType
  {
    public string ToString(object obj)
    {
      if (!(obj is byte[]))
        return null;
      byte[] bytes = (byte[])obj;
      StringWriter Writer = new StringWriter();
      Writer.Write('[');
      if (bytes.Length > 0)
      {
        Writer.Write(' ');
        Writer.Write(bytes[0]);
      }
      for (int i = 1; i < bytes.Length; i++)
      {
        Writer.Write(", ");
        Writer.Write(bytes[i]);
      }
      Writer.Write(" ]");
      return Writer.ToString();
    }
  }

  public class LogTypeDateTime : ILogType
  {
    public string ToString(object obj)
    {
      if (!(obj is DateTime))
        return null;
      return ((DateTime)obj).ToString();
    }
  }
}