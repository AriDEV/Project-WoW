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
using Framework.Misc;
using Framework.Network.Packets;
using Framework.Objects;
using World.Shared.Game.Entities.Object.Descriptors;

namespace World.Shared.Game.Entities.Object
{
    public abstract class WorldObjectBase
    {
        public SmartGuid Guid { get; set; }
        public ObjectData ObjectData { get; }

        DescriptorData descriptors;

        public WorldObjectBase(int descriptorLength)
        {
            descriptors = new DescriptorData(descriptorLength);
            ObjectData = new ObjectData();
        }

        public void WriteToPacket(Packet pkt)
        {
            descriptors.WriteToPacket(pkt);
        }

        #region Descriptors Set Functions
        public void Set(DescriptorField descriptor, sbyte value, int offset = 0)
        {
            descriptors.Mask.Set(descriptor.Index, true);

            if (descriptors.Data.ContainsKey(descriptor.Index))
                descriptors.Data[descriptor.Index] = descriptors.Data[descriptor.Index].ChangeType<int>() | value << (offset << 3);
            else
                descriptors.Data[descriptor.Index] = value << (offset << 3);

            descriptor.Value = descriptors.Data[descriptor.Index];
        }

        public void Set(DescriptorField descriptor, byte value, int offset = 0)
        {
            descriptors.Mask.Set(descriptor.Index, true);

            if (descriptors.Data.ContainsKey(descriptor.Index))
                descriptors.Data[descriptor.Index] = descriptors.Data[descriptor.Index].ChangeType<uint>() | (uint)((value) << (offset << 3));
            else
                descriptors.Data[descriptor.Index] = (uint)((value) << (offset << 3));

            descriptor.Value = descriptors.Data[descriptor.Index];
        }

        public void Set(DescriptorField descriptor, short value, int offset = 0)
        {
            descriptors.Mask.Set(descriptor.Index, true);

            if (descriptors.Data.ContainsKey(descriptor.Index))
                descriptors.Data[descriptor.Index] = descriptors.Data[descriptor.Index].ChangeType<int>() | value << (offset << 4);
            else
                descriptors.Data[descriptor.Index] = value << (offset << 4);

            descriptor.Value = descriptors.Data[descriptor.Index];
        }

        public void Set(DescriptorField descriptor, ushort value, int offset = 0)
        {
            descriptors.Mask.Set(descriptor.Index, true);

            if (descriptors.Data.ContainsKey(descriptor.Index))
                descriptors.Data[descriptor.Index] = descriptors.Data[descriptor.Index].ChangeType<uint>() | (uint)((value) << (offset << 4));
            else
                descriptors.Data[descriptor.Index] = (uint)((value) << (offset << 4));

            descriptor.Value = descriptors.Data[descriptor.Index];
        }

        public void Set(DescriptorField descriptor, int value, int offset = 0)
        {
            descriptors.Mask.Set(descriptor.Index, true);

            descriptors.Data[descriptor.Index] = value;

            descriptor.Value = value;
        }

        public void Set(DescriptorField descriptor, uint value, int offset = 0)
        {
            descriptors.Mask.Set(descriptor.Index, true);

            descriptors.Data[descriptor.Index] = value;

            descriptor.Value = value;
        }

        public void Set(DescriptorField descriptor, float value, int offset = 0)
        {
            descriptors.Mask.Set(descriptor.Index, true);

            descriptors.Data[descriptor.Index] = value;

            descriptor.Value = value;
        }

        public void Set(DescriptorField descriptor, long value, int offset = 0)
        {
            descriptors.Mask.Set(descriptor.Index, true);
            descriptors.Mask.Set(descriptor.Index + 1, true);

            descriptors.Data[descriptor.Index] = (int)(value & int.MaxValue);
            descriptors.Data[descriptor.Index + 1] = (int)((value >> 32) & int.MaxValue);

            descriptor.Value = value;
        }

        public void Set(DescriptorField descriptor, ulong value, int offset = 0)
        {
            descriptors.Mask.Set(descriptor.Index, true);
            descriptors.Mask.Set(descriptor.Index + 1, true);

            descriptors.Data[descriptor.Index] = (uint)(value & uint.MaxValue);
            descriptors.Data[descriptor.Index + 1] = (uint)((value >> 32) & uint.MaxValue);

            descriptor.Value = value;
        }
        #endregion

        public T Get<T>(DescriptorField descriptor)
        {
            throw new NotImplementedException();
        }
    }
}
