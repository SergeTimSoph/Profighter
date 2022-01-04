namespace Profighter.Client.SceneManagement
{
    public abstract class ItemIdentity : IItemIdentity
    {
        public virtual bool IsIdentical(IItemIdentity other) => other?.GetType() == GetType();
    }
}