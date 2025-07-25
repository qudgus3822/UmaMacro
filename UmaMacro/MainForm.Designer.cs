namespace UmaMacro
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            button1 = new Button();
            label1 = new Label();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            활성비활성ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            종료ToolStripMenuItem = new ToolStripMenuItem();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(207, 54);
            button1.Name = "button1";
            button1.Size = new Size(106, 23);
            button1.TabIndex = 0;
            button1.Text = "활성/비활성";
            button1.UseVisualStyleBackColor = true;
            button1.Click += ToggleButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(121, 58);
            label1.Name = "label1";
            label1.Size = new Size(0, 15);
            label1.TabIndex = 1;
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Text = "우마컨트롤러";
            notifyIcon1.Visible = true;
            notifyIcon1.DoubleClick += NotifyIcon1_DoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { 활성비활성ToolStripMenuItem, toolStripSeparator1, 종료ToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(153, 54);
            // 
            // 활성비활성ToolStripMenuItem
            // 
            활성비활성ToolStripMenuItem.Name = "활성비활성ToolStripMenuItem";
            활성비활성ToolStripMenuItem.Size = new Size(152, 22);
            활성비활성ToolStripMenuItem.Text = "활성/비활성";
            활성비활성ToolStripMenuItem.Click += 활성비활성ToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(149, 6);
            // 
            // 종료ToolStripMenuItem
            // 
            종료ToolStripMenuItem.Name = "종료ToolStripMenuItem";
            종료ToolStripMenuItem.Size = new Size(152, 22);
            종료ToolStripMenuItem.Text = "종료";
            종료ToolStripMenuItem.Click += 종료ToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(359, 134);
            Controls.Add(label1);
            Controls.Add(button1);
            Name = "MainForm";
            Text = "우마컨트롤러";
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            FormClosing += MainForm_FormClosing;
            Resize += MainForm_Resize;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem 활성비활성ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem 종료ToolStripMenuItem;
    }
}
