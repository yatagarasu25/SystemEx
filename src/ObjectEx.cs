﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SystemEx {
	public static class ObjectEx
	{
#if UNITY || UNITY_64
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNull<T>(this T self)
		{
			return self == null || self.Equals(null);
		}
#else
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNull<T>(this T self)
		{
			return self == null;
		}
#endif

		// Kotlin: fun <T, R> T.let(block: (T) -> R): R
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static R Let<T, R>(this T self, Func<T, R> block)
		{
			return block(self);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Let<T>(this T self, Action<T> block)
		{
			block(self);
		}

		// Kotlin: fun <T> T.also(block: (T) -> Unit): T
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Also<T>(this T self, Action<T> block)
		{
			block(self);
			return self;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T IfValid<T>(this T self, Action<T> block)
		{
			if (!self.IsNull())
				block(self);
			return self;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T UnlessValid<T>(this T self, Action<T> block)
		{
			if (self.IsNull())
				block(self);
			return self;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Or<T>(this T self, Func<T> block)
		{
			return (self.IsNull()) ? block() : self;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static U Or<T, U>(this T self, Func<U> block, Func<U> def)
		{
			return (self.IsNull()) ? block() : def();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Elvis<T>(this T self, Action<T> block)
		{
			if (!self.IsNull()) block(self);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static R Elvis<T, R>(this T self, Func<T, R> block, R defualtValue = default)
		{
			return (!self.IsNull()) ? block(self) : defualtValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static R Elvis<T, R>(this T self, Func<T, R> block, Func<R> defualtValue)
		{
			return (!self.IsNull()) ? block(self) : defualtValue();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Elvis<T>(this T self, Func<T, string> block, string defualtValue = "")
		{
			return (!self.IsNull()) ? block(self) : defualtValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetFieldValue<T>(this object o, string name)
		{
			return (T)o.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(o);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetPropertyValue<T>(this object o, string name)
		{
			return (T)o.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(o, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetHashCode(params int[] hashcodes)
		{
			unchecked // Overflow is fine, just wrap
			{
				int hash = 17;
				foreach (var hashcode in hashcodes)
					hash = hash * 23 + hashcode;
				return hash;
			}
		}
	}
}
