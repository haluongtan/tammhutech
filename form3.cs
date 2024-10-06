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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            LoadFacultyToComboBox(); 
            rbFemale.Checked = true;      
            LoadDataGridView();
            CapNhatSoLuongSinhVien();


        }
        private void LoadDataGridView()
        {
    
            dgvSinhVien.ColumnCount = 5;
            dgvSinhVien.Columns[0].Name = "Mã SV";        
            dgvSinhVien.Columns[1].Name = "Họ và Tên";  
            dgvSinhVien.Columns[2].Name = "Giới Tính";  
            dgvSinhVien.Columns[3].Name = "Điểm TB";     
            dgvSinhVien.Columns[4].Name = "Khoa";       
            using (var context = new QuanLySinhVienEntities())
            {
        
                var sinhVienList = (from sv in context.Student
                                    join faculty in context.Faculty on sv.FacultyID equals faculty.FacultyID
                                    select new
                                    {
                                        StudentID = sv.StudentID,
                                        FullName = sv.FullName,
                                        Gender = sv.Gender,
                                        AverageScore = sv.AverageScore,
                                        FacultyName = faculty.FacultyName
                                    }).ToList();

                dgvSinhVien.Rows.Clear();

                foreach (var sinhVien in sinhVienList)
                {
                    dgvSinhVien.Rows.Add(sinhVien.StudentID, sinhVien.FullName, sinhVien.Gender, sinhVien.AverageScore, sinhVien.FacultyName);
                }

                dgvSinhVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void LoadFacultyToComboBox()
        {
            using (var context = new QuanLySinhVienEntities())
            {
                var faculties = context.Faculty.ToList();
                cmbKhoa.DataSource = faculties;
                cmbKhoa.DisplayMember = "FacultyName"; 
                cmbKhoa.ValueMember = "FacultyID"; 
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                Application.Exit();  
            }
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

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtMaSV.Text, @"^\d{10}$"))
            {
                MessageBox.Show("Mã số sinh viên không hợp lệ. Mã SV phải là 10 ký tự số.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtHoTen.Text, @"^[\p{L}\s]{3,100}$"))
            {
                MessageBox.Show("Tên sinh viên không hợp lệ. Tên chỉ chứa ký tự chữ và phải có độ dài từ 3 đến 100 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            decimal diemTB;
            if (!decimal.TryParse(txtDTB.Text, out diemTB) || diemTB < 0 || diemTB > 10)
            {
                MessageBox.Show("Điểm trung bình không hợp lệ. Điểm phải là số thập phân trong khoảng từ 0 đến 10.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraThongTin())
            {
                return;
            }

            using (var context = new QuanLySinhVienEntities())
            {
                var sinhVienTonTai = context.Student.FirstOrDefault(sv => sv.StudentID == txtMaSV.Text);

                if (sinhVienTonTai == null)
                {
                    var sinhVienMoi = new Student
                    {
                        StudentID = txtMaSV.Text,
                        FullName = txtHoTen.Text,
                        AverageScore = float.Parse(txtDTB.Text),
                        FacultyID = (int)cmbKhoa.SelectedValue,
                        Gender = rbMale.Checked ? "Male" : "Female"
                    };

                    context.Student.Add(sinhVienMoi);
                    context.SaveChanges();
                    MessageBox.Show("Thêm mới dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    sinhVienTonTai.FullName = txtHoTen.Text;
                    sinhVienTonTai.AverageScore = float.Parse(txtDTB.Text);
                    sinhVienTonTai.FacultyID = (int)cmbKhoa.SelectedValue;
                    sinhVienTonTai.Gender = rbMale.Checked ? "Male" : "Female";

                    context.SaveChanges();
                    MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            LoadDataGridView();
            CapNhatSoLuongSinhVien();
            ResetForm();
        }
        private void ResetForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtDTB.Clear();
            cmbKhoa.SelectedIndex = 0;
            rbFemale.Checked = true;
        }

        private void dgvSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];

                txtMaSV.Text = row.Cells["Mã SV"].Value.ToString();
                txtHoTen.Text = row.Cells["Họ và Tên"].Value.ToString();
                txtDTB.Text = row.Cells["Điểm TB"].Value.ToString();

                if (row.Cells["Giới Tính"].Value.ToString() == "Male")
                {
                    rbMale.Checked = true;
                }
                else
                {
                    rbFemale.Checked = true;
                }

                cmbKhoa.SelectedIndex = cmbKhoa.FindStringExact(row.Cells["Khoa"].Value.ToString());
            }
        }
        private void CapNhatSoLuongSinhVien()
        {
            using (var context = new QuanLySinhVienEntities())
            {
                int soLuongNam = context.Student.Count(sv => sv.Gender == "Male");

                int soLuongNu = context.Student.Count(sv => sv.Gender == "Female");

                txtTongNam.Text = soLuongNam.ToString();
                txtTongNu.Text = soLuongNu.ToString();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new QuanLySinhVienEntities())
            {
                var sinhVienCanXoa = context.Student.FirstOrDefault(sv => sv.StudentID == txtMaSV.Text);
                if (sinhVienCanXoa == null)
                {
                    MessageBox.Show("Mã sinh viên không tồn tại trong hệ thống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    context.Student.Remove(sinhVienCanXoa);
                    context.SaveChanges();
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadDataGridView();
                    CapNhatSoLuongSinhVien();
                    ResetForm();
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!KiemTraThongTin())
            {
                return;
            }

            using (var context = new QuanLySinhVienEntities())
            {
                var sinhVienTonTai = context.Student.FirstOrDefault(sv => sv.StudentID == txtMaSV.Text);

                if (sinhVienTonTai == null)
                {
                    var sinhVienMoi = new Student
                    {
                        StudentID = txtMaSV.Text,
                        FullName = txtHoTen.Text,
                        AverageScore = float.Parse(txtDTB.Text),
                        FacultyID = (int)cmbKhoa.SelectedValue,
                        Gender = rbMale.Checked ? "Male" : "Female"
                    };

                    context.Student.Add(sinhVienMoi);
                    context.SaveChanges();
                    MessageBox.Show("Thêm mới dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    sinhVienTonTai.FullName = txtHoTen.Text;
                    sinhVienTonTai.AverageScore = float.Parse(txtDTB.Text);
                    sinhVienTonTai.FacultyID = (int)cmbKhoa.SelectedValue;
                    sinhVienTonTai.Gender = rbMale.Checked ? "Male" : "Female";

                    context.SaveChanges();
                    MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            LoadDataGridView();
            CapNhatSoLuongSinhVien();
            ResetForm();
        }

        private void toolStripQuanLyKhoa_Click(object sender, EventArgs e)
        {
            frmQuanLyKhoa Khoa = new frmQuanLyKhoa();
            Khoa.ShowDialog();
        }

        private void toolStripTimKiem_Click(object sender, EventArgs e)
        {
            frmTimKiem timKiem = new frmTimKiem();
            timKiem.ShowDialog();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
