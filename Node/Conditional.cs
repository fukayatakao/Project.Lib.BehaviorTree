using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Project.Lib {
	[System.Serializable]
	public class NodeDataConditional : NodeData {
		//判定関数
		public MethodData conditional = new MethodData();
	}


	public partial class BehaviorTree<T> {
		/// <summary>
		/// 条件分岐ノード
		/// </summary>
		public class Conditional : Leaf {
			EvaluateMethod method_;
			object[] args_;

			/// <summary>
			/// 呼び出し関数設定
			/// </summary>
			public void Init(EvaluateMethod method, object[] args) {
				method_ = method;
				args_ = args;
			}


			public override Node Exec(T entity) {
				BehaviorStatus status = method_(entity, args_) ? BehaviorStatus.Success : BehaviorStatus.Failure;
				//終わっていない場合
				if (status == BehaviorStatus.Running) {
					return this;
				} else {
					//親に実行結果を渡す
					return parent.Next(status);
				}
			}
		}

	}
}
