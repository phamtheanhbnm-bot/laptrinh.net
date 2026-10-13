
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BaiTap48_AsyncProgress
{
    public partial class FormMain : Form
    {
        private readonly StatusStrip statusStrip;
        private readonly ToolStripStatusLabel statusLabel;

        public FormMain()
        {
            Text = "Bài 4.8 - Async/Await và ProgressBar";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            IsMdiContainer = true;

            MenuStrip menuStrip = new MenuStrip();

            ToolStripMenuItem menuFile = new ToolStripMenuItem("Tệp");
            ToolStripMenuItem menuStudent = new ToolStripMenuItem("Quản lý sinh viên");
            ToolStripMenuItem menuExit = new ToolStripMenuItem("Thoát");

            menuStudent.Click += (s, e) => OpenStudentForm();
            menuExit.Click += (s, e) => Close();

            menuFile.DropDownItems.Add(menuStudent);
            menuFile.DropDownItems.Add(new ToolStripSeparator());
            menuFile.DropDownItems.Add(menuExit);

            menuStrip.Items.Add(menuFile);
            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);

            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Sẵn sàng");
            statusStrip.Items.Add(statusLabel);
            Controls.Add(statusStrip);

            Shown += (s, e) => OpenStudentForm();
        }

        private void OpenStudentForm()
        {
            foreach (Form child in MdiChildren)
            {
                if (child is FormStudent)
                {
                    child.Activate();
                    return;
                }
            }

            FormStudent form = new FormStudent(UpdateProgress);
            form.MdiParent = this;
            form.Show();
        }

        public void UpdateProgress(int percent)
        {
            statusLabel.Text = $"Tiến trình nạp dữ liệu: {percent}%";
        }
    }
}
