using UnityEngine;

namespace Profighter.Client.WorldObjects
{
    public abstract class InteractableObjectBase : IInteractable
    {
        public string Name { get; }

        public Transform Transform { get; }

        public Collider Collider { get; }

        protected InteractableObjectBase(string name, Transform transform, Collider collider)
        {
            Name = name;
            Transform = transform;
            Collider = collider;
        }
    }
}