using UnityEngine;

namespace Avelog
{
	public static class Layers
	{
		private static int colliderLayer;

		private static int triggerLayer;

		public static int ColliderLayer
		{
			get
			{
				if (colliderLayer == 0)
				{
					colliderLayer = LayerMask.NameToLayer("Collider");
				}
				return colliderLayer;
			}
		}

		public static int TriggerLayer
		{
			get
			{
				if (triggerLayer == 0)
				{
					triggerLayer = LayerMask.NameToLayer("Trigger");
				}
				return triggerLayer;
			}
		}
	}
}
