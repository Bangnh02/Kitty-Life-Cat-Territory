using System.Collections.Generic;
using UnityEngine;

namespace Avelog.Tools
{
	public class DebugSnapshotData : ScriptableObject
	{
		public string openedFilePath = "";

		public List<int> selectedGOHierarchyPath = new List<int>();

		public string selectedGOName = "";

		public List<TreeNode<DebugSnapshot.HierarchyNode>> rootNodes;
	}
}
