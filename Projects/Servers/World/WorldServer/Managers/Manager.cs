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

namespace WorldServer.Managers
{
    class Manager
    {
        public static ClientDBManager ClientDB;
        public static RedirectManager Redirect;
        public static SessionManager Session;
        public static PlayerManager Player;

        public static void Initialize()
        {
            ClientDB = ClientDBManager.GetInstance();
            Redirect = RedirectManager.GetInstance();
            Session  = SessionManager.GetInstance();
            Player   = PlayerManager.GetInstance();
        }
    }
}
