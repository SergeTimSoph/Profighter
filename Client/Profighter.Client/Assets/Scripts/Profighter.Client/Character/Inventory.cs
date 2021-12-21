using System.Collections.Generic;
using Profighter.Client.WorldObjects;

namespace Profighter.Client.Character
{
    public class Inventory
    {
        private List<IInteractable> objects = new List<IInteractable>();

        public void Add(IInteractable objectToAdd)
        {
            objects.Add(objectToAdd);
        }

        public void Remove(IInteractable objectToRemove)
        {
            objects.Remove(objectToRemove);
        }
    }
}