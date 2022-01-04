using System;
using System.Collections.Generic;
using System.Linq;
using Profighter.Client.Configuration;
using Profighter.Client.Data;
using Profighter.Client.SceneManagement;
using UnityEngine;

namespace Profighter.Client.Entities
{
    public class Area
    {
        private readonly AreaState areaState;

        public string Id => areaState.Id;

        public IReadOnlyList<AreaItemStack> Items =>
            areaState.AreaItems.Select(_ => new AreaItemStack(_)).ToList();

        public AreaConfig AreaConfig { get; }

        public Area(AreaState areaState, AreaConfig areaConfig)
        {
            this.areaState = areaState;
            this.AreaConfig = areaConfig;
        }

        public void PutItem(IItemIdentity itemIdentity, Vector3 position)
        {
            PutItems(itemIdentity, 1, position);
        }

        public void PutItems(IItemIdentity itemIdentity, int count, Vector3 position)
        {
            var itemStack = GetItemStack(itemIdentity) ?? AddItemStack(itemIdentity, position);

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

        public IReadOnlyList<IItemIdentity> TakeAllItems(IItemIdentity itemIdentity)
        {
            var itemsCount = CountItems(itemIdentity);
            if (!HasItems(itemIdentity, itemsCount))
            {
                throw new InvalidOperationException($"Can not take all items with identity of tupe {itemIdentity.GetType().Name}");
            }

            var items = new List<IItemIdentity>();
            var itemStack = GetItemStack(itemIdentity);
            for (int i = 0; i < itemsCount; i++)
            {
                items.Add(itemStack.Pop());
            }

            var itemStackToRemove = areaState.AreaItems.FirstOrDefault(x => itemIdentity.IsIdentical(x.ItemStackState.Item));
            areaState.AreaItems.Remove(itemStackToRemove);

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

        private AreaItemStack AddItemStack(IItemIdentity itemIdentity, Vector3 position)
        {
            var newStackState = new AreaItemStackState()
            {
                ItemStackState = new ItemStackState()
                {
                    Item = itemIdentity,
                    Count = 0
                },
                ItemStackPosition = position,
                Guid = Guid.NewGuid()
            };

            areaState.AreaItems.Add(newStackState);

            return new AreaItemStack(newStackState);
        }

        private AreaItemStack GetItemStack(IItemIdentity itemIdentity)
            => Items.FirstOrDefault(x => itemIdentity.IsIdentical(x.Item));
    }
}