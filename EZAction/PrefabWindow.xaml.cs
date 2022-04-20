using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestAppWPF
{
	/// <summary>
	/// Interaction logic for PrefabWindow.xaml
	/// </summary>
	public partial class PrefabWindow : Window
	{
		MainWindow parentCache;
		AceEvent selectedEvent;
		int selectedIndex;

		public PrefabWindow(MainWindow parent, AceEvent aceEvent, int index)
		{
			InitializeComponent();

			parentCache = parent;
			selectedEvent = aceEvent;
			selectedIndex = index;

			parentCache.RegisterChange();

			this.Title = aceEvent.FunctionName;

			foreach (PrefabCondition condition in PrefabCondition.conditionList)
			{
				ConditionBox.Items.Add(condition.ConditionName);
			}

			foreach (PrefabSnippet snippet in PrefabSnippet.snippetsList)
			{
				SnippetBox.Items.Add(snippet.SnippetName);
			}

			PopulateCondition();
			PopulateSnippets();
		}

		private void PopulateCondition()
		{
			if (selectedEvent.AppliedCondition != null)
			{
				ConditionBox.SelectedItem = selectedEvent.AppliedCondition.ConditionName;
			}
		}

		private void PopulateSnippets()
		{
			foreach (PrefabSnippet snippet in selectedEvent.prefabSnippets)
			{
				SnippetList.Items.Add(snippet.SnippetName);
			}
			//SnippetBox.Items.Add(SnippetList.FindName(selectedEvent.prefabSnippets.);
			//throw new NotImplementedException();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (ConditionBox.SelectedIndex != -1) {
				selectedEvent.AppliedCondition = PrefabCondition.conditionList[ConditionBox.SelectedIndex];
			}
			AceEvent.eventList[selectedIndex] = selectedEvent;

			//Reactivate the main window
			parentCache.IsEnabled = true;
		}

		
		private void AddPrefabButton_Click(object sender, RoutedEventArgs e)
		{
			selectedEvent.prefabSnippets.Add(PrefabSnippet.snippetsList[SnippetBox.SelectedIndex]);
			SnippetList.Items.Add(SnippetBox.Items[SnippetBox.SelectedIndex]);
			DetermineDelete();
		}

		private void RemovePrefabButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				selectedEvent.prefabSnippets.RemoveAt(SnippetList.SelectedIndex);
				SnippetList.Items.RemoveAt(SnippetList.SelectedIndex);
				DetermineDelete();
			}
			catch (Exception ea) { }
		}

		private void SnippetList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RemovePrefabButton.IsEnabled = true;
		}

		private void DetermineDelete()
		{
			if (SnippetList.Items.Count > 0)
			{
				RemovePrefabButton.IsEnabled = true;
			}
			else
			{
				RemovePrefabButton.IsEnabled = false;
			}
		}
	}
}
