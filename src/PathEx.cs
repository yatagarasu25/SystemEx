using System;
using System.IO;

namespace SystemEx
{
	public static class PathEx
	{
		public static string AsPath(this ValueTuple<string, string> paths)
			=> Path.Combine(paths.Item1, paths.Item2);

		public static string AsLinuxPath(this string path)
			=> path.Replace(Path.DirectorySeparatorChar, '/');
	}
}
