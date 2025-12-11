using System;
using UnityEngine;

public class NewBehaviourScript1 : MonoBehaviour
{
	[SerializeField]
	private int fps = 60;

	public GameObject m_Ship;

	public Vector3 m_RelativePos;

	private float targetPos;

	private float myPos;

	private void Start()
	{
		float targetSpeed = 1f;
		Calc(0.0166666657f, targetSpeed);
		Calc(0.03333333f, targetSpeed);
		Calc(0.01111111f, targetSpeed);
	}

	private void Calc(float deltaTime, float targetSpeed)
	{
		float num = 1f - (float)Math.Pow(0.95, deltaTime * 60f);
		targetPos = targetSpeed * deltaTime;
		float num2 = targetPos - myPos;
		UnityEngine.Debug.Log($"lerp {num}, Pos difference {num2}, move pos {num2 * num}");
		myPos += num2 * num;
	}

	private void LateUpdate()
	{
		Application.targetFrameRate = fps;
		float d = 1f - (float)Math.Pow(0.95, Time.deltaTime * 60f);
		Vector3 a = m_Ship.transform.position + m_RelativePos;
		base.transform.position += (a - base.transform.position) * d;
	}
}
