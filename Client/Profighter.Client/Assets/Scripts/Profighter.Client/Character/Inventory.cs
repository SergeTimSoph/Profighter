using Profighter.Client.WorldObjects;

namespace Profighter.Client.Character
{
    public class Inventory
    {
        private IInteractableEntity interactableEntity;

        public IInteractableEntity GetItem()
        {
            return interactableEntity;
        }

        public void Add(IInteractableEntity objectToAdd)
        {
            interactableEntity = objectToAdd;
        }

        public void RemoveItem()
        {
            interactableEntity = null;
        }
    }
}