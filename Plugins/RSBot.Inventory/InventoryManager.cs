using RSBot.Core;
using System.Collections.Generic;
using System.Linq;

namespace RSBot.Inventory
{
    public class InventoryManager
    {
        /// <summary>
        /// Uses an item from a specific slot
        /// </summary>
        public static void UseItemBySlot(byte slot)
        {
            var item = Game.Player.Inventory.GetItemAt(slot);
            item?.Use();
        }
        /// <summary>
        /// Drops an item from a specific slot
        /// </summary>
        public static void DropItemBySlot(byte slot)
        {
            var item = Game.Player.Inventory.GetItemAt(slot);
            item?.Drop();
        }
        /// <summary>
        /// Toggeles the state of an item for the use at the Trainingplace
        /// </summary>
        public static bool ToggleItemAtTrainplaceByCodeName(string codeName)
        {
            var items = PlayerConfig.GetArray<string>("RSBot.Inventory.ItemsAtTrainplace").ToList();
            bool isEnabled;

            if (items.Contains(codeName))
            {
                items.Remove(codeName);
                isEnabled = false;
            }
            else
            {
                items.Add(codeName);
                isEnabled = true;
            }

            PlayerConfig.SetArray("RSBot.Inventory.ItemsAtTrainplace", items);
            return isEnabled;
        }
        /// <summary>
        /// Toggeles the state of an item for the automatic use
        /// </summary>
        public static bool ToggleAutoUseByCodeName(string codeName)
        {
            var items = PlayerConfig.GetArray<string>("RSBot.Inventory.AutoUseAccordingToPurpose").ToList();
            bool isNowEnabled;

            if (items.Contains(codeName))
            {
                items.Remove(codeName);
                isNowEnabled = false;
            }
            else
            {
                items.Add(codeName);
                isNowEnabled = true;
            }

            PlayerConfig.SetArray("RSBot.Inventory.AutoUseAccordingToPurpose", items);
            return isNowEnabled;
        }

        public static List<string> GetItemsAtTrainplace() => PlayerConfig.GetArray<string>("RSBot.Inventory.ItemsAtTrainplace").ToList();
        public static List<string> GetItemsByPurpose() => PlayerConfig.GetArray<string>("RSBot.Inventory.AutoUseAccordingToPurpose").ToList();
    }
}
