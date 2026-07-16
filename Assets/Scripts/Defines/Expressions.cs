using System;

namespace Defines
{
    namespace Expressions
    {
        [Flags]
        public enum ERESULT : uint
        {
            FALSE = 0,
            TRUE = 1 << 0,

            END = uint.MaxValue
        }
    }
}