using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Editor {
	public static class MethodUtil {
		/// <summary>
		/// 引数編集用のカスタム属性を取得する
		/// </summary>
		public static ArgAttribute[] CreateArgAttributeArray(System.Reflection.MethodInfo methodInfo) {
			//引数の属性を拾う
			Attribute[] attr = Attribute.GetCustomAttributes(methodInfo, typeof(ArgAttribute));
			ArgAttribute[] attributes = new ArgAttribute[attr.Length];

			//GetCustomAttributesしたときの配列の順番と引数の順番は一致しないので明示的に並び替える
			for (int i = 0; i < attr.Length; i++) {
				ArgAttribute temp = attr[i] as ArgAttribute;
				attributes[temp.Index] = temp;
			}

			for (int i = 0; i < attr.Length; i++) {
				Debug.Assert(attributes[i] != null, string.Format("method {0} arg index {1} is not found", methodInfo.Name, i));
			}

			return attributes;
		}
		/// <summary>
		/// デフォルト値配列を生成
		/// </summary>
		public static object[] CreateDefaultValueArray(ArgAttribute[] argAttributes) {
			object[] args = new object[argAttributes.Length];
			for (int i = 0; i < argAttributes.Length; i++) {
				args[argAttributes[i].Index] = argAttributes[i].Value;
			}
			return args;
		}


		/// <summary>
		/// 引数が列挙型か判定
		/// </summary>
		public static bool IsEnumArg(ArgAttribute[] argAttributes, int index) {
			if (index >= argAttributes.Length)
				return false;
			return argAttributes[index].Type.IsEnum;
		}
	}
}
