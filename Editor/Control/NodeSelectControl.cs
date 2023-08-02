using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Editor {
	/// <summary>
	/// ノード選択
	/// </summary>
	public class NodeSelectControl : IHaveControl {
		//操作中か
		bool enable_;

		// 操作プライオリティ
		public int Priority { get { return (int)OperationPriority.SelectNode; } }

		BehaviorTreeEditor window_;
		Node select_;

		Rect drawRect = new Rect();

		int parent_;
		int index_;

		int node_;
		/// <summary>
		/// ウィンドウをセットアップ
		/// </summary>
		public NodeSelectControl(BehaviorTreeEditor window, UserOperation operation) {
			window_ = window;
			//自分を制御振り分け機能に登録要求
			operation.Register(this, false);
		}
		/// <summary>
		/// 制御開始
		/// </summary>
		public bool Interrupt() {
			if (Event.current.type != EventType.MouseDown)
				return false;


			Node root = window_.root;
			select_ = SelectNode(root, Event.current.mousePosition - window_.basePoint);

			if (select_ == null)
				return false;
			drawRect = new Rect();
			parent_ = -1;
			node_ = -1;
			return true;
		}

		/// <summary>
		/// 制御開始
		/// </summary>
		public void Begin() {
			window_.current = select_;
			enable_ = true;

			window_.Repaint();
		}
		/// <summary>
		/// 制御終了
		/// </summary>
		public bool IsEnd() {
			return !enable_;
		}
		/// <summary>
		/// 制御却下
		/// </summary>
		public void Reject() {
			enable_ = false;
			window_.Repaint();
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute() {
			if (!enable_)
				return;
			 
			Node root = window_.root;
			switch (Event.current.type) {
			case EventType.MouseUp:
				//どこかの領域にドロップしたら
				if(parent_ >= 0) {
					window_.MoveParts(select_, parent_, index_);
				}else if(node_ >= 0) {
					window_.MoveParents(select_, node_);
				}
				Reject();
				break;
			case EventType.MouseDrag:
				CalcDropRect();
				break;
			default:
				break;
			}
			Color col = GUI.color;
			GUI.color = new Color(1f, 0.5f, 0.5f, 0.5f);
			GUI.Box(drawRect, "", NodeConst.NodeStyle);
			GUI.color = col;
		}

		Node SelectNode(Node node, Vector2 pos) {
			if (node.rect.Contains(pos)) {
				return node;
			}
			if (node.Collapse)
				return null;
			for(int i = 0, max = node.child.Count; i < max; i++) {
				Node n = SelectNode(node.child[i], pos);
				if (n != null)
					return n;
			}
			return null;
		}


		void CalcDropRect() {
			//ルートノードは移動させない
			if (select_.parent == null)
				return;
			parent_ = -1;
			node_ = -1;
			//間の判定を先に行う
			foreach (KeyValuePair<int, List<Rect>> nodeRects in window_.InterNodeRect) {
				List<Rect> rects = nodeRects.Value;
				for (int i = 0, max = rects.Count; i < max; i++) {
					Rect r = rects[i];
					r.position += window_.basePoint;
					if (r.Contains(Event.current.mousePosition)) {
						drawRect = r;
						window_.Repaint();
						parent_ = nodeRects.Key;
						index_ = i;
						return;
					}

				}
			}


			Node n = SelectNode(window_.root, Event.current.mousePosition - window_.basePoint);
			if(n != null && n.Id != select_.Id) {
				drawRect = new Rect(n.rect.position + window_.basePoint, n.rect.size);
				window_.Repaint();
				node_ = n.Id;
			}
		}

	}
}
