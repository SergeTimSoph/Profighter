using System;
using Profighter.Client.Data;
using Profighter.Client.SceneManagement;
using UnityEngine;

namespace Profighter.Client.Entities
{
    public class AreaItemStack
    {
        private readonly AreaItemStackState state;

        public IItemIdentity Item => state.ItemStackState.Item;

        public Vector3 Position => state.ItemStackPosition;

        public Guid Guid => state.Guid;

        public int Count
        {
            get => state.ItemStackState.Count;
            private set => state.ItemStackState.Count = value;
        }

        public bool IsEmpty => Count == 0;

        public AreaItemStack(AreaItemStackState state)
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