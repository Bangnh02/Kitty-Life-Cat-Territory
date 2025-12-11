using UnityEngine;

public class FarmResidentHabitats : MonoBehaviour
{
	private static FarmResidentHabitats _instance;

	[SerializeField]
	private FarmResidentHabitatArea farmerStartHabitat;

	[SerializeField]
	private FarmResidentHabitatArea goatStartHabitat;

	[SerializeField]
	private FarmResidentHabitatArea pigStartHabitat;

	public static FarmResidentHabitats Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<FarmResidentHabitats>();
			}
			return _instance;
		}
	}

	public FarmResidentHabitatArea FarmerStartHabitat => farmerStartHabitat;

	public FarmResidentHabitatArea GoatStartHabitat => goatStartHabitat;

	public FarmResidentHabitatArea PigStartHabitat => pigStartHabitat;
}
