using Avelog;
using System.Diagnostics;
using UnityEngine;

public class NewBehaviourScript3 : MonoBehaviour
{
	private string json;

	private void Start()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		json = DebugUtils.ToJson(base.gameObject);
		stopwatch.Stop();
		UnityEngine.Debug.Log($"Time ms = {stopwatch.ElapsedMilliseconds}, s = {(float)stopwatch.ElapsedMilliseconds / 1000f}");
	}
}
