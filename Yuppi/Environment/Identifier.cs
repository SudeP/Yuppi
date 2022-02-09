using System;
using System.Collections.Generic;
using System.Linq;

namespace Yuppi.Environment
{
    public class Identifier
    {
        public const uint IdentitySeed = 100;
        public static List<SpecialId> specialIdList =  Enum.GetValues(typeof(SpecialId)).Cast<SpecialId>().ToList();
        private uint lastNewIdentity = IdentitySeed;
        public uint GetNewIdentity()
        {
            return ++lastNewIdentity;
        }
        public bool IsCorrect(uint identity)
        {
            return identity > ((int)SpecialId.Server) && uint.MaxValue > identity;
        }
    }
}
