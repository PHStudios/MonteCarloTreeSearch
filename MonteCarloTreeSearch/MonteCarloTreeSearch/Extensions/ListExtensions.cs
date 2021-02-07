using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarloTreeSearch.Extensions
{
    public static class ListExtensions
    {
        public static T GetRandomValue<T>(this IList<T> source)
        {
            var random = new Random(); // This is not thread safe and will be addressed in a later tutorial - EDUCATION PURPOSE
            return source[random.Next(source.Count)];
        }
    }
}
