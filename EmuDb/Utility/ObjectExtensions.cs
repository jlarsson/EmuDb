using System;

namespace EmuDb.Utility
{
    public static class ObjectExtensions
    {
        public static T With<T>(this T @object, Action<T> with)
        {
            with(@object);
            return @object;
        }
    }
}
