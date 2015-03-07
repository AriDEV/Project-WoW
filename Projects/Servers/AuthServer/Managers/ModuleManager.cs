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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AuthServer.Network.Sessions;
using Framework.Constants.Account;
using Framework.Constants.Misc;
using Framework.Database;
using Framework.Database.Auth.Entities;
using Framework.Logging;
using Framework.Misc;
using Framework.Network.Packets;

namespace AuthServer.Managers
{
    class ModuleManager : Singleton<ModuleManager>
    {
        public readonly List<Module> Modules;

        ModuleManager()
        {
            Modules = new List<Module>();

            UpdateModules();
        }

        public void UpdateModules()
        {
            Log.Normal("Loading auth modules...");

            var modules = DB.Auth.Select<Module>();

            modules.ForEach(m =>
            {
                if (AddModule(m, Modules))
                    Log.Debug("New auth module '{0}, {1}' loaded.", m.Name, m.System);
            });

            Log.Normal($"Successfully loaded {Modules.Count} auth modules.");

            IsInitialized = true;
        }

        bool AddModule(Module module, IList list)
        {
            if (!list.Contains(module))
                return list.Add(module) != -1;

            return false;
        }

        public void WriteModuleHeader(Client client, AuthPacket packet, Module module, int size = 0)
        {
            packet.WriteFourCC(module.Type);
            packet.WriteFourCC("\0\0" + Enum.GetName(typeof(Region), client.Session.Account.Region));
            packet.Write(module.Hash.ToByteArray());
            packet.Write(size == 0 ? module.Size : (uint)size, 10);
        }

        public void WriteRiskFingerprint(Client client, AuthPacket packet)
        {
            var riskFingerprintModule = client.Modules.SingleOrDefault(m => m.Name == "RiskFingerprint");

            WriteModuleHeader(client, packet, riskFingerprintModule);
        }
    }
}
