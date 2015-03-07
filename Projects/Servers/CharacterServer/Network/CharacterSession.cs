﻿/*
 * Copyright (C) 2012-2015 Arctium Emulation <http://arctium.org>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using CharacterServer.Constants.Net;
using CharacterServer.Packets;
using Framework.Constants.Account;
using Framework.Constants.Misc;
using Framework.Database.Auth.Entities;
using Framework.Logging;
using Framework.Logging.IO;
using Framework.Misc;
using Framework.Network;
using Framework.Network.Packets;
using ClientPacket = Framework.Packets.Client.Authentication;
using ServerPacket = Framework.Packets.Server.Authentication;

namespace CharacterServer.Network
{
    class CharacterSession : SessionBase
    {
        public Realm Realm { get; set; }
        public Account Account { get; set; }
        public GameAccount GameAccount { get; set; }
        public uint Challenge { get; private set; }

        public CharacterSession(Socket clientSocket) : base(clientSocket) { }

        public override void OnConnection(object sender, SocketAsyncEventArgs e)
        {
            var recievedBytes = e.BytesTransferred;

            if (recievedBytes == 0x32 && !isTransferInitiated[1])
            {
                var clientToServer = "WORLD OF WARCRAFT CONNECTION - CLIENT TO SERVER\0";
                var transferInit = new ClientPacket.TransferInitiate { Packet = new Packet(dataBuffer, 2) } as ClientPacket.TransferInitiate;

                transferInit.Read();

                if (transferInit.Msg == clientToServer)
                {
                    State = SessionState.Initiated;

                    isTransferInitiated[1] = true;

                    e.Completed -= OnConnection;
                    e.Completed += Process;

                    Log.Debug($"Initial packet transfer for Client '{GetClientInfo()}' successfully initialized.");

                    client.ReceiveAsync(e);

                    // Assign server challenge for auth digest calculations
                    Challenge = BitConverter.ToUInt32(new byte[0].GenerateRandomKey(4), 0);

                    Send(new ServerPacket.AuthChallenge { Challenge = Challenge });
                }
                else
                {
                    Log.Debug($"Wrong initial packet transfer data for Client '{GetClientInfo()}'.");

                    Dispose();
                }
            }
            else
                Dispose();
        }

        public override void Process(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                var socket = e.UserToken as Socket;
                var recievedBytes = e.BytesTransferred;

                if (recievedBytes != 0)
                {
                    if (Crypt != null && Crypt.IsInitialized)
                    {
                        while (recievedBytes > 0)
                        {
                            Decrypt(dataBuffer);

                            var length = BitConverter.ToUInt16(dataBuffer, 0) + 4;
                            var packetData = new byte[length];

                            Buffer.BlockCopy(dataBuffer, 0, packetData, 0, length);

                            var packet = new Packet(dataBuffer, 4);

                            if (length > recievedBytes)
                                packetQueue.Enqueue(packet);

                            Task.Run(() => ProcessPacket(packet));

                            recievedBytes -= length;

                            Buffer.BlockCopy(dataBuffer, length, dataBuffer, 0, recievedBytes);
                        }
                    }
                    else
                    {
                        var packet = new Packet(dataBuffer);

                        Task.Run(() => ProcessPacket(packet));
                    }

                    client.ReceiveAsync(e);
                }
            }
            catch (Exception ex)
            {
                Dispose();

                ExceptionLog.Write(ex);

                Log.Error(ex.Message);
            }
        }

        public override async Task ProcessPacket(Packet packet)
        {
            if (packetQueue.Count > 0)
                packet = packetQueue.Dequeue();

            PacketLog.Write<ClientMessage>(packet.Header.Message, packet.Data, client.RemoteEndPoint);

            await PacketManager.InvokeHandler<ClientMessage>(packet, this);
        }

        public override void Send(Framework.Network.Packets.ServerPacket packet)
        {
            try
            {
                packet.Write();
                packet.Packet.Finish();

                if (packet.Packet.Header != null)
                    PacketLog.Write<ServerMessage>(packet.Packet.Header.Message, packet.Packet.Data, client.RemoteEndPoint);

                if (Crypt != null && Crypt.IsInitialized)
                    Encrypt(packet.Packet);

                var socketEventargs = new SocketAsyncEventArgs();

                socketEventargs.SetBuffer(packet.Packet.Data, 0, packet.Packet.Data.Length);

                socketEventargs.Completed += SendCompleted;
                socketEventargs.UserToken = packet;
                socketEventargs.RemoteEndPoint = client.RemoteEndPoint;
                socketEventargs.SocketFlags = SocketFlags.None;

                client.SendAsync(socketEventargs);
            }
            catch (SocketException ex)
            {
                Log.Error($"{ex}");

                client.Close();
            }
        }

        void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
        }
    }
}
