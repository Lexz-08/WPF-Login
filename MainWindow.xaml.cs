using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace WPF_Login
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			if (Directory.Exists(Environment.CurrentDirectory + "\\Accounts\\") == false)
			{
				Directory.CreateDirectory(Environment.CurrentDirectory + "\\Accounts\\");
			}

			DirectoryInfo dirInfo = new DirectoryInfo(Environment.CurrentDirectory + "\\Accounts\\");

			if ((dirInfo.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
			{
				dirInfo.Attributes = FileAttributes.Hidden;
			}
		}

		private bool debug = false;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			if (debug == true)
			{
				// debugging code for main window to modify it since login window is finished
				Visibility = Visibility.Hidden;
				ShowInTaskbar = false;

				ApplicationWindow appWindow = new ApplicationWindow("J3rryChu", "sh4d0wD347H32") { Owner = this };

				appWindow.Closing += (s, ee) =>
				{
					Application.Current.Shutdown();
				};
				appWindow.Show();
			}
		}

		[DllImport("user32.dll")]
		private static extern bool ReleaseCapture();

		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hwnd, int msg, int wp, int lp);

		private void DragWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && CancelLogin.IsMouseOver == false)
			{
				ReleaseCapture();
				SendMessage(new WindowInteropHelper(this).Handle, 161, 2, 0);
			}
		}

		private void CancelLogin_MouseEnter(object sender, RoutedEventArgs e)
		{
			CancelLogin.Foreground = Brushes.Red;
		}

		private void CancelLogin_MouseLeave(object sender, RoutedEventArgs e)
		{
			CancelLogin.Foreground = Brushes.Black;
		}

		private void CancelAccountLogin(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Application.Current.Shutdown();
			}
		}

		private void AccountLogin(object sender, RoutedEventArgs e)
		{
			if (CheckAccountFolder(Username.Text, Password.Text) == true)
			{
				Visibility = Visibility.Hidden;
				ShowInTaskbar = false;

				ApplicationWindow appWindow = new ApplicationWindow(Username.Text, Password.Text) { Owner = this };

				appWindow.Closing += (s, ee) =>
				{
					Application.Current.Shutdown();
				};
				appWindow.Show();
			}
			else
			{
				MessageBox.Show("No accounts matching the given information were found.\n" +
					"Are you sure you typed the correct information?", "Account Not Found",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void AccountCreate(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(Username.Text) == true && string.IsNullOrEmpty(Password.Text) == true)
			{
				MessageBox.Show("Please specify a username and password to register.", "Username and Password Required",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else if (string.IsNullOrEmpty(Username.Text) == true)
			{
				MessageBox.Show("Please specify a username to register.", "Username Required",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else if (string.IsNullOrEmpty(Password.Text) == true)
			{
				MessageBox.Show("Please specify a password to register.", "Password Required",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else if (CheckAccountFolder(Username.Text, Password.Text) == true)
			{
				MessageBox.Show("Cannot overwrite existing account from login window.", "Login Before Modifying Account",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				string username = Username.Text;
				string password = Password.Text;

				string encryptUsername = string.Empty;
				string encryptPassword = string.Empty;

				foreach (char infoChar in username)
				{
					encryptUsername += Encrypt(infoChar) + "-";
				}
				encryptUsername = encryptUsername.TrimEnd('-');

				foreach (char passChar in password)
				{
					encryptPassword += Encrypt(passChar) + "-";
				}
				encryptPassword = encryptPassword.TrimEnd('-');

				string accountFile = Environment.CurrentDirectory + "\\Accounts\\" + encryptUsername;

				File.WriteAllText(accountFile, encryptPassword);
				File.SetAttributes(accountFile, FileAttributes.ReadOnly | FileAttributes.Hidden);
			}
		}

		/// <summary>
		/// Checks the accounts folder for any account that matches the given <paramref name="Username"/> and <paramref name="Password"/>.
		/// </summary>
		/// <param name="Username">The Username to check for</param>
		/// <param name="Password">The Password to check for</param>
		private bool CheckAccountFolder(string Username, string Password)
		{
			// get a string array containing the path and name of each account file
			string[] accounts = Directory.GetFiles(Environment.CurrentDirectory + "\\Accounts\\");

			// variables for account validation
			bool matchingUsername = false;
			bool matchingPassword = false;

			// check each account file
			for (int i = 0; i < accounts.Length; i++)
			{
				// get each character code separated by a '-'
				string[] codesStr = Path.GetFileNameWithoutExtension(accounts[i]).Split('-');
				int[] codes = new int[codesStr.Length];

				// convert each chararcter integer to an integer
				int index = 0;
				foreach (string codeStr in codesStr)
				{
					codes[index] = int.Parse(codeStr);
					index++;
				}

				// username variable for validation
				string username = string.Empty;

				// add each character converted integer to the username
				foreach (int code in codes)
				{
					username += Decrypt(code);
				}

				// get each character code separated by a '-'
				string[] passwordCodesStr = File.ReadAllText(accounts[i]).Split('-');
				int[] passwordCodes = new int[passwordCodesStr.Length];

				// convert each character integer to an integer
				int index2 = 0;
				foreach (string passwordCodeStr in passwordCodesStr)
				{
					passwordCodes[index2] = int.Parse(passwordCodeStr);
					index2++;
				}

				// password variable for validation
				string password = string.Empty;

				// add each character converted integer to the password
				foreach (int passwordCode in passwordCodes)
				{
					password += Decrypt(passwordCode);
				}

				// check to see if both the username and password match whats typed into the login window
				matchingUsername = username == Username;
				matchingPassword = password == Password;

				// if they both match, exit the loop
				if ((matchingUsername == true && matchingPassword == true) == true)
				{
					break;
				}
			}

			// just return true since they match
			if ((matchingUsername == true && matchingPassword == true) == true)
			{
				return true;
			}
			else return false;
			// if the validation fails, return false
		}

		/// <summary>
		/// Converts the given <paramref name="Code"/> to a character.
		/// </summary>
		/// <param name="Code">The Code to convert</param>
		private char Decrypt(int Code)
		{
			return (char)Code;
		}

		/// <summary>
		/// Converts the given <paramref name="Character"/> to an integer.
		/// </summary>
		/// <param name="Character">The Character to convert</param>
		private int Encrypt(char Character)
		{
			return Character;
		}
	}
}
