using UnityEngine;

public class ProcessingSwitchUpdater : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Time.deltaTime == 0f || ProcessingSwitch.Instances.Count == 0)
		{
			return;
		}
		int num = (int)(30f * ProcessingSwitch.Instances[0].UpdateFrequency);
		if (num == 0)
		{
			num = 1;
		}
		int num2 = Time.frameCount % num;
		for (int i = 0; i < ProcessingSwitch.Instances.Count; i++)
		{
			int num3 = i % num;
			if (ProcessingSwitch.Instances[i].enabled && ProcessingSwitch.Instances[i].gameObject.activeInHierarchy && num2 == num3)
			{
				ProcessingSwitch.Instances[i].UpdateByTime();
			}
		}
	}
}
