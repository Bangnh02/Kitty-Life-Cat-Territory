using System.Collections.Generic;

namespace Avelog
{
	public class Pathfinding
	{
		private class Node
		{
			public readonly IPathNode pathNode;

			public Node nodeCameFrom;

			public float gScore;

			public float hScore;

			public float fScore;

			public Node(IPathNode pathNode, IPathNode goalPathNode, Node nodeCameFrom)
			{
				this.pathNode = pathNode;
				hScore = pathNode.GetHeuristicDistanceEstimate(goalPathNode);
				if (nodeCameFrom != null)
				{
					this.nodeCameFrom = nodeCameFrom;
					gScore = nodeCameFrom.gScore + pathNode.GetDistance(nodeCameFrom.pathNode);
				}
				else
				{
					gScore = 0f;
				}
				fScore = gScore + hScore;
			}
		}

		public interface IPathNode
		{
			IEnumerable<IPathNode> Neighbors
			{
				get;
			}

			float GetDistance(IPathNode otherPathNode);

			float GetHeuristicDistanceEstimate(IPathNode otherPathNode);
		}

		private static List<Node> allNodes = new List<Node>();

		private static LinkedList<Node> openNodes = new LinkedList<Node>();

		private static List<Node> closedNodes = new List<Node>();

		public static List<T> FindPath<T>(T start, T goal) where T : class, IPathNode
		{
			if (start == goal)
			{
				return new List<T>
				{
					start
				};
			}
			Node node = new Node(start, goal, null);
			openNodes.AddLast(node);
			allNodes.Add(node);
			while (openNodes.Count > 0)
			{
				Node node2 = openNodes.First.Value;
				foreach (Node openNode in openNodes)
				{
					if (openNode.fScore < node2.fScore)
					{
						node2 = openNode;
					}
				}
				openNodes.Remove(node2);
				closedNodes.Add(node2);
				foreach (IPathNode neighbor in node2.pathNode.Neighbors)
				{
					foreach (Node closedNode in closedNodes)
					{
						IPathNode pathNode = closedNode.pathNode;
						IPathNode pathNode2 = neighbor;
					}
					Node node3 = allNodes.Find((Node x) => x.pathNode == neighbor);
					if (node3 == null)
					{
						node3 = new Node(neighbor, goal, node2);
						allNodes.Add(node3);
						openNodes.AddFirst(node3);
						if (node3.pathNode == goal)
						{
							List<T> result = ReconstructPath<T>(node3);
							allNodes.Clear();
							openNodes.Clear();
							closedNodes.Clear();
							return result;
						}
					}
					else
					{
						float num = node2.gScore + node2.pathNode.GetDistance(neighbor);
						if (num < node3.gScore)
						{
							node3.gScore = num;
							node3.fScore = node3.gScore + node3.hScore;
							node3.nodeCameFrom = node2;
							openNodes.AddFirst(node3);
						}
					}
				}
			}
			allNodes.Clear();
			openNodes.Clear();
			closedNodes.Clear();
			return null;
		}

		private static List<T> ReconstructPath<T>(Node goalNode) where T : class, IPathNode
		{
			List<T> list = new List<T>();
			list.Add(goalNode.pathNode as T);
			Node node = goalNode;
			while (node.nodeCameFrom != null)
			{
				node = node.nodeCameFrom;
				list.Add(node.pathNode as T);
			}
			list.Reverse();
			return list;
		}
	}
}
