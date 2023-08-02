using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;


namespace Project.Editor {
	/// <summary>
	/// メインウィンドウ操作
	/// </summary>
	public class MainControl : IHaveControl {

		const float SCALE_MIN = 0.5f;
		const float SCALE_MAX = 2f;

		//操作中か
		bool enable_;
		bool touch_;
		// 操作プライオリティ
		public int Priority { get { return (int)OperationPriority.MainView; } }

		BehaviorTreeEditor window_;
		Vector2 clickPos = new Vector2();
		Vector2 clickBase = new Vector2();

		/// <summary>
		/// レイを飛ばすカメラとUIをセットアップ
		/// </summary>
		public MainControl(BehaviorTreeEditor window, UserOperation operation) {
			window_ = window;
			operation.Register(this, false);
		}
		/// <summary>
		/// 制御開始
		/// </summary>
		public bool Interrupt() {
			return false;
		}

		/// <summary>
		/// 制御開始
		/// </summary>
		public void Begin() {
			enable_ = true;
		}
		/// <summary>
		/// 制御終了
		/// </summary>
		public bool IsEnd() {
			return false;
		}
		/// <summary>
		/// 制御却下
		/// </summary>
		public void Reject() {
			enable_ = false;
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute() {
			if (!enable_)
				return;

			Rect rect = new Rect(0, 0, int.MaxValue, int.MaxValue);
			if (rect.Contains(Event.current.mousePosition)) {
				switch (Event.current.type) {
				case EventType.MouseUp:
					touch_ = false;
					break;
				case EventType.MouseDrag:
					if (touch_) {
						window_.basePoint = clickBase + Event.current.mousePosition - clickPos;
						window_.Repaint();
					}
					break;
				case EventType.MouseDown:
					clickPos = Event.current.mousePosition;
					clickBase = window_.basePoint;
					touch_ = true;
					break;
				case EventType.ScrollWheel:
					window_.scale += Event.current.delta.y / 20f;
					if(window_.scale < SCALE_MIN) {
						window_.scale = SCALE_MIN;
					}
					if(window_.scale > SCALE_MAX) {
						window_.scale = SCALE_MAX;

					}

					//GUI.matrix
					window_.Repaint();
					break;

				default:
					break;
				}
			}
		}

	}
}
