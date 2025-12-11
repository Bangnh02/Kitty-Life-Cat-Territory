using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Avelog
{
	public class DebugUtils
	{
		public class NodeData
		{
			public object obj;

			public string name;

			public Type type;

			public int depth;
		}

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass2_0
		{
			public StringBuilder result;
		}

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass2_1
		{
			public TreeNode<NodeData> node;
		}

		public static TreeNode<NodeData> ToTree(object objectToConvert)
		{
			TreeNode<NodeData> treeNode = new TreeNode<NodeData>(new NodeData
			{
				obj = objectToConvert,
				name = objectToConvert.GetType().Name,
				type = objectToConvert.GetType(),
				depth = 0
			});
			_003CToTree_003Eg__AddChildNodes_007C1_0(treeNode, objectToConvert);
			return treeNode;
		}

		public static string ToJson(object objectToConvert)
		{
			_003C_003Ec__DisplayClass2_0 _003C_003Ec__DisplayClass2_ = default(_003C_003Ec__DisplayClass2_0);
			_003C_003Ec__DisplayClass2_.result = new StringBuilder();
			foreach (TreeNode<NodeData> item in ToTree(objectToConvert))
			{
				_003C_003Ec__DisplayClass2_1 _003C_003Ec__DisplayClass2_2 = default(_003C_003Ec__DisplayClass2_1);
				_003C_003Ec__DisplayClass2_2.node = item;
				TreeNode<NodeData> nextNodeWithSameDepth = GetNextNodeWithSameDepth(_003C_003Ec__DisplayClass2_2.node);
				Type type = _003C_003Ec__DisplayClass2_2.node.Data.type;
				bool flag = object.Equals(nextNodeWithSameDepth, null);
				if (object.Equals(_003C_003Ec__DisplayClass2_2.node.Data, null))
				{
					for (int i = 0; i < _003C_003Ec__DisplayClass2_2.node.Data.depth; i++)
					{
						_003C_003Ec__DisplayClass2_.result.Append("   ");
					}
					string text = flag ? "" : ",";
					_003C_003Ec__DisplayClass2_.result.Append("\"" + _003C_003Ec__DisplayClass2_2.node.Data.name + "\": \"\"" + text + "\n");
					_003CToJson_003Eg__AddLastElement_007C2_0(ref _003C_003Ec__DisplayClass2_, ref _003C_003Ec__DisplayClass2_2);
				}
				else if (type.IsArray)
				{
					for (int j = 0; j < _003C_003Ec__DisplayClass2_2.node.Data.depth; j++)
					{
						_003C_003Ec__DisplayClass2_.result.Append("   ");
					}
					_003C_003Ec__DisplayClass2_.result.Append("\"" + _003C_003Ec__DisplayClass2_2.node.Data.name + "\": \n");
					for (int k = 0; k < _003C_003Ec__DisplayClass2_2.node.Data.depth; k++)
					{
						_003C_003Ec__DisplayClass2_.result.Append("   ");
					}
					_003C_003Ec__DisplayClass2_.result.Append("[\n");
				}
				else if (type.IsPrimitive || type == typeof(string))
				{
					for (int l = 0; l < _003C_003Ec__DisplayClass2_2.node.Data.depth; l++)
					{
						_003C_003Ec__DisplayClass2_.result.Append("   ");
					}
					string text2 = flag ? "" : ",";
					string text3 = (!object.Equals(_003C_003Ec__DisplayClass2_2.node.Data.obj, null)) ? _003C_003Ec__DisplayClass2_2.node.Data.obj.ToString() : "";
					_003C_003Ec__DisplayClass2_.result.Append("\"" + _003C_003Ec__DisplayClass2_2.node.Data.name + "\": \"" + text3 + "\"" + text2 + "\n");
					_003CToJson_003Eg__AddLastElement_007C2_0(ref _003C_003Ec__DisplayClass2_, ref _003C_003Ec__DisplayClass2_2);
				}
				else
				{
					for (int m = 0; m < _003C_003Ec__DisplayClass2_2.node.Data.depth; m++)
					{
						_003C_003Ec__DisplayClass2_.result.Append("   ");
					}
					_003C_003Ec__DisplayClass2_.result.Append("\"" + _003C_003Ec__DisplayClass2_2.node.Data.name + "\": ");
					if (_003C_003Ec__DisplayClass2_2.node.Children.Count > 0)
					{
						_003C_003Ec__DisplayClass2_.result.Append("\n");
						for (int n = 0; n < _003C_003Ec__DisplayClass2_2.node.Data.depth; n++)
						{
							_003C_003Ec__DisplayClass2_.result.Append("   ");
						}
						_003C_003Ec__DisplayClass2_.result.Append("{\n");
					}
					else if (!object.Equals(_003C_003Ec__DisplayClass2_2.node.Data.obj, null))
					{
						_003C_003Ec__DisplayClass2_.result.Append("\"" + _003C_003Ec__DisplayClass2_2.node.Data.obj.ToString() + "\"\n");
					}
					else
					{
						_003C_003Ec__DisplayClass2_.result.Append("\"\"\n");
					}
				}
			}
			return _003C_003Ec__DisplayClass2_.result.ToString();
		}

		public static string ToJson(GameObject gameObject)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MonoBehaviour item in (IEnumerable<MonoBehaviour>)gameObject.GetComponents<MonoBehaviour>())
			{
				stringBuilder.AppendLine(item.GetType().Name);
				stringBuilder.AppendLine(JsonUtility.ToJson(item, prettyPrint: true));
			}
			return stringBuilder.ToString();
		}

		private static TreeNode<NodeData> GetNextNodeWithSameDepth(TreeNode<NodeData> node)
		{
			if (node.Parent != null)
			{
				bool flag = false;
				foreach (TreeNode<NodeData> child in node.Parent.Children)
				{
					if (flag)
					{
						return child;
					}
					if (child == node)
					{
						flag = true;
					}
				}
			}
			return null;
		}

		[CompilerGenerated]
		private static void _003CToTree_003Eg__AddChildNodes_007C1_0(TreeNode<NodeData> node, object obj)
		{
			if (obj == null || obj.Equals(null))
			{
				return;
			}
			Type type = obj.GetType();
			if (type.IsPrimitive || type == typeof(string))
			{
				return;
			}
			bool flag = false;
			TreeNode<NodeData> treeNode = (node != null) ? node.Parent : null;
			while (!object.Equals(treeNode, null) && !object.Equals(treeNode.Parent, null))
			{
				if (treeNode.Data.obj.GetType() == obj.GetType())
				{
					flag = true;
					break;
				}
				treeNode = treeNode.Parent;
			}
			if (flag)
			{
				return;
			}
			List<PropertyInfo> list = new List<PropertyInfo>(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			List<FieldInfo> list2 = new List<FieldInfo>(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			int count = list2.Count;
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].CanRead)
				{
					continue;
				}
				if (CustomAttributeExtensions.IsDefined(list[i], typeof(ObsoleteAttribute)) || (obj.GetType().BaseType != null && (obj.GetType().BaseType.IsEquivalentTo(typeof(Component)) || obj.GetType().BaseType.IsEquivalentTo(typeof(UnityEngine.Object)) || obj.GetType().BaseType.IsEquivalentTo(typeof(MulticastDelegate)) || obj.GetType().BaseType.IsEquivalentTo(typeof(Delegate)) || obj.GetType().BaseType.IsEquivalentTo(typeof(MonoBehaviour)))))
				{
					object obj2 = null;
					node.AddChild(new NodeData
					{
						obj = obj2,
						name = list[i].Name,
						type = list[i].PropertyType,
						depth = node.Data.depth + 1
					});
					continue;
				}
				ParameterInfo[] indexParameters = list[i].GetIndexParameters();
				if (!object.Equals(indexParameters, null) && indexParameters.Length != 0)
				{
					object[] array = new object[indexParameters.Length];
					for (int j = 0; j < indexParameters.Length; j++)
					{
						array[j] = indexParameters[j].Position;
					}
					object obj3 = null;
					try
					{
						obj3 = list[i].GetValue(obj, array);
					}
					catch
					{
					}
					TreeNode<NodeData> node2 = node.AddChild(new NodeData
					{
						obj = obj3,
						name = list[i].Name,
						type = list[i].PropertyType,
						depth = node.Data.depth + 1
					});
					if (obj3 != null)
					{
						_003CToTree_003Eg__AddChildNodes_007C1_0(node2, obj3);
					}
				}
				else if (list[i].GetType().IsArray)
				{
					Array array2 = obj as Array;
					for (int k = 0; k < array2.Length; k++)
					{
						TreeNode<NodeData> treeNode2 = node.AddChild(new NodeData
						{
							obj = array2.GetValue(k),
							name = list[i].Name,
							type = list[i].PropertyType,
							depth = node.Data.depth + 1
						});
						_003CToTree_003Eg__AddChildNodes_007C1_0(treeNode2, treeNode2.Data.obj);
					}
				}
				else
				{
					object obj5 = null;
					obj5 = list[i].GetValue(obj, null);
					TreeNode<NodeData> node3 = node.AddChild(new NodeData
					{
						obj = obj5,
						name = list[i].Name,
						type = list[i].PropertyType,
						depth = node.Data.depth + 1
					});
					if (obj5 != null)
					{
						_003CToTree_003Eg__AddChildNodes_007C1_0(node3, obj5);
					}
				}
			}
			for (int l = 0; l < list2.Count; l++)
			{
				if (list2[l].GetType().IsArray)
				{
					Array array3 = obj as Array;
					for (int m = 0; m < array3.Length; m++)
					{
						TreeNode<NodeData> treeNode3 = node.AddChild(new NodeData
						{
							obj = array3.GetValue(m),
							name = list2[l].Name,
							type = list2[l].FieldType,
							depth = node.Data.depth + 1
						});
						_003CToTree_003Eg__AddChildNodes_007C1_0(treeNode3, treeNode3.Data.obj);
					}
				}
				else
				{
					object value = list2[l].GetValue(obj);
					TreeNode<NodeData> treeNode4 = node.AddChild(new NodeData
					{
						obj = value,
						name = list2[l].Name,
						type = list2[l].FieldType,
						depth = node.Data.depth + 1
					});
					_003CToTree_003Eg__AddChildNodes_007C1_0(treeNode4, treeNode4.Data.obj);
				}
			}
		}

		[CompilerGenerated]
		private static void _003CToJson_003Eg__AddLastElement_007C2_0(ref _003C_003Ec__DisplayClass2_0 P_0, ref _003C_003Ec__DisplayClass2_1 P_1)
		{
			TreeNode<NodeData> treeNode = P_1.node;
			while (!object.Equals(treeNode, null) && !object.Equals(treeNode.Parent, null))
			{
				bool flag = object.Equals(GetNextNodeWithSameDepth(treeNode), null);
				if (object.Equals(treeNode.Parent, null) || !flag)
				{
					break;
				}
				string str = object.Equals(GetNextNodeWithSameDepth(treeNode.Parent), null) ? "" : ",";
				if (treeNode.Parent.Data.type.IsArray)
				{
					for (int i = 0; i < treeNode.Parent.Data.depth; i++)
					{
						P_0.result.Append("   ");
					}
					P_0.result.Append("]" + str + "\n");
				}
				else
				{
					for (int j = 0; j < treeNode.Parent.Data.depth; j++)
					{
						P_0.result.Append("   ");
					}
					P_0.result.Append("}" + str + "\n");
				}
				treeNode = treeNode.Parent;
			}
		}
	}
}
