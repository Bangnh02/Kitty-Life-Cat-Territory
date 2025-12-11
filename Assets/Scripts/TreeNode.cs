using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class TreeNode<T> : IEnumerable<TreeNode<T>>, IEnumerable
{
	public T Data
	{
		get;
		private set;
	}

	public TreeNode<T> Parent
	{
		get;
		private set;
	}

	public ICollection<TreeNode<T>> Children
	{
		get;
		private set;
	}

	public bool IsRoot => Parent == null;

	public bool IsLeaf => Children.Count == 0;

	public int Level
	{
		get
		{
			if (IsRoot)
			{
				return 0;
			}
			return Parent.Level + 1;
		}
	}

	public ICollection<TreeNode<T>> ElementsIndex
	{
		get;
		set;
	}

	private TreeNode()
	{
		Data = default(T);
		Children = new LinkedList<TreeNode<T>>();
		ElementsIndex = new LinkedList<TreeNode<T>>();
		ElementsIndex.Add(this);
	}

	public TreeNode(T data)
	{
		Data = data;
		Children = new LinkedList<TreeNode<T>>();
		ElementsIndex = new LinkedList<TreeNode<T>>();
		ElementsIndex.Add(this);
	}

	public TreeNode<T> AddChild(T child)
	{
		TreeNode<T> treeNode = new TreeNode<T>(child)
		{
			Parent = this
		};
		Children.Add(treeNode);
		RegisterChildForSearch(treeNode);
		return treeNode;
	}

	public override string ToString()
	{
		if (Data == null)
		{
			return "[data null]";
		}
		return Data.ToString();
	}

	private void RegisterChildForSearch(TreeNode<T> node)
	{
		ElementsIndex.Add(node);
	}

	public TreeNode<T> FindTreeNode(Func<TreeNode<T>, bool> predicate)
	{
		return ElementsIndex.FirstOrDefault(predicate);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<TreeNode<T>> GetEnumerator()
	{
		yield return this;
		foreach (TreeNode<T> child in Children)
		{
			foreach (TreeNode<T> item in child)
			{
				yield return item;
			}
		}
	}
}
