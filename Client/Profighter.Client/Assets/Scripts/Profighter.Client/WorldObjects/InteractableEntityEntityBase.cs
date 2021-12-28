using UnityEngine;

namespace Profighter.Client.WorldObjects
{
    public abstract class InteractableEntityEntityBase : IInteractableEntity
    {
        public string Name { get; }

        public Transform Transform { get; }

        public Collider Collider { get; }

        protected InteractableEntityEntityBase(string name, Transform transform, Collider collider)
        {
            Name = name;
            Transform = transform;
            Collider = collider;
        }
    }
}