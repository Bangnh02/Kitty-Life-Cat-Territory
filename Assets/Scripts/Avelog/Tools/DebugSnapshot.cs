using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Avelog.Tools
{
	public class DebugSnapshot
	{
		[Serializable]
		public class HierarchyNode
		{
			public int siblingIndex;

			public string gameObjectName;

			public string jsonData;

			public int depth;

			public bool isExpanded;
		}

		public static List<TreeNode<HierarchyNode>> MakeSnapshot()
		{
			List<TreeNode<HierarchyNode>> list = new List<TreeNode<HierarchyNode>>();
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(i);
				if (sceneByBuildIndex.IsValid())
				{
					TreeNode<HierarchyNode> treeNode = new TreeNode<HierarchyNode>(new HierarchyNode
					{
						gameObjectName = sceneByBuildIndex.name,
						siblingIndex = 0,
						depth = 0
					});
					list.Add(treeNode);
					_003CMakeSnapshot_003Eg__BuildSceneTree_007C1_0(treeNode, sceneByBuildIndex);
				}
			}
			return list;
		}

		[CompilerGenerated]
		private static void _003CMakeSnapshot_003Eg__BuildSceneTree_007C1_0(TreeNode<HierarchyNode> sceneRootNode, Scene scene)
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				_003CMakeSnapshot_003Eg__BuildGameObjectTree_007C1_1(sceneRootNode.AddChild(new HierarchyNode
				{
					gameObjectName = gameObject.name,
					siblingIndex = gameObject.transform.GetSiblingIndex(),
					jsonData = DebugUtils.ToJson(gameObject),
					depth = 1
				}), gameObject);
			}
		}

		[CompilerGenerated]
		private static void _003CMakeSnapshot_003Eg__BuildGameObjectTree_007C1_1(TreeNode<HierarchyNode> node, GameObject gameObject)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
				_003CMakeSnapshot_003Eg__BuildGameObjectTree_007C1_1(node.AddChild(new HierarchyNode
				{
					gameObjectName = gameObject2.name,
					siblingIndex = i,
					depth = node.Data.depth + 1,
					jsonData = DebugUtils.ToJson(gameObject2)
				}), gameObject2);
			}
		}
	}
}
