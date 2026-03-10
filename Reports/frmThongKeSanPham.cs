using Microsoft.Reporting.WinForms;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entity;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyBanHang.Reports
{
    public partial class frmThongKeSanPham : Form
    {
        private QLBHDbContext context = new QLBHDbContext();
        private QLBHDataSet.DanhSachSanPhamDataTable danhSachSanPhamDataTable =
            new QLBHDataSet.DanhSachSanPhamDataTable();

        public frmThongKeSanPham()
        {
            InitializeComponent();
        }

        private void frmThongKeSanPham_Load(object sender, EventArgs e)
        {
            try
            {
                string reportPath = Path.GetFullPath(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Reports\rptThongKeSanPham.rdlc")
                );

                if (!File.Exists(reportPath))
                {
                    MessageBox.Show("Không tìm thấy file report:\n" + reportPath,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var danhSachSanPham = context.SanPham
                    .Select(r => new
                    {
                        r.ID,
                        r.HangSanXuatID,
                        TenHangSanXuat = r.HangSanXuat.TenHangSanXuat,
                        r.LoaiSanPhamID,
                        TenLoai = r.LoaiSanPham.TenLoai,
                        r.TenSanPham,
                        r.DonGia,
                        r.SoLuong,
                        r.HinhAnh,
                        r.MoTa
                    })
                    .ToList();

                danhSachSanPhamDataTable.Clear();

                foreach (var row in danhSachSanPham)
                {
                    danhSachSanPhamDataTable.AddDanhSachSanPhamRow(
                        row.ID,
                        row.HangSanXuatID,
                        row.TenHangSanXuat,
                        row.LoaiSanPhamID,
                        row.TenLoai,
                        row.TenSanPham,
                        row.DonGia,
                        row.SoLuong,
                        row.HinhAnh,
                        row.MoTa
                    );
                }

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.ReportPath = reportPath;
                reportViewer1.LocalReport.DataSources.Add(
                    new ReportDataSource("DanhSachSanPham", (DataTable)danhSachSanPhamDataTable)
                );

                reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị report: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}