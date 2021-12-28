using UnityEngine;

namespace Profighter.Client.WorldObjects
{
    public interface IInteractableEntity
    {
        string Name { get; }

        Transform Transform { get; }

        Collider Collider { get; }
    }
}