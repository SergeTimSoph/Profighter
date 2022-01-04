using System;
using Profighter.Client.Data;
using Profighter.Client.SceneManagement;

namespace Profighter.Client.Entities
{
    public class ItemStack
    {
        private readonly ItemStackState state;

        public IItemIdentity Item => state.Item;

        public int Count
        {
            get => state.Count;
            private set => state.Count = value;
        }

        public bool IsEmpty => Count == 0;

        public ItemStack(ItemStackState state)
        {
            this.state = state;
        }

        public void Push(IItemIdentity itemIdentity)
        {
            if (Item.IsIdentical(itemIdentity))
            {
                Count++;
            }
            else
            {
                throw new InvalidOperationException("Only identical item can be pushed.");
            }
        }

        public IItemIdentity Pop()
        {
            if (!IsEmpty)
            {
                Count--;
                return Item;
            }
            else
            {
                throw new InvalidOperationException("The stack is empty.");
            }
        }
    }
}