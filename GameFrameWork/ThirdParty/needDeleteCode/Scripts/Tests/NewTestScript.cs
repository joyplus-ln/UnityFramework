 using System.Collections;
 using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
 using UnityEngine;
 using UnityEngine.TestTools;

 namespace Tests
 {
     public class NewTestScript
     {
         // A Test behaves as an ordinary method
         [Test]
         public void NewTestScriptSimplePasses()
         {
 			// Use the Assert class to test conditions


 		}

		[Test]
		public void StringCompare()
		{
			List<string> a = new List<string> { "a", "m", "z", "xa" };
			a.Sort(delegate (string x, string y) {
				return string.Compare(x, y, System.StringComparison.Ordinal);
			});
			Assert.AreEqual("z", a.Last());
		}

		[TestCase("01001001", "01")]
 		[TestCase(null,null)]
 		[TestCase("0",null)]
 		[TestCase("0800","08")]
 		//[TestCase(01010, null)]
 		public void Type(string aId, string result) {
 			string type = GetType(aId);
 			Assert.AreEqual(type, result, "is {0}", type);
 		}

 		[TestCase("01001001", "001")]
 		[TestCase("0",null)]
 		[TestCase("0123",null)]
 		[TestCase("01234","234")]
 		[TestCase("012345","234")]
 		[TestCase("080020","002")]
 		[TestCase(null,null)]
 		public void ConditionRegion(string aId, string result)
 		{
 			string aresult = GetConditionRegion(aId);
 			Assert.AreEqual(aresult, result);
 		}

 		[TestCase("01001001", "001")]
 		[TestCase("0", null)]
 		[TestCase("0123", null)]
 		[TestCase("01234", null)]
 		[TestCase("012345", null)]
 		[TestCase("0800200", null)]
 		[TestCase("08002007", "007")]
 		[TestCase(null, null)]
 		public void Number(string aId,string result) {
 			string aresult = GetNumber(aId);
 			Assert.AreEqual(aresult, result);
 		}

 		[TestCase(1,10,0,false)]
 		[TestCase(10,1,10,false)]
 		[TestCase(11,1,10,false)]
 		[TestCase(1,1,2,true)]
 		[TestCase(100000,2,-1,true)]
 		public void InRegion(int a,int b,int e,bool c)
 		{
 			bool flag = IsNumberInRegion(a,b,e);
 			Assert.AreEqual(c, flag);
 		}

 		[Test]
 		public void TestOToFloat()
 		{
 			object a = 10;
 			int b = (int)a;
 			float c = (float)b;
 		}

 		[TestCase(10,20,30,40)]
 		[TestCase(0,0,0,0)]
 		[TestCase(1,0,0,0)]
 		public void TaskId(int a,int b,int c,int d) {
 			Dictionary<string, int> dic = new Dictionary<string, int> {
 				{"10",a},
 				{"11",b},
 				{"12",c},
 				{"13",d}
 			};
 			string result = GetTaskId(dic, null);
 			Assert.IsFalse(string.IsNullOrEmpty(result));
 		}
 		[Test]
 		public void TaskId2() {
 			string result = GetTaskId(null, null);
 			Assert.IsTrue(string.IsNullOrEmpty(result));

 			Dictionary<string, int> dic = new Dictionary<string, int> { };
 			Assert.IsTrue(string.IsNullOrEmpty(result));
 		}

 		[TestCase("123|456", 2)]
 		[TestCase("abc|333", 1)]
 		[TestCase(null,0)]
 		public void Values1(string str, int num)
 		{
 			var Result = GetValues1(str);
 			Assert.AreEqual(Result.Count, num);
 		}
		[Test]
		public void TestDateTime()
		{

			string s = "2019-06-05 18:17  ";
			s = s.Replace("：", ":");
			System.DateTime dateTime = System.Convert.ToDateTime(s);
			Assert.AreEqual(18, dateTime.Hour);
			dateTime = System.Convert.ToDateTime("2019-06-05 1:17");
			Assert.AreEqual(1, dateTime.Hour);
			System.DateTime date19700101 = new System.DateTime(1970, 01, 01, 0, 0, 0);
			System.TimeSpan span = dateTime.ToUniversalTime() - date19700101;
			double x = span.TotalSeconds;
			dateTime = System.Convert.ToDateTime("2019/6/5  1:17:00");
			dateTime = System.Convert.ToDateTime("2019/6/5  1:17:00");
			dateTime = System.Convert.ToDateTime("2019/6/5     1:17:00");
			UnityEngine.Debug.Log("x----- " + x);
			dateTime = System.Convert.ToDateTime("2019/9/3 00:00:00");
			System.DateTime d2 = System.DateTime.Now;
			span = d2  - dateTime;
			d2 = d2.ToUniversalTime();
			span = d2 - dateTime;
			Debug.Log("");
		}

		[Test]
		public void TestListSubRange() {
			List<int> l = new List<int>() { 1, 2, 3, 4, 5 };
			l.GetRange(0, 2);
			//if (2 + 3 < l.Count)
			var r = l.GetRange(2, 3);
			Assert.IsTrue(r != null);
		}

		[Test]
		public void TestDicKeyOrder() {
			Dictionary<int, string> v = new Dictionary<int, string> {
				{10,"a"},
				{1,"aa"},
				{2,"b"},
				{40,"c"}
			};
			var l = new List<int>(v.Keys);
			l.Sort();
			foreach (var m in l) {
				Debug.Log(m);
			}
			Assert.AreEqual(2, l[1]);
		}

		[Test]
		public void TestSplitStringToInt() {
			var x = "1|2|3|";
			var m = x.Split('|').Select(int.Parse).ToList();
			Assert.AreEqual(0, m.Last());
		}

		[Test]
		public void TestCharToInt() {
			char a = 'a';
			a = (char)(a + 4);
			Debug.Log("a = " + a.ToString());
			Assert.AreEqual('e', a);
			int x = a - 'a';
			Debug.Log("x = " + x);
			Assert.AreEqual(4, x);
		}

		[Test]
		public void TestABName() {
			var s = "shared/assets/assetspackage/custom/textures/Mind_effect_png.ab";
			var r = GetBundle_Name(s);
			Debug.LogFormat("{0}--{1}", r[0], r[1]);
			Assert.AreEqual(r[0], "png_Mind_effect");
			Assert.AreEqual(r[1], "Mind_effect.png");
			s = "shaderS_14.ab";
			r = GetBundle_Name(s);
			Assert.AreEqual(r[0], "shaderS_14");
			Assert.AreEqual(r[1], "shaderS_14");

			Assert.AreEqual(true, Regex.IsMatch("14", "^[0-9]{0,}$"));
			Assert.AreEqual(false, Regex.IsMatch("x14", "^[0-9]{0,}$"));
		}

		#region method

		static string[] GetBundle_Name(string bundleName)
		{
			var name = bundleName;
			bool flag = false;//斜杠标志
			if (name.Contains('/')) {
				flag = true;
				string[] names = name.Split('/');
				name = names[names.Length - 1];
			}

			if (name.Contains('.')) {
				string[] type_name = name.Split('.');
				var x = type_name[0].Split('_');
				if (Regex.IsMatch(x[x.Length - 1], "^[0-9]{0,}$")) {
					string[] back = new[] { type_name[0], type_name[0] };
					return back;
				} else {
					List<string> s = new List<string>();
					for (int i = 0; i < x.Length - 1; i++) {
						s.Add(x[i]);
					}
					var assetName = string.Join("_", s);
					string[] back = new[] { string.Format("{0}_{1}", x[x.Length - 1], assetName), string.Format("{0}.{1}", assetName, x[x.Length - 1]) };
					return back;
				}
			} else {
				if (flag) {
					string[] back = new[] { string.Format("{0}_{1}", "unknown", name), name };
					return back;
				} else {
					string[] back = new[] { string.Format("{0}_{1}", "bundle", bundleName), bundleName };
					return back;
				}
			}
		}

		public List<int> GetValues1(string astr)
 		{
 			List<int> result = new List<int>();
 			if (string.IsNullOrEmpty(astr)) return result;
 			string[] x = astr.Split('|');
 			for (int i=0;i<x.Length;i++) {
 				int m;
 				if (int.TryParse(x[i], out m))
 					result.Add(m);
 			}
 			return result;
 		}

 		private string GetTaskId(Dictionary<string, int> objs, string number)
 		{
 			string result = "";
 			if (objs == null) return result;
 			int sum = 0;
 			foreach (var kp in objs) {
 				sum += kp.Value; 
 			}
 			if (sum == 0) {
 				foreach(var kp in objs) {
 					return kp.Key;
 				}
 			}
 			int aRand = Random.Range(0, sum);
 			foreach (var kp in objs) { 
 				if (aRand>=0 && aRand<kp.Value) {
 					result = kp.Key;
 					break;
 				} else {
 					aRand -= kp.Value;
 				}
 			}
 			return result;	
 		}

 		public bool IsNumberInRegion(int currentLevel, int level_Min, int level_Max)
 		{
 			bool result = false;
 			if (currentLevel >= level_Min) { 
 				if (level_Max == -1) {
 					result = true;
 				} else if (currentLevel<level_Max){
 					result = true;
 				}
 			}
 			return result;
 		}

 		public string GetType(string aId)
 		{
 			if (!(aId is string)) return null;
 			if (string.IsNullOrEmpty(aId)) return null;
 			if (aId.Length < 2) return null;
 			return aId.Substring(0, 2);
 		}

 		public string GetConditionRegion(string aId)
 		{
 			if (string.IsNullOrEmpty(aId)) return null;
 			if (aId.Length < 5) return null;
 			return aId.Substring(2,3);
 		}
 		public string GetNumber(string aId)
 		{
 			if (string.IsNullOrEmpty(aId)) return null;
 			if (aId.Length < 8) return null;
 			return aId.Substring(5,3);
 		}
 		#endregion

 		// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
 		// `yield return null;` to skip a frame.
 		[UnityTest]
         public IEnumerator NewTestScriptWithEnumeratorPasses()
         {
             // Use the Assert class to test conditions.
             // Use yield to skip a frame.
             yield return null;
         }
     }
 }
