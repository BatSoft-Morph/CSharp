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
      if (x is SocketException)
        return SocketErrorMessage((SocketException)x);
      else
        return nl +
          "Class: " + x.GetType().Name + nl +
          "Message: " + x.Message + nl +
          "StackTrace: " + nl + x.StackTrace + nl;
    }

    private string SocketErrorMessage(SocketException x)
    {
      string ErrorCode;
      string ErrorMessage;
      switch (x.ErrorCode)
      {
        case 6:
          ErrorCode = "6 - WSA_INVALID_HANDLE";
          ErrorMessage = "Specified event object handle is invalid.";
          break;

        case 8:
          ErrorCode = "8 - WSA_NOT_ENOUGH_MEMORY";
          ErrorMessage = "Insufficient memory available.";
          break;

        case 87:
          ErrorCode = "87 - WSA_INVALID_PARAMETER";
          ErrorMessage = "One or more parameters are invalid.";
          break;

        case 995:
          ErrorCode = "995 - WSA_OPERATION_ABORTED";
          ErrorMessage = "Overlapped operation aborted.";
          break;

        case 996:
          ErrorCode = "996 - WSA_IO_INCOMPLETE";
          ErrorMessage = "Overlapped I/O event object not in signaled state.";
          break;

        case 997:
          ErrorCode = "997 - WSA_IO_PENDING";
          ErrorMessage = "Overlapped operations will complete later.";
          break;

        case 10004:
          ErrorCode = "10004 - WSAEINTR";
          ErrorMessage = "Interrupted function call.";
          break;

        case 10009:
          ErrorCode = "10009 - WSAEBADF";
          ErrorMessage = "File handle is not valid.";
          break;

        case 10013:
          ErrorCode = "10013 - WSAEACCES";
          ErrorMessage = "Permission denied.";
          break;

        case 10014:
          ErrorCode = "10014 - WSAEFAULT";
          ErrorMessage = "Bad address.";
          break;

        case 10022:
          ErrorCode = "10022 - WSAEINVAL";
          ErrorMessage = "Invalid argument.";
          break;

        case 10024:
          ErrorCode = "10024 - WSAEMFILE";
          ErrorMessage = "Too many open files.";
          break;

        case 10035:
          ErrorCode = "10035 - WSAEWOULDBLOCK";
          ErrorMessage = "Resource temporarily unavailable.";
          break;

        case 10036:
          ErrorCode = "10036 - WSAEINPROGRESS";
          ErrorMessage = "Operation now in progress.";
          break;

        case 10037:
          ErrorCode = "10037 - WSAEALREADY";
          ErrorMessage = "Operation already in progress.";
          break;

        case 10038:
          ErrorCode = "10038 - WSAENOTSOCK";
          ErrorMessage = "Socket operation on nonsocket.";
          break;

        case 10039:
          ErrorCode = "10039 - WSAEDESTADDRREQ";
          ErrorMessage = "Destination address required.";
          break;

        case 10040:
          ErrorCode = "10040 - WSAEMSGSIZE";
          ErrorMessage = "Message too long.";
          break;

        case 10041:
          ErrorCode = "10041 - WSAEPROTOTYPE";
          ErrorMessage = "Protocol wrong type for socket.";
          break;

        case 10042:
          ErrorCode = "10042 - WSAENOPROTOOPT";
          ErrorMessage = "Bad protocol option.";
          break;

        case 10043:
          ErrorCode = "10043 - WSAEPROTONOSUPPORT";
          ErrorMessage = "Protocol not supported.";
          break;

        case 10044:
          ErrorCode = "10044 - WSAESOCKTNOSUPPORT";
          ErrorMessage = "Socket type not supported.";
          break;

        case 10045:
          ErrorCode = "10045 - WSAEOPNOTSUPP";
          ErrorMessage = "Operation not supported.";
          break;

        case 10046:
          ErrorCode = "10046 - WSAEPFNOSUPPORT";
          ErrorMessage = "Protocol family not supported.";
          break;

        case 10047:
          ErrorCode = "10047 - WSAEAFNOSUPPORT";
          ErrorMessage = "Address family not supported by protocol family.";
          break;

        case 10048:
          ErrorCode = "10048 - WSAEADDRINUSE";
          ErrorMessage = "Address already in use.";
          break;

        case 10049:
          ErrorCode = "10049 - WSAEADDRNOTAVAIL";
          ErrorMessage = "Cannot assign requested address.";
          break;

        case 10050:
          ErrorCode = "10050 - WSAENETDOWN";
          ErrorMessage = "Network is down.";
          break;

        case 10051:
          ErrorCode = "10051 - WSAENETUNREACH";
          ErrorMessage = "Network is unreachable.";
          break;

        case 10052:
          ErrorCode = "10052 - WSAENETRESET";
          ErrorMessage = "Network dropped connection on reset.";
          break;

        case 10053:
          ErrorCode = "10053 - WSAECONNABORTED";
          ErrorMessage = "Software caused connection abort.";
          break;

        case 10054:
          ErrorCode = "10054 - WSAECONNRESET";
          ErrorMessage = "Connection reset by peer.";
          break;

        case 10055:
          ErrorCode = "10055 - WSAENOBUFS";
          ErrorMessage = "No buffer space available.";
          break;

        case 10056:
          ErrorCode = "10056 - WSAEISCONN";
          ErrorMessage = "Socket is already connected.";
          break;

        case 10057:
          ErrorCode = "10057 - WSAENOTCONN";
          ErrorMessage = "Socket is not connected.";
          break;

        case 10058:
          ErrorCode = "10058 - WSAESHUTDOWN";
          ErrorMessage = "Cannot send after socket shutdown.";
          break;

        case 10059:
          ErrorCode = "10059 - WSAETOOMANYREFS";
          ErrorMessage = "Too many references.";
          break;

        case 10060:
          ErrorCode = "10060 - WSAETIMEDOUT";
          ErrorMessage = "Connection timed out.";
          break;

        case 10061:
          ErrorCode = "10061 - WSAECONNREFUSED";
          ErrorMessage = "Connection refused.";
          break;

        case 10062:
          ErrorCode = "10062 - WSAELOOP";
          ErrorMessage = "Cannot translate name.";
          break;

        case 10063:
          ErrorCode = "10063 - WSAENAMETOOLONG";
          ErrorMessage = "Name too long.";
          break;

        case 10064:
          ErrorCode = "10064 - WSAEHOSTDOWN";
          ErrorMessage = "Host is down.";
          break;

        case 10065:
          ErrorCode = "10065 - WSAEHOSTUNREACH";
          ErrorMessage = "No route to host.";
          break;

        case 10066:
          ErrorCode = "10066 - WSAENOTEMPTY";
          ErrorMessage = "Directory not empty.";
          break;

        case 10067:
          ErrorCode = "10067 - WSAEPROCLIM";
          ErrorMessage = "Too many processes.";
          break;

        case 10068:
          ErrorCode = "10068 - WSAEUSERS";
          ErrorMessage = "User quota exceeded.";
          break;

        case 10069:
          ErrorCode = "10069 - WSAEDQUOT";
          ErrorMessage = "Disk quota exceeded.";
          break;

        case 10070:
          ErrorCode = "10070 - WSAESTALE";
          ErrorMessage = "Stale file handle reference.";
          break;

        case 10071:
          ErrorCode = "10071 - WSAEREMOTE";
          ErrorMessage = "Item is remote.";
          break;

        case 10091:
          ErrorCode = "10091 - WSASYSNOTREADY";
          ErrorMessage = "Network subsystem is unavailable.";
          break;

        case 10092:
          ErrorCode = "10092 - WSAVERNOTSUPPORTED";
          ErrorMessage = "Winsock.dll version out of range.";
          break;

        case 10093:
          ErrorCode = "10093 - WSANOTINITIALISED";
          ErrorMessage = "Successful WSAStartup not yet performed.";
          break;

        case 10101:
          ErrorCode = "10101 - WSAEDISCON";
          ErrorMessage = "Graceful shutdown in progress.";
          break;

        case 10102:
          ErrorCode = "10102 - WSAENOMORE";
          ErrorMessage = "No more results.";
          break;

        case 10103:
          ErrorCode = "10103 - WSAECANCELLED";
          ErrorMessage = "Call has been canceled.";
          break;

        case 10104:
          ErrorCode = "10104 - WSAEINVALIDPROCTABLE";
          ErrorMessage = "Procedure call table is invalid.";
          break;

        case 10105:
          ErrorCode = "10105 - WSAEINVALIDPROVIDER";
          ErrorMessage = "Service provider is invalid.";
          break;

        case 10106:
          ErrorCode = "10106 - WSAEPROVIDERFAILEDINIT";
          ErrorMessage = "Service provider failed to initialize.";
          break;

        case 10107:
          ErrorCode = "10107 - WSASYSCALLFAILURE";
          ErrorMessage = "System call failure.";
          break;

        case 10108:
          ErrorCode = "10108 - WSASERVICE_NOT_FOUND";
          ErrorMessage = "Service not found.";
          break;

        case 10109:
          ErrorCode = "10109 - WSATYPE_NOT_FOUND";
          ErrorMessage = "Class type not found.";
          break;

        case 10110:
          ErrorCode = "10110 - WSA_E_NO_MORE";
          ErrorMessage = "No more results.";
          break;

        case 10111:
          ErrorCode = "10111 - WSA_E_CANCELLED";
          ErrorMessage = "Call was canceled.";
          break;

        case 10112:
          ErrorCode = "10112 - WSAEREFUSED";
          ErrorMessage = "Database query was refused.";
          break;

        case 11001:
          ErrorCode = "11001 - WSAHOST_NOT_FOUND";
          ErrorMessage = "Host not found.";
          break;

        case 11002:
          ErrorCode = "11002 - WSATRY_AGAIN";
          ErrorMessage = "Nonauthoritative host not found.";
          break;

        case 11003:
          ErrorCode = "11003 - WSANO_RECOVERY";
          ErrorMessage = "This is a nonrecoverable error.";
          break;

        case 11004:
          ErrorCode = "11004 - WSANO_DATA";
          ErrorMessage = "Valid name, no data record of requested type.";
          break;

        case 11005:
          ErrorCode = "11005 - WSA_QOS_RECEIVERS";
          ErrorMessage = "QOS receivers.";
          break;

        case 11006:
          ErrorCode = "11006 - WSA_QOS_SENDERS";
          ErrorMessage = "QOS senders.";
          break;

        case 11007:
          ErrorCode = "11007 - WSA_QOS_NO_SENDERS";
          ErrorMessage = "No QOS senders.";
          break;

        case 11008:
          ErrorCode = "11008 - WSA_QOS_NO_RECEIVERS";
          ErrorMessage = "QOS no receivers.";
          break;

        case 11009:
          ErrorCode = "11009 - WSA_QOS_REQUEST_CONFIRMED";
          ErrorMessage = "QOS request confirmed.";
          break;

        case 11010:
          ErrorCode = "11010 - WSA_QOS_ADMISSION_FAILURE";
          ErrorMessage = "QOS admission error.";
          break;

        case 11011:
          ErrorCode = "11011 - WSA_QOS_POLICY_FAILURE";
          ErrorMessage = "QOS policy failure.";
          break;

        case 11012:
          ErrorCode = "11012 - WSA_QOS_BAD_STYLE";
          ErrorMessage = "QOS bad style.";
          break;

        case 11013:
          ErrorCode = "11013 - WSA_QOS_BAD_OBJECT";
          ErrorMessage = "QOS bad object.";
          break;

        case 11014:
          ErrorCode = "11014 - WSA_QOS_TRAFFIC_CTRL_ERROR";
          ErrorMessage = "QOS traffic control error.";
          break;

        case 11015:
          ErrorCode = "11015 - WSA_QOS_GENERIC_ERROR";
          ErrorMessage = "QOS generic error.";
          break;

        case 11016:
          ErrorCode = "11016 - WSA_QOS_ESERVICETYPE";
          ErrorMessage = "QOS service type error.";
          break;

        case 11017:
          ErrorCode = "11017 - WSA_QOS_EFLOWSPEC";
          ErrorMessage = "QOS flowspec error.";
          break;

        case 11018:
          ErrorCode = "11018 - WSA_QOS_EPROVSPECBUF";
          ErrorMessage = "Invalid QOS provider buffer.";
          break;

        case 11019:
          ErrorCode = "11019 - WSA_QOS_EFILTERSTYLE";
          ErrorMessage = "Invalid QOS filter style.";
          break;

        case 11020:
          ErrorCode = "11020 - WSA_QOS_EFILTERTYPE";
          ErrorMessage = "Invalid QOS filter type.";
          break;

        case 11021:
          ErrorCode = "11021 - WSA_QOS_EFILTERCOUNT";
          ErrorMessage = "Incorrect QOS filter count.";
          break;

        case 11022:
          ErrorCode = "11022 - WSA_QOS_EOBJLENGTH";
          ErrorMessage = "Invalid QOS object length.";
          break;

        case 11023:
          ErrorCode = "11023 - WSA_QOS_EFLOWCOUNT";
          ErrorMessage = "Incorrect QOS flow count.";
          break;

        case 11024:
          ErrorCode = "11024 - WSA_QOS_EUNKOWNPSOBJ";
          ErrorMessage = "Unrecognized QOS object.";
          break;

        case 11025:
          ErrorCode = "11025 - WSA_QOS_EPOLICYOBJ";
          ErrorMessage = "Invalid QOS policy object.";
          break;

        case 11026:
          ErrorCode = "11026 - WSA_QOS_EFLOWDESC";
          ErrorMessage = "Invalid QOS flow descriptor.";
          break;

        case 11027:
          ErrorCode = "11027 - WSA_QOS_EPSFLOWSPEC";
          ErrorMessage = "Invalid QOS provider-specific flowspec.";
          break;

        case 11028:
          ErrorCode = "11028 - WSA_QOS_EPSFILTERSPEC";
          ErrorMessage = "Invalid QOS provider-specific filterspec.";
          break;

        case 11029:
          ErrorCode = "11029 - WSA_QOS_ESDMODEOBJ";
          ErrorMessage = "Invalid QOS shape discard mode object.";
          break;

        case 11030:
          ErrorCode = "11030 - WSA_QOS_ESHAPERATEOBJ";
          ErrorMessage = "Invalid QOS shaping rate object.";
          break;

        case 11031:
          ErrorCode = "11031 - WSA_QOS_RESERVED_PETYPE";
          ErrorMessage = "Reserved policy QOS element type.";
          break;

        default:
          ErrorCode = x.ErrorCode.ToString();
          ErrorMessage = x.Message;
          break;
      }
      return nl +
        "Class: " + x.GetType().Name + nl +
        "Code: " + ErrorCode + nl +
        "Message: " + ErrorMessage + nl +
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