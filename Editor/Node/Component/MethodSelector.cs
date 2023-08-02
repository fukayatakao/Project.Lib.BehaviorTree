using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Editor {

	public class MethodSelector {

		public enum Control {
			None,
			Add,
			Del,
			Up,
			Down,
		}

		int index_;
		object[] args_;

		private List<MethodEditor> methods_;
		private string[] text_ { get; set; }

		bool deploy_;

		public MethodSelector(List<MethodEditor> list, string[] array) {
			methods_ = list;
			text_ = array;
			deploy_ = false;
		}
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		public MethodSelector(MethodSelector org) {
			//ここはシャローでok
			methods_ = org.methods_;
			text_ = org.text_;
			deploy_ = false;

			index_ = org.index_;
			args_ = MethodArgs.DeserializeArgs(MethodArgs.SerializeArgs(org.args_));
		}


		/// <summary>
		/// データ構造をロード
		/// </summary>
		public void Load(MethodData data) {
			args_ = MethodArgs.DeserializeArgs(data.args);
			for (int i = 0, max = methods_.Count; i < max; i++) {
				if (data.method == methods_[i].GetFunctionName()) {
					index_ = i;
					break;
				}
			}
		}
		public MethodData Save() {
			MethodData data = new MethodData();
			data.args = MethodArgs.SerializeArgs(args_);
			data.method = methods_[index_].GetFunctionName();
			return data;
		}


		/// <summary>
		/// GUI描画
		/// </summary>
		public Control DrawGUI(ref Vector2 pos, bool control=true) {

			Color orgColor = GUI.color;
			if (control) {
				GUI.color = Color.yellow;
				if (GUI.Button(new Rect(pos + NodeConst.AddRect.position, NodeConst.AddRect.size), "＋")) {
					return Control.Add;
				}
				if (GUI.Button(new Rect(pos + NodeConst.DelRect.position, NodeConst.DelRect.size), "－")) {
					return Control.Del;
				}
				if (GUI.Button(new Rect(pos + NodeConst.UpRect.position, NodeConst.UpRect.size), "↑")) {
					return Control.Up;
				}
				if (GUI.Button(new Rect(pos + NodeConst.DownRect.position, NodeConst.DownRect.size), "↓")) {
					return Control.Down;
				}
			}


			if (!deploy_) {
				GUI.color = new Color(0.2f, 1f, 0.7f);
				if(GUI.Button(new Rect(pos + NodeConst.ArgDeployRect.position, NodeConst.ArgDeployRect.size), ">")){
					deploy_ = !deploy_;
				}
			} else {
				GUI.color = new Color(0.5f, 0.5f, 0.7f);
				if(GUI.Button(new Rect(pos + NodeConst.ArgDeployRect.position, NodeConst.ArgDeployRect.size), "<")) {
					deploy_ = !deploy_;
				}
			}
			GUI.color = orgColor;

			EditorGUI.BeginChangeCheck();
			int select = index_;
			string original = text_[select];
			text_[select] = methods_[select].DecorateText(ref args_);
			index_ = EditorGUI.Popup(new Rect(pos + NodeConst.MethodRect.position, NodeConst.MethodRect.size), index_, text_, NodeConst.MethodEditorStyle);
			text_[select] = original;
			if (EditorGUI.EndChangeCheck()) {
				args_ = methods_[index_].CreateDefaultValueArray();
			}
			pos.y += NodeConst.MethodArgsHeight;
			if (deploy_) {
				methods_[index_].DrawArgGUI(ref pos, ref args_);
			}
			return Control.None;
		}
	}
}
