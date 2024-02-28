using System;
using Morph.Core;

namespace Morph.Base
{
    public class LinkEnd : Link
    {
        internal LinkEnd()
          : base(LinkTypeID.End)
        {
        }

        #region Link

        public override int Size()
        {
            return 1;
        }

        public override void Write(MorphWriter writer)
        {
            writer.WriteInt8((byte)LinkTypeID.End);
        }

        #endregion

        public override bool Equals(object obj)
        {
            return obj is LinkEnd;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "{End}";
        }
    }

    public class LinkTypeEnd : ILinkTypeReader, ILinkTypeAction
    {
        private static readonly LinkEnd s_linkEnd = new LinkEnd();
        static public LinkEnd End
        {
            get => s_linkEnd;
        }

        public LinkTypeID ID
        {
            get => LinkTypeID.End;
        }

        public Link ReadLink(MorphReader reader)
        {
            reader.ReadInt8();
            return s_linkEnd;
        }

        public void ActionLink(LinkMessage message, Link currentLink)
        {
            if (message.ContextIs(typeof(IDisposable)))
                ((IDisposable)message.Context).Dispose();
        }
    }
}