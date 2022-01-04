using System.Collections.Generic;

namespace Profighter.Client.Data
{
    public class InventoryState
    {
        public ItemStackState CurrentItem { get; set; }

        public List<ItemStackState> Items { get; } = new();
    }
}