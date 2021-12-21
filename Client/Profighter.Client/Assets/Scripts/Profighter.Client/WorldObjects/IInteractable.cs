using UnityEngine;

namespace Profighter.Client.WorldObjects
{
    public interface IInteractable
    {
        string Name { get; }

        Transform Transform { get; }

        Collider Collider { get; }
    }
}