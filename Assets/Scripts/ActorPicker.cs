using Avelog;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorPicker : MonoBehaviour
{
	public delegate void ChangePickedItemHandler(bool havePickedItem);

	public delegate void PickEventHandler(Item pickedItem);

	public delegate void DropEventHandler(Item droppedItem, bool isSatisfyFarmResident);

	public delegate void UpdateNearPickableHandler(bool haveNearPickableItem);

	[Header("Отладка")]
	[SerializeField]
	[ReadonlyInspector]
	private Item pickedItem;

	[Header("Ссылки")]
	[SerializeField]
	private ItemUser itemUser;

	[SerializeField]
	private Transform lowestDropPos;

	private List<Item> nearItems = new List<Item>();

	public bool HavePickedItem => pickedItem != null;

	public Item PickedItem => pickedItem;

	private Transform PickSlot => PlayerSpawner.PlayerInstance.Model.PickSlot;

	private IEnumerable<Item> PickableItems => nearItems.FindAll((Item x) => x.CanBePicked(itemUser));

	public bool HavePickableItems => PickableItems.Count() > 0;

	public event ChangePickedItemHandler changePickedItemEvent;

	public event PickEventHandler pickEvent;

	public event DropEventHandler dropEvent;

	public event UpdateNearPickableHandler updatePickableItemsEvent;

	private void Start()
	{
		Item.updatePickableStateEvent += OnItemUpdatePickableState;
	}

	private void OnDestroy()
	{
		Item.updatePickableStateEvent -= OnItemUpdatePickableState;
	}

	private void OnItemUpdatePickableState(Item item)
	{
		if (nearItems.Contains(item))
		{
			if (!item.gameObject.activeInHierarchy)
			{
				nearItems.Remove(item);
			}
			this.updatePickableItemsEvent?.Invoke(HavePickableItems);
		}
	}

	public bool CanPick()
	{
		if (!HavePickedItem)
		{
			return HavePickableItems;
		}
		return false;
	}

	public void Pick()
	{
		if (CanPick())
		{
			List<(Item, float)> list = new List<(Item, float)>();
			foreach (Item nearItem in nearItems)
			{
				if (nearItem.Pickable)
				{
					float sqrMagnitude = (nearItem.transform.position - PickSlot.position).sqrMagnitude;
					list.Add((nearItem, sqrMagnitude));
				}
			}
			list.Sort((Comparison<(Item, float)>)(((Item item, float sqrDistance) item1, (Item item, float sqrDistance) item2) => item1.sqrDistance.CompareTo(item2.sqrDistance)));
			(pickedItem = list[0].Item1).Pick(PickSlot);
			this.changePickedItemEvent?.Invoke(HavePickedItem);
			this.pickEvent?.Invoke(pickedItem);
		}
	}

	public bool CanDrop()
	{
		return pickedItem != null;
	}

	public void Drop(bool isSatisfyFarmResident = false)
	{
		if (CanDrop())
		{
			(Vector3 position, Quaternion rotation) dropPosAndRot = GetDropPosAndRot();
			Vector3 item = dropPosAndRot.position;
			Quaternion item2 = dropPosAndRot.rotation;
			pickedItem.Drop(item, item2);
			Item droppedItem = pickedItem;
			pickedItem = null;
			this.changePickedItemEvent?.Invoke(HavePickedItem);
			this.dropEvent?.Invoke(droppedItem, isSatisfyFarmResident);
		}
	}

	public (Vector3 position, Quaternion rotation) GetDropPosAndRot()
	{
		Vector3 zero = Vector3.zero;
		Quaternion item = Quaternion.identity;
		if (Physics.Raycast(PickSlot.position, Vector3.down, out RaycastHit hitInfo, 100f, 1 << Layers.ColliderLayer))
		{
			if (hitInfo.point.y < lowestDropPos.position.y)
			{
				if (PickedItem != null)
				{
					item = Quaternion.FromToRotation(PickedItem.gameObject.transform.up, Vector3.up) * PickedItem.gameObject.transform.rotation;
				}
				zero = base.transform.position;
			}
			else
			{
				if (PickedItem != null)
				{
					item = Quaternion.FromToRotation(PickedItem.gameObject.transform.up, hitInfo.normal) * PickedItem.gameObject.transform.rotation;
				}
				zero = hitInfo.point;
			}
		}
		else
		{
			zero = lowestDropPos.position;
		}
		return (zero, item);
	}

	public void Respawn()
	{
		if (nearItems.Count > 0)
		{
			nearItems.Clear();
			this.updatePickableItemsEvent?.Invoke(HavePickableItems);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "PickableItem" || other.gameObject.tag == "Food")
		{
			Item component = other.gameObject.GetComponent<Item>();
			if (component != null && !nearItems.Contains(component))
			{
				nearItems.Add(component);
			}
			this.updatePickableItemsEvent?.Invoke(HavePickableItems);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == Layers.TriggerLayer && (other.gameObject.tag == "PickableItem" || other.gameObject.tag == "Food" || other.gameObject.tag == "AI"))
		{
			Item component = other.gameObject.GetComponent<Item>();
			if (component != null && nearItems.Contains(component))
			{
				nearItems.Remove(component);
			}
			this.updatePickableItemsEvent?.Invoke(HavePickableItems);
		}
	}
}
