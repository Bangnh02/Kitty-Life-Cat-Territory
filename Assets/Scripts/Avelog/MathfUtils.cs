namespace Avelog
{
	public static class MathfUtils
	{
		public static int GreatestCommonDivisor(int a, int b)
		{
			while (b != 0)
			{
				b = a % (a = b);
			}
			if (a >= 0)
			{
				return a;
			}
			return -a;
		}
	}
}
