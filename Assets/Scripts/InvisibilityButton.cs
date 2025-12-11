using Avelog;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InvisibilityButton : XORButton
{
	[SerializeField]
	private Sprite enableInvisibilitySprite;

	[SerializeField]
	private Sprite disableInvisibilitySprite;

	private Button button;

	private PlayerCombat PlayerCombat => PlayerSpawner.PlayerInstance.PlayerCombat;

	private void Start()
	{
		button = GetComponent<Button>();
		PlayerCombat.InvisibilityAbility.switchInvisibilityEvent += OnSwitchInvisibility;
	}

	private void OnDestroy()
	{
		PlayerCombat.InvisibilityAbility.switchInvisibilityEvent -= OnSwitchInvisibility;
	}

	private void OnSwitchInvisibility(bool state)
	{
		button.image.sprite = (state ? disableInvisibilitySprite : enableInvisibilitySprite);
	}

	public void SwitchInvisibilityButton()
	{
		Avelog.Input.FireInvisibilityPressed();
	}

	public override bool WantToEnable()
	{
		return true;
	}
}
