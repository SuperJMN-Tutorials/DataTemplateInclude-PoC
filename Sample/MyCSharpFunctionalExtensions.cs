using System;
using CSharpFunctionalExtensions;

namespace Sample;

public static class MyCSharpFunctionalExtensions
{
    public static Maybe<T> Tap<T>(this Maybe<T> result, Action<T> action)
    {
        if (result.HasValue)
        {
            action(result.Value);
        }

        return result;
    }
}