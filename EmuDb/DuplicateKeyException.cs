using System;

namespace EmuDb
{
    public enum KeyStrategy
    {
        Unique,
        Update,
        Append
    }

    public class DuplicateKeyException: ArgumentException
    {
    }
}
