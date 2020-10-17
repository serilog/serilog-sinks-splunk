using System.Net.Sockets;

namespace Serilog.Sinks.Splunk
{
    internal static class SocketExtensions
    {
        internal static void SecureDispose(this Socket socket)
        {
            if (socket == null)
                return;
            try
            {
                socket.Close();
                socket.Dispose();
            }
            catch { }
        }
    }
}
