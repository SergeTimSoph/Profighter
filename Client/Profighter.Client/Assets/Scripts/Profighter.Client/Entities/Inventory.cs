using System;
using System.Collections.Generic;
using System.Linq;
using Profighter.Client.Data;
using Profighter.Client.SceneManagement;

namespace Profighter.Client.Entities
{
    public class Inventory
    {
        private readonly InventoryState state;

        private CharacterHoldObject currentHoldObject;

        public IReadOnlyList<ItemStack> Items =>
            state.Items.Select(_ => new ItemStack(_)).ToList();

        public Inventory(InventoryState state)
        {
            this.state = state;
        }

        public void SetCurrentItem(CharacterHoldObject item)
        {
            state.CurrentItem = new ItemStackState()
            {
                Item = item.ItemIdentity,
                Count = 1
            };
            currentHoldObject = item;
            PutItem(item.ItemIdentity);
        }

        public CharacterHoldObject GetCurrentItem()
        {
            return currentHoldObject;
        }

        public void RemoveCurrentItem()
        {
            if (state.CurrentItem != null)
            {
                state.CurrentItem = null;
            }
        }

        public void PutItem(IItemIdentity itemIdentity)
        {
            PutItems( itemIdentity, 1);
        }

        public void PutItems(IItemIdentity itemIdentity, int count)
        {
            var itemStack = GetItemStack(itemIdentity) ?? AddItemStack(itemIdentity);

            for (int i = 0; i < count; i++)
            {
                itemStack.Push(itemIdentity);
            }
        }

        public IItemIdentity TakeItem(IItemIdentity itemIdentity)
        {
            var itemStack = GetItemStack(itemIdentity);
            if (itemStack != null && !itemStack.IsEmpty)
            {
                var item = itemStack.Pop();

                return item;
            }

            return null;
        }

        public IReadOnlyList<IItemIdentity> TakeItems(IItemIdentity itemIdentity, int count)
        {
            if (!HasItems(itemIdentity, count))
            {
                throw new InvalidOperationException($"Can not take {count} items with identity of tupe {itemIdentity.GetType().Name}");
            }

            var items = new List<IItemIdentity>();
            var itemStack = GetItemStack(itemIdentity);
            for (int i = 0; i < count; i++)
            {
                items.Add(itemStack.Pop());
            }

            return items;
        }

        private bool HasItems(IItemIdentity itemIdentity, int count)
        {
            return CountItems(itemIdentity) >= count;
        }

        private int CountItems(IItemIdentity itemIdentity)
        {
            var itemStack = GetItemStack(itemIdentity);
            return itemStack?.Count ?? 0;
        }

        private ItemStack AddItemStack(IItemIdentity itemIdentity)
        {
            var newStackState = new ItemStackState()
            {
                Item = itemIdentity,
                Count = 0
            };

            state.Items.Add(newStackState);

            return new ItemStack(newStackState);
        }

        private ItemStack GetItemStack(IItemIdentity itemIdentity)
            => Items.FirstOrDefault(x => itemIdentity.IsIdentical(x.Item));
    }
}