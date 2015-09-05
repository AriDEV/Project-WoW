﻿// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Framework.Constants.Items;
using Framework.Datastore;
using Lappa_ORM;
using InvType = Framework.Constants.Items.InventoryType;

namespace Framework.Database.Data.Entities
{
    public class Item : Entity
    {
        public uint Id                     { get; set; }
        public byte Class                  { get; set; }
        public int SubClass                { get; set; }
        public int SoundOverrideSubClassId { get; set; }
        public int Material                { get; set; }
        public int InventoryType           { get; set; }
        public int SheatheType             { get; set; }
        public int FileDataId              { get; set; } // Icon
        public int Unknown2                { get; set; }

        // Non table properties (defined with get & private set), ignored on database queries.
        public int DisplayId { get; private set; }
        public EquipmentSlot Slot { get; private set; }

        public override void InitializeNonTableProperties()
        {
            DisplayId = GetDisplayId();
            Slot = GetEquipmentSlot();
        }

        // Set default DisplayId for every Item using Mode = 0 & Version = 0.
        // The correct DisplayId is chosen based on the CharacterItems table data.
        int GetDisplayId()
        {
            var appearanceId = ClientDB.ItemModifiedAppearances[(int)Id].SingleOrDefault(ima => ima.Mode == 0 && ima.Version == 0)?.AppearanceId;
            var displayId = 0;

            if (appearanceId != null && appearanceId != 0)
                displayId = ClientDB.ItemAppearances[(uint)appearanceId].DisplayId;

            return displayId;
        }

        EquipmentSlot GetEquipmentSlot()
        {
            var slot = EquipmentSlot.NonEquip;

            switch ((InvType)InventoryType)
            {
                case InvType.Usable:
                    slot = EquipmentSlot.NonEquip;
                    break;
                case InvType.Finger:
                    slot = EquipmentSlot.Finger0;
                    break;
                case InvType.Trinket:
                    slot = EquipmentSlot.Trinket0;
                    break;
                case InvType.Weapon:
                case InvType.TwoHandWeapon:
                case InvType.WeaponMainHand:
                case InvType.Thrown:
                case InvType.RangedRight:
                case InvType.Quiver:
                case InvType.Relic:
                    slot = EquipmentSlot.MainHand;
                    break;
                case InvType.Shield:
                case InvType.WeaponOffHand:
                case InvType.Holdable:
                    slot = EquipmentSlot.SecondaryHand;
                    break;
                case InvType.Cloak:
                    slot = EquipmentSlot.Back;
                    break;
                case InvType.Tabard:
                    slot = EquipmentSlot.Tabard;
                    break;
                case InvType.Robe:
                    slot = EquipmentSlot.Chest;
                    break;
                default:
                    slot = (EquipmentSlot)(InventoryType - 1);
                    break;
            }

            return slot;
        }
    }
}
