using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Editor {
	/// <summary>
	/// ノードのふるまい
	/// </summary>
	public interface INodeBehavior {
		string text { get; }
		bool IsChild();
		Color color{ get; }

		NodeType nodeType { get; }

		void DrawGUI(Vector2 pos);

		/// <summary>
		/// セーブ用データ構造作成
		/// </summary>
		NodeData Save();

		/// <summary>
		/// データ構造をロード
		/// </summary>
		void Load(NodeData data);
	}
}
