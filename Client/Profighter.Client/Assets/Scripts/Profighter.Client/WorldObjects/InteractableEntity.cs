using UnityEngine;

namespace Profighter.Client.WorldObjects
{
    public class InteractableEntity : InteractableEntityEntityBase
    {
        public InteractableEntity(string name, Transform transform, Collider collider)
            : base(name, transform, collider)
        {
        }
    }
}