using Profighter.Client.Entities;
using UnityEngine;

namespace Profighter.Client.SceneManagement
{
    public class CharacterHoldObject
    {
        public IItemIdentity ItemIdentity { get; set; }

        public GameObject GameObject { get; set; }

        public Collider Collider { get; set; }
    }
}