using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace QuanLySInhVien
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private bool KiemTraThongTin()
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text) ||
                string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtDTB.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtMaSV.Text.Length != 10)
            {
                MessageBox.Show("Mã số sinh viên phải có 10 ký tự!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            float diemTB;
            if (!float.TryParse(txtDTB.Text, out diemTB))
            {
                MessageBox.Show("Điểm trung bình phải là một số!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;  
        }

        private void dgvSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dgvSinhVien.Rows[e.RowIndex];

                txtMaSV.Text = row.Cells["StudentID"].Value.ToString();
                txtHoTen.Text = row.Cells["FullName"].Value.ToString();
                

                cmbKhoa.SelectedValue = row.Cells["FacultyID"].Value;
                txtDTB.Text = row.Cells["AverageScore"].Value.ToString();  
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (var context = new QuanLySinhVien())
            {
                var faculties = context.Faculty.ToList();
                cmbKhoa.DataSource = faculties;
                cmbKhoa.DisplayMember = "FacultyName";  
                cmbKhoa.ValueMember = "FacultyID";      

                var sinhVienList = (from sv in context.Student
                                    join faculty in context.Faculty on sv.FacultyID equals faculty.FacultyID
                                    select new
                                    {
                                        StudentID = sv.StudentID,
                                        FullName = sv.FullName,
                                        FacultyID = sv.FacultyID,
                                        FacultyName = faculty.FacultyName,
                                        AverageScore = sv.AverageScore
                                    }).ToList();

                dgvSinhVien.DataSource = sinhVienList;

                dgvSinhVien.Columns["StudentID"].HeaderText = "Mã Số SV";
                dgvSinhVien.Columns["FullName"].HeaderText = "Họ Tên";
                dgvSinhVien.Columns["FacultyName"].HeaderText = "Tên Khoa";
                dgvSinhVien.Columns["AverageScore"].HeaderText = "Điểm TB";
                dgvSinhVien.Columns["FacultyID"].Visible = false; 
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraThongTin())
            {
                return;
            }

            using (var context = new QuanLySinhVien())
            {
                var sinhVienMoi = new Student
                {
                    StudentID = Convert.ToInt32(txtMaSV.Text),  
                    FullName = txtHoTen.Text,
                    AverageScore = float.Parse(txtDTB.Text),
                    FacultyID = (int)cmbKhoa.SelectedValue
                };

                context.Student.Add(sinhVienMoi);
                context.SaveChanges();

                var sinhVienList = (from sv in context.Student
                                    join faculty in context.Faculty on sv.FacultyID equals faculty.FacultyID
                                    select new
                                    {
                                        StudentID = sv.StudentID,
                                        FullName = sv.FullName,
                                        FacultyID = sv.FacultyID,
                                        FacultyName = faculty.FacultyName,
                                        AverageScore = sv.AverageScore
                                    }).ToList();
                dgvSinhVien.DataSource = sinhVienList;

                MessageBox.Show("Thêm mới dữ liệu thành công!");
                ResetForm();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!KiemTraThongTin())
            {
                return; 
            }

            using (var context = new QuanLySinhVien())
            {
                var sinhVienCanSua = context.Student.Find(Convert.ToInt32(txtMaSV.Text));

                if (sinhVienCanSua == null)
                {
                    MessageBox.Show("Không tìm thấy MSSV cần sửa!");
                    return;
                }

                sinhVienCanSua.FullName = txtHoTen.Text;
                sinhVienCanSua.AverageScore = float.Parse(txtDTB.Text);
                sinhVienCanSua.FacultyID = (int)cmbKhoa.SelectedValue;

                context.SaveChanges();

                var sinhVienList = (from sv in context.Student
                                    join faculty in context.Faculty on sv.FacultyID equals faculty.FacultyID
                                    select new
                                    {
                                        StudentID = sv.StudentID,
                                        FullName = sv.FullName,
                                        FacultyID = sv.FacultyID,
                                        FacultyName = faculty.FacultyName,
                                        AverageScore = sv.AverageScore
                                    }).ToList();
                dgvSinhVien.DataSource = sinhVienList;

                MessageBox.Show("Cập nhật dữ liệu thành công!");
                ResetForm();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            using (var context = new QuanLySinhVien())
            {
                var sinhVienCanXoa = context.Student.Find(Convert.ToInt32(txtMaSV.Text));

                if (sinhVienCanXoa == null)
                {
                    MessageBox.Show("Không tìm thấy MSSV cần xóa!");
                    return;
                }

                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xóa sinh viên", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    context.Student.Remove(sinhVienCanXoa);
                    context.SaveChanges();

                    dgvSinhVien.DataSource = context.Student.ToList();
                    MessageBox.Show("Xóa sinh viên thành công!");
                    ResetForm();
                }
            }
        }
        private void ResetForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtDTB.Clear();
            cmbKhoa.SelectedIndex = 0;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
