using System;

namespace Yuppi.Environment
{
    [Serializable]
    public enum SpecialId : uint
    {
        Server = 100,
        Create = 0,
        Join,
        Success,
        Error,
        WithMe,
        WithoutMe
    }
}