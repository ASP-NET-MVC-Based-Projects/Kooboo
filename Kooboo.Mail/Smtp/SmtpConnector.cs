//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Kooboo.Mail.Smtp
{
    public class SmtpConnector
    {
        private TcpClient _client;
        private Stream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private SmtpServer _server;

        public SmtpConnector(SmtpServer server, TcpClient client)
        {
            _server = server;
            _client = client;
            Local = CopyIPEndPoint(_client.Client.LocalEndPoint as IPEndPoint);
            Client = CopyIPEndPoint(_client.Client.RemoteEndPoint as IPEndPoint);
        }

        public IPEndPoint Local { get; set; }

        public IPEndPoint Client { get; set; }

        public async Task Accept()
        {
            Exception exception = null;

            try
            {
                _stream = _client.GetStream();
                if (_server.Certificate != null)
                {
                    var ssl = new SslStream(_stream, true);
                    await ssl.AuthenticateAsServerAsync(_server.Certificate);
                    _stream = ssl;
                }
                _reader = new System.IO.StreamReader(_stream);
                _writer = new System.IO.StreamWriter(_stream) {AutoFlush = true};

                Kooboo.Mail.Smtp.SmtpSession session = new Smtp.SmtpSession(this.Client.Address.ToString());

                // Service ready
                await _writer.WriteLineAsync(session.ServiceReady().Render());

                var commandline = await _reader.ReadLineAsync();
                while (commandline != null)
                {
                    var response = session.Command(commandline);
                    if (response.SendResponse)
                    {
                        var responseline = response.Render();
                        await _writer.WriteLineAsync(responseline);
                    }

                    if (response.SessionCompleted)
                    {
                        await Kooboo.Mail.Transport.Incoming.Receive(session);
                        session.ReSet();
                    }

                    if (response.Close)
                    {
                        _client.Close();
                        break;
                    }

                    // When enter the session state, read till the end .
                    if (session.State == SmtpSession.CommandState.Data)
                    {
                        var reptcounts = session.Log.Keys.Count(o => o.Name == SmtpCommandName.RCPTTO);

                        if (!Kooboo.Data.Infrastructure.InfraManager.Test(session.OrganizationId, Data.Infrastructure.InfraType.Email, reptcounts))
                        {
                            await _writer.WriteLineAsync("550 you have no enough credit to send emails");
                            _client.Close();
                            break;
                        }

                        var data = await _stream.ReadToDotLine(TimeSpan.FromSeconds(60));

                        var dataresponse = session.Data(data);

                        if (dataresponse.SendResponse)
                        {
                            await _writer.WriteLineAsync(dataresponse.Render());
                        }

                        if (dataresponse.SessionCompleted)
                        {
                            await Kooboo.Mail.Transport.Incoming.Receive(session);

                            var tos = session.Log.Keys.Where(o => o.Name == SmtpCommandName.RCPTTO);

                            string subject = "TO: ";
                            if (tos != null)
                            {
                                foreach (var item in tos)
                                {
                                    if (item != null && !string.IsNullOrWhiteSpace(item.Value))
                                    {
                                        subject += item.Value;
                                    }
                                }
                            }

                            Kooboo.Data.Infrastructure.InfraManager.Add(session.OrganizationId, Data.Infrastructure.InfraType.Email, reptcounts, subject);

                            session.ReSet();
                        }

                        if (dataresponse.Close)
                        {
                            _client.Close();
                            break;
                        }
                    }

                    commandline = await _reader.ReadLineAsync();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                Kooboo.Data.Log.Instance.Exception.Write(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Source);
            }

            if (exception != null)
            {
                // 有异常一定要关闭session
                try
                {
                    if (_client.Connected)
                    {
                        await _writer.WriteLineAsync("550 Internal Server Error");
                        _client.Close();
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        private IPEndPoint CopyIPEndPoint(IPEndPoint ip)
        {
            return new IPEndPoint(ip.Address, ip.Port);
        }
    }
}