using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;


namespace Project.Editor {
	public class BehaviorData {
		static BehaviorData instance_;
		private BehaviorData(){ }
		/// <summary>
		/// シングルトン
		/// </summary>
		public static BehaviorData Instance{
			get{
				if(instance_ == null) {
					instance_ = new BehaviorData();
				}

				return instance_;
			}
		}

		private class BehaviourType {
			public System.Type entityType;

			public System.Type evaluateClass;
			public System.Type orderClass;

			public string assetPath;
		}
		static Dictionary<string, BehaviourType> behaviourEditDict_ = new Dictionary<string, BehaviourType>()
		{
			{ "character", new BehaviourType{
				entityType =Type.GetType("Project.Game.CharacterEntity, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"),
				evaluateClass =Type.GetType("Project.Game.CharacterEvaluate, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"),
				orderClass =Type.GetType("Project.Game.CharacterOrder, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"),
				assetPath = "Assets/Application/Addressable/Character/AI/",
			}},
			{ "platoon", new BehaviourType{
				entityType =Type.GetType("Project.Game.PlatoonEntity, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"),
				evaluateClass =Type.GetType("Project.Game.PlatoonOrder, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"),
				orderClass =Type.GetType("Project.Game.PlatoonOrder, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"),
				assetPath = "Assets/Application/Addressable/Platoon/AI/",
			}},
		};
		/// <summary>
		/// 関数リストを取得
		/// </summary>
		private static List<MethodEditor> CreateMethodList(System.Type entityType, Type EventClass) {
			List<MethodEditor> method = new List<MethodEditor>();
			System.Reflection.MethodInfo[] list = EventClass.GetMethods();
			//関数の引数をチェック
			for (int i = 0, max = list.Length; i < max; i++) {
				System.Reflection.ParameterInfo[] param = list[i].GetParameters();
				//引数は２つで１つ目にEntity、２つ目にobject配列のもののみ正規の関数とみなす
				if (param.Length == 2 && param[0].ParameterType == entityType && param[1].ParameterType == typeof(object[])) {
					method.Add(new MethodEditor(list[i]));
				}
			}
			return method;
		}

		/// <summary>
		/// Entity切り替えしたときの処理
		/// </summary>
		public void Setting(string entityType) {
			//現在選択中のentityの設定を取得する
			currentEditType_ = behaviourEditDict_[entityType];
			//popup用のindexも更新する
			int count = 0;
			foreach (string key in behaviourEditDict_.Keys) {
				if (key == entityType) {
					selectIndex_ = count;
					break;
				}
				count++;
			}

			//評価関数の情報を取得
			Setup(currentEditType_.evaluateClass, ref Evaluate, ref EvaluateText);
			//継続関数の情報を取得
			Setup(currentEditType_.orderClass, ref Order, ref OrderText);
		}

		void Setup(Type classType, ref List<MethodEditor> methods, ref string[] text) {
			methods = CreateMethodList(currentEditType_.entityType, classType);
			text = new string[methods.Count];
			for (int i = 0, max = methods.Count; i < max; i++) {
				text[i] = methods[i].GetDefaultText();
			}

		}


		BehaviourType currentEditType_;
		public string key { get { return new List<string>(behaviourEditDict_.Keys).ToArray()[selectIndex_]; } }
		public string path{get{ return currentEditType_.assetPath; } }

		//評価関数
		public List<MethodEditor> Evaluate;
		public string[] EvaluateText;
		//実行関数
		public List<MethodEditor> Order;
		public string[] OrderText;


		int selectIndex_ = 0;
		bool editFlag_ = false;

		/// <summary>
		/// 表示処理
		/// </summary>
		public void DrawHeaderGUI() {
			//Entityタイプの設定
			EditorGUI.BeginChangeCheck();

			int oldSelect = selectIndex_;
			selectIndex_ = EditorGUILayout.Popup("Entity", selectIndex_, new List<BehaviourType>(behaviourEditDict_.Values).ConvertAll(x => x.entityType.Name).ToArray());
			//変更有った時
			if (EditorGUI.EndChangeCheck()) {
				if (!editFlag_ || EditorUtility.DisplayDialog("", "データが消える可能性があります", "ok", "cancel")) {
					Setting(new List<string>(behaviourEditDict_.Keys)[selectIndex_]);


					editFlag_ = false;
				} else {
					selectIndex_ = oldSelect;
				}
			}

		}
		/// <summary>
		/// 表示処理
		/// </summary>
		public void DrawHeaderGUI(Dictionary<string, Node> nodes) {
		}

	}
}
