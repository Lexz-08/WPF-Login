using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace WPF_Login
{
	/// <summary>
	/// Interaction logic for AccountInformation.xaml
	/// </summary>
	public partial class AccountInformation : Window
	{
		public AccountInformation(string Username, string Password)
		{
			InitializeComponent();

			AccountUsername.Text = Username;
			displayPassword = "**********";
			hiddenPassword = Password;
			AccountPassword.Text = displayPassword;
		}

		private string hiddenPassword = string.Empty;
		private string displayPassword = string.Empty;

		private string EncryptToSingleChar(string Str, char Char)
		{
			string encStr = string.Empty;

			foreach (char strChar in Str)
			{
				encStr += Char;
			}

			return encStr;
		}

		private void CloseWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Close();
			}
		}

		private void ShowPasscode(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				AccountPassword.Text = hiddenPassword;

				Timer timeout = new Timer { Interval = 2500 };
				timeout.Tick += (s, ee) =>
				{
					AccountPassword.Text = displayPassword;

					timeout.Enabled = false;
				};
				timeout.Start();
			}
		}
	}
}
