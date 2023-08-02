using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Editor {
	public static class NodeConst {
		public static void Init(float scale) {
			LabelTextSize = (int)(14 * scale);
			TypeTextSize = (int)(20 * scale);
			BorderSize = (int)(10 * scale);
			Size = new Vector2(120f * scale, 100f * scale);
			OuterRect = new Rect(new Vector2(0, 0), new Vector2(Size.x + BorderSize, Size.y + BorderSize));
			InnerRect = new Rect(new Vector2(BorderSize / 2, BorderSize / 2), new Vector2(Size.x, Size.y));
			LabelRect = new Rect(new Vector2(BorderSize / 2, BorderSize / 2 + (Size.y - TypeTextSize) * 0.5f), new Vector2(Size.x, TypeTextSize));
			TypeRect = new Rect(new Vector2(BorderSize / 2, BorderSize / 2 + (Size.y - TypeTextSize) * 0.1f), new Vector2(Size.x, TypeTextSize));
			TextRect = new Rect(new Vector2(BorderSize / 2, BorderSize / 2), new Vector2(Size.x, TypeTextSize));

			//ノード種別の選択リスト(rootは選択不可なので省く)
			nodeString = new List<string>(Enum.GetNames(typeof(NodeType))).GetRange(1, Enum.GetNames(typeof(NodeType)).Length - 1);
			MethodArgsFontSize = (int)(16 * scale);
			MethodArgsWidth = (int)(320 * scale);
			MethodArgsHeight = (int)(24 * scale);
			ArgsLabelWidth = (int)(80 * scale);
			MethodRect = new Rect(new Vector2(-MethodArgsWidth / 2, OuterRect.height), new Vector2(MethodArgsWidth, MethodArgsHeight));

			ButtonWidth = (int)(24 * scale);
			ButtonHeight = (int)(24 * scale);
			ArgDeployRect = new Rect(MethodRect.x - ButtonWidth, MethodRect.y, ButtonWidth, MethodRect.height);

			AddRect = new Rect(MethodRect.x - ButtonWidth * 5, MethodRect.y, ButtonWidth, MethodRect.height);
			DelRect = new Rect(MethodRect.x - ButtonWidth * 4, MethodRect.y, ButtonWidth, MethodRect.height);
			UpRect = new Rect(MethodRect.x - ButtonWidth * 3, MethodRect.y, ButtonWidth, MethodRect.height);
			DownRect = new Rect(MethodRect.x - ButtonWidth * 2, MethodRect.y, ButtonWidth, MethodRect.height);

			DeployRect = new Rect[4]{
				new Rect(0 + 0 * ButtonWidth, 0f, ButtonWidth, ButtonHeight),
				new Rect(0 + 1 * ButtonWidth, 0f, ButtonWidth, ButtonHeight),
				new Rect(0 + 2 * ButtonWidth, 0f, ButtonWidth, ButtonHeight),
				new Rect(0 + 3 * ButtonWidth, 0f, ButtonWidth, ButtonHeight),
			};

			ArgLabelRect = new Rect(new Vector2(-MethodArgsWidth / 2, Size.y + BorderSize), new Vector2(ArgsLabelWidth, MethodArgsHeight));
			ArgRect = new Rect(new Vector2(-MethodArgsWidth / 2 + ArgsLabelWidth, Size.y + BorderSize), new Vector2(MethodArgsWidth - ArgsLabelWidth, MethodArgsHeight));

		}
		/// <summary>
		/// StyleはOnGUIの中で設定しないとエラーになるので分離
		/// </summary>
		public static void InitStyle(float scale) {
			if(scale_ != scale) {
				MethodEditorStyle = new GUIStyle("Button") { alignment = TextAnchor.MiddleCenter, fontSize = NodeConst.MethodArgsFontSize };
				NodeStyle = new GUIStyle {
					normal =
					{
						background = (Texture2D)Resources.Load("NodeFrame"),
						textColor = Color.black,
					},
					border = new RectOffset(NodeConst.BorderSize, NodeConst.BorderSize, NodeConst.BorderSize, NodeConst.BorderSize),

					alignment = TextAnchor.MiddleCenter,
					wordWrap = true,
					clipping = TextClipping.Clip,
					fontSize = NodeConst.TypeTextSize,
				};

				LabelStyle = new GUIStyle
				{
					normal =
					{
						background = (Texture2D)Resources.Load("NodeFrame"),
						textColor = Color.black,
					},
					border = new RectOffset(NodeConst.BorderSize, NodeConst.BorderSize, NodeConst.BorderSize, NodeConst.BorderSize),
					alignment = TextAnchor.MiddleCenter,
					wordWrap = true,
					clipping = TextClipping.Clip,
					fontSize = NodeConst.LabelTextSize,
				};

			}
		}

		public static int LabelTextSize = 14;
		public static int TypeTextSize = 20;
		public static int BorderSize = 10;
		public static Vector2 Size = new Vector2(120f, 100f);
		public static Rect OuterRect = new Rect(new Vector2(0, 0), new Vector2(Size.x + BorderSize, Size.y + BorderSize));
		public static Rect InnerRect = new Rect(new Vector2(BorderSize / 2, BorderSize / 2), new Vector2(Size.x, Size.y));
		public static Rect LabelRect = new Rect(new Vector2(BorderSize / 2, BorderSize / 2 + (Size.y - TypeTextSize) * 0.5f), new Vector2(Size.x, TypeTextSize));
		public static Rect TypeRect = new Rect(new Vector2(BorderSize / 2, BorderSize / 2 + (Size.y - TypeTextSize) * 0.5f), new Vector2(Size.x, TypeTextSize));
		public static Rect TextRect = new Rect(new Vector2(BorderSize / 2, BorderSize / 2), new Vector2(Size.x, TypeTextSize));

		//ノード種別の選択リスト(rootは選択不可なので省く)
		public static List<string> nodeString = new List<string>(Enum.GetNames(typeof(NodeType))).GetRange(1, Enum.GetNames(typeof(NodeType)).Length - 1);
		public static int MethodArgsFontSize = 16;
		public static int MethodArgsWidth = 320;
		public static int MethodArgsHeight = 24;
		public static int ArgsLabelWidth = 80;
		public static Rect MethodRect = new Rect(new Vector2(-MethodArgsWidth / 2, OuterRect.height), new Vector2(MethodArgsWidth, MethodArgsHeight));

		public static int ButtonWidth = 24;
		public static int ButtonHeight = 24;
		public static Rect ArgDeployRect = new Rect(MethodRect.x - ButtonWidth, MethodRect.y, ButtonWidth, MethodRect.height);

		public static Rect AddRect = new Rect(MethodRect.x - ButtonWidth * 5, MethodRect.y, ButtonWidth, MethodRect.height);
		public static Rect DelRect = new Rect(MethodRect.x - ButtonWidth * 4, MethodRect.y, ButtonWidth, MethodRect.height);
		public static Rect UpRect = new Rect(MethodRect.x - ButtonWidth * 3, MethodRect.y, ButtonWidth, MethodRect.height);
		public static Rect DownRect = new Rect(MethodRect.x - ButtonWidth * 2, MethodRect.y, ButtonWidth, MethodRect.height);

		public static Rect[] DeployRect = new Rect[4]{
			new Rect(0 + 0 * ButtonWidth, 0f, ButtonWidth, ButtonHeight),
			new Rect(0 + 1 * ButtonWidth, 0f, ButtonWidth, ButtonHeight),
			new Rect(0 + 2 * ButtonWidth, 0f, ButtonWidth, ButtonHeight),
			new Rect(0 + 3 * ButtonWidth, 0f, ButtonWidth, ButtonHeight),
		};


		public static Rect ArgLabelRect = new Rect(new Vector2(-MethodArgsWidth / 2, Size.y + BorderSize), new Vector2(ArgsLabelWidth, MethodArgsHeight));
		public static Rect ArgRect = new Rect(new Vector2(-MethodArgsWidth / 2 + ArgsLabelWidth, Size.y + BorderSize), new Vector2(MethodArgsWidth - ArgsLabelWidth, MethodArgsHeight));

		public static float scale_ = -1;
		public static GUIStyle NodeStyle;
		public static GUIStyle LabelStyle;
		public static GUIStyle MethodEditorStyle;
	}


	public class Node {
		public int Id;
		public string Label;
		public Node parent;
		public List<Node> child = new List<Node>();
		public bool IsChild() { return behavior_.IsChild(); }
		//子ノードの折り畳みフラグ
		public bool Collapse;

		INodeBehavior behavior_;

		public static UniqueCounter Counter;
		//表示座標
		Vector2 position_;
		public Vector2 position {
			get{
				return position_;
			}
			set {
				position_ = value;
				SetRect();

			}
		}

		public Rect rect{ get; private set; }
		private Rect outerRect_;
		//カレント表示制御用一時変数
		bool current_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Node(INodeBehavior behavior) {
			behavior_ = behavior;
			Id = Counter.GetUniqueId();
			Label = behavior_.GetType().Name + ":" + Id;
		}
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		public Node(Node org) {
			if(org.behavior_ is Root) {
				behavior_ = new Root();
			}
			if (org.behavior_ is Selector) {
				behavior_ = new Selector((Selector)org.behavior_);
			}
			if (org.behavior_ is Sequence) {
				behavior_ = new Sequence((Sequence)org.behavior_);
			}
			if (org.behavior_ is Action) {
				behavior_ = new Action((Action)org.behavior_);
			}
			if (org.behavior_ is Conditional) {
				behavior_ = new Conditional((Conditional)org.behavior_);
			}
			Id = Counter.GetUniqueId();
			Label = org.Label;

			for(int i = 0, max = org.child.Count; i < max; i++) {
				Node node = new Node(org.child[i]);
				child.Add(node);
				node.parent = this;
			}

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Node() {
		}
		/// <summary>
		/// セーブ処理
		/// </summary>
		public NodeData Save() {

			NodeData data = behavior_.Save();

			data.id = Id;
			data.label = Label;
			data.type = behavior_.nodeType;
			//親がいる場合
			if (parent != null) {

				int p = -1;
				for (int i = 0, max = parent.child.Count; i < max; i++) {
					if(parent.child[i] == this) {
						p = i;
					}
				}
				Debug.Assert(p >= 0, "child priority error:" + Id);
				//親のIdと親から見たときの自分の順番を記録
				data.parent = parent.Id;
			//親がない場合
			} else {
				data.parent = -1;
			}


			return data;
		}
		/// <summary>
		/// ロード処理
		/// </summary>
		/// <remarks>
		/// 親子関係は外部で構築する
		/// </remarks>
		public void Load(NodeData data) {
			//親子関係以外の情報をセット
			Id = data.id;
			Label = data.label;
			switch (data.type) {
			case NodeType.Root:
				behavior_ = new Root();
				break;
			case NodeType.Selector:
				behavior_ = new Selector();
				break;
			case NodeType.Sequence:
				behavior_ = new Sequence();
				break;
			case NodeType.Action:
				behavior_ = new Action();
				break;
			case NodeType.Conditional:
				behavior_ = new Conditional();
				break;
			}

			behavior_.Load(data);
		}

		public void SetRect() {
			outerRect_ = new Rect(position.x - (NodeConst.Size.x + NodeConst.BorderSize) / 2, position.y, NodeConst.Size.x + NodeConst.BorderSize, NodeConst.Size.y + NodeConst.BorderSize);
			rect = new Rect(position.x - (NodeConst.Size.x + NodeConst.BorderSize) / 2, position.y, NodeConst.Size.x + NodeConst.BorderSize, NodeConst.Size.y + NodeConst.BorderSize);

		}

		/// <summary>
		/// 選択中の外枠表示
		/// </summary>
		public bool DrawFrame(Vector2 pos) {
			bool result = false;
			GUI.BeginGroup(new Rect(outerRect_.position + pos, outerRect_.size));
			Color col = GUI.color;
			GUI.color = Color.yellow;
			GUI.Box(NodeConst.OuterRect, "", NodeConst.NodeStyle);
			GUI.color = col;
			GUI.EndGroup();


			//折り畳み操作ボタン
			if (!Collapse && child.Count > 0) {
				GUI.BeginGroup(new Rect(outerRect_.position + pos - new Vector2(0f, NodeConst.ButtonHeight), outerRect_.size));
				if (GUI.Button(NodeConst.DeployRect[0], "∧")) {
					Collapse = !Collapse;
					result = true;
				}
				GUI.EndGroup();
			}


			current_ = true;
			return result;
		}

		/// <summary>
		/// 描画
		/// </summary>
		public bool Draw(Vector2 pos) {
			bool result = false;
			//オフセット+中央揃え
			GUI.BeginGroup(new Rect(rect.position + pos, rect.size));


			if (current_ && behavior_.nodeType != NodeType.Root) {
				Color col = GUI.color;
				GUI.color = behavior_.color;
				GUI.Box(NodeConst.InnerRect, "", NodeConst.NodeStyle);

				Label = GUI.TextField(NodeConst.LabelRect, Label, NodeConst.LabelStyle);
				EditorGUI.BeginChangeCheck();
				int sel = EditorGUI.Popup(NodeConst.TypeRect, (int)behavior_.nodeType - 1, NodeConst.nodeString.ToArray(), NodeConst.NodeStyle);
				GUI.color = Color.white;
				GUI.color = col;
				if (EditorGUI.EndChangeCheck()) {
					switch ((NodeType)(sel + 1)) {
					case NodeType.Selector:
						behavior_ = new Selector();
						break;
					case NodeType.Sequence:
						behavior_ = new Sequence();
						break;
					case NodeType.Conditional:
						if(child.Count == 0){
							behavior_ = new Conditional();
						}
						break;
					case NodeType.Action:
						if (child.Count == 0) {
							behavior_ = new Action();
						}
						break;
					}
				}
			} else {
				Color col = GUI.color;
				GUI.color = behavior_.color;
				GUI.Box(NodeConst.InnerRect, "", NodeConst.NodeStyle);
				Label = GUI.TextField(NodeConst.LabelRect, Label, NodeConst.LabelStyle);
				GUI.color = col;
			}

			GUI.EndGroup();

			if (Collapse && child.Count > 0) {
				Vector2 p = rect.position + pos + new Vector2(0f, rect.size.y);
				if(GUI.Button(new Rect(p, new Vector2(rect.size.x, NodeConst.MethodArgsFontSize)), "・・・", NodeConst.MethodEditorStyle)) {
					Collapse = false;
					result = true;
				}
			}

			if (current_ && behavior_.nodeType != NodeType.Root) {
				behavior_.DrawGUI(position_ + pos);
			}
			current_ = false;
			return result;
		}
	}
}
