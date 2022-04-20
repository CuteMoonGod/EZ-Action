using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppWPF
{
	[Serializable]
	public class PrefabSnippet
	{
		public static List<PrefabSnippet> snippetsList = new List<PrefabSnippet>();

		private string snippetName;
		private string snippetText;

		public string SnippetName
		{
			get { return snippetName; }
			set { snippetName = value; }
		}
		public string SnippetText
		{
			get { return snippetText; }
			set { snippetText = value; }
		}


		public PrefabSnippet() {
			
		}

		public PrefabSnippet(string name, string text) {
			snippetName = name;
			snippetText = text;
			snippetsList.Add(this);
		}

		public bool ValidateSelf()
		{
			return	snippetName != "" && snippetName != null &&
					snippetText != "" && snippetText != null;
		}
	}
}
