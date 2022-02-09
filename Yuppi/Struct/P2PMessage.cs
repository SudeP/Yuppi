using System;

namespace Yuppi.Struct
{
    [Serializable]
    public struct P2PMessage
    {
        internal P2PMessage(uint __source, uint __destination, object __data)
        {
            source = __source;
            destination = __destination;
            data = __data;
        }

        public uint source;

        public uint destination;

        public object data;
    }
}