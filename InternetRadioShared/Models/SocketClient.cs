using Helpers.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace InternetRadioShared.Models
{
    public class SocketClient
    {
        private StreamSocket socket;
        private string hostName;
        private int port;
        private DataReader reader;
        private DataWriter writer;

        public SocketClient(string hostName, int port)
        {
            this.hostName = hostName;
            this.port = port;
        }

        /// <summary>
        /// Connects client to socket server.
        /// </summary>
        /// <param name="responseTimeout"></param>
        /// <returns></returns>
        private async Task Connect(int responseTimeout)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            try
            {
                cts.CancelAfter(responseTimeout);

                //Create the StreamSocket and establish a connection to the echo server.
                socket = new StreamSocket();

                // If necessary, tweak the socket's control options before carrying out the connect operation.
                // Refer to the StreamSocketControl class' MSDN documentation for the full list of control options.
                socket.Control.KeepAlive = false;
                socket.Control.NoDelay = true;

                //The server hostname that we will be establishing a connection to. We will be running the server and client locally,
                //so we will use localhost as the hostname.
                HostName serverHost = new HostName(hostName);

                //Every protocol typically has a standard port number. For example HTTP is typically 80, FTP is 20 and 21, etc.
                string serverPort = port.ToString();
                await socket.ConnectAsync(serverHost, serverPort).AsTask(cts.Token);

                reader = new DataReader(socket.InputStream);
                writer = new DataWriter(socket.OutputStream);
            }
            catch
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Semaphore to allow only one command at time.
        /// </summary>
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        /// <summary>
        /// Sends command to socket server.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="connectionTimeout"></param>
        /// <returns></returns>
        public async Task<SocketResponse> SendCommand(SocketCommand command, int connectionTimeout = 2000)
        {
            // Wait while previous command is proccessed
            await _semaphoreSlim.WaitAsync();

            try
            {
                // Connect to socket if not connected
                if (socket == null)
                    await Connect(connectionTimeout);

                // Serialize command to json and send
                string request = JsonHelper.ToJson<SocketCommand>(command);
                // Write first the length of the string as UINT32 value followed up by the string. 
                // Writing data to the writer will just store data in memory.
                writer.WriteUInt32(writer.MeasureString(request));
                writer.WriteString(request);
                // Write the locally buffered data to the network.
                await writer.StoreAsync();

                // Read first 4 bytes (length of the subsequent string).
                uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                if (sizeFieldCount != sizeof(uint))
                {
                    // The underlying socket was closed before we were able to read the whole data.
                    return new SocketResponse(SocketResponse.StatusCode.EXCEPTION, "The underlying socket was closed before we were able to read the whole data.");
                }
                // Read the string.
                uint stringLength = reader.ReadUInt32();
                uint actualStringLength = await reader.LoadAsync(stringLength);
                if (stringLength != actualStringLength)
                {
                    // The underlying socket was closed before we were able to read the whole data.
                    return new SocketResponse(SocketResponse.StatusCode.EXCEPTION, "The underlying socket was closed before we were able to read the whole data.");
                }
                string responseString = reader.ReadString(actualStringLength);
                return JsonHelper.FromJson<SocketResponse>(responseString);
            }
            catch (Exception ex)
            {
                Disconnect();
                throw ex;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Disconnects socket client from server.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                writer.DetachStream();
                writer.Dispose();
                reader.DetachStream();
                reader.Dispose();
            }
            catch { }

            try
            {
                socket.Dispose();
                socket = null;
            }
            catch { }
        }
    }
}
