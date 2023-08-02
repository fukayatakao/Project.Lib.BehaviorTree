using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Lib {
	/// <summary>
	/// 行動制御のデータ
	/// </summary>
	public class BehaviorTreeData : ScriptableObject {
		public const string MainModule = "Root";

		//適用するEntityの型情報（主に編集・確認用）
		public string key;
		//idカウンタ
		public int counter;

		public List<NodeData> nodeDatas{
			get{
				List<NodeData> list = new List<NodeData>();
				list.AddRange(roots);
				list.AddRange(actions);
				list.AddRange(contidionals);
				list.AddRange(selectors);
				list.AddRange(sequences);
				return list;
			}
		}

		public List<NodeDataRoot> roots;
		public List<NodeDataAction> actions;
		public List<NodeDataConditional> contidionals;
		public List<NodeDataSelector> selectors;
		public List<NodeDataSequence> sequences;


	}

}
