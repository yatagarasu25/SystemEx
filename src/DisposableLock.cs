﻿using System;

namespace SystemEx
{
	public static class DisposableLock
	{
		public static DisposableLock<T> Lock<T>(T v, Action<T> fn)
		{
			return new DisposableLock<T>(v, fn);
		}

		public static DisposableLock<T> Lock<T>(Func<T> v, Action<T> fn)
		{
			return new DisposableLock<T>(v(), fn);
		}
	}

	public class DisposableLock<T> : IDisposable
	{
		T v;
		Action<T> disposeFn;

		public DisposableLock(T v_, Action<T> disposeFn_)
		{
			v = v_;
			disposeFn = disposeFn_;
		}

		public void Dispose()
		{
			disposeFn(v);
		}

		public T Value => v;
		public static implicit operator T(DisposableLock<T> dl) => dl.v;
	}
}
