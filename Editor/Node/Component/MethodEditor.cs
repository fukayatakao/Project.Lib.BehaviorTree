using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Editor {
	public class MethodEditor {
		System.Reflection.MethodInfo method_;
		string methodText_;
		string methodText2_;

		ArgAttribute[] argAttributes_;
		Dictionary<int, string[]> argEnumNames_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MethodEditor(System.Reflection.MethodInfo method) {
			method_ = method;
			argEnumNames_ = new Dictionary<int, string[]>();


			//通常関数と真偽切り替え用関数で分岐
			if (Attribute.GetCustomAttribute(method, typeof(FunctionAttribute)) as FunctionAttribute != null) {
				FunctionAttribute funcAttributes_ = Attribute.GetCustomAttribute(method, typeof(FunctionAttribute)) as FunctionAttribute;
				methodText_ = funcAttributes_.Text;
				//引数の属性を拾う
				argAttributes_ = MethodUtil.CreateArgAttributeArray(method);

			} else {
				CheckFunctionAttribute funcAttributes_ = Attribute.GetCustomAttribute(method, typeof(CheckFunctionAttribute)) as CheckFunctionAttribute;
				methodText_ = funcAttributes_.Affiermative;
				methodText2_ = funcAttributes_.Negative;

				argAttributes_ = new ArgAttribute[] { new ArgAttribute(0, typeof(bool), "真偽値", true) };

			}


			for (int i = 0; i < argAttributes_.Length; i++) {
				argEnumNames_[i] = FieldAttribute.GetFields(argAttributes_[i].Type);
			}
		}
		/// <summary>
		/// 関数名を取得
		/// </summary>
		public string GetFunctionName() {
			return method_.Name;
		}
		/// <summary>
		/// 関数説明テキストを取得
		/// </summary>
		public string GetDefaultText() {
			return methodText_;
		}
		/// <summary>
		/// 関数説明テキストを取得
		/// </summary>
		public string GetText(object[] args) {
			//@todo 雑処理なので直す
			//methodText2_がある場合は真偽切り替え用関数と断定
			if (methodText2_ != null) {
				if ((bool)args[0]) {
					return methodText_;
				} else {
					return methodText2_;
				}
			} else {
				return methodText_;
			}
		}
		/// <summary>
		/// デフォルト値配列を生成
		/// </summary>
		public object[] CreateDefaultValueArray() {
			return MethodUtil.CreateDefaultValueArray(argAttributes_);
		}
		/// <summary>
		/// 関数の説明に引数を埋め込む
		/// </summary>
		public string DecorateText(ref object[] args) {
			if (args == null)
				return GetDefaultText();
			//関数に変更があったなどで引数が一致しない場合
			if (args.Length != argAttributes_.Length) {
				Debug.LogError("引数の数が一致しないのでデフォルト値で初期化します:" + GetDefaultText());
				args = MethodUtil.CreateDefaultValueArray(argAttributes_);
			}

			//argsをそのまま使うと引数が置き換わってしまうのでコピーをちゃんと作る
			object[] convArgs = new object[args.Length];
			for (int i = 0, max = args.Length; i < max; i++) {
				//列挙型はフィールドの文字列を入れる
				if (MethodUtil.IsEnumArg(argAttributes_, i)) {
					convArgs[i] = FieldAttribute.GetField(args[i]);
					//それ以外はそのまま
				} else {
					convArgs[i] = args[i];
				}
			}
			try {
				string txt = GetText(args);
				txt = txt.Replace("{", "[{");
				txt = txt.Replace("}", "}]");
				string result = string.Format(txt, convArgs);
				return result;
			} catch (Exception e) {
				Debug.LogError(e);
				return GetDefaultText();
			}

		}
		/// <summary>
		/// 引数GUI表示
		/// </summary>
		public bool DrawArgGUI(ref Vector2 pos, ref object[] args) {
			if (args == null)
				return false;
			//関数に変更があったなどで引数が一致しない場合
			if (args.Length != argAttributes_.Length) {
				Debug.LogError("引数の数が一致しないのでデフォルト値で初期化します:" + GetDefaultText());
				args = MethodUtil.CreateDefaultValueArray(argAttributes_);
			}
			EditorGUI.BeginChangeCheck();
			for (int i = 0; i < args.Length; i++) {
				Rect labelRect = new Rect(pos + NodeConst.ArgLabelRect.position, NodeConst.ArgLabelRect.size);
				Rect argRect = new Rect(pos + NodeConst.ArgRect.position, NodeConst.ArgRect.size);
				try {
					EditorGUI.LabelField(labelRect, argAttributes_[i].Text, new GUIStyle("Box") { alignment = TextAnchor.MiddleCenter, fontSize = NodeConst.MethodArgsFontSize, normal = new GUIStyleState() { textColor = Color.white } });
					if (argAttributes_[i].Type == typeof(string)) {
						args[i] = EditorGUI.TextField(argRect, (string)args[i], new GUIStyle("Button") { alignment = TextAnchor.MiddleLeft, fontSize = NodeConst.MethodArgsFontSize });
					} else if (argAttributes_[i].Type == typeof(int)) {
						args[i] = EditorGUI.IntField(argRect, (int)args[i], new GUIStyle("Button") { alignment = TextAnchor.MiddleLeft, fontSize = NodeConst.MethodArgsFontSize });
					} else if (argAttributes_[i].Type == typeof(long)) {
						args[i] = EditorGUI.LongField(argRect, (long)args[i], new GUIStyle("Button") { alignment = TextAnchor.MiddleLeft, fontSize = NodeConst.MethodArgsFontSize });
					} else if (argAttributes_[i].Type == typeof(float)) {
						args[i] = EditorGUI.FloatField(argRect, (float)args[i], new GUIStyle("Button") { alignment = TextAnchor.MiddleLeft, fontSize = NodeConst.MethodArgsFontSize });
					} else if (argAttributes_[i].Type == typeof(bool)) {
						args[i] = EditorGUI.Toggle(argRect, (bool)args[i], new GUIStyle("Toggle") { alignment = TextAnchor.MiddleLeft, fontSize = NodeConst.MethodArgsFontSize });
					} else if (argAttributes_[i].Type == typeof(Vector3)) {
						args[i] = EditorGUI.Vector3Field(argRect, "", (Vector3)args[i]);
						//enumの場合popuplist使う
					} else if (argAttributes_[i].Type.IsEnum) {
						string[] names = Enum.GetNames(argAttributes_[i].Type);
						string str = Enum.ToObject(argAttributes_[i].Type, args[i]).ToString();


						int sel = Array.IndexOf(names, str);
						sel = EditorGUI.Popup(argRect, sel, argEnumNames_[i], new GUIStyle("Button") { alignment = TextAnchor.MiddleLeft, fontSize = NodeConst.MethodArgsFontSize });
						args[i] = Enum.Parse(argAttributes_[i].Type, names[sel]);
					}
				} catch (Exception e) {
					Debug.LogError(e);
					Debug.LogError("引数のキャストエラーが発生したのでデフォルト値を代入します:" + GetDefaultText());


					args = MethodUtil.CreateDefaultValueArray(argAttributes_);

					return false;
				}
				pos.y += NodeConst.MethodArgsHeight;
			}
			//変更有ったのでtrueを返す
			if (EditorGUI.EndChangeCheck()) {
				return true;
			} else {
				return false;
			}
		}
	}

}
