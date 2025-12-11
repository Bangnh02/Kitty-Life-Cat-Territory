using UnityEngine;
using UnityEngine.AI;

public class Managers : MonoBehaviour
{
	private static Managers instance;

	[SerializeField]
	private int pbiLimitLevel = 100;

	public static Managers Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<Managers>();
			}
			return instance;
		}
	}

	public int PBILimitLevel => pbiLimitLevel;

	private void Start()
	{
		NavMesh.avoidancePredictionTime = 0.3f;
	}
}
