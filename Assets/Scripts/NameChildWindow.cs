using Avelog;
using UnityEngine;
using UnityEngine.UI;

public class NameChildWindow : WindowSingleton<NameChildWindow>
{
	[SerializeField]
	private InputField inputField;

	protected override void OnInitialize()
	{
	}

	private void OnEnable()
	{
		inputField.text = "";
	}

	public void CompleteNameChild()
	{
		if (!string.IsNullOrEmpty(inputField.text))
		{
			Avelog.Input.FireMakeChildPressed(inputField.text);
			WindowSingleton<GameWindow>.Instance.Open();
		}
	}
}
