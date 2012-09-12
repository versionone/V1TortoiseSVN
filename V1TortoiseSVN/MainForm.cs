using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VersionOne.SDK.ObjectModel;
using Message=System.Windows.Forms.Message;

namespace V1TortoiseSVN
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private Thread _monitoringThread;
		private bool _mouseDown = false;
		private bool _dragging = false;
		private ManualResetEvent stopEvent = new ManualResetEvent(false);
		private const string DefaultTitle = "V1 TortoiseSVN";

		private void MainForm_Load(object sender, EventArgs e)
		{
			HideToTray();
			trayIcon.Text = "Authenticating...";
			LoginForm login = new LoginForm();
			login.LoginSuccesfull += login_LoginSuccesfull;

			// Kind of a weak way to do it, but if we cancel out of the login dialog, exit
			DialogResult loginResult = login.ShowDialog( this );

			if (loginResult == DialogResult.OK)
			{
				BeginWindowMonitoring();
			}
			else
				Close();
		}

		void login_LoginSuccesfull( object sender, LoginSuccessfulEventArgs e )
		{
			Retriever = new WorkitemRetriever(e.Instance);

			if (InvokeRequired)
			{
				Invoke(new InvokeMethod(delegate
											{
												trayIcon.Text = DefaultTitle;
												PopulateWorkitems();
											}
						));
			}
			else
				PopulateWorkitems();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(Retriever != null)
				Retriever.ShutDown();

			stopEvent.Set();
		}
		
		private void PopulateWorkitems()
		{
			Cursor = Cursors.WaitCursor;
			treeViewWorkitems.Enabled = false;
			trayIcon.Text = "Retrieving Workitems";

			if (Retriever != null)
				Retriever.BeginRetrieveWorkitems();
			else
				throw new InvalidOperationException("Cannot Populate workitems with no Retriever");
		}

		private WorkitemRetriever _retriever;
		/// <summary>
		/// Gets the _retriever field; setting also hooks/unhooks events
		/// </summary>
		private WorkitemRetriever Retriever
		{
			get { return _retriever; }

			set
			{
				// unhook old, if exists
				if (_retriever != null)
				{
					_retriever.WorkitemsError -= HandleBackgroundError;
					_retriever.WorkitemsReady -= HandleWorkitemsReady;
				}

				_retriever = value;

				// hook new, if not null
				if(_retriever != null)
				{
					_retriever.WorkitemsError += HandleBackgroundError;
					_retriever.WorkitemsReady += HandleWorkitemsReady;
				}
			}
		}

		private void PopulateTree(Dictionary<Workitem, List<Task>> workitems)
		{
			
			treeViewWorkitems.Nodes.Clear();
			foreach(PrimaryWorkitem primaryWorkitem in workitems.Keys)
			{
				string primaryWorkitemDisplay = string.Format("{0} ({1})", primaryWorkitem.Name, primaryWorkitem.DisplayID);
				
				TreeNode primaryWorkitemNode = new TreeNode(primaryWorkitemDisplay);
				SetIcon(primaryWorkitemNode, primaryWorkitem);
				primaryWorkitemNode.Tag = primaryWorkitem;
				treeViewWorkitems.Nodes.Add(primaryWorkitemNode);

				foreach (Task task in workitems[primaryWorkitem])
				{
					string taskDisplay = string.Format("{0} ({1})", task.Name, task.DisplayID);
					
					TreeNode taskNode = new TreeNode(taskDisplay);
					SetIcon(taskNode, task);
					taskNode.Tag = task;
					primaryWorkitemNode.Nodes.Add(taskNode);
				}
				primaryWorkitemNode.ExpandAll();
			}
			FinishWorkitemPopulation();
		}

		private void FinishWorkitemPopulation()
		{
			Cursor = Cursors.Default;
			treeViewWorkitems.Enabled = true;
			trayIcon.Text = DefaultTitle;
		}

		private static void SetIcon(TreeNode node, Workitem workitem)
		{
			int index;

			if (workitem is Story)
				index = 0;
			else if (workitem is Defect)
				index = 1;
			else
				index = 2;

			node.ImageIndex = index;
			node.SelectedImageIndex = index;
		}

		void HandleWorkitemsReady(object sender, WorkitemsReadyEventArgs e)
		{
			if(InvokeRequired)
			{
				Invoke(new InvokeMethod(delegate
				                        	{
												PopulateTree(e.Workitems);
				                        	}
				       	));
			}
			else
				PopulateTree(e.Workitems);
		}

		void HandleBackgroundError(object sender, BackgroundErrorEventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new InvokeMethod(delegate
											{
												FinishWorkitemPopulation();
												ShowError(e);
											}
						));
			}
			else
			{
				FinishWorkitemPopulation();
				ShowError(e);
			}
		}

		private void ShowError(BackgroundErrorEventArgs e)
		{
			if (Visible)
			{
				MessageBox.Show(e.Exception.ToString(), 
								"Don't panic, but something went wrong", 
								MessageBoxButtons.OK,
								MessageBoxIcon.Error);
			}
			else
			{
				trayIcon.ShowBalloonTip(5000, DefaultTitle, e.Exception.ToString(), ToolTipIcon.Error);
			}
		}

		#region Window Monitor/Handle

		private IntPtr _commitWindowHandle = IntPtr.Zero;

		private void BeginWindowMonitoring()
		{
			_monitoringThread = new Thread(MonitorWindows);
			_monitoringThread.Name = "V1SVN Commit Window Watcher";
			_monitoringThread.IsBackground = true;
			_monitoringThread.Priority = ThreadPriority.Lowest;
			_monitoringThread.Start();
		}

		private enum TortWindowHandleState
		{
			Have,
			Lost,
			DontHave
		} ;

		private void MonitorWindows()
		{
			if (!Thread.CurrentThread.IsBackground)
				throw new InvalidOperationException("Cannot call MonitorWindows() from a foreground thread.");

			while (true)
			{
				if (stopEvent.WaitOne(350, false))
					return;

				TortWindowHandleState state = TortWindowHandleState.DontHave;

				if (_commitWindowHandle == IntPtr.Zero)
				{
					IntPtr handle = Win32.GetForegroundWindow();
					if (IsWindowTortoiseSVN(handle))
					{
						_commitWindowHandle = handle;
						state = TortWindowHandleState.Have;
					}
				}
				else
				{
					if (!Win32.IsWindowVisible(_commitWindowHandle))
					{
						_commitWindowHandle = IntPtr.Zero;
						state = TortWindowHandleState.Lost;
					}
					else
						state = TortWindowHandleState.Have;
				}

				if (state == TortWindowHandleState.Have)
				{
					Win32.RECT rect;
					Win32.GetWindowRect(_commitWindowHandle, out rect);

					Invoke(new MethodInvoker(delegate
												{
													FollowTortoise(rect);
												}));
				}
				else if (state == TortWindowHandleState.Lost)
					Invoke(new MethodInvoker(HideWindow));
			}
		}

		private void HideWindow()
		{
			HideToTray();
		}

		private void FollowTortoise(Win32.RECT rect)
		{
			if ((rect.Top != Top) || (rect.Left != Left) || ((rect.Bottom - rect.Top) != Height))
			{
				Win32.SetWindowPos(this.Handle, _commitWindowHandle, rect.Right, rect.Top, Width, rect.Bottom - rect.Top, (uint)WindowsMessages.SWP_SHOWWINDOW);
			}
			if (!Visible)
				RestoreFromTray();
		}

		private static bool IsWindowTortoiseSVN(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return false;

			IntPtr textboxHandle = Win32.FindWindowEx(handle, IntPtr.Zero, "Scintilla", null);
			if (textboxHandle == IntPtr.Zero)
				return false;

			try
			{
				Int32 pid = Win32.GetWindowProcessID(handle);
				Process p = Process.GetProcessById(pid);
				string appName = p.ProcessName;

				// This is our secondary weakness: if the process exe name changes, we will incorectly reject it
				if (appName.ToLower() != "tortoiseproc")
					return false;
			}
			catch (ArgumentException) // The Process is no longer running.
			{
				return false;
			}

			
			
			return true;
		}

		private static void WriteToTortoise(string text)
		{
			IntPtr handle = Win32.FindWindow("#32770", null);

			if (IsWindowTortoiseSVN(handle))
			{
				IntPtr textboxHandle = Win32.FindWindowEx(handle, IntPtr.Zero, "Scintilla", null);
				foreach (char c in text)
				{
					Win32.SendMessage(textboxHandle, (uint)WindowsMessages.WM_CHAR, c, null);
				}

				Win32.SetForegroundWindow(handle);
				Win32.SendMessage( textboxHandle, (uint) WindowsMessages.WM_SETFOCUS, 0, null );
			}
		}

		#endregion

		private void treeViewWorkitems_MouseDown(object sender, MouseEventArgs e)
		{
			_mouseDown = true;

			treeViewWorkitems.SelectedNode = treeViewWorkitems.GetNodeAt(e.X, e.Y);
		}

		private void treeViewWorkitems_MouseMove(object sender, MouseEventArgs e)
		{
			if (_mouseDown && !_dragging)
			{
				if (treeViewWorkitems.SelectedNode != null)
				{
					treeViewWorkitems.DoDragDrop(((Workitem) treeViewWorkitems.SelectedNode.Tag).DisplayID, DragDropEffects.Copy);
					_dragging = true;
				}
			}
		}

		private void treeViewWorkitems_MouseUp(object sender, MouseEventArgs e)
		{
			_mouseDown = false;
			_dragging = false;
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeViewWorkitems.SelectedNode != null)
			{
				Clipboard.SetText(((Workitem)treeViewWorkitems.SelectedNode.Tag).DisplayID);
			}
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RefreshWorkitems();
		}

		private void viewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string url = ((Workitem) treeViewWorkitems.SelectedNode.Tag).URL;
			Process.Start(url);
		}

		private void RefreshWorkitems()
		{
			PopulateWorkitems();
		}

		private void treeViewWorkitems_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			SendSelectedIDToTortoise();
		}

		private void treeViewWorkitems_KeyPress( object sender, KeyPressEventArgs e )
		{
			SendSelectedIDToTortoise();
		}

		private void SendSelectedIDToTortoise()
		{
			if ( treeViewWorkitems.SelectedNode != null )
				WriteToTortoise( ( (Workitem) treeViewWorkitems.SelectedNode.Tag ).DisplayID );
		}
		
		private void trayMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem == menuItemExit)
				Close();
			if (e.ClickedItem == menuItemShow)
				RestoreFromTray();
		}

		private void trayIcon_DoubleClick(object sender, EventArgs e)
		{
			RestoreFromTray();
		}

		private void RestoreFromTray()
		{
			Show();
			WindowState = FormWindowState.Normal;
		}

		private void HideToTray()
		{
			Hide();
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			if (FormWindowState.Minimized == WindowState)
				HideToTray();
		}

		protected override void WndProc(ref Message message)
		{
			WindowsMessages messageID = (WindowsMessages)message.Msg;

			base.WndProc(ref message);
		}

		private void btnCancel_Click( object sender, EventArgs e )
		{
			HideToTray();
		}		
	}
}
