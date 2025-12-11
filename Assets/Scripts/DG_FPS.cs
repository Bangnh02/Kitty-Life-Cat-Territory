using System;
using UnityEngine;
using UnityEngine.UI;

public class DG_FPS : MonoBehaviour
{
	private float deltaTime;

	private float cooldown;

	private float msec;

	public float fps;

	private Text fps_text;

	public bool debug = true;

	private void Start()
	{
		if (debug)
		{
			fps_text = GetComponentInChildren<Text>();
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		cooldown -= Time.deltaTime;
		if (cooldown <= 0f)
		{
			fps = 1f / deltaTime;
			if (debug)
			{
				fps_text.text = Math.Round(fps, 2).ToString();
			}
			cooldown = 1f;
		}
	}
}
