using Helpers.Storage;
using InternetRadioShared.Helpers;
using InternetRadioShared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace InternetRadioShared.Models
{
    public class SocketServer
    {
        private InternetRadioHelper internetRadioHelper = new InternetRadioHelper();

        /// <summary>
        /// Starts server.
        /// </summary>
        /// <param name="port">Port</param>
        public async void Start(int port)
        {
            try
            {
                //Create a StreamSocketListener to start listening for TCP connections.
                StreamSocketListener socketListener = new StreamSocketListener();

                //Hook up an event handler to call when connections are received.
                socketListener.ConnectionReceived += SocketListener_ConnectionReceived;

                //Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await socketListener.BindServiceNameAsync(port.ToString());
            }
            catch { }
        }

        /// <summary>
        /// Event for receiving new connection. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void SocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
        try
        {
            using (DataReader reader = new DataReader(args.Socket.InputStream))
            {
                using (DataWriter writer = new DataWriter(args.Socket.OutputStream))
                {
                    while (true)
                    {
                        // Read first 4 bytes (length of the subsequent string).
                        uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                        if (sizeFieldCount != sizeof(uint))
                        {
                            // The underlying socket was closed before we were able to read the whole data.
                            return;
                        }
                        // Read the string.
                        uint stringLength = reader.ReadUInt32();
                        uint actualStringLength = await reader.LoadAsync(stringLength);
                        if (stringLength != actualStringLength)
                        {
                            // The underlying socket was closed before we were able to read the whole data.
                            return;
                        }
                        // Get request string
                        string request = reader.ReadString(actualStringLength);

                        // Process command and get response
                        SocketResponse response = ProcessRequest(request);
                        string responseString = JsonHelper.ToJson<SocketResponse>(response);

                        // Write first the length of the string as UINT32 value followed up by the string. 
                        // Writing data to the writer will just store data in memory.
                        writer.WriteUInt32(writer.MeasureString(responseString));
                        writer.WriteString(responseString);
                        // Write the locally buffered data to the network.
                        await writer.StoreAsync();
                    }
                }
            }
        }
        catch(Exception ex)
        {
            // If this is an unknown status it means that the error if fatal and retry will likely fail.
            if (Windows.Networking.Sockets.SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
            {
                throw;
            }
        }
}

        /// <summary>
        /// Custom void for recognizing and processing command.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private SocketResponse ProcessRequest(string request)
        {
            SocketCommand command = JsonHelper.FromJson<SocketCommand>(request);
            Debug.WriteLine("Recieved command: " + command.Type.ToString());

            switch (command.Type)
            {
                #region BackgroundAudioManipulation

                case SocketCommand.CommandType.PLAY:
                    Radio currentRadio = internetRadioHelper.GetRadio(command.Parameters[0].ToString());
                    internetRadioHelper.CurrentRadio = currentRadio;
                    return BackgroundAudioHelper.PlayRadio(currentRadio);

                case SocketCommand.CommandType.STOP:
                    return BackgroundAudioHelper.Stop();

                case SocketCommand.CommandType.SET_VOLUME:
                    return BackgroundAudioHelper.SetVolume(Double.Parse(command.Parameters[0].ToString()));

                case SocketCommand.CommandType.GET_VOLUME:
                    return BackgroundAudioHelper.GetVolume();

                #endregion

                #region InternetRadioManipulation

                case SocketCommand.CommandType.GET_STATUS:
                    return InternetRadioStatusHelper.GetStatus(internetRadioHelper.CurrentRadio);

                case SocketCommand.CommandType.GET_RADIOS:
                      return internetRadioHelper.GetRadios();

                case SocketCommand.CommandType.ADD_RADIO:
                    return internetRadioHelper.AddRadio(JsonHelper.FromJson<Radio>(command.Parameters[0].ToString()));

                case SocketCommand.CommandType.DELETE_RADIO:
                    return internetRadioHelper.DeleteRadio(command.Parameters[0].ToString());

                #endregion

                default:
                    return new SocketResponse(SocketResponse.StatusCode.BAD_REQUEST);
            }
        }
    }
}
