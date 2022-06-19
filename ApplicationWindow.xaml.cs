using AMS.Profile;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace WPF_Login
{
	/// <summary>
	/// Interaction logic for ApplicationWindow.xaml
	/// </summary>
	public partial class ApplicationWindow : Window
	{
		public ApplicationWindow(string Username, string Password)
		{
			InitializeComponent();

			username = Username;
			password = Password;
		}

		private string username, password;

		[DllImport("user32.dll")]
		private static extern bool ReleaseCapture();

		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);

			if (WindowState == WindowState.Maximized)
			{
				Maximize.Text = "2";
				MaximizeToolTip.Content = "Restores the window";
				ContentWindow.Padding = new Thickness(8);
			}
			else if (WindowState == WindowState.Normal)
			{
				Maximize.Text = "1";
				MaximizeToolTip.Content = "Maximizes the window";
				ContentWindow.Padding = new Thickness(0);
			}
		}

		private void DragWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				ReleaseCapture();
				SendMessage(new WindowInteropHelper(this).Handle, 161, 2, 0);
			}
		}

		private void CloseWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				SystemCommands.CloseWindow(this);
			}
		}

		private void MaximizeWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (WindowState == WindowState.Maximized)
				{
					SystemCommands.RestoreWindow(this);
				}
				else
				{
					SystemCommands.MaximizeWindow(this);
				}
			}
		}

		private void MinimizeWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				SystemCommands.MinimizeWindow(this);
				
				if (minToTaskbar == false)
				{
					TrayIcon.Visibility = Visibility.Visible;
					Visibility = Visibility.Hidden;
					ShowInTaskbar = false;
				}
			}
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			if (minToTaskbar == false)
			{
				TrayIcon.Visibility = Visibility.Hidden;
				Visibility = Visibility.Visible;
				ShowInTaskbar = true;
			}
		}

		private void ToggleSettingsContext(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				TextBlock label = (TextBlock)sender;

				if (label.Text == "6")
				{
					label.Text = "5";
					SettingsToolTip.Content = "Closes the context menu";
					SettingsMenu.Visibility = Visibility.Visible;
				}
				else if (label.Text == "5")
				{
					label.Text = "6";
					SettingsToolTip.Content = "Opens a context menu with a list of options relating to the application's settings";
					SettingsMenu.Visibility = Visibility.Hidden;
				}
			}
		}

		private void Close_MouseEnter(object sender, MouseEventArgs e)
		{
			((TextBlock)sender).Background = Brushes.Gainsboro;
		}

		private void Close_MouseLeave(object sender, MouseEventArgs e)
		{
			((TextBlock)sender).Background = Brushes.Transparent;
		}

		private void Maximize_MouseEnter(object sender, MouseEventArgs e)
		{
			((TextBlock)sender).Background = Brushes.Gainsboro;
		}

		private void Maximize_MouseLeave(object sender, MouseEventArgs e)
		{
			((TextBlock)sender).Background = Brushes.Transparent;
		}

		private void Minimize_MouseEnter(object sender, MouseEventArgs e)
		{
			((TextBlock)sender).Background = Brushes.Gainsboro;
		}

		private void Minimize_MouseLeave(object sender, MouseEventArgs e)
		{
			((TextBlock)sender).Background = Brushes.Transparent;
		}

		private void Settings_MouseEnter(object sender, MouseEventArgs e)
		{
			((TextBlock)sender).Background = Brushes.Gainsboro;
		}

		private void Settings_MouseLeave(object sender, MouseEventArgs e)
		{
			((TextBlock)sender).Background = Brushes.Transparent;
		}

		private void Topmost_MouseEnter(object sender, MouseEventArgs e)
		{
			((Label)sender).Background = Brushes.Gainsboro;
		}

		private void Topmost_MouseLeave(object sender, MouseEventArgs e)
		{
			((Label)sender).Background = Brushes.Transparent;
		}

		private void MinToTaskbar_MouseEnter(object sender, MouseEventArgs e)
		{
			((Label)sender).Background = Brushes.Gainsboro;
		}

		private void MinToTaskbar_MouseLeave(object sender, MouseEventArgs e)
		{
			((Label)sender).Background = Brushes.Transparent;
		}

		private void Account_MouseEnter(object sender, MouseEventArgs e)
		{
			((Label)sender).Background = Brushes.Gainsboro;
		}

		private void Account_MouseLeave(object sender, MouseEventArgs e)
		{
			((Label)sender).Background = Brushes.Transparent;
		}

		private void SignOut_MouseEnter(object sender, MouseEventArgs e)
		{
			((Label)sender).Background = Brushes.Gainsboro;
		}

		private void SignOut_MouseLeave(object sender, MouseEventArgs e)
		{
			((Label)sender).Background = Brushes.Transparent;
		}

		private Ini config = new Ini(Environment.CurrentDirectory + "\\config.ini");
		private bool minToTaskbar = true;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			Topmost = bool.Parse(config.GetValue("window config", "topmost").ToString());
			minToTaskbar = bool.Parse(config.GetValue("window config", "min_to_taskbar").ToString());

			TOPMOST.Content = "Always On Top: " + (Topmost ? "√" : "X");
			MinToTaskbar.Content = "Min To Taskbar: " + (minToTaskbar ? "√" : "X");

			TopmostItem.Header = TOPMOST.Content;
			MinToTaskbarItem.Header = MinToTaskbar.Content;

			TopmostItem.ToolTip = TOPMOST.ToolTip;
			MinToTaskbarItem.ToolTip = MinToTaskbar.ToolTip;
			AccountItem.ToolTip = Account.ToolTip;
			SignOutItem.ToolTip = Signout.ToolTip;
		}

		private void Toggle_Topmost(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (Topmost == true)
				{
					Topmost = false;
					((Label)sender).Content = "Always On Top: X";
					TopmostItem.Header = ((Label)sender).Content;
					config.SetValue("window config", "topmost", false.ToString().ToLower());
				}
				else if (Topmost == false)
				{
					Topmost = true;
					((Label)sender).Content = "Always On Top: √";
					TopmostItem.Header = ((Label)sender).Content;
					config.SetValue("window config", "topmost", true.ToString().ToLower());
				}
			}
		}

		private void Toggle_MinToTaskbar(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (minToTaskbar == true)
				{
					minToTaskbar = false;
					((Label)sender).Content = "Min To Taskbar: X";
					MinToTaskbarItem.Header = "Min To Taskbar: X";
					config.SetValue("window config", "min_to_taskbar", false.ToString().ToLower());
				}
				else if (minToTaskbar == false)
				{
					minToTaskbar = true;
					((Label)sender).Content = "Min To Taskbar: √";
					MinToTaskbarItem.Header = "Min To Taskbar: √";
					config.SetValue("window config", "min_to_taskbar", true.ToString().ToLower());
				}
			}
		}

		private void AccountInformation(object sender, MouseButtonEventArgs e)
		{
			AccountInformation accInfo = new AccountInformation(username, password);
			accInfo.Owner = this;
			accInfo.ShowDialog();
		}

		private void SignOut(object sender, MouseButtonEventArgs e)
		{
			CloseWindow(sender, e);
		}

		private void ReopenWindow(object sender, RoutedEventArgs e)
		{
			TrayIcon.Visibility = Visibility.Hidden;
			Visibility = Visibility.Visible;
			ShowInTaskbar = true;
			SystemCommands.RestoreWindow(this);
		}

		private void Toggle_Topmost_Click(object sender, RoutedEventArgs e)
		{
			if (Topmost == true)
			{
				Topmost = false;
				TOPMOST.Content = "Always On Top: X";
				TopmostItem.Header = "Always On Top: X";
				config.SetValue("window config", "topmost", false.ToString().ToLower());
			}
			else if (Topmost == false)
			{
				Topmost = true;
				TOPMOST.Content = "Always On Top: √";
				TopmostItem.Header = "Always On Top: √";
				config.SetValue("window config", "topmost", true.ToString().ToLower());
			}
		}

		private void Toggle_MinToTaskbar_Click(object sender, RoutedEventArgs e)
		{
			if (minToTaskbar == true)
			{
				minToTaskbar = false;
				MinToTaskbar.Content = "Min To Taskbar: X";
				MinToTaskbarItem.Header = "Min To Taskbar: X";
				config.SetValue("window config", "min_to_taskbar", false.ToString().ToLower());
			}
			else if (minToTaskbar == false)
			{
				minToTaskbar = true;
				MinToTaskbar.Content = "Min To Taskbar: √";
				MinToTaskbarItem.Header = "Min To Taskbar: √";
				config.SetValue("window config", "min_to_taskbar", true.ToString().ToLower());
			}
		}

		private void AccountInformation_Click(object sender, RoutedEventArgs e)
		{
			AccountInformation accInfo = new AccountInformation(username, password);
			accInfo.Owner = this;
			accInfo.ShowDialog();
		}

		private void SignOut_Click(object sender, RoutedEventArgs e)
		{
			SystemCommands.CloseWindow(this);
		}
	}
}
