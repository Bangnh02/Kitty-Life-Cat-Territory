using UnityEngine;

public class MenuPriorityRingController : MonoBehaviour, IInitializableUI
{
	[SerializeField]
	private MenuPriorityRing priorityRing;

	[SerializeField]
	private Transform playButtonParent;

	[SerializeField]
	private Transform skinsButtonParent;

	public void OnInitializeUI()
	{
		SaveManager.LoadEndEvent += OnLoadEnd;
	}

	private void OnDestroy()
	{
		SaveManager.LoadEndEvent -= OnLoadEnd;
	}

	private void OnEnable()
	{
		TryActivatePriorityRing();
	}

	private void OnLoadEnd()
	{
		TryActivatePriorityRing();
	}

	private void TryActivatePriorityRing()
	{
		if (!ManagerBase<GameConfigManager>.Instance.isFirstPlay && ManagerBase<GameConfigManager>.Instance.isVisitedSkinsWindow)
		{
			DisablePriorityRing();
		}
		if (ManagerBase<GameConfigManager>.Instance.isFirstPlay)
		{
			EnablePriorityRing(playButtonParent);
		}
		else if (!ManagerBase<GameConfigManager>.Instance.isVisitedSkinsWindow)
		{
			EnablePriorityRing(skinsButtonParent);
		}
	}

	private void EnablePriorityRing(Transform parent)
	{
		priorityRing.RectTransform.SetParent(parent);
		SetRingRectIndent(priorityRing.RectIndent);
		priorityRing.gameObject.SetActive(value: true);
	}

	private void SetRingRectIndent(float rectIndent)
	{
		priorityRing.RectTransform.offsetMin = new Vector2(0f - rectIndent, 0f - rectIndent);
		priorityRing.RectTransform.offsetMax = new Vector2(rectIndent, rectIndent);
	}

	private void DisablePriorityRing()
	{
		priorityRing.RectTransform.SetParent(base.transform);
		priorityRing.gameObject.SetActive(value: false);
	}
}
