using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Editor {
	/// <summary>
	/// Rootノード
	/// </summary>
	public class Root : INodeBehavior {
		public string text { get { return "Root"; } }
		public bool IsChild() { return true; }

		public NodeType nodeType { get { return NodeType.Root; } }
		static readonly Color color_ = new Color(196f / 255f, 196f / 255f, 196f / 255f);
		public Color color { get { return color_; } }

		/// <summary>
		/// GUI描画
		/// </summary>
		public void DrawGUI(Vector2 pos) {
		}

		/// <summary>
		/// セーブ用データ構造作成
		/// </summary>
		public NodeData Save() {
			NodeDataRoot data = new NodeDataRoot();
			return data;
		}

		/// <summary>
		/// データ構造をロード
		/// </summary>
		public void Load(NodeData data) {
		}
	}
}
