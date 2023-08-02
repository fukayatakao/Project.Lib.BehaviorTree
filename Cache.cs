using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Lib {
	public partial class BehaviorTree<T> {
		public delegate bool EvaluateMethod(T entity, object[] args);
		public delegate void OrderMethod(T entity, object[] args);

		class Cache {

			//評価用関数の一覧
			Dictionary<string, EvaluateMethod> evaluateMethods_;
			public Dictionary<string, EvaluateMethod> EvaluateMethods {get{return evaluateMethods_;} }
			//指示用関数の一覧
			Dictionary<string, OrderMethod> orderMethods_;
			public Dictionary<string, OrderMethod> OrderMethods { get { return orderMethods_; } }

			/// <summary>
			/// クラス内にある関数のキャッシュを作る
			/// </summary>
			public Cache(Type evaluateClass, Type orderClass) {
				evaluateMethods_ = new Dictionary<string, EvaluateMethod>();
				orderMethods_ = new Dictionary<string, OrderMethod>();
				//この辺は重くなりそうなとAIすべてで共通なので全体で１回だけ行って結果を保持する
				{
					System.Reflection.MethodInfo[] methods = evaluateClass.GetMethods();
					for (int i = 0, max = methods.Length; i < max; i++) {
						System.Reflection.ParameterInfo[] param = methods[i].GetParameters();
						if (param.Length == 2 && param[0].ParameterType == typeof(T) && param[1].ParameterType == typeof(object[]) && methods[i].ReturnType == typeof(bool)) {
							EvaluateMethod evaluate = (EvaluateMethod)Delegate.CreateDelegate(typeof(EvaluateMethod), methods[i]);
							evaluateMethods_.Add(methods[i].Name, evaluate);
						}
					}
				}
				{
					System.Reflection.MethodInfo[] methods = orderClass.GetMethods();
					for (int i = 0, max = methods.Length; i < max; i++) {
						System.Reflection.ParameterInfo[] param = methods[i].GetParameters();
						if (param.Length == 2 && param[0].ParameterType == typeof(T) && param[1].ParameterType == typeof(object[]) && methods[i].ReturnType == typeof(void)) {
							OrderMethod order = (OrderMethod)Delegate.CreateDelegate(typeof(OrderMethod), methods[i]);
							orderMethods_.Add(methods[i].Name, order);
						}
					}
				}
			}
		}

	}

}
