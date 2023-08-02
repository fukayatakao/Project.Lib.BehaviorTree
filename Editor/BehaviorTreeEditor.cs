using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;


namespace Project.Editor {
	public class BehaviorTreeEditor : EditorWindow {
		/// <summary>
		/// 編集ダイアログのオープン
		/// </summary>
		[MenuItem("Editor/BehaviorTree", false, 98)]
		private static void Open() {
			BehaviorTreeEditor window = EditorWindow.GetWindow<BehaviorTreeEditor>(false, "BehaviorTree");
		}
		/// <summary>
		/// ウィンドウがアクティブになったとき
		/// </summary>
		private void OnEnable() {
			Init();
		}
		private void OnDestroy() {
		}
		/// <summary>
		/// 初期化
		/// </summary>
		private void Init() {
			InterNodeRect = new Dictionary<int, List<Rect>>();
			operation_ = new UserOperation();
			maincontrol = new MainControl(this, operation_);
			nodeSelectControl = new NodeSelectControl(this, operation_);

			operation_.SetDefault((int)OperationPriority.MainView);

			Create();

			BehaviorData.Instance.Setting("character");
		}
		/// <summary>
		/// 新規作成
		/// </summary>
		private void Create() {
			Node.Counter = new UniqueCounter();
			root = new Node(new Root());
			modules_ = new Dictionary<string, Node>();
			modules_[BehaviorTreeData.MainModule] = root;
			current = root;
			ResetLayout();

			moduleIndex_ = 0;
			newModuleName = "new Module";
			moduleNames = new List<string>(modules_.Keys).ToArray();
		}
		/// <summary>
		/// 表示をリセット
		/// </summary>
		private void ResetLayout() {
			scale = 1f;
			root.position = new Vector2(0, 0);
			basePoint = new Vector2(-root.position.x + position.size.x / scale / 2, root.position.y);
		}

		bool editFlag_;
		public Vector2 basePoint;
		public float scale_;
		public float scale{
			get{ return scale_; }
			set{
				scale_ = value;
				NodeConst.Init(scale_);
				Layout();
			}
		}
		public Node current;

		UserOperation operation_;
		MainControl maincontrol;
		NodeSelectControl nodeSelectControl;

		public Node root;
		public Dictionary<string, Node> modules_;

		public Dictionary<int, List<Rect>> InterNodeRect;

		int moduleIndex_;
		string newModuleName;
		string[] moduleNames;
		/// <summary>
		/// 表示処理
		/// </summary>
		private void OnGUI() {
			if (KeyControl())
				return;
			//GUI.TextFieldで日本語入力を可能にす
			Input.imeCompositionMode = IMECompositionMode.On;

			NodeConst.InitStyle(scale);
			GUILayout.BeginHorizontal();
			using (new GUILayout.VerticalScope()) {
				BehaviorData.Instance.DrawHeaderGUI();

				using (new GUILayout.HorizontalScope()) {
					EditorGUI.BeginChangeCheck();
					moduleIndex_ = EditorGUILayout.Popup("Module", moduleIndex_, moduleNames);
					if (EditorGUI.EndChangeCheck()) {
						root = modules_[moduleNames[moduleIndex_]];
						current = root;
						ResetLayout();
						Layout();
					}
					if (GUILayout.Button("Remove", GUILayout.MaxWidth(80f))) {
						//メイン以外は削除可能
						if(moduleNames[moduleIndex_] != BehaviorTreeData.MainModule) {
							modules_.Remove(moduleNames[moduleIndex_]);
							moduleNames = new List<string>(modules_.Keys).ToArray();
							moduleIndex_ = 0;
							root = modules_[BehaviorTreeData.MainModule];
							current = root;
							ResetLayout();
							Layout();
						}
					}
					newModuleName = EditorGUILayout.TextField(newModuleName, GUILayout.MaxWidth(240f));
					if (GUILayout.Button("Add", GUILayout.MaxWidth(80f))) {
						if (!modules_.ContainsKey(newModuleName)) {
							modules_[newModuleName] = new Node(new Root());
							root = modules_[newModuleName];
							current = root;
							moduleNames = new List<string>(modules_.Keys).ToArray();

							moduleIndex_ = moduleNames.Length - 1;
							ResetLayout();
							Layout();
						}
					}
				}
			}
			Rect lastHeaderRect = GUILayoutUtility.GetLastRect();


			//GUILayout.Label("");
			using (new GUILayout.VerticalScope(GUILayout.Width(160))) {
				Vector2 scroll_ = Vector2.zero;
				scroll_ = EditorGUILayout.BeginScrollView(scroll_);
				if (GUILayout.Button("selector")) {
					AddNode(current, new Node(new Selector()));
				}
				if (GUILayout.Button("sequence")) {
					AddNode(current, new Node(new Sequence()));
				}
				if (GUILayout.Button("action")) {
					AddNode(current, new Node(new Action()));
				}
				if (GUILayout.Button("conditional")) {
					AddNode(current, new Node(new Conditional()));
				}
				GUILayout.Label("");
				if (GUILayout.Button("delete") && current != null && current != root) {
					DelNode(current);
					current = null;
				}
				EditorGUILayout.EndScrollView();
				string path = BehaviorData.Instance.path;
				if (GUILayout.Button("ResetLayout", GUILayout.Width(160))) {
					ResetLayout();
				}
				if (GUILayout.Button("Create", GUILayout.Width(160))) {
					if (!editFlag_ || EditorUtility.DisplayDialog("", "データが消える可能性があります", "ok", "cancel")) {
						//editData_ = new Dictionary<int, BehaviourDecisionState>();
						Create();
						editFlag_ = false;
					}

				}
				if (GUILayout.Button("Load", GUILayout.Width(160))) {
					//選択したファイルを取得
					string file = EditorUtility.OpenFilePanelWithFilters("select file", Application.dataPath + path.Substring("Assets".Length), new string[] { "asset", "asset" });
					//ファイルを選んでいたら場合は先頭のプロジェクトまでのパスを削除して"Assets"を入れる
					if (!string.IsNullOrEmpty(file)) {
						string assetFile = "Assets" + file.Substring(Application.dataPath.Length);

						//データをロード
						BehaviorTreeData data = AssetDatabase.LoadAssetAtPath(assetFile, typeof(ScriptableObject)) as BehaviorTreeData;
						if (data == null)
							return;

						Load(data);
						editFlag_ = false;
					}
				}
				if (GUILayout.Button("Save", GUILayout.Width(160))) {
					//データをセーブ
					string file = EditorUtility.SaveFilePanel("save file", Application.dataPath + path.Substring("Assets".Length), "new asset", "asset");
					if (string.IsNullOrEmpty(file))
						return;
					Save(file);
					editFlag_ = false;
				}

			}
			GUILayout.EndHorizontal();


			//ノードの描画範囲を指定
			GUI.BeginGroup(new Rect(0, lastHeaderRect.y + lastHeaderRect.height, position.size.x - 160, position.size.y));

			if (current != null) {
				bool layout = current.DrawFrame(basePoint);
				//折り畳みが発生したらツリーを再計算
				if (layout) {
					Layout();
					return;
				}
			}

			DrawNode(root);
			//ロードダイアログのように前に他のウィンドウが出てる時も反応してしまうので対策
			nodeSelectControl.Execute();
			maincontrol.Execute();
			operation_.Execute();


			//デバッグ用
			/*Color col = GUI.color;
			foreach (List<Rect> rects in InterNodeRect.Values) {
				for(int i = 0, max = rects.Count; i < max; i++) {
					Rect r = rects[i];
					r.position += basePoint;
					if (nodeSelectControl.drawRect.position  == r.position) {

						GUI.color = new Color(1f, 0.5f, 0.5f, 0.5f);
						GUI.Box(r, "", NodeConst.NodeStyle);
					}
				}
			}
			GUI.color = col;*/
			GUI.EndGroup();
		}

		/// <summary>
		/// Node描画
		/// </summary>
		private void DrawNode(Node node) {
			if(node.parent != null) {
				Vector3 start = new Vector3(node.position.x + basePoint.x, node.position.y + basePoint.y, 0f);
				Vector3 end = new Vector3(node.parent.position.x + basePoint.x, node.parent.position.y + basePoint.y + NodeConst.Size.y, 0f);
				
				//Handles.DrawBezier(start, end, start + new Vector3(100, 0f ,0f), end + new Vector3(100, 0f, 0f), Color.black, null, 4f);

				Vector3[] P = new Vector3[]
				{
					start,end
				};
				Handles.DrawPolyLine(P);
			}
			bool layout = node.Draw(basePoint);
			if (layout) {
				Layout();
				return;
			}
			if (node.Collapse)
				return;
			for(int i = 0, max = node.child.Count; i < max; i++) {
				DrawNode(node.child[i]);
			}
		}

		/// <summary>
		/// ノードを追加する
		/// </summary>
		private void AddNode(Node parent, Node child) {
			if (!parent.IsChild())
				return;
			child.parent = parent;
			parent.child.Add(child);
			Layout();
		}
		/// <summary>
		/// ノードを削除する
		/// </summary>
		private void DelNode(Node node) {
			node.parent.child.Remove(node);
			Layout();
		}


		/// <summary>
		/// 親ノードを移動
		/// </summary>
		public void MoveParents(Node node, int parent) {
			//親が同一の場合は移動なし
			if (node.parent.Id == parent) {
				return;
			}
			Node parentNode = SerchNode(root, parent);
			//親ノードが子を持てない場合は無視
			if (!parentNode.IsChild())
				return;
			//親子逆転させるような移動は無視
			Node n = SerchNode(node, parentNode.Id);
			if (n != null)
				return;

			node.parent.child.Remove(node);

			parentNode.child.Add(node);
			node.parent = parentNode;
			Layout();
		}
		/// <summary>
		/// ノードの隙間に移動
		/// </summary>
		public void MoveParts(Node node, int parent, int index) {
			//親が同一の場合は移動なし
			if (node.Id == parent) {
				return;
			}
			//親が移動しない場合は移動前に自分が居なくなることによる要素数減少の対策を入れる
			if (node.parent.Id == parent) {
				if(node.parent.child.FindIndex(n => n == node) < index) {
					index--;
				}
			}
			node.parent.child.Remove(node);

			//新しい親を探す
			Node parentNode = SerchNode(root, parent);
			Debug.Assert(parentNode != null, "not found parent node:" + parent);
			if(index >= 0 && index < parentNode.child.Count) {
				parentNode.child.Insert(index, node);
			} else {
				if(index < 0) {
					parentNode.child.Insert(0, node);
				} else {
					parentNode.child.Add(node);
				}
			}
			node.parent = parentNode;

			Layout();
		}


		private Node SerchNode(Node node, int id) {
			if (node.Id == id)
				return node;

			for(int i = 0, max = node.child.Count; i < max; i++) {
				Node n = SerchNode(node.child[i], id);
				if (n != null)
					return n;
			}
			return null;
		}

		Dictionary<Node, int> nodeWeight = new Dictionary<Node, int>();
		private void Layout() {
			nodeWeight.Clear();
			Weighting(root);

			InterNodeRect.Clear();
			root.SetRect();
			Layout(root, root.position, 1);
			CalcInter(root);
		}

		/// <summary>
		/// ノードごとの重みを計算する
		/// </summary>
		private int Weighting(Node node) {
			if (node.Collapse) {
				nodeWeight[node] = 1;
				return 1;
			}
			int weight = node.child.Count + 1;
			for (int i = 0, max = node.child.Count; i < max; i++) {
				weight += Weighting(node.child[i]) - 1;
			}

			Debug.Assert(!nodeWeight.ContainsKey(node), "already node exists");
			nodeWeight[node] = weight;

			return weight;
		}

		/// <summary>
		/// 重みをもとにノード配置を計算
		/// </summary>
		private void Layout(Node node, Vector2 pos, int depth) {
			//nodeの全体の重み=必要な幅
			int total = nodeWeight[node] - 1;

			//階層ごとにノードの高さx2の幅を開ける
			float y = pos.y + NodeConst.Size.y * 2 * depth;
			int weight = 0;
			float left = node.position.x - (total * NodeConst.Size.x / 2);

			if (node.Collapse)
				return;

			for (int i = 0, max = node.child.Count; i < max; i++) {
				int w = nodeWeight[node.child[i]];
				float x = left + NodeConst.Size.x * weight + NodeConst.Size.x * w *0.5f;

				node.child[i].position = new Vector2(x, y);
				Layout(node.child[i], pos, depth + 1);

				weight += w;
			}
		}

		/// <summary>
		/// 狭間領域の計算
		/// </summary>
		private void CalcInter(Node node) {
			if (node.Collapse) {
				InterNodeRect[node.Id] = new List<Rect>();
				return;
			}

			List<Rect> list = new List<Rect>();
			//先頭分を生成
			if(node.child.Count > 0) {
				float left = node.child[0].rect.x - node.child[0].rect.width * 0.25f;
				float right = left + node.child[0].rect.width * 0.5f;
				float top = node.child[0].rect.y;
				float height = node.child[0].rect.height;

				Rect r = new Rect(left, top, right - left, height);
				list.Add(r);
			}
			//ノード間の矩形を生成
			for (int i = 0, max = node.child.Count - 1; i < max; i++) {
				float left = node.child[i].rect.x + node.child[i].rect.width * 0.75f;
				float right = node.child[i + 1].rect.x + node.child[i + 1].rect.width * 0.25f;
				float top = node.child[i].rect.y;
				float height = node.child[i].rect.height;

				Rect r = new Rect(left, top, right - left, height);
				list.Add(r);
			}
			//後端分を生成
			if (node.child.Count > 0) {
				int index = node.child.Count - 1;
				float left = node.child[index].rect.x + node.child[index].rect.width * 0.75f;
				float right = left + node.child[index].rect.width * 0.5f;
				float top = node.child[index].rect.y;
				float height = node.child[index].rect.height;

				Rect r = new Rect(left, top, right - left, height);
				list.Add(r);
			}

			InterNodeRect[node.Id] = list;
			for (int i = 0, max = node.child.Count; i < max; i++) {
				CalcInter(node.child[i]);
			}

		}


		/// <summary>
		/// ScriptableObjectから編集用データを生成
		/// </summary>
		private void Load(BehaviorTreeData data) {
			modules_ = new Dictionary<string, Node>();

			BehaviorData.Instance.Setting(data.key);
			Dictionary<int, NodeData> nodeDataDict = new Dictionary<int, NodeData>();
			Dictionary<int, Node> nodeDict = new Dictionary<int, Node>();

			List<NodeData> nodes = data.nodeDatas;
			for (int i = 0, max = nodes.Count; i < max; i++) {
				var nodeData = nodes[i];
				int id = nodeData.id;
				nodeDataDict[id] = nodeData;
				var node = new Node();
				node.Load(nodeData);

				if (nodeData.type == NodeType.Root) {
					NodeDataRoot r = nodeData as NodeDataRoot;
					//モジュール名が重複することはないはず
					Debug.Assert(!modules_.ContainsKey(r.module), "dupplicate module:" + r.module);
					modules_[r.module] = node;
				}
				nodeDict[id] = node;
			}

			//ルートが存在しない場合はデータがおかしい
			Debug.Assert(modules_.Count > 0, "not found root");

			//優先度でソート
			nodes.Sort((a, b) => a.priority - b.priority);
			//親子ノードを接続する
			for (int i = 0, max = nodes.Count; i < max; i++) {
				int key = nodes[i].id;
				if (nodeDataDict[key].parent < 0)
					continue;
				int parentId = nodeDataDict[key].parent;
				nodeDict[parentId].child.Add(nodeDict[key]);
				nodeDict[key].parent = nodeDict[parentId];
			}

			//メインのRootを最初に設定
			Debug.Assert(modules_.ContainsKey(BehaviorTreeData.MainModule), "not found main root");
			root = modules_[BehaviorTreeData.MainModule];
			current = root;
			ResetLayout();
			Layout();

			//エディタ用の設定をリセット
			moduleIndex_ = 0;
			moduleNames = new List<string>(modules_.Keys).ToArray();

			Node.Counter = new UniqueCounter(data.counter);

		}


		/// <summary>
		/// 編集用データからScriptableObjectにして保存
		/// </summary>
		private void Save(string path) {
			BehaviorTreeData data = ScriptableObject.CreateInstance<BehaviorTreeData>();
			data.key = BehaviorData.Instance.key;
			data.counter = Node.Counter.Id;

			data.roots = new List<NodeDataRoot>();
			data.actions = new List<NodeDataAction>();
			data.contidionals = new List<NodeDataConditional>();
			data.selectors = new List<NodeDataSelector>();
			data.sequences = new List<NodeDataSequence>();


			List<NodeData> nodes = new List<NodeData>();
			foreach (KeyValuePair<string, Node> pair in modules_) {
				NodeDataRoot module = pair.Value.Save() as NodeDataRoot;
				module.module = pair.Key;
				nodes.Add(module);
				nodes.AddRange(SaveImpl(pair.Value));
			}

			for(int i = 0, max = nodes.Count; i < max; i++) {
				Type type = nodes[i].GetType();
				nodes[i].priority = i;
				if (type == typeof(NodeDataRoot)) {
					data.roots.Add(nodes[i] as NodeDataRoot);
				} else if(type == typeof(NodeDataAction)) {
					data.actions.Add(nodes[i] as NodeDataAction);
				} else if (type == typeof(NodeDataConditional)) {
					data.contidionals.Add(nodes[i] as NodeDataConditional);
				} else if (type == typeof(NodeDataSelector)) {
					data.selectors.Add(nodes[i] as NodeDataSelector);
				} else if (type == typeof(NodeDataSequence)) {
					data.sequences.Add(nodes[i] as NodeDataSequence);
				}
			}


			//セーブ
			EditorUtility.SetDirty(data);
			string assetFile = "Assets" + path.Substring(Application.dataPath.Length);
			AssetDatabase.CreateAsset(data, assetFile);
			AssetDatabase.SaveAssets();
		}


		/// <summary>
		/// ノードごとの重みを計算する
		/// </summary>
		private List<NodeData> SaveImpl(Node node) {
			List<NodeData> list = new List<NodeData>();
			for (int i = 0, max = node.child.Count; i < max; i++) {
				list.Add(node.child[i].Save());
				list.AddRange(SaveImpl(node.child[i]));
			}

			return list;
		}


		public Node clipboard_ = null;
		private bool ctrl_ = false;

		private bool KeyControl() {
			Event e = Event.current;
			//ctrlキーチェック
			if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftControl) {
				ctrl_ = true;
			}
			if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftControl) {
				ctrl_ = false;
			}

			//ctrl+c
			if (ctrl_ && e.type == EventType.KeyDown && e.keyCode == KeyCode.C) {
				clipboard_ = current;
			}
			//ctrl+c
			if (ctrl_ && e.type == EventType.KeyDown && e.keyCode == KeyCode.V) {
				if(clipboard_.GetType() == typeof(Root)) {
					return false;
				}
				CopyNode(clipboard_, current);
			}

			return false;
		}

		/// <summary>
		/// Nodeをコピーする
		/// </summary>
		private void CopyNode(Node src, Node dest) {
			//srcと同じ内容をdestの子にぶら下げる

			AddNode(dest, new Node(src));





			ResetLayout();
		}

	}
}
