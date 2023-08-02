using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Project.Lib {

	[System.Serializable]
	public class NodeDataSelector : NodeData {
	}

	public partial class BehaviorTree<T> {
		/// <summary>
		/// 順次実行
		/// </summary>
		public class Selector : Branch {
			public override Node Next(BehaviorStatus status) {
				//子ノードが成功したら親に戻る
				if (status == BehaviorStatus.Success) {
					//親に実行結果を渡す
					return parent.Next(status);
				}
				current++;
				//最後まで終わった
				if (current >= child.Count) {
					return parent.Next(status);
				}

				return child[current];
			}


			public override Node Exec(T entity) {
				if (child.Count == 0)
					return Next(BehaviorStatus.Failure);
				current = 0;
				return child[current];
			}
		}
	}
}
