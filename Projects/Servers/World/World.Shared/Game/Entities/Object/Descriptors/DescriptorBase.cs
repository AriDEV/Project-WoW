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

using World.Shared.Constants.Objects;

namespace World.Shared.Game.Entities.Object.Descriptors
{
    public class DescriptorBase
    {
        public int BaseEnd { get; set; }

        public DescriptorBase() { }

        protected DescriptorBase(int baseEnd)
        {
            BaseEnd = baseEnd;
        }

        public DescriptorField this[int index, uint size, MirrorFlags flags = MirrorFlags.All] => new DescriptorField { Index = index + BaseEnd, Size = size, Flags = flags };
        public DescriptorField this[int index] => new DescriptorField { Index = index + BaseEnd };

        public static int End { get; set; }
    }
}
