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
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Framework.Attributes;
using Framework.Constants.Misc;
using Framework.Logging;

namespace AuthServer.Commands
{
    class ConsoleCommandManager
    {
        protected static Dictionary<string, HandleCommand> CommandHandlers = new Dictionary<string, HandleCommand>();
        public delegate void HandleCommand(string[] args);

        public static void DefineCommands()
        {
            var currentAsm = Assembly.GetEntryAssembly();

            foreach (var type in currentAsm.GetTypes())
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    foreach (var commandAttr in methodInfo.GetCustomAttributes<ConsoleCommandAttribute>())
                        if (commandAttr != null)
                            CommandHandlers[commandAttr.Command] = (HandleCommand)Delegate.CreateDelegate(typeof(HandleCommand), methodInfo);
                }
            }
        }

        public static void InitCommands()
        {
            DefineCommands();

            Log.Normal("AuthServer successfully started");
            Log.Normal("Total Memory: {0} Kilobytes", GC.GetTotalMemory(false) / 1024);

            while (true)
            {
                Thread.Sleep(1);

                Log.Normal("AuthServer >> ");
                 
                var line = Console.ReadLine().Split(new string[] { " " }, StringSplitOptions.None);

                if (line.Length > 0)
                {
                    var args = new string[line.Length - 1];

                    if (args.Length > 0)
                        Array.Copy(line, 1, args, 0, args.Length);

                    InvokeHandler(line[0].ToLower(), args);
                }
            }
        }

        static void InvokeHandler(string command, params string[] args)
        {
            if (CommandHandlers.ContainsKey(command.ToLower()))
                CommandHandlers[command].Invoke(args);
            else if (command != "")
                Log.Error($"'{command}' isn't a valid console command.");
        }
    }
}
