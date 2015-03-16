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
using System.IO;
using Framework.Constants.Misc;
using Framework.Logging;
using Framework.Logging.IO;
using Framework.Misc;
using Lappa_ORM;

namespace AuthServer.Configuration
{
    class AuthConfig
    {
        public static bool IsInitialized = false;
        static Config config;

        #region Config Options
        public static LogType LogLevel;
        public static string LogDirectory;
        public static string LogConsoleFile;
        public static string LogPacketFile;

        public static ConnectionType AuthDBType;

        public static string AuthDBHost; 
        public static int AuthDBPort;
        public static string AuthDBUser;
        public static string AuthDBPassword;
        public static string AuthDBDataBase;

        public static int AuthDBMinPoolSize;
        public static int AuthDBMaxPoolSize;

        public static string BindIP;
        public static int BindPort;

        public static int RealmListUpdateTime;

        public static string PatchFileDirectory;
        #endregion

        public static void Initialize(string file)
        {
            // Initialize exception logger
            if (!Directory.Exists("Crashes"))
                Directory.CreateDirectory("Crashes");

            var el = new LogWriter("Crashes", "AuthServer.log");

            ExceptionLog.Initialize(el);

            config = new Config(file);

            if (config != null)
            {
                IsInitialized = true;

                LogLevel         = (LogType)config.Read("Log.Level", 0x7, true);
                LogDirectory     = config.Read("Log.Directory", "Logs/Auth");
                LogConsoleFile   = config.Read("Log.Console.File", "");
                LogPacketFile    = config.Read("Log.Packet.File", "");

                LogWriter fl = null;

                if (LogConsoleFile != "")
                {
                    if (!Directory.Exists(LogDirectory))
                        Directory.CreateDirectory(LogDirectory);

                    fl = new LogWriter(LogDirectory, LogConsoleFile);
                }

                Log.Initialize(LogLevel, fl);
                
                if (LogPacketFile != "")
                    PacketLog.Initialize(LogDirectory, LogPacketFile);
            }

            ReadConfig();
        }

        static void ReadConfig()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("AuthServer config not initialized.");

            AuthDBType          = config.Read("AuthDB.Type", ConnectionType.MYSQL);
            AuthDBHost          = config.Read("AuthDB.Host", "127.0.0.1");
            AuthDBPort          = config.Read("AuthDB.Port", 3306);
            AuthDBUser          = config.Read("AuthDB.User", "root");
            AuthDBPassword      = config.Read("AuthDB.Password", "");
            AuthDBDataBase      = config.Read("AuthDB.Database", "AuthDB");

            AuthDBMinPoolSize   = config.Read("AuthDB.MinPoolSize", 5);
            AuthDBMaxPoolSize   = config.Read("AuthDB.MaxPoolSize", 30);

            BindIP              = config.Read("Bind.IP", "0.0.0.0");
            BindPort            = config.Read("Bind.Port", 1119);

            RealmListUpdateTime = config.Read("RealmList.UpdateTime", 5) * 60000;

            PatchFileDirectory  = config.Read("Patch.File.Directory", "PatchFiles");
        }
    }
}
