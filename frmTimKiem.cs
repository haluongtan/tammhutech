using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp12
{
    public partial class frmTimKiem : Form
    {
        public frmTimKiem()
        {
            InitializeComponent();
        }

        private void frmTimKiem_Load(object sender, EventArgs e)
        {
            rbFemale.Checked = true;

            LoadFacultyToComboBox();
            LoadDataGridView();



        }
        private void LoadFacultyToComboBox()
        {
            using (var context = new QuanLySinhVienEntities())
            {
                var faculties = context.Faculty.ToList();

                cmbKhoa.DataSource = faculties;
                cmbKhoa.DisplayMember = "FacultyName";  
                cmbKhoa.ValueMember = "FacultyID";     

                cmbKhoa.SelectedIndex = -1;
            }
        }
        private void LoadDataGridView()
        {
            using (var context = new QuanLySinhVienEntities())
            {
                var danhSachSinhVien = (from sv in context.Student
                                        join faculty in context.Faculty on sv.FacultyID equals faculty.FacultyID
                                        select new
                                        {
                                            MaSV = sv.StudentID,
                                            HoTen = sv.FullName,
                                            GioiTinh = sv.Gender,
                                            TenKhoa = faculty.FacultyName,
                                            DiemTB = sv.AverageScore
                                        }).ToList();

                dgvTimKiem.Rows.Clear();

                foreach (var sv in danhSachSinhVien)
                {
                    dgvTimKiem.Rows.Add(sv.MaSV, sv.HoTen, sv.GioiTinh, sv.TenKhoa, sv.DiemTB);
                }

                txtKetQuaTimKiem.Text = danhSachSinhVien.Count.ToString();
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            using (var context = new QuanLySinhVienEntities())
            {
                string maSV = txtMaSV.Text.Trim();
                string hoTen = txtHoTen.Text.Trim();
                string gioiTinh = rbMale.Checked ? "Male" : "Female";
                int facultyID = cmbKhoa.SelectedValue != null ? (int)cmbKhoa.SelectedValue : 0;

                var query = context.Student.AsQueryable();

                if (!string.IsNullOrEmpty(maSV))
                {
                    query = query.Where(sv => sv.StudentID.Contains(maSV));
                }

                if (!string.IsNullOrEmpty(hoTen))
                {
                    query = query.Where(sv => sv.FullName.Contains(hoTen));
                }

                if (!string.IsNullOrEmpty(gioiTinh))
                {
                    query = query.Where(sv => sv.Gender == gioiTinh);
                }

                if (facultyID != 0)
                {
                    query = query.Where(sv => sv.FacultyID == facultyID);
                }

                var danhSachTimKiem = (from sv in query
                                       join faculty in context.Faculty on sv.FacultyID equals faculty.FacultyID
                                       select new
                                       {
                                           MaSV = sv.StudentID,
                                           HoTen = sv.FullName,
                                           GioiTinh = sv.Gender,
                                           TenKhoa = faculty.FacultyName,
                                           DiemTB = sv.AverageScore
                                       }).ToList();

                if (danhSachTimKiem.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy kết quả", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    dgvTimKiem.Rows.Clear();

                    foreach (var sv in danhSachTimKiem)
                    {
                        dgvTimKiem.Rows.Add(sv.MaSV, sv.HoTen, sv.GioiTinh, sv.TenKhoa, sv.DiemTB);
                    }

                    txtKetQuaTimKiem.Text = danhSachTimKiem.Count.ToString();
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên để xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool maSinhVienTonTai = false;

            foreach (DataGridViewRow row in dgvTimKiem.Rows)
            {
                if (row.Cells["MaSV"].Value != null && row.Cells["MaSV"].Value.ToString() == txtMaSV.Text)
                {
                    maSinhVienTonTai = true;
                    break;
                }
            }

            if (!maSinhVienTonTai)
            {
                MessageBox.Show("Mã sinh viên không tồn tại trong hệ thống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (var context = new QuanLySinhVienEntities())
                {
                    var sinhVienCanXoa = context.Student.FirstOrDefault(sv => sv.StudentID == txtMaSV.Text);
                    if (sinhVienCanXoa != null)
                    {
                        context.Student.Remove(sinhVienCanXoa);
                        context.SaveChanges();

                        MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadDataGridView();

                        ResetForm();
                    }
                }
            }
        }

        private void btnTroVe_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ResetForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            cmbKhoa.SelectedIndex = -1; 
            rbFemale.Checked = true;       
        }

    }
}
