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

using AuthServer.Attributes;
using AuthServer.Constants.Authentication;
using AuthServer.Constants.Net;
using AuthServer.Managers;
using AuthServer.Network.Sessions;
using Framework.Constants.Misc;
using Framework.Database;
using Framework.Database.Auth.Entities;
using Framework.Logging;
using Framework.Network.Packets;

namespace AuthServer.Network.Packets.Handlers
{
    class MiscHandler
    {
        [AuthMessage(AuthClientMessage.InformationRequest, AuthChannel.Authentication)]
        public static void OnInformationRequest(AuthPacket packet, Client client)
        {
            var session = client.Session;
            if (!Manager.GetState())
            {
                AuthHandler.SendAuthComplete(true, AuthResult.ServerBusy, client);
                return;
            }

            var game = packet.ReadFourCC();
            var os = packet.ReadFourCC();
            var language = packet.ReadFourCC();

            Log.Debug($"Program: {game}");
            Log.Debug($"Platform: {os}");
            Log.Debug($"Locale: {language}");

            var componentCount = packet.Read<int>(6);

            for (int i = 0; i < componentCount; i++)
            {
                var program = packet.ReadFourCC();
                var platform = packet.ReadFourCC();
                var build = packet.Read<int>(32);

                Log.Debug($"Program: {program}");
                Log.Debug($"Platform: {platform}");
                Log.Debug($"Locale: {build}");

                if (DB.Auth.Any<Component>(c => c.Program == program && c.Platform == platform && c.Build == build))
                    continue;

                if (!DB.Auth.Any<Component>(c => c.Program == program))
                {
                    AuthHandler.SendAuthComplete(true, AuthResult.InvalidProgram, client);
                    return;
                }

                if (!DB.Auth.Any<Component>(c => c.Platform == platform))
                {
                    AuthHandler.SendAuthComplete(true, AuthResult.InvalidPlatform, client);
                    return;
                }

                if (!DB.Auth.Any<Component>(c => c.Build == build))
                {
                    AuthHandler.SendAuthComplete(true, AuthResult.InvalidGameVersion, client);
                    return;
                }
            }

            var hasAccountName = packet.Read<bool>(1);

            if (hasAccountName)
            {
                var accountLength = packet.Read<int>(9) + 3;
                var accountName = packet.ReadString(accountLength);
                var account = DB.Auth.Single<Account>(a => a.Email == accountName);

                // First account lookup on database
                if ((session.Account = account) != null)
                {
                    // Global base account.
                    session.Account.IP = session.GetClientInfo();
                    session.Account.Language = language;

                    // Assign the possible game accounts based on the game.
                    //session.GameAccounts.ForEach(ga => ga.OS = os);

                    // Save the last changes
                    DB.Auth.Update(session.Account, "IP", "Language");

                    // Used for module identification.
                    client.Game = game;
                    client.OS = os;

                    AuthHandler.SendProofRequest(client);
                }
                else
                    AuthHandler.SendAuthComplete(true, AuthResult.BadLoginInformation, client);
            }
        }

        [AuthMessage(AuthClientMessage.Ping, AuthChannel.Connection)]
        public static void OnPing(AuthPacket packet, Client client)
        {
            var pong = new AuthPacket(AuthServerMessage.Pong, AuthChannel.Connection);

            client.SendPacket(pong);
        }

        [AuthMessage(AuthClientMessage.Disconnect, AuthChannel.Connection)]
        public static void OnDisconnect(AuthPacket packet, Client client)
        {
            Log.Debug($"Client '{client.ConnectionInfo}' disconnected.");

            Manager.SessionMgr.RemoveClient(client.Id);
        }
    }
}
