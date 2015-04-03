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

using CharacterServer.Attributes;
using CharacterServer.Constants.Net;
using CharacterServer.Managers;
using CharacterServer.Network;
using CharacterServer.Packets.Client.Misc;
using Framework.Constants.Account;
using Framework.Logging;
using Framework.Packets.Server.Net;

namespace CharacterServer.Packets.Handlers
{
    class MiscHandler
    {
        [Message(ClientMessage.LoadingScreenNotify, SessionState.Authenticated)]
        public static void HandleLoadingScreenNotify(LoadingScreenNotify loadingScreenNotify, CharacterSession session)
        {
            Log.Debug("Loading screen for map '{0}' {1}.", loadingScreenNotify.MapID, loadingScreenNotify.Showing ? "enabled" : "disabled");

            if (loadingScreenNotify.Showing)
            {
                var worldServer = Manager.Redirect.GetWorldServer(loadingScreenNotify.MapID);

                if (worldServer != null)
                {
                    NetHandler.SendConnectTo(session, worldServer.Address, worldServer.Port);

                    // Suspend the current connection
                    session.Send(new SuspendComms { Serial = 0x14 });
                }
            }
        }
    }
}
