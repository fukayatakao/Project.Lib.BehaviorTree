using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Project.Editor {
	//制御優先度
	public enum OperationPriority {
		SelectNode,              //ノードの選択→ドラッグ＆ドロップ


		//デフォルトなので一番優先度を下げる
		MainView,              //メインのビュー操作中
	}
}
