using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using Microsoft.Win32;

namespace TestAppWPF
{
	[Serializable]
	class aceEvent
	{
		//Global Variables
		public static List<aceEvent> eventList = new List<aceEvent>();
		public static string file;

		private int id;

		private string functionName;
		private string displayText;

		private string actionLabel;

		private bool progressBar;
		private int duration;

		private string targetEntity;
		private bool classCheck;


		public string FunctionName
		{
			get { return functionName; }
			set { functionName = value; }
		}
		public string DisplayText
		{
			get { return displayText; }
			set { displayText = value; }
		}

		public string ActionLabel
		{
			get { return actionLabel; }
			set { actionLabel = value; }
		}

		public bool ProgressBar
		{
			get { return progressBar; }
			set { progressBar = value; }
		}
		public int Duration
		{
			get { return duration; }
			set { duration = value; }
		}

		public string TargetEntity
		{
			get { return targetEntity; }
			set { targetEntity = value; }
		}
		public bool ClassCheck
		{
			get { return classCheck; }
			set { classCheck = value; }
		}

		//Test default
		public aceEvent() {

		}

		public aceEvent(int impID, string impFunction, string impDisplay, bool? impProgress, int impDuration, string impTarget, bool? classBox, string impLabel)
		{
			id = impID;

			functionName = impFunction;
			displayText = impDisplay;
			actionLabel = impLabel;

			progressBar = Convert.ToBoolean(impProgress);
			duration = impDuration;

			targetEntity = impTarget;
			classCheck = Convert.ToBoolean(classBox);
		}

		public static void ResetList()
		{
			eventList.Clear();
		}

		public bool ValidateSelf()
		{
			if (this.functionName == null ||
				this.actionLabel == null ||
				this.targetEntity == null ||
				(this.progressBar == true &&
					(this.duration == 0 ||
					this.displayText == null
					)
				)
			)
			{
				// Something didn't get set correctly
				return false;
			}

			else {
				return true;
			}
		}

		public static string ExportJSONString() {
			string json = "";

			foreach(aceEvent element in eventList)
			{
				json = ConcatWithNewLine(json, JsonSerializer.Serialize(element));
			}

			return json;
		}

		public static int BuildSQF()
		{
			StringBuilder buildBuddy = new StringBuilder();

			if (eventList.Count <= 0)
			{
				//MessageBox.Show("No actions to declare. Skipping step.");
				return 2;
			}

			//First step: declare save folder and filename
			SaveFileDialog saveDialog = new SaveFileDialog
			{
				Filter = "ArmA 3 Scripting Files (*.sqf)|*.sqf",
				FileName = "output_actions.sqf",
				InitialDirectory = Properties.Settings.Default.defaultSavePath
			};

			if (saveDialog.ShowDialog() == true)
			{
				//Cleanup file before writing
				File.Delete(saveDialog.FileName);

				file = saveDialog.FileName;
			}
			else
			{
				return 1;
			}

			string buildReturn = BuildAll();

			//string targets = DeclareTargets();
			//string functions = BuildFunctions();
			//string actions = CreateActions();
			//string adds = AddActions();

			//buildBuddy.Append(targets);
			//buildBuddy.Append(functions);
			//buildBuddy.Append(actions);
			//buildBuddy.Append(adds);

			File.AppendAllText(file, buildBuddy.ToString());

			return 0;
		}

		public static int BuildToClipboard()
		{
			StringBuilder buildBuddy = new StringBuilder();

			if (eventList.Count <= 0)
			{
				//MessageBox.Show("No actions to declare. Skipping step.");
				return 1;
			}

			string buildReturn = BuildAll();

			//string targets = DeclareTargets();
			//string functions = BuildFunctions();
			//string actions = CreateActions();
			//string adds = AddActions();

			//buildBuddy.Append(targets);
			//buildBuddy.Append(functions);
			//buildBuddy.Append(actions);
			//buildBuddy.Append(adds);

			try
			{
				System.Windows.Clipboard.SetText(buildReturn);
			}
			catch(Exception e)
			{
				return 2;
			}

			return 0;
		}

		// Standard for only base and one thing to add
		public static string ConcatWithNewLine(string baseString, string toAdd)
		{
			return String.Concat(baseString, toAdd, Environment.NewLine);
		}

		// Overloading for n number of strings
		public static string ConcatWithNewLine(string baseString, string[] toAdd)
		{
			string returnString;

			returnString = baseString;

			foreach (string target in toAdd) {
				returnString = String.Concat(returnString, target);
			}

			return String.Concat(returnString, Environment.NewLine);
		}

		// To be used only for LineBreaks
		public static string ConcatNewLine(string baseString)
		{
			return String.Concat(baseString, Environment.NewLine);
		}

		private static string DeclareTargets()
		{
			StringBuilder builder = new StringBuilder();
			string concat;

			//concat = "//*--------------------------------------------------------------------------------*";
			//concat = ConcatNewLine(concat);
			//concat = ConcatWithNewLine(concat, new[] { "//| Generated by EzACT RC-3" });
			//concat = ConcatWithNewLine(concat, new[] { "//| Total actions in this script:", eventList.Count.ToString() });
			//concat = ConcatWithNewLine(concat, "//*--------------------------------------------------------------------------------*");
			//concat = ConcatNewLine(concat);
			//concat = ConcatNewLine(concat);
			
			builder.AppendLine("//*--------------------------------------------------------------------------------*");
			builder.AppendLine("//| Generated by EzACT RC-3");
			builder.Append("//| Total actions in this script: ");
			builder.AppendLine(eventList.Count.ToString());
			builder.AppendLine();
			builder.AppendLine();

			/*concat = ConcatWithNewLine(concat, "// Add your behavior here; Keep in mind, they have to be code blocks!");
			foreach (aceEvent element in eventList)
			{
				int index = eventList.IndexOf(element) + 1;
				string behavior = String.Concat("_", element.functionName, index, "Behavior");

				concat = ConcatWithNewLine(concat, new[] { behavior, " = { hint \"Hello World!\"};" });
			}
			concat = ConcatNewLine(concat);*/

			//concat = ConcatWithNewLine(concat, "// Change your conditions here; Keep in mind, they have to be code blocks!");
			builder.AppendLine("// Change your conditions here; Keep in mind, they have to be code blocks!");
			foreach (aceEvent element in eventList)
			{
				int index = eventList.IndexOf(element) + 1;
				string condition = String.Concat("_", element.functionName, "Condition");

				//concat = ConcatWithNewLine(concat, new[] { condition, " = { true };" });

				builder.Append(condition);
				builder.AppendLine(" = { true };");
			}
			//concat = ConcatNewLine(concat);
			//concat = ConcatWithNewLine(concat, "//Target Declarations");

			builder.AppendLine("");
			builder.AppendLine("//Target Declarations");

			foreach (aceEvent element in eventList)
			{
				int index = eventList.IndexOf(element) + 1;
				string objectNum = String.Concat("_", element.functionName, "Target");

				concat = String.Concat(new[] { objectNum, " = ", element.targetEntity, ";" });

				builder.AppendLine(concat);
			}

			builder.AppendLine();
			builder.AppendLine();

			return builder.ToString();
		}


		private static string BuildFunctions()
		{
			// Initialize cause else C# cries
			StringBuilder builder = new StringBuilder();
			string concat = "";

			foreach (aceEvent element in eventList)
			{

				concat = String.Concat(new[] { "_", element.functionName, " = {" });

				builder.AppendLine(concat);

				if (element.progressBar)
				{
					concat = String.Concat(new[] { "[", element.duration.ToString(), ", [], {" });
					builder.AppendLine(concat);
				}


				//concat = ConcatWithNewLine(concat, "//Your code goes here!");
				//concat = ConcatNewLine(concat);
				//concat = ConcatNewLine(concat);
				//concat = ConcatNewLine(concat);

				builder.AppendLine("//Your code goes here!");
				builder.AppendLine("");
				builder.AppendLine("");
				builder.AppendLine("");

				if (element.progressBar) {
					//concat = String.Concat(new[] { "}, {}, \"", element.displayText, "\"] call ace_common_fnc_progressBar;" });
					builder.AppendLine(concat);
				}

				//concat = ConcatWithNewLine(concat, "};");
				//concat = ConcatNewLine(concat);
				//concat = ConcatNewLine(concat);

				builder.AppendLine("};");
				builder.AppendLine("");
				builder.AppendLine("");
			}

			return builder.ToString();
		}


		private static string CreateActions()
		{
			string concat;
			StringBuilder builder = new StringBuilder();

			//concat = "// Creating the actions; the condition defaults to true, so don't forget to adjust the conditions in the actions!";
			//concat = ConcatNewLine(concat);

			builder.AppendLine("// Creating the actions; the condition defaults to true, so don't forget to adjust the conditions in the actions!");
			builder.AppendLine("");

			foreach (aceEvent element in eventList)
			{
				int index = eventList.IndexOf(element) + 1;

				string condition = String.Concat("_", element.functionName, "Condition");

				concat = String.Concat(new[] { "_generatedAction", index.ToString(), " = [\"missionaction", index.ToString(), "\", \"", element.actionLabel, "\", \"\", _", element.functionName, ", ", condition ,"] call ace_interact_menu_fnc_createAction;" });
				builder.AppendLine(concat);

			}

			//concat = ConcatNewLine(concat);
			//concat = ConcatNewLine(concat);

			builder.AppendLine("");
			builder.AppendLine("");

			return builder.ToString();
		}

		private static string AddActions()
		{
			string concat;
			StringBuilder builder = new StringBuilder();

			//concat = "// Adding the created actions to the chosen objects";
			//concat = ConcatNewLine(concat);

			builder.AppendLine("// Adding the created actions to the chosen objects");
			builder.AppendLine("");

			foreach (aceEvent element in eventList)
			{
				int index = eventList.IndexOf(element) + 1;
				string objectNum = String.Concat("_", element.functionName, "Target");


				if (element.classCheck == true)
				{
					concat = String.Concat("[typeOf ", objectNum, ", 0, [\"ACE_MainActions\"], _generatedAction", index, "] call ace_interact_menu_fnc_addActionToClass;");
					builder.AppendLine(concat);
				}
				else
				{
					concat = String.Concat("[", objectNum, ", 0, [\"ACE_MainActions\"], _generatedAction", index, "] call ace_interact_menu_fnc_addActionToObject;");
					builder.AppendLine(concat);
				}
				//concat = ConcatNewLine(concat);
				builder.AppendLine("");
			}
			return builder.ToString();
		}

		private static string BuildAll() {
			StringBuilder buildBuddy = new StringBuilder();
			
			string targets = DeclareTargets();
			string functions = BuildFunctions();
			string actions = CreateActions();
			string adds = AddActions();

			buildBuddy.Append(targets);
			buildBuddy.Append(functions);
			buildBuddy.Append(actions);
			buildBuddy.Append(adds);

			return buildBuddy.ToString();
		}
	}

	[Serializable]
	class metadata {
		//string projectName;
		private DateTime lastSave;

		public DateTime LastSave
		{
			get { return lastSave; }
			set { lastSave = value; }
		}

		public metadata() {
		}

		public bool ValidateSelf()
		{
			if (lastSave.ToString() == "01.01.0001 00:00:00")
			{
				return false;
			}
			else return true;
		}

		public string ExportMetadata() {
			lastSave = DateTime.Now;
			return JsonSerializer.Serialize(this);
		}
	}



	//class variableContent {
	//	public static List<variableContent> variableList = new List<variableContent>();

	//	private string name;
	//	private string value;

	//	public variableContent(string varName, string varVal) {
	//		name = varName;
	//		value = varVal;
	//	}

	//	public static int BuildSQF()
	//	{
	//		if (variableList.Count <= 0) {
	//			MessageBox.Show("No variables to declare. Skipping step.");
	//			return 2;
	//		}

	//		SaveFileDialog saveDialog = new SaveFileDialog
	//		{
	//			Filter = "ArmA 3 Scripting Files (*.sqf)|*.sqf",
	//			FileName = "output_variables.sqf",
	//			InitialDirectory = Properties.Settings.Default.defaultSavePath
	//		};

	//		if (saveDialog.ShowDialog() == true)
	//		{
	//			//Cleanup file before writing
	//			File.Delete(saveDialog.FileName);

	//			foreach (variableContent element in variableList)
	//			{
	//				string line;
	//				//If it's only numbers OR bool values, treat as such
	//				if (Regex.IsMatch(element.value, "^[0-9]*$") || element.value == "true" || element.value == "false")
	//				{
	//					line = String.Concat(element.name, " = ", element.value, "; publicVariable \"", element.name, "\";");
	//				}
	//				//Else treat as foreign
	//				else
	//				{
	//					line = String.Concat(element.name, " = \"", element.value, "\"; publicVariable \"", element.name, "\";");
	//				}
	//			}
	//			return 0;
	//		}
	//		else {
	//			return 1;
	//		}
	//	}
		
	//}

	class errorPrint
	{

		public errorPrint(int evtBuild) {
			string displayString = "";


			displayString = String.Concat(displayString, Environment.NewLine);

			switch (evtBuild){
				case 0:
					displayString = String.Concat(displayString, "Actions successfully exported");
					break;
				case 1:
					displayString = String.Concat(displayString, "Action write aborted by user");
					break;
				case 2:
					displayString = String.Concat(displayString, "No actions to write");
					break;
				case 3:
					displayString = String.Concat(displayString, "An unspecified error occured while writing actions!");
					break;
			}

			MessageBox.Show(displayString);
		}
	}
}


/* Be warned all ye who enter
 * For this be the graveyard of code
 * It lieth in rest, as is bestowed
 * Free, from the demands of it's inventor
 * ...
 * Yeah, not fully free as it turns out, most of this is just taken out until I redo the variable panel

//switch (varBuild) {
//	case 0:
//		displayString = "Variables successfully exported";
//		break;
//	case 1:
//		displayString = "Variable write aborted by user";
//		break;
//	case 2:
//		displayString = "No variables to write";
//		break;
//	case 3:
//		displayString = "An unspecified error occured while writing variables!";
//		break;
//}

 
 */