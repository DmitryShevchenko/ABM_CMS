using System;

namespace ABM_CMS.Extensions
{
    public static class Extensions
    {
        public static R Using<T, R>(this T item, Func<T, R> func) where T : IDisposable
        {
            using (item)
            {
                return func(item);
            }
        }
    }
}