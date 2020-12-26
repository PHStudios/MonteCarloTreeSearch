using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarloTreeSearch.Extensions
{
    public static class ListExtensions
    {
        public static T GetRandomValue<T>(this IList<T> source)
        {
            var random = new Random();
            return source[random.Next(source.Count)];
        }
    }
}
