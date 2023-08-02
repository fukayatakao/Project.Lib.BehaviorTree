using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Project.Lib {
	[System.Serializable]
	public class NodeDataRoot : NodeData {
		public string module;
	}


	public partial class BehaviorTree<T> {
		/// <summary>
		/// ルートノード
		/// </summary>
		public class Root : Branch {
			public override Node Next(BehaviorStatus status) {
				current++;
				//最後まで実行したら一旦自分に戻す
				if (current > child.Count - 1) {
					return null;
				}
				return child[current];
			}

			public override Node Exec(T entity) {
				current = 0;
				return child[current];
			}
		}


	}
}
