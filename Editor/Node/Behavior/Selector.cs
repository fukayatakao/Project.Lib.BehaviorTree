using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Editor {
	/// <summary>
	/// Selectorノード
	/// </summary>
	public class Selector : INodeBehavior {
		public string text { get { return "Selector"; } }
		public bool IsChild() { return true; }
		public NodeType nodeType { get { return NodeType.Selector; } }

		static readonly Color color_ = new Color(0 / 255f, 170 / 255f, 238 / 255f);
		public Color color { get { return color_; } }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Selector() {
		}
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		public Selector(Selector org) {
		}
		/// <summary>
		/// GUI描画
		/// </summary>
		public void DrawGUI(Vector2 pos) {
		}
		/// <summary>
		/// セーブ用データ構造作成
		/// </summary>
		public NodeData Save() {
			NodeDataSelector data = new NodeDataSelector();
			return data;
		}

		/// <summary>
		/// データ構造をロード
		/// </summary>
		public void Load(NodeData data) {
		}
	}
}
