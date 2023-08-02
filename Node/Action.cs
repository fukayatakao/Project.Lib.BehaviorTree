using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Project.Lib {

	[System.Serializable]
	public class NodeDataAction : NodeData {
		//実行関数
		public List<MethodData> order = new List<MethodData>();
		public List<MethodData> evaluate = new List<MethodData>();
		public List<MethodData> continuous = new List<MethodData>();
	}

	public partial class BehaviorTree<T> {
		/// <summary>
		/// 実行ノード
		/// </summary>
		public class Action : Leaf {
			//実行処理
			List<OrderMethod> order_ = new List<OrderMethod>();
			List<object[]> orderArgs_ = new List<object[]>();
			//失敗条件チェック
			List<EvaluateMethod> evaluate_ = new List<EvaluateMethod>();
			List<object[]> evaluateArgs_ = new List<object[]>();
			//終了条件チェック
			List<EvaluateMethod> continuous_ = new List<EvaluateMethod>();
			List<object[]> continuousArgs_ = new List<object[]>();
			/// <summary>
			/// 呼び出し関数設定
			/// </summary>
			public void AddOrder(OrderMethod method, object[] args) {
				//関数と引数のリストが一致しない場合は何かバグってる
				Debug.Assert(order_.Count == orderArgs_.Count, "order data error");
				order_.Add(method);
				orderArgs_.Add(args);
			}

			/// <summary>
			/// 失敗チェック関数設定
			/// </summary>
			public void AddEvaluate(EvaluateMethod method, object[] args) {
				//関数と引数のリストが一致しない場合は何かバグってる
				Debug.Assert(evaluate_.Count == evaluateArgs_.Count, "evaluate data error");
				evaluate_.Add(method);
				evaluateArgs_.Add(args);
			}

			/// <summary>
			/// 継続チェック関数設定
			/// </summary>
			public void AddContinuous(EvaluateMethod method, object[] args) {
				//関数と引数のリストが一致しない場合は何かバグってる
				Debug.Assert(continuous_.Count == continuousArgs_.Count, "continuous data error");
				continuous_.Add(method);
				continuousArgs_.Add(args);
			}


			public override Node Exec(T entity) {
				//失敗チェック
				{
					for(int i = 0, max = evaluate_.Count; i < max; i++) {
						bool result = evaluate_[i](entity, evaluateArgs_[i]);
						//どれか一つでもチェックでtrueだったら失敗で終了
						if (result)
							return parent.Next(BehaviorStatus.Failure);
					}
				}
				for (int i = 0, max = order_.Count; i < max; i++) {
					order_[i](entity, orderArgs_[i]);
				}
				{
					int max = continuous_.Count;
					if (max == 0)
						return parent.Next(BehaviorStatus.Success);
					//継続チェック
					for (int i = 0; i < max; i++) {
						bool result = continuous_[i](entity, continuousArgs_[i]);
						//どれか一つでもtrueだったら終わり
						if (result) {
							return parent.Next(BehaviorStatus.Success);
						}
					}
				}
				//終了条件を満たしてないので継続
				status = BehaviorStatus.Running;
				return this;
			}
		}
	}
}
