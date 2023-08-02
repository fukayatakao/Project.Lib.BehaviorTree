using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Editor {

	/// <summary>
	/// Actionノード
	/// </summary>
	public class Action : INodeBehavior {
		public string text { get { return "Action"; } }
		public bool IsChild() { return false; }

		public NodeType nodeType { get{return NodeType.Action; } }

		//ノードのカラー定義
		static readonly Color color_ = new Color(255f / 255f, 187f / 255f, 187f / 255f);
		public Color color { get { return color_; } }

		static readonly Color EvaluateColor = new Color(255f / 255f, 210f / 255f, 210f / 255f);
		static readonly Color OrderColor = new Color(210f / 255f, 210f / 255f, 255f / 255f);
		static readonly Color ContinuousColor = new Color(255f / 255f, 255f / 255f, 210f / 255f);

		List<MethodSelector> evaluate_ = new List<MethodSelector>();
		List<MethodSelector> order_ = new List<MethodSelector>();
		List<MethodSelector> continuous_ = new List<MethodSelector>();


		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		public Action(Action org) {
			for(int i = 0, max = org.evaluate_.Count; i < max; i++) {
				evaluate_.Add(new MethodSelector(org.evaluate_[i]));
			}
			for (int i = 0, max = org.order_.Count; i < max; i++) {
				order_.Add(new MethodSelector(org.order_[i]));
			}
			for (int i = 0, max = org.continuous_.Count; i < max; i++) {
				continuous_.Add(new MethodSelector(org.continuous_[i]));
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Action() {
		}

		/// <summary>
		/// GUI描画
		/// </summary>
		public void DrawGUI(Vector2 pos) {
			Color col = GUI.color;
			GUI.color = EvaluateColor;
			//行動前評価：falseだったら行動失敗
			DrawMethodSelector(evaluate_, BehaviorData.Instance.Evaluate, BehaviorData.Instance.EvaluateText, ref pos);
			GUI.color = OrderColor;
			//行動：返り値は使わない予定
			DrawMethodSelector(order_, BehaviorData.Instance.Order, BehaviorData.Instance.OrderText, ref pos);
			GUI.color = ContinuousColor;
			//行動前評価：falseだったら行動失敗
			DrawMethodSelector(continuous_, BehaviorData.Instance.Evaluate, BehaviorData.Instance.EvaluateText, ref pos);
			GUI.color = col;
		}

		void DrawMethodSelector(List<MethodSelector> list, List<MethodEditor> methods, string[] text, ref Vector2 pos) {
			if (list.Count == 0) {
				if (GUI.Button(new Rect(pos + NodeConst.MethodRect.position, NodeConst.MethodRect.size), "create")) {
					list.Add(new MethodSelector(methods, text));
				}
				pos.y += NodeConst.MethodArgsHeight;
			} else {
				for (int i = 0, max = list.Count; i < max; i++) {
					MethodSelector.Control result = list[i].DrawGUI(ref pos);
					switch (result) {
					case MethodSelector.Control.Add:
						list.Insert(i + 1, new MethodSelector(methods, text));
						return;
					case MethodSelector.Control.Del:
						list.RemoveAt(i);
						return;
					case MethodSelector.Control.Up:
						if (i > 0) {
							var d = list[i - 1];
							list[i - 1] = list[i];
							list[i] = d;
							return;
						}

						break;
					case MethodSelector.Control.Down:
						if (i < max - 1) {
							var d = list[i + 1];
							list[i + 1] = list[i];
							list[i] = d;
							return;
						}
						break;
					case MethodSelector.Control.None:
						break;
					}

				}
			}

		}

		/// <summary>
		/// セーブ用データ構造作成
		/// </summary>
		public NodeData Save() {
			NodeDataAction data = new NodeDataAction();
			for (int i = 0, max = order_.Count; i < max; i++) {
				data.order.Add(order_[i].Save());
			}
			for(int i = 0, max = evaluate_.Count; i < max; i++) {
				data.evaluate.Add(evaluate_[i].Save());
			}
			for (int i = 0, max = continuous_.Count; i < max; i++) {
				data.continuous.Add(continuous_[i].Save());
			}
			return data;
		}

		/// <summary>
		/// データ構造をロード
		/// </summary>
		public void Load(NodeData n) {
			NodeDataAction data = n as NodeDataAction;
			for (int i = 0, max = data.order.Count; i < max; i++) {
				MethodSelector ev = new MethodSelector(BehaviorData.Instance.Order, BehaviorData.Instance.OrderText);
				ev.Load(data.order[i]);
				order_.Add(ev);
			}
			for (int i = 0, max = data.evaluate.Count; i < max; i++) {
				MethodSelector ev = new MethodSelector(BehaviorData.Instance.Evaluate, BehaviorData.Instance.EvaluateText);
				ev.Load(data.evaluate[i]);
				evaluate_.Add(ev);
			}
			for (int i = 0, max = data.continuous.Count; i < max; i++) {
				MethodSelector ev = new MethodSelector(BehaviorData.Instance.Evaluate, BehaviorData.Instance.EvaluateText);
				ev.Load(data.continuous[i]);
				continuous_.Add(ev);
			}
		}
	}
}
