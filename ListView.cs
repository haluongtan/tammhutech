using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void add_Click(object sender, EventArgs e)
        {
            //tao 1 listviewitem => dong du lieu
            ListViewItem lvi = new ListViewItem(txtFirstName.Text);
            //them du lieu cho cac cot con lai cua dong 
            lvi.SubItems.Add(txtLastName.Text);
            lvi.SubItems.Add(txtPhone.Text);
            //dua dong du lieu len listview
            lvSinhVien.Items.Add(lvi);
        }

        private void delete_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvSinhVien.SelectedItems)
            {
                lvSinhVien.Items.Remove(lvi);
            }
        }

        private void lvSinhVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSinhVien.SelectedItems.Count > 0)
            {
                //lay dong duoc chon 
                ListViewItem lvi = lvSinhVien.SelectedItems[0];
                //gan tung cot cua dong cho cac gia trij tuong ung
                string fn = lvi.SubItems[0].Text;
                string ln = lvi.SubItems[1].Text;
                string phone = lvi.SubItems[2].Text;
                //gan nguoc len cac text box
                txtFirstName.Text = fn;
                txtLastName.Text = ln;
               
                txtPhone.Text = phone;

              
            }
        }

        private void update_Click(object sender, EventArgs e)
        {
            ListViewItem lvi = lvSinhVien.SelectedItems[0];

            lvi.SubItems[0].Text = txtFirstName.Text;  
            lvi.SubItems[1].Text = txtLastName.Text;  
            lvi.SubItems[2].Text = txtPhone.Text;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
