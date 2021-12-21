using UnityEngine;

namespace Profighter.Client.WorldObjects
{
    public class InteractableObject : InteractableObjectBase
    {
        public InteractableObject(string name, Transform transform, Collider collider) : base(name, transform, collider)
        {
        }
    }
}