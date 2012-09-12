using System;
using System.Windows.Forms;

namespace V1TortoiseSVN
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.treeViewWorkitems = new System.Windows.Forms.TreeView();
			this.contextMenuWorkitem = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.imageListIcons = new System.Windows.Forms.ImageList(this.components);
			this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemShow = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
			this.btnCancel = new System.Windows.Forms.Button();
			this.contextMenuWorkitem.SuspendLayout();
			this.trayMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeViewWorkitems
			// 
			this.treeViewWorkitems.ContextMenuStrip = this.contextMenuWorkitem;
			this.treeViewWorkitems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewWorkitems.ImageIndex = 0;
			this.treeViewWorkitems.ImageList = this.imageListIcons;
			this.treeViewWorkitems.Location = new System.Drawing.Point(0, 0);
			this.treeViewWorkitems.Name = "treeViewWorkitems";
			this.treeViewWorkitems.SelectedImageIndex = 0;
			this.treeViewWorkitems.Size = new System.Drawing.Size(314, 494);
			this.treeViewWorkitems.TabIndex = 1;
			this.treeViewWorkitems.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewWorkitems_NodeMouseDoubleClick);
			this.treeViewWorkitems.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeViewWorkitems_MouseUp);
			this.treeViewWorkitems.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeViewWorkitems_MouseMove);
			this.treeViewWorkitems.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewWorkitems_MouseDown);
			this.treeViewWorkitems.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeViewWorkitems_KeyPress);
			// 
			// contextMenuWorkitem
			// 
			this.contextMenuWorkitem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.viewToolStripMenuItem});
			this.contextMenuWorkitem.Name = "contextMenuWorkitem";
			this.contextMenuWorkitem.ShowImageMargin = false;
			this.contextMenuWorkitem.Size = new System.Drawing.Size(99, 70);
			this.contextMenuWorkitem.Text = "Copy";
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// refreshToolStripMenuItem
			// 
			this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			this.refreshToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.refreshToolStripMenuItem.Text = "Refresh";
			this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.viewToolStripMenuItem.Text = "View";
			this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
			// 
			// imageListIcons
			// 
			this.imageListIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListIcons.ImageStream")));
			this.imageListIcons.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListIcons.Images.SetKeyName(0, "story-icon.gif");
			this.imageListIcons.Images.SetKeyName(1, "defect-icon.gif");
			this.imageListIcons.Images.SetKeyName(2, "task-icon.gif");
			// 
			// trayIcon
			// 
			this.trayIcon.ContextMenuStrip = this.trayMenu;
			this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
			this.trayIcon.Text = "V1 TortoiseSVN";
			this.trayIcon.Visible = true;
			this.trayIcon.DoubleClick += new System.EventHandler(this.trayIcon_DoubleClick);
			// 
			// trayMenu
			// 
			this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemShow,
            this.menuItemExit});
			this.trayMenu.Name = "trayMenu";
			this.trayMenu.ShowImageMargin = false;
			this.trayMenu.Size = new System.Drawing.Size(90, 48);
			this.trayMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.trayMenu_ItemClicked);
			// 
			// menuItemShow
			// 
			this.menuItemShow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.menuItemShow.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.menuItemShow.Name = "menuItemShow";
			this.menuItemShow.Size = new System.Drawing.Size(89, 22);
			this.menuItemShow.Text = "&Open";
			// 
			// menuItemExit
			// 
			this.menuItemExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.menuItemExit.Name = "menuItemExit";
			this.menuItemExit.Size = new System.Drawing.Size(89, 22);
			this.menuItemExit.Text = "E&xit";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(0, 0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Hide";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(314, 494);
			this.Controls.Add(this.treeViewWorkitems);
			this.Controls.Add(this.btnCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "V1 TortoiseSVN";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.contextMenuWorkitem.ResumeLayout(false);
			this.trayMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView treeViewWorkitems;
		private System.Windows.Forms.ContextMenuStrip contextMenuWorkitem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.NotifyIcon trayIcon;
		private System.Windows.Forms.ContextMenuStrip trayMenu;
		private System.Windows.Forms.ToolStripMenuItem menuItemShow;
		private System.Windows.Forms.ToolStripMenuItem menuItemExit;
		private ImageList imageListIcons;
		private ToolStripMenuItem refreshToolStripMenuItem;
		private Button btnCancel;
		private ToolStripMenuItem viewToolStripMenuItem;

	}
}


