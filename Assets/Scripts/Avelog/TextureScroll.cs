using UnityEngine;

namespace Avelog
{
	public class TextureScroll : MonoBehaviour
	{
		public Renderer renderer;

		public Material mtl;

		[SerializeField]
		private bool useFixedUpdate = true;

		public float speedX = 0.05f;

		public float speedY = 0.05f;

		private void OnEnable()
		{
			renderer = GetComponent<Renderer>();
			mtl = renderer.material;
		}

		private void Update()
		{
			if (!useFixedUpdate)
			{
				if (renderer.material != mtl)
				{
					mtl = renderer.material;
				}
				mtl.mainTextureOffset += new Vector2(speedX * Time.fixedDeltaTime, speedY * Time.deltaTime);
			}
		}

		private void FixedUpdate()
		{
			if (useFixedUpdate)
			{
				if (renderer.material != mtl)
				{
					mtl = renderer.material;
				}
				mtl.mainTextureOffset += new Vector2(speedX * Time.fixedDeltaTime, speedY * Time.fixedDeltaTime);
			}
		}
	}
}
