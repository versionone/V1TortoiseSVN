using System;
using System.Windows.Forms;
using VersionOne.SDK.ObjectModel;

namespace V1TortoiseSVN
{
	public partial class LoginForm : Form
	{
	    readonly Config _config = new Config();
		public LoginForm()
		{
			InitializeComponent();
			DialogResult = DialogResult.Cancel;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			textBoxApplicationURL.Text = _config.ApplicationPath;
		    cbIntegratedSecurity.Checked = _config.UseWindowsIntegrated;
			textBoxUsername.Text = _config.Username;
			textBoxPassword.Text = _config.Password;
			cbSavePassword.Checked = _config.RememberPassword;
		}

		private void ButtonLoginClick(object sender, EventArgs e)
		{
			try
			{
				Hide();
				var v1 = new V1Instance(textBoxApplicationURL.Text, textBoxUsername.Text, textBoxPassword.Text, cbIntegratedSecurity.Checked);
				v1.Validate();

				_config.ApplicationPath = textBoxApplicationURL.Text;
			    _config.UseWindowsIntegrated = cbIntegratedSecurity.Checked;
				_config.Username = textBoxUsername.Text;

				_config.Password = cbSavePassword.Checked ? textBoxPassword.Text : null;
				_config.RememberPassword = cbSavePassword.Checked;

				if ( LoginSuccesfull != null )
					LoginSuccesfull(this, new LoginSuccessfulEventArgs(v1, _config));

				// Set dialog result, so when ShowDialog returns, the main form stays up
				DialogResult = DialogResult.OK;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error logging on", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Show();
				BringToFront();
			}
		}

		public event EventHandler<LoginSuccessfulEventArgs> LoginSuccesfull;

        private void cbIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            lblIntegratedUsername.Visible = cbIntegratedSecurity.Checked;
        }
	}

	public class LoginSuccessfulEventArgs : EventArgs
	{
		public LoginSuccessfulEventArgs( V1Instance instance, Config config )
		{
			Instance = instance;
			Config = config;
		}

		public readonly Config Config;
		public readonly V1Instance Instance;
	}
}