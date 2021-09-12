using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace TestAppWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			//Set up basic settings
			if (Properties.Settings.Default.defaultSavePath == "")
			{
				Properties.Settings.Default.defaultSavePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				Properties.Settings.Default.Save();
			}
			if (Properties.Settings.Default.profileDir == "") {
				Properties.Settings.Default.profileDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArmA 3";
				Properties.Settings.Default.Save();
			}

			InitializeComponent();
		}

		private void AddVar_Button_Click(object sender, RoutedEventArgs e)
		{
			/*if (variableText != null)
			{
				string concat = String.Concat("Variable: ", variableText.Text, "; Value: ", variableValue.Text);
				variableList.Items.Add(concat);

				variableContent var = new variableContent(variableText.Text, variableValue.Text);
				variableContent.variableList.Add(var);

				variableText.Text = "";
				variableValue.Text = "";
			}*/
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
			eventList.Items.Add(concated);

			int intDuration = Convert.ToInt32(durationText.Text);

			aceEvent newEvent = new aceEvent(functionText.Text, actionText.Text, condition.IsChecked, progressCheck.IsChecked, intDuration, targetText.Text, classCheck.IsChecked, eventLabelText.Text);
			aceEvent.eventList.Add(newEvent);

			functionText.Text = "";


			actionText.Text = "";
			//conditionText.Text = "";
			eventLabelText.Text = "";

			progressCheck.IsChecked = true;
			durationText.Text = "10";
		}

		private void EventList_MouseDoubleClick(object sender, RoutedEventArgs e)
		{
			object selectedItem = eventList.SelectedItem;
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

		private void VarDel_Button_Click(object sender, RoutedEventArgs e)
		{
			/*object selectedItem = variableList.SelectedItem;

			if (selectedItem != null)
			{
				variableContent.variableList.RemoveAt(variableList.Items.IndexOf(selectedItem));

				variableList.Items.Remove(selectedItem);
			}*/
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
	}
}
