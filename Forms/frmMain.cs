using QuanLyBanHang.Data.Entity;
using QuanLyBanHang.Reports;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyBanHang.Forms
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        QLBHDbContext context = new QLBHDbContext();
        frmLoaiSanPham loaiSanPham = null;
        frmHangSanXuat hangSanXuat = null;
        frmSanPham sanPham = null;
        frmKhachHang khachHang = null;
        frmNhanVien nhanVien = null;
        frmHoaDon hoaDon = null;
        frmDangNhap dangNhap = null;
        
        frmThongKeSanPham thongKeSanPham = null;
        
        string hoVaTenNhanVien = "";

        private void DangNhap()
        {
        LamLai:
            if (dangNhap == null || dangNhap.IsDisposed)
                dangNhap = new frmDangNhap();

            if (dangNhap.ShowDialog() == DialogResult.OK)
            {
                string tenDangNhap = dangNhap.txtTenDangNhap.Text;
                string matKhau = dangNhap.txtMatKhau.Text;

                if (tenDangNhap.Trim() == "")
                {
                    MessageBox.Show("Tên đăng nhập không được bỏ trống!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dangNhap.txtTenDangNhap.Focus();
                    goto LamLai;
                }
                else if (matKhau.Trim() == "")
                {
                    MessageBox.Show("Mật khẩu không được bỏ trống!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dangNhap.txtMatKhau.Focus();
                    goto LamLai;
                }
                else
                {
                    var nv = context.NhanVien
                                    .Where(r => r.TenDangNhap == tenDangNhap)
                                    .SingleOrDefault();

                    if (nv == null)
                    {
                        MessageBox.Show("Tên đăng nhập không chính xác!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dangNhap.txtTenDangNhap.Focus();
                        goto LamLai;
                    }
                    else
                    {
                        if (BCrypt.Net.BCrypt.Verify(matKhau, nv.MatKhau))
                        {
                            hoVaTenNhanVien = nv.HoVaTen;

                            if (nv.QuyenHan == true)
                                QuyenQuanLy();
                            else
                                QuyenNhanVien();
                        }
                        else
                        {
                            MessageBox.Show("Mật khẩu không chính xác!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dangNhap.txtMatKhau.Focus();
                            goto LamLai;
                        }
                    }
                }
            }
        }

        public void ChuaDangNhap()
        {
            mnuDangNhap.Enabled = true;

            mnuDangXuat.Enabled = false;
            mnuDoiMatKhau.Enabled = false;
            mnuLoaiSanPham.Enabled = false;
            mnuHangSanXuat.Enabled = false;
            mnuSanPham.Enabled = false;
            mnuKhachHang.Enabled = false;
            mnuNhanVien.Enabled = false;
            mnuHoaDon.Enabled = false;
            mnuThongKeSanPham.Enabled = false;
            mnuThongKeDoanhThu.Enabled = false;

            lblTrangThai.Text = "Chưa đăng nhập.";
        }

        public void QuyenQuanLy()
        {
            mnuDangNhap.Enabled = false;

            mnuDangXuat.Enabled = true;
            mnuDoiMatKhau.Enabled = true;
            mnuLoaiSanPham.Enabled = true;
            mnuHangSanXuat.Enabled = true;
            mnuSanPham.Enabled = true;
            mnuKhachHang.Enabled = true;
            mnuNhanVien.Enabled = true;
            mnuHoaDon.Enabled = true;
            mnuThongKeSanPham.Enabled = true;
            mnuThongKeDoanhThu.Enabled = true;

            lblTrangThai.Text = "Quản lý: " + hoVaTenNhanVien;
        }

        public void QuyenNhanVien()
        {
            mnuDangNhap.Enabled = false;

            mnuLoaiSanPham.Enabled = false;
            mnuHangSanXuat.Enabled = false;
            mnuSanPham.Enabled = false;
            mnuNhanVien.Enabled = false;

            mnuDangXuat.Enabled = true;
            mnuDoiMatKhau.Enabled = true;
            mnuKhachHang.Enabled = true;
            mnuHoaDon.Enabled = true;
            mnuThongKeSanPham.Enabled = true;
            mnuThongKeDoanhThu.Enabled = true;

            lblTrangThai.Text = "Nhân viên: " + hoVaTenNhanVien;
        }



        private void mnuLoaiSanPham_Click(object sender, EventArgs e)
        {
            if (loaiSanPham == null || loaiSanPham.IsDisposed)
            {
                loaiSanPham = new frmLoaiSanPham();
                loaiSanPham.MdiParent = this;
                loaiSanPham.Show();
            }
            else
                loaiSanPham.Activate();
        }

        private void mnuHangSanXuat_Click(object sender, EventArgs e)
        {
            if (hangSanXuat == null || hangSanXuat.IsDisposed)
            {
                hangSanXuat = new frmHangSanXuat();
                hangSanXuat.MdiParent = this;
                hangSanXuat.Show();
            }
            else
                hangSanXuat.Activate();
        }

        private void mnuSanPham_Click(object sender, EventArgs e)
        {
            if (sanPham == null || sanPham.IsDisposed)
            {
                sanPham = new frmSanPham();
                sanPham.MdiParent = this;
                sanPham.Show();
            }
            else
                sanPham.Activate();
        }

        private void mnuKhachHang_Click(object sender, EventArgs e)
        {
            if (khachHang == null || khachHang.IsDisposed)
            {
                khachHang = new frmKhachHang();
                khachHang.MdiParent = this;
                khachHang.Show();
            }
            else
                khachHang.Activate();
        }

        private void mnuNhanVien_Click(object sender, EventArgs e)
        {
            if (nhanVien == null || nhanVien.IsDisposed)
            {
                nhanVien = new frmNhanVien();
                nhanVien.MdiParent = this;
                nhanVien.Show();
            }
            else
                nhanVien.Activate();
        }

        private void mnuHoaDon_Click(object sender, EventArgs e)
        {
            if (hoaDon == null || hoaDon.IsDisposed)
            {
                hoaDon = new frmHoaDon();
                hoaDon.MdiParent = this;
                hoaDon.Show();
            }
            else
                hoaDon.Activate();
        }

        private void lblLienKet_Click(object sender, EventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "explorer.exe";
            info.Arguments = "https://fit.agu.edu.vn";
            Process.Start(info);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ChuaDangNhap();
            DangNhap();
        }



        private void mnuDangXuat_Click(object sender, EventArgs e)
        {
            foreach (Form child in MdiChildren)
            {
                child.Close();
            }
            ChuaDangNhap();
        }

        private void mnuDangNhap_Click(object sender, EventArgs e)
        {
            DangNhap();
        }

        private void mnuThongKeSanPham_Click(object sender, EventArgs e)
        {
            if (thongKeSanPham == null || thongKeSanPham.IsDisposed)
            {
                thongKeSanPham = new frmThongKeSanPham();
                thongKeSanPham.MdiParent = this;
                thongKeSanPham.Show();
            }
            else
            {
                thongKeSanPham.Activate();
            }
        }
    }
}