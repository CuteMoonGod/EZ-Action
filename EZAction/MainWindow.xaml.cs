using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Win32;

namespace TestAppWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	///

	public partial class MainWindow : Window
	{
		// Global flag to check if editing mode is active
		bool editing = false;
		bool fileOpen = false;

		string targetCache;
		string windowTitle_Base = "Easy Action for ArmA 3";
		string openedFile = "";

		public MainWindow()
		{
			//Set up basic settings
			if (Properties.Settings.Default.defaultSavePath == "")
			{
				Properties.Settings.Default.defaultSavePath = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "\\EZ-Action");
				Properties.Settings.Default.Save();
			}
			if (Properties.Settings.Default.profileDir == "") {
				Properties.Settings.Default.profileDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArmA 3";
				Properties.Settings.Default.Save();
			}

			InitializeComponent();
		}

		private void DelEvent_Button_Click(object sender, RoutedEventArgs e)
		{
			object selectedItem = eventList.SelectedItem;

			if (selectedItem != null)
			{
				aceEvent.eventList.RemoveAt(eventList.Items.IndexOf(selectedItem));

				eventList.Items.Remove(selectedItem);
			}
		}

		private void RegEvent_Button_Click(object sender, RoutedEventArgs e)
		{
			// Action Validation
			if (validateAction())
			{
				return;
			}

			string concated = String.Concat("Function: ", functionText.Text);
			int intDuration = Convert.ToInt32(durationText.Text);

			// When editing, delete previous index and insert action at previous index
			if (editing) {
				int indexCache = eventList.SelectedIndex;
				eventList.Items.RemoveAt(indexCache);

				eventList.Items.Insert(indexCache, concated);

				aceEvent.eventList[indexCache] = new aceEvent(indexCache, functionText.Text, actionText.Text, progressCheck.IsChecked, intDuration, targetText.Text, classCheck.IsChecked, eventLabelText.Text);

				targetText.Text = targetCache;
				targetCache = "";

				DeactiveEditingMode();
			}
			// When adding, can just straight add and create object
			else {
				int id = eventList.Items.Add(concated);

				aceEvent newEvent = new aceEvent(id, functionText.Text, actionText.Text, progressCheck.IsChecked, intDuration, targetText.Text, classCheck.IsChecked, eventLabelText.Text);
				aceEvent.eventList.Add(newEvent);
			}
			functionText.Text = "";

			classCheck.IsChecked = false;

			actionText.Text = "";
			//conditionText.Text = "";
			eventLabelText.Text = "";

			progressCheck.IsChecked = true;
			durationText.Text = "10";
		}


		private void EventList_MouseDoubleClick(object sender, RoutedEventArgs e)
		{
			ActivateEditingMode();
			try
			{
				Editing_Label.Visibility = Visibility.Visible;
				object selectedItem = eventList.SelectedItem;
				int index = eventList.Items.IndexOf(selectedItem);

				aceEvent editAction = aceEvent.eventList[index];

				//Disable the event list so only one thing can be edited at the same time
				eventList.IsEnabled = false;

				//If we're here, everything worked out, so let's get cracking
				targetCache = targetText.Text;

				targetText.Text = editAction.TargetEntity;
				classCheck.IsChecked = editAction.ClassCheck;

				functionText.Text = editAction.FunctionName;
				eventLabelText.Text = editAction.ActionLabel;

				progressCheck.IsChecked = editAction.ProgressBar;
				actionText.Text = editAction.DisplayText;
				durationText.Text = editAction.Duration.ToString();
			}
			// Catch OutOfRangeExceptions when the user clicks a non-existing item
			catch (System.ArgumentOutOfRangeException exception) {
				editing = false;
				Editing_Label.Visibility = Visibility.Hidden;
				return;
			}
		}

		private void ActivateEditingMode()
		{
			editing = true;
			Editing_Label.Visibility = Visibility.Visible;

			eventList.IsEnabled = false;
			Generate_Button.IsEnabled = false;
			Clipboard_Button.IsEnabled = false;
		}

		private void DeactiveEditingMode()
		{
			editing = false;
			Editing_Label.Visibility = Visibility.Hidden;

			eventList.IsEnabled = true;
			Generate_Button.IsEnabled = true;
			Clipboard_Button.IsEnabled = true;
		}

		private bool validateAction()
		{
			bool validationFail = false;

			if (targetText.Text == "")
			{
				validationFail = true;
				targetText.Background = Brushes.Salmon;
			}
			else
			{
				targetText.Background = Brushes.Transparent;
			}

			if (functionText.Text == "")
			{
				validationFail = true;
				functionText.Background = Brushes.Salmon;
			}
			else
			{
				functionText.Background = Brushes.Transparent;
			}

			if (actionText.Text == "")
			{
				validationFail = true;
				actionText.Background = Brushes.Salmon;
			}
			else
			{
				actionText.Background = Brushes.Transparent;
			}

			if (eventLabelText.Text == "")
			{
				validationFail = true;
				eventLabelText.Background = Brushes.Salmon;
			}
			else
			{
				eventLabelText.Background = Brushes.Transparent;
			}

			if (durationText.Text == "")
			{
				validationFail = true;
				durationText.Background = Brushes.Salmon;
			}
			else
			{
				durationText.Background = Brushes.Transparent;
			}

			return validationFail;
		}

		private void Generate_Button_Click(object sender, RoutedEventArgs e)
		{
			//Return codes:
			//0 - success
			//1 - user abort
			//2 - nothing to declare
			//3 - other error
			//int varBuild = variableContent.BuildSQF();
			int evtBuild = aceEvent.BuildSQF();

			new errorPrint(evtBuild);
		}

		private void ArmaDir_Button_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(Properties.Settings.Default.profileDir);
		}

		private void SettingsButton_Click(object sender, RoutedEventArgs e)
		{
			Window1 win1 = new Window1();
			win1.Show();
		}

		private void ExportDir_Button_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(Properties.Settings.Default.defaultSavePath);
		}

		private void OpenVarPage_Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Clipboard_Button_Click(object sender, RoutedEventArgs e)
		{
			aceEvent.BuildToClipboard();
			MessageBox.Show("Text copied to clipboard!");
		}

		private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void New_File_Click(object sender, RoutedEventArgs e)
		{
			ResetWindow();
		}

		private void ResetWindow()
		{
			this.Title = windowTitle_Base;
			openedFile = "";

			editing = false;
			fileOpen = false;

			DeactiveEditingMode();

			aceEvent.ResetList();

			eventList.Items.Clear();

			targetText.Text = "";
			functionText.Text = "";
			eventLabelText.Text = "";
			eventLabelText.Text = "";

			durationText.Text = "10";

			classCheck.IsChecked = false;
			progressCheck.IsChecked = true;
		}

		private void OpenFile_Button_Click(object sender, RoutedEventArgs e)
		{
			//Flag to indicate the program ran
			bool ran = false;
			bool metaImported = false;
			//bool actionImported = false;
			bool errorAction = false;

			int index = 0;

			

			OpenFileDialog file = new OpenFileDialog
			{
				Filter = "EZ-Action Projects (*.ezp)|*.ezp",
				InitialDirectory = Properties.Settings.Default.defaultSavePath
			};

			if (file.ShowDialog() == true)
			{
				ResetWindow();

				fileOpen = true;
				openedFile = file.FileName;

				this.Title = SetNewTitle(openedFile);

				StreamReader readingFile = new StreamReader(file.FileName);

				string line;
				bool first = true; 
				while ((line = readingFile.ReadLine()) != null)
				{
					ran = true;

					if (first)
					{ 
						metadata data = JsonSerializer.Deserialize<metadata>(line);

						if (!data.validateSelf())
						{
							//metaImported = true;
							MessageBox.Show("The metadata couldn't be loaded!");
						}
						//else
						//{
							
						//}
						
						first = false;
					}
					else
					{
						aceEvent deserializedEvent = JsonSerializer.Deserialize<TestAppWPF.aceEvent>(line);
						
						if (deserializedEvent.ValidateSelf())
						{
							//actionImported = true;

							aceEvent.eventList.Add(deserializedEvent);

							string concated = String.Concat("Function: ", deserializedEvent.FunctionName);
							eventList.Items.Add(concated);
						}
						else
						{
							MessageBox.Show("Action " + index + " was incorrect and couldn't be imported.");
						}
					}

					index++;
				}

				readingFile.Close();

				if (errorAction)
				{
					MessageBox.Show("At least one error occured during the import of the actions.");
				}

				if (!ran)
				{
					MessageBox.Show("The file was empty or couldn't be loaded!");
				}
			}
		}

		private void Save_Button_Click(object sender, RoutedEventArgs e)
		{
			if (fileOpen)
			{
				SaveProject();
			}
			else
			{
				SaveAs();
			}
		}

		private void SaveAs_Button_Click(object sender, RoutedEventArgs e)
		{
			SaveAs();
		}

		private void SaveProject()
		{
			string json = GetJSONs();
			string file = openedFile;

			File.WriteAllText(file, json);
		}

		private void SaveAs()
		{
			string json = GetJSONs();
			string file;

			SaveFileDialog saveDialog = new SaveFileDialog
			{
				AddExtension = true,
				DefaultExt = ".ezp",
				Filter = "EZ-Action Projects (*.ezp)|*.ezp",
				InitialDirectory = Properties.Settings.Default.defaultSavePath
			};

			if (saveDialog.ShowDialog() == true)
			{
				file = saveDialog.FileName;

				this.Title = SetNewTitle(file);

				openedFile = file;
				fileOpen = true;

				File.WriteAllText(file, json);
			}
		}

		private string SetNewTitle(string file)
		{
			return String.Concat(windowTitle_Base, ": ", file);
		}

		private string GetJSONs()
		{
			metadata Data = new metadata();
			String Data_JSON = Data.ExportMetadata();

			string actions = aceEvent.ExportJSONString();

			return String.Concat(Data_JSON, Environment.NewLine, actions);
		}
	}
}


/* The officially sanctioned graveyard
 * Stuff that's not implemented but will be (mostly variable stuff) goes here
 *---------------------------------------------------------------------------*
 private void VarDel_Button_Click(object sender, RoutedEventArgs e)
		{
			object selectedItem = variableList.SelectedItem;

			if (selectedItem != null)
			{
				variableContent.v ariableList.RemoveAt(variableList.Items.IndexOf(selectedItem));

				variableList.Items.Remove(selectedItem);
			}
		}
 
 		private void AddVar_Button_Click(object sender, RoutedEventArgs e)
{
	if (variableText != null)
	{
		string concat = String.Concat("Variable: ", variableText.Text, "; Value: ", variableValue.Text);
		variableList.Items.Add(concat);

		variableContent var = new variableContent(variableText.Text, variableValue.Text);
		variableContent.variableList.Add(var);

		variableText.Text = "";
		variableValue.Text = "";
	}

*/