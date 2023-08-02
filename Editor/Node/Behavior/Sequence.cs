using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Editor {
	/// <summary>
	/// Sequenceノード
	/// </summary>
	public class Sequence : INodeBehavior {
		public string text { get { return "Sequence"; } }
		public bool IsChild() { return true; }
		public NodeType nodeType { get { return NodeType.Sequence; } }
		static readonly Color color_ = new Color(153 / 255f, 122 / 255f, 255 / 255f);
		public Color color { get { return color_; } }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Sequence() {
		}
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		public Sequence(Sequence org) {
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
			NodeDataSequence data = new NodeDataSequence();
			return data;
		}

		/// <summary>
		/// データ構造をロード
		/// </summary>
		public void Load(NodeData data) {
		}
	}
}
