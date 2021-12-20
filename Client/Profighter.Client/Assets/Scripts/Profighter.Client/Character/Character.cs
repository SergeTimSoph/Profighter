using Profighter.Client.Camera;
using Profighter.Client.PlayerInput;
using UnityEngine;

namespace Profighter.Client.Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private CharacterInputController inputController;

        private Inventory inventory;

        public Character(Inventory inventory)
        {
            this.inventory = inventory;
        }

        public void Setup(OrbitCamera orbitCamera)
        {
            inputController.Setup(orbitCamera);
        }
    }
}