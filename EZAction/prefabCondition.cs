using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppWPF
{
	[Serializable]
	public class PrefabCondition
	{
		public static List<PrefabCondition> conditionList = new List<PrefabCondition>();

		private string conditionName;
		private string conditionText;

		public string ConditionName {
			get { return conditionName; }
			set { conditionName = value; }
		}
		public string ConditionText {
			get { return conditionText; }
			set { conditionText = value; }
		}

		static PrefabCondition() {

		}

		public PrefabCondition() {
		
		}

		public PrefabCondition(string name, string text) {
			conditionName = name;
			conditionText = text;
			conditionList.Add(this);
		}

		public bool ValidateSelf() {
			return	conditionName != "" && conditionName != null && 
					conditionText != "" && conditionText != null;
		}
	}
}
