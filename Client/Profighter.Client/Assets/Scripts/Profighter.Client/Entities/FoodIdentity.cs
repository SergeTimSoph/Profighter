using System;

namespace Profighter.Client.SceneManagement
{
    [Serializable]
    public sealed class FoodIdentity : ItemIdentity
    {
        public string FoodId { get; private set; }

        public FoodIdentity(string foodId)
        {
            FoodId = foodId;
        }

        public FoodIdentity()
        {
        }
    }
}