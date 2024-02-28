using Morph.Base;

namespace Morph.Core
{
    public abstract class Link
    {
        protected Link(LinkTypeID linkTypeID)
        {
            _linkTypeID = linkTypeID;
        }

        private readonly LinkTypeID _linkTypeID;
        public LinkTypeID LinkTypeID
        { get => _linkTypeID; }

        public abstract int Size();
        public abstract void Write(MorphWriter writer);
    }

    public interface IActionLink
    {
        void ActionLink(LinkMessage message, Link currentLink);
    }

    public interface IActionLast
    {
        void ActionLast(LinkMessage message);
    }
}