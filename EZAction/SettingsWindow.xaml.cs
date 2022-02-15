using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//using Microsoft.Win32;
using WK.Libraries.BetterFolderBrowserNS;
using System.Diagnostics;

namespace TestAppWPF
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		bool armaInstalled = true;

		public Window1()
		{
			InitializeComponent();
			PopulateListBox();
		}

		private void SaveSettings_Click(object sender, RoutedEventArgs e)
		{
			settingsText.Visibility = System.Windows.Visibility.Visible;

			Properties.Settings.Default.defaultSavePath = saveDirText.Text;

			if (armaInstalled) {
				Properties.Settings.Default.profileName = profileDropDown.SelectedItem.ToString();
				Properties.Settings.Default.profileDir = profileDir.Text;
			}

			Properties.Settings.Default.Save();
		}

		private void PopulateListBox()
		{
			if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArmA 3"))
			{
				profileDropDown.Items.Add("Default");

				string otherProfilesDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArmA 3 - Other Profiles";
				string[] profileList;

				if (Directory.Exists(otherProfilesDir))
				{
					profileList = Directory.GetDirectories(otherProfilesDir);

					foreach (string profile in profileList)
					{
						string corrected = profile.Replace(otherProfilesDir + "\\", "");
						profileDropDown.Items.Add(corrected);
					}

					if (Properties.Settings.Default.profileName == "Default" ||
						Properties.Settings.Default.profileName == ""
						)
					{
						profileDropDown.SelectedItem = "Default";
					}
					else
					{
						profileDropDown.SelectedItem = Properties.Settings.Default.profileName;
					}
				}
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("It appears that you do not have ArmA 3 installed." + "\n" + "No profiles have been read.");
				armaInstalled = false;

				profileDropDown.IsEnabled = false;
			}
		}

		private void profileDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (profileDropDown.SelectedItem == "Default")
			{
				profileDir.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArmA 3";
			}

			else
			{
				profileDir.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArmA 3 - Other Profiles\\" + profileDropDown.SelectedItem;
			}
		}

		private void Pick_Folder_Click(object sender, RoutedEventArgs e)
		{
			//OpenFileDialog folder2 = new OpenFileDialog();

			//folder2.ValidateNames = false;
			//folder2.CheckFileExists = false;
			//folder2.CheckPathExists = false;

			// Always default to Folder Selection.
			//folder2.FileName = "";

			//folder2.InitialDirectory = Properties.Settings.Default.defaultSavePath;

			BetterFolderBrowser browser = new BetterFolderBrowser();
			browser.Multiselect = false;
			browser.RootFolder = Properties.Settings.Default.defaultSavePath;

			DialogResult result = browser.ShowDialog();

			//FolderBrowserDialog folder = new FolderBrowserDialog
			//{
			//	SelectedPath = Properties.Settings.Default.defaultSavePath,
			//};

			 //result = folder.ShowDialog();

			

			if (result.ToString() == "OK")
			{
				//string toReplaceFolder = folder2.FileName;
				//string safeFileName = folder2.SafeFileName;

				//string replacedFolder = toReplaceFolder.Replace(safeFileName, "");
				//replacedFolder = replacedFolder.Replace("\\\\", "\\");

				saveDirText.Text = browser.SelectedFolder;
			}
		}
	}
}
