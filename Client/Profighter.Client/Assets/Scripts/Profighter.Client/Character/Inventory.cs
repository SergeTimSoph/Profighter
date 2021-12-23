using Profighter.Client.WorldObjects;

namespace Profighter.Client.Character
{
    public class Inventory
    {
        private IInteractable interactable;

        public IInteractable GetItem()
        {
            return interactable;
        }

        public void Add(IInteractable objectToAdd)
        {
            interactable = objectToAdd;
        }

        public void RemoveItem()
        {
            interactable = null;
        }
    }
}