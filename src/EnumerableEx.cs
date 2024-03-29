﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SystemEx
{
	public static class EnumerableEx
	{
		public static IEnumerable<int> Infinite() => Infinite(CancellationToken.None);
		public static IEnumerable<int> Infinite(CancellationToken cancel)
		{
			int i = 0;
			while (!cancel.IsCancellationRequested) yield return i++;
		}
		public static IEnumerable<T> Infinite<T>(T v) => Infinite(v, CancellationToken.None);
		public static IEnumerable<T> Infinite<T>(T v, CancellationToken cancel)
		{
			while (!cancel.IsCancellationRequested) yield return v;
		}


		/*
		public static IEnumerable<int> Count(int num) => Count(CancellationToken.None);
		public static IEnumerable<int> Count(int num, CancellationToken cancel)
		{
			int i = 0;
			while (i < num && !cancel.IsCancellationRequested) yield return i++;
		}
		*/

		public static IEnumerable<(T v, int index)> Indexed<T>(this IEnumerable<T> e)
			=> e.Select((v, i) => (v, i));

		public static bool Any<T>(this IEnumerable<T> e, out T v)
		{
			if (e.Any())
			{
				v = e.First();
				return true;
			}

			v = default;
			return false;
		}

		//public static IEnumerable<TResult> SelectValid<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> fn)
		//{
		//	using (var aes = new AggregateExceptionScope())
		//	{
		//		return source.SelectValid(fn, aes);
		//	}
		//}

		public static IEnumerable<TResult> SelectValid<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> fn, AggregateExceptionScope aes = null)
		{
			return source.Select(v => {
					try { return new { result = fn(v), error = false }; }
					catch (Exception e) { aes?.Aggregate(e); }
					return new { result = default(TResult), error = true };
				})
				.Where(r => !r.error)
				.Select(r => r.result);
		}


		public static IEnumerable<TSource> Call<TSource>(this IEnumerable<TSource> source, Action<TSource> fn)
			=> source.Where(a => { fn(a); return true; });


		public static void ExecuteFast<TSource>(this IEnumerable<TSource> source, Action<TSource> fn)
		{
			foreach (var i in source)
				fn(i);
		}

		public static IEnumerable<TSource> Execute<TSource>(this IEnumerable<TSource> source, Action<TSource> fn)
		{
			using var aes = new AggregateExceptionScope();

			return source.Execute(fn, aes);
		}

		public static IEnumerable<TSource> Execute<TSource>(this IEnumerable<TSource> source, Action<TSource> fn, AggregateExceptionScope aes)
		{
			aes.Aggregate(
				source.Select(v =>
				{
					try { fn(v); }
					catch (Exception e) { return e; }
					return null;
				})
				.Where(e => e != null));

			return source;
		}

		public static IEnumerable<TSource> TakeWhileAndLast<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			int index = -1;
			foreach (var v in source.TakeWhile((c, i) => {
				if (!predicate(c)) index = i + 1;
				return index != i;
			}))
			{
				yield return v;
			}
		}

		public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, Func<TAccumulate, TSource, TAccumulate> func)
			=> source.Aggregate(default(TAccumulate), func);

		[Obsolete("Use Linq .Cast instead.")]
		public static IEnumerable<T> convert<T>(this IEnumerable e)
		{
			foreach (object o in e)
				yield return (T)o;

			yield break;
		}

		[Obsolete("Use Linq .Select instead.")]
		public static IEnumerable<U> transform<T, U>(this IEnumerable<T> e, Func<T, U> trf)
		{
			foreach (var i in e)
				yield return trf(i);

			yield break;
		}

		public static IEnumerable<T> Repeat<T>(this T v, int count)
			=> Enumerable.Repeat(v, count);

		public static IEnumerable<T> repeat<T>(this T v, int count)
		{
			for (int i = 0; i < count; i++)
				yield return v;

			yield break;
		}

		public static T max<T, V>(this IEnumerable<T> e, Func<T, V> transformFn)
		{
			var mo = MathOperations.Get<V>();

			V maxv = mo.min;
			T r = default(T);

			foreach (var i in e)
			{
				V v = transformFn(i);
				if (mo.gt(v, maxv))
				{
					maxv = v;
					r = i;
				}
			}

			return r;
		}

		public static T min<T, V>(this IEnumerable<T> e, Func<T, V> transformFn)
		{
			var mo = MathOperations.Get<V>();

			V minv = mo.max;
			T r = default(T);

			foreach (var i in e)
			{
				V cv = transformFn(i);
				if (mo.lt(cv, minv))
				{
					minv = cv;
					r = i;
				}
			}

			return r;
		}

		public static IEnumerable<T[]> Tuples<T>(this IEnumerable<T> e, int count)
		{
			T[] array = e.ToArray<T>();
			T[] result = new T[count];

			foreach (T[] r in array.Tuples(count, result, 0, 0))
			{
				yield return r;
			}

			yield break;
		}

		private static IEnumerable<T[]> Tuples<T>(this T[] array, int count, T[] result, int startIndex, int resultIndex)
		{
			for (int i = startIndex; i < array.Length - count + 1; i++)
			{
				result[resultIndex] = array[i];
				if (count > 1)
				{
					foreach (T[] r in array.Tuples(count - 1, result, i + 1, resultIndex + 1))
					{
						yield return r;
					}
				}
				else
				{
					yield return result;
				}
			}

			yield break;
		}
	}
}

#if NET35
namespace System.Linq
{
    public static class EnumerableEx
    {
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));
            return EnumerableEx.ZipIterator<TFirst, TSecond, TResult>(first, second, resultSelector);
        }

        private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            IEnumerator<TFirst> e1 = first.GetEnumerator();
            try
            {
                IEnumerator<TSecond> e2 = second.GetEnumerator();
                try
                {
                    while (e1.MoveNext() && e2.MoveNext())
                        yield return resultSelector(e1.Current, e2.Current);
                }
                finally
                {
                    if (e2 != null)
                        e2.Dispose();
                }
                e2 = (IEnumerator<TSecond>)null;
            }
            finally
            {
                if (e1 != null)
                    e1.Dispose();
            }
            e1 = (IEnumerator<TFirst>)null;
        }
    }
}
#endif
