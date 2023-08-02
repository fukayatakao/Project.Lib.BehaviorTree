using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Lib {
	public enum NodeType {
		Root,
		Selector,
		Sequence,
		Conditional,
		Action,
	}

	public enum BehaviorStatus : int {
		Success,
		Failure,
		Running,
	}
	public partial class BehaviorTree<T>{
		//関数デリゲートのキャッシュクラス
		static Cache cache_;
		/// <summary>
		/// 関数情報をロードしてキャッシュ
		/// </summary>
		public static void Initiate(System.Type evaluate, System.Type order) {
			Debug.Assert(cache_ == null, "already alloc instance: BehaviorTree<" + typeof(T) + ">");
			cache_ = new Cache(evaluate, order);
		}
		/// <summary>
		/// 関数情報をクリア
		/// </summary>
		public static void Terminate() {
			Debug.Assert(cache_ != null, "already free instance: BehaviorTree<" + typeof(T) + ">");
			cache_ = null;
		}


		Node current_;
#if DEVELOP_BUILD
		public string CurrentLabel{
			get{
				if (current_ == null)
					return "";
				return current_.NodeLable;
			}
		}
#endif
		Dictionary<string, Node> modules_;
		Node root_;

		/// <summary>
		/// データを解析して実行できるようにする
		/// </summary>
		public void Create(BehaviorTreeData data) {
			modules_ = new Dictionary<string, Node>();
			Dictionary<int, Node> nodeDict = new Dictionary<int, Node>();

			//rootノード
			for(int i = 0, max = data.roots.Count; i < max; i++) {
				//同名のルートが複数ある場合はデータがおかしい
				Debug.Assert(!modules_.ContainsKey(data.roots[i].module), "root node error");
				modules_[data.roots[i].module] = nodeDict[data.roots[i].id] = CreateNode<Root>(data.roots[i]);
			}
			//selectorノード
			for (int i = 0, max = data.selectors.Count; i < max; i++) {
				nodeDict[data.selectors[i].id] = CreateNode<Selector>(data.selectors[i]);
			}
			//sequenceノード
			for (int i = 0, max = data.sequences.Count; i < max; i++) {
				nodeDict[data.sequences[i].id] = CreateNode<Sequence>(data.sequences[i]);
			}
			//actionノード
			for (int i = 0, max = data.actions.Count; i < max; i++) {
				Action action = CreateNode<Action>(data.actions[i]);
				for (int j = 0, max2 = data.actions[i].order.Count; j < max2; j++) {
					OrderMethod method = cache_.OrderMethods[data.actions[i].order[j].method];
					object[] args = MethodArgs.DeserializeArgs(data.actions[i].order[j].args);
					action.AddOrder(method, args);
				}
				for(int j = 0, max2 = data.actions[i].evaluate.Count; j < max2; j++) {
					EvaluateMethod method = cache_.EvaluateMethods[data.actions[i].evaluate[j].method];
					object[] args = MethodArgs.DeserializeArgs(data.actions[i].evaluate[j].args);
					action.AddEvaluate(method, args);
				}
				for (int j = 0, max2 = data.actions[i].continuous.Count; j < max2; j++) {
					EvaluateMethod method = cache_.EvaluateMethods[data.actions[i].continuous[j].method];
					object[] args = MethodArgs.DeserializeArgs(data.actions[i].continuous[j].args);
					action.AddContinuous(method, args);
				}

				nodeDict[data.actions[i].id] = action;
			}
			//conditionalノード
			for (int i = 0, max = data.contidionals.Count; i < max; i++) {
				NodeDataConditional conditionalData = data.contidionals[i];
				Conditional conditional = CreateNode<Conditional>(conditionalData);
				{
					EvaluateMethod method = cache_.EvaluateMethods[conditionalData.conditional.method];
					object[] args = MethodArgs.DeserializeArgs(conditionalData.conditional.args);
					conditional.Init(method, args);
				}
				nodeDict[data.contidionals[i].id] = conditional;
			}

			//親子関係を構築
			List<NodeData> nodes = data.nodeDatas;
			//優先度順に処理することで並びを保全する
			nodes.Sort((a, b) => a.priority - b.priority);
			for (int i = 0, max = nodes.Count; i < max; i++) {
				int id = nodes[i].id;
				int parent = nodes[i].parent;
				if (parent < 0)
					continue;
				nodeDict[id].parent = nodeDict[parent];

				Branch branch = nodeDict[parent] as Branch;
				Debug.Assert(branch != null, "branch error:" + nodeDict[parent].GetType());
				branch.child.Add(nodeDict[id]);

			}
			//ルートが存在しない場合はデータがおかしい
			Debug.Assert(modules_.ContainsKey(BehaviorTreeData.MainModule), "not found root");
			root_ = modules_[BehaviorTreeData.MainModule];
			current_ = root_;
		}

		/// <summary>
		/// ノードの生成
		/// </summary>
		N CreateNode<N>(NodeData node) where N : Node, new() {
			N n = new N();
			n.Id = node.id;
#if DEVELOP_BUILD
			n.NodeLable = node.label;
#endif
			return n;
		}

		/// <summary>
		/// サブモジュールに切り替え
		/// </summary>
		public void ChangeModule(string module) {
			Debug.Assert(modules_.ContainsKey(module), "not found sub module");
			current_ = modules_[module];
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(T entity) {
			//未初期化でcurrentがnullの場合無限ループになるのでその前に終わらせる
			if (current_ == null)
				return;
			Node node = null;
			//ノード変化がなければタスク継続とみなして一旦ループ抜ける
			while (current_ != node) {
				//実行前に現在のノードを退避
				node = current_;
				//実行して次のノードを入れる
				current_ = current_.Exec(entity);
				//巡回が終わったときはcurrentがnullになるのでメインに戻す
				if (current_ == null) {
					current_ = root_;
					break;
				}
			}
		}


	}


}
