using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Project.Lib {
	[System.Serializable]
	public class MethodData {
		//関数名
		public string method;
		//関数引数
		public string[] args;
	}
	[System.Serializable]
	public class NodeData {
		//ノードId
		public int id;
		//並び順（若い方が左
		public int priority;
		//ラベル表示
		public string label;
		//親ノードのId
		public int parent;
		//ノード種別
		public NodeType type;
	}

	public partial class BehaviorTree<T> {
		public abstract class Node {
			//自分の識別子
			public int Id;
#if DEVELOP_BUILD
			public string NodeLable;
#endif

			//親ノード
			public Node parent;
			/// <summary>
			/// 次に実行するノードを返す
			/// </summary>
			public virtual Node Next(BehaviorStatus status) {
				Debug.Assert(false, "cant use this function");
				return null;
			}
			//
			public abstract Node Exec(T entity);
		}

		//execution nodeの基底
		public abstract class Leaf : Node {
			protected BehaviorStatus status;
		}


		//control flow nodeの基底
		public abstract class Branch : Node {
			protected int current;
			public List<Node> child = new List<Node>();

		}
	}


}
