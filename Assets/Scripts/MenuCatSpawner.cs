using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuCatSpawner : Singleton<MenuCatSpawner>
{
	public delegate void MenuCatSpawnHandler(GameObject menuCatObject);

	[SerializeField]
	private GameObject menuCatPrefab;

	public List<MenuCat> menuCats = new List<MenuCat>();

	private List<MenuCatSpawnPointsGroup> spawnPointsGroups;

	private MenuCatSpawnPointsGroup curSpawnPointsGroup;

	public bool IsCatsSpawned
	{
		get;
		private set;
	}

	public static event MenuCatSpawnHandler menuCatSpawnEvent;

	protected override void OnInit()
	{
		spawnPointsGroups = new List<MenuCatSpawnPointsGroup>(Object.FindObjectsOfType<MenuCatSpawnPointsGroup>());
		SelectSpawnPointsGroup();
		SpawnCat();
		if (ManagerBase<FamilyManager>.Instance.HaveFamily)
		{
			foreach (FamilyManager.FamilyMemberData item in ManagerBase<FamilyManager>.Instance.family)
			{
				SpawnFamilyMember(item);
			}
		}
		IsCatsSpawned = true;
		PlayerFamilyController.addFamilyMemberEvent += OnAddFamilyMember;
		SaveManager.LoadEndEvent += OnLoadEnd;
	}

	private void OnDestroy()
	{
		PlayerFamilyController.addFamilyMemberEvent -= OnAddFamilyMember;
		SaveManager.LoadEndEvent -= OnLoadEnd;
	}

	private void SelectSpawnPointsGroup()
	{
		if (ManagerBase<FamilyManager>.Instance.HaveFamily)
		{
			curSpawnPointsGroup = spawnPointsGroups.Find((MenuCatSpawnPointsGroup x) => x.PointsCount == ManagerBase<FamilyManager>.Instance.family.Count + 1);
		}
		else
		{
			curSpawnPointsGroup = spawnPointsGroups.Find((MenuCatSpawnPointsGroup x) => x.PointsCount == 1);
		}
	}

	private void ReplaceCats()
	{
		foreach (MenuCat menuCat in menuCats)
		{
			MenuCatSpawnPoint menuCatSpawnPoint = curSpawnPointsGroup.SpawnPoints.First((MenuCatSpawnPoint x) => !x.isBusy);
			menuCatSpawnPoint.isBusy = true;
			menuCat.gameObject.transform.position = menuCatSpawnPoint.transform.position;
			menuCat.gameObject.transform.rotation = menuCatSpawnPoint.transform.rotation;
			menuCat.animationNumber = ((menuCatSpawnPoint.CurAnimationType == MenuCatSpawnPoint.AnimationType.Sitting) ? 0f : 1f);
		}
	}

	private void SpawnCat()
	{
		MenuCatSpawnPoint menuCatSpawnPoint = curSpawnPointsGroup.SpawnPoints.First((MenuCatSpawnPoint x) => !x.isBusy);
		menuCatSpawnPoint.isBusy = true;
		GameObject gameObject = Object.Instantiate(menuCatPrefab, menuCatSpawnPoint.transform.position, menuCatSpawnPoint.transform.rotation, base.transform);
		MenuCat component = gameObject.GetComponent<MenuCat>();
		component.animationNumber = ((menuCatSpawnPoint.CurAnimationType == MenuCatSpawnPoint.AnimationType.Sitting) ? 0f : 1f);
		component.Spawn(isOldCat: true, isPlayerCat: true, isSpouseCat: false, ManagerBase<PlayerManager>.Instance.gender);
		menuCats.Add(component);
		MenuCatSpawner.menuCatSpawnEvent?.Invoke(gameObject);
	}

	private void SpawnFamilyMember(FamilyManager.FamilyMemberData familyMemberData)
	{
		bool flag = familyMemberData.role == FamilyManager.FamilyMemberRole.Spouse || familyMemberData.role == FamilyManager.FamilyMemberRole.ThirdStageChild;
		MenuCatSpawnPoint menuCatSpawnPoint = curSpawnPointsGroup.SpawnPoints.First((MenuCatSpawnPoint x) => !x.isBusy);
		menuCatSpawnPoint.isBusy = true;
		GameObject gameObject = Object.Instantiate(menuCatPrefab, menuCatSpawnPoint.transform.position, menuCatSpawnPoint.transform.rotation, base.transform);
		MenuCat component = gameObject.GetComponent<MenuCat>();
		component.animationNumber = ((menuCatSpawnPoint.CurAnimationType == MenuCatSpawnPoint.AnimationType.Sitting) ? 0f : 1f);
		component.Spawn(flag, isPlayerCat: false, familyMemberData.role == FamilyManager.FamilyMemberRole.Spouse, familyMemberData.genderType, familyMemberData);
		if (!flag)
		{
			component.KittenModel.transform.localScale = component.StartModelScale * familyMemberData.CurScalePart;
		}
		else if (familyMemberData.role == FamilyManager.FamilyMemberRole.ThirdStageChild)
		{
			component.CatModel.transform.localScale = component.CatModel.transform.localScale * familyMemberData.CurScalePart;
		}
		menuCats.Add(component);
		MenuCatSpawner.menuCatSpawnEvent?.Invoke(gameObject);
	}

	private void OnAddFamilyMember(FamilyManager.FamilyMemberData familyMember)
	{
		SelectSpawnPointsGroup();
		ReplaceCats();
		SpawnFamilyMember(familyMember);
	}

	private void OnLoadEnd()
	{
		menuCats.ForEach(delegate(MenuCat x)
		{
			x.SetPreviewSkin(ManagerBase<SkinManager>.Instance.CurrentSkinIndex);
		});
		int num = 1 + ManagerBase<FamilyManager>.Instance.family.Count;
		if (menuCats.Count != num)
		{
			SelectSpawnPointsGroup();
			ReplaceCats();
			foreach (FamilyManager.FamilyMemberData familyMemberData in ManagerBase<FamilyManager>.Instance.family)
			{
				if (!menuCats.Any((MenuCat x) => x.FamilyMemberData == familyMemberData))
				{
					SpawnFamilyMember(familyMemberData);
				}
			}
		}
	}
}
