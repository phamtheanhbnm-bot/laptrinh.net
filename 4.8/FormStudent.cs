
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiTap48_AsyncProgress
{
    public class Student
    {
        public string StudentId { get; set; } = "";
        public string FullName { get; set; } = "";
        public string ClassName { get; set; } = "";
        public double GPA { get; set; }
    }

    public class FormStudent : Form
    {
        private readonly DataGridView dgvStudents;
        private readonly ProgressBar progressBar;
        private readonly Label lblProgress;
        private readonly Button btnLoad;
        private readonly Action<int> updateParentProgress;

        public FormStudent(Action<int> updateProgress)
        {
            updateParentProgress = updateProgress;

            Text = "Quản lý sinh viên - Nạp dữ liệu bất đồng bộ";
            Size = new Size(950, 600);
            MinimumSize = new Size(700, 450);
            StartPosition = FormStartPosition.CenterScreen;

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 4;
            layout.Padding = new Padding(12);
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

            btnLoad = new Button();
            btnLoad.Text = "Nạp dữ liệu sinh viên";
            btnLoad.Dock = DockStyle.Fill;
            btnLoad.Font = new Font("Segoe UI", 11);
            btnLoad.Click += async (s, e) => await LoadStudentsWithProgressAsync();

            dgvStudents = new DataGridView();
            dgvStudents.Dock = DockStyle.Fill;
            dgvStudents.AutoGenerateColumns = true;
            dgvStudents.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvStudents.ReadOnly = true;
            dgvStudents.AllowUserToAddRows = false;
            dgvStudents.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            progressBar = new ProgressBar();
            progressBar.Dock = DockStyle.Fill;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;

            lblProgress = new Label();
            lblProgress.Text = "Tiến trình: 0%";
            lblProgress.Dock = DockStyle.Fill;
            lblProgress.TextAlign = ContentAlignment.MiddleLeft;
            lblProgress.Font = new Font("Segoe UI", 10);

            layout.Controls.Add(btnLoad, 0, 0);
            layout.Controls.Add(dgvStudents, 0, 1);
            layout.Controls.Add(progressBar, 0, 2);
            layout.Controls.Add(lblProgress, 0, 3);

            Controls.Add(layout);
        }

        private async Task<List<Student>> LoadStudentsAsync(
            IProgress<int> progress)
        {
            return await Task.Run(async () =>
            {
                List<Student> students = new List<Student>();

                for (int i = 1; i <= 100; i++)
                {
                    await Task.Delay(50);

                    students.Add(new Student
                    {
                        StudentId = $"SV{i:000}",
                        FullName = $"Sinh viên {i}",
                        ClassName = $"CNTT{(i % 3) + 1}",
                        GPA = Math.Round(2.0 + (i % 20) * 0.1, 1)
                    });

                    if (i % 5 == 0 || i == 100)
                    {
                        int percent = i;
                        progress.Report(percent);
                    }
                }

                return students;
            });
        }

        private async Task LoadStudentsWithProgressAsync()
        {
            btnLoad.Enabled = false;
            progressBar.Value = 0;
            lblProgress.Text = "Tiến trình: 0%";
            updateParentProgress(0);

            try
            {
                IProgress<int> progress = new Progress<int>(percent =>
                {
                    progressBar.Value = Math.Max(
                        progressBar.Minimum,
                        Math.Min(progressBar.Maximum, percent));

                    lblProgress.Text = $"Tiến trình: {percent}%";
                    updateParentProgress(percent);
                });

                List<Student> students =
                    await LoadStudentsAsync(progress);

                dgvStudents.DataSource = students;
                lblProgress.Text = "Hoàn tất: 100%";
                updateParentProgress(100);

                MessageBox.Show(
                    $"Nạp thành công {students.Count} sinh viên!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi nạp dữ liệu: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnLoad.Enabled = true;
            }
        }
    }
}
