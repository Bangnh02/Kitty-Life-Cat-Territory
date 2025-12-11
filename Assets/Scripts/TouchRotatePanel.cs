using Avelog;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchRotatePanel : Graphic
{
	private float sensScreenCorrection;

	public override void SetMaterialDirty()
	{
	}

	public override void SetVerticesDirty()
	{
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}

	private new void Start()
	{
		float num = (Screen.dpi + 96f) / 2f;
		sensScreenCorrection = 96f / num;
	}

	public void OnDrag(BaseEventData eventData)
	{
		PointerEventData pointerEventData = eventData as PointerEventData;
		pointerEventData.delta = new Vector2(pointerEventData.delta.x * ManagerBase<SettingsManager>.Instance.sensitivity, pointerEventData.delta.y) * sensScreenCorrection;
		int num = (!ManagerBase<SettingsManager>.Instance.invertRotation) ? 1 : (-1);
		pointerEventData.delta = new Vector2((float)num * pointerEventData.delta.x, (float)(-num) * pointerEventData.delta.y);
		Avelog.Input.FireTouchRotate(pointerEventData.delta);
	}
}
