using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Editor {
	/// <summary>
	/// Conditionalノード
	/// </summary>
	public class Conditional : INodeBehavior {
		public string text { get { return "Conditional"; } }
		public bool IsChild() { return false; }
		public NodeType nodeType { get { return NodeType.Conditional; } }

		static readonly Color color_ = new Color(255 / 255f, 128 / 255f, 128 / 255f);
		public Color color { get { return color_; } }


		MethodSelector conditional;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Conditional() {
			conditional = new MethodSelector(BehaviorData.Instance.Evaluate, BehaviorData.Instance.EvaluateText);
		}
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		public Conditional(Conditional org) {
			conditional = new MethodSelector(org.conditional);
		}
		/// <summary>
		/// GUI描画
		/// </summary>　
		public void DrawGUI(Vector2 pos) {
			conditional.DrawGUI(ref pos, false);
		}
		/// <summary>
		/// セーブ用データ構造作成
		/// </summary>
		public NodeData Save() {
			NodeDataConditional data = new NodeDataConditional();
			data.conditional = conditional.Save();
			return data;
		}

		/// <summary>
		/// データ構造をロード
		/// </summary>
		public void Load(NodeData n) {
			NodeDataConditional data = n as NodeDataConditional;

			conditional.Load(data.conditional);
		}
	}
}
