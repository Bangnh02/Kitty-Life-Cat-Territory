namespace Avelog
{
	public class ConvertUtils
	{
		public static void ToInts(long value, out int leftPart, out int rightPart)
		{
			leftPart = (int)(value >> 32);
			rightPart = (int)(value & uint.MaxValue);
		}

		public static long ToLong(int leftPart, int rightPart)
		{
			return ((long)leftPart << 32) | (uint)rightPart;
		}
	}
}
