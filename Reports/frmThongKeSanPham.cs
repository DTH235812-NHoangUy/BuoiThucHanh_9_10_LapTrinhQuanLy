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
        private readonly QLBHDbContext context = new QLBHDbContext();
        private readonly QLBHDataSet.DanhSachSanPhamDataTable danhSachSanPhamDataTable =
            new QLBHDataSet.DanhSachSanPhamDataTable();

        private readonly string reportsFolder = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Reports")
        );

        public frmThongKeSanPham()
        {
            InitializeComponent();
        }

        public void LayLoaiSanPhamVaoComboBox()
        {
            cboLoaiSanPham.DataSource = context.LoaiSanPham.ToList();
            cboLoaiSanPham.ValueMember = "ID";
            cboLoaiSanPham.DisplayMember = "TenLoai";
            cboLoaiSanPham.SelectedIndex = -1;
        }

        public void LayHangSanXuatVaoComboBox()
        {
            cboHangSanXuat.DataSource = context.HangSanXuat.ToList();
            cboHangSanXuat.ValueMember = "ID";
            cboHangSanXuat.DisplayMember = "TenHangSanXuat";
            cboHangSanXuat.SelectedIndex = -1;
        }

        private void frmThongKeSanPham_Load(object sender, EventArgs e)
        {
            try
            {
                LayLoaiSanPhamVaoComboBox();
                LayHangSanXuatVaoComboBox();

                var danhSachSanPham = context.SanPham
                    .Select(r => new DanhSachSanPham
                    {
                        ID = r.ID,
                        HangSanXuatID = r.HangSanXuatID,
                        TenHangSanXuat = r.HangSanXuat.TenHangSanXuat,
                        LoaiSanPhamID = r.LoaiSanPhamID,
                        TenLoai = r.LoaiSanPham.TenLoai,
                        TenSanPham = r.TenSanPham,
                        DonGia = r.DonGia,
                        SoLuong = r.SoLuong,
                        HinhAnh = r.HinhAnh,
                        MoTa = r.MoTa
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

                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DanhSachSanPham";
                reportDataSource.Value = (DataTable)danhSachSanPhamDataTable;

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                reportViewer1.LocalReport.ReportPath = Path.Combine(reportsFolder, "rptThongKeSanPham.rdlc");

                ReportParameter reportParameter = new ReportParameter("MoTaKetQuaHienThi", "(Tất cả sản phẩm)");
                reportViewer1.LocalReport.SetParameters(reportParameter);

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

        private void btnLocKetQua_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboHangSanXuat.Text == "" && cboLoaiSanPham.Text == "")
                {
                    frmThongKeSanPham_Load(sender, e);
                }
                else
                {
                    var danhSachSanPham = context.SanPham.Select(r => new DanhSachSanPham
                    {
                        ID = r.ID,
                        HangSanXuatID = r.HangSanXuatID,
                        TenHangSanXuat = r.HangSanXuat.TenHangSanXuat,
                        LoaiSanPhamID = r.LoaiSanPhamID,
                        TenLoai = r.LoaiSanPham.TenLoai,
                        TenSanPham = r.TenSanPham,
                        DonGia = r.DonGia,
                        SoLuong = r.SoLuong,
                        HinhAnh = r.HinhAnh,
                        MoTa = r.MoTa
                    });

                    string hangSanXuat = "";
                    string loaiSanPham = "";

                    if (cboHangSanXuat.Text != "")
                    {
                        int hangSanXuatID = Convert.ToInt32(cboHangSanXuat.SelectedValue.ToString());
                        hangSanXuat = "Hãng sản xuất: " + cboHangSanXuat.Text;
                        danhSachSanPham = danhSachSanPham.Where(r => r.HangSanXuatID == hangSanXuatID);
                    }

                    if (cboLoaiSanPham.Text != "")
                    {
                        int loaiSanPhamID = Convert.ToInt32(cboLoaiSanPham.SelectedValue.ToString());
                        loaiSanPham = "Phân loại: " + cboLoaiSanPham.Text;
                        danhSachSanPham = danhSachSanPham.Where(r => r.LoaiSanPhamID == loaiSanPhamID);
                    }

                    danhSachSanPhamDataTable.Clear();

                    foreach (var row in danhSachSanPham.ToList())
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

                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "DanhSachSanPham";
                    reportDataSource.Value = (DataTable)danhSachSanPhamDataTable;

                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer1.LocalReport.ReportPath = Path.Combine(reportsFolder, "rptThongKeSanPham.rdlc");

                    string moTa = "";

                    if (hangSanXuat != "" && loaiSanPham != "")
                        moTa = "(" + hangSanXuat + " - " + loaiSanPham + ")";
                    else if (hangSanXuat != "")
                        moTa = "(" + hangSanXuat + ")";
                    else if (loaiSanPham != "")
                        moTa = "(" + loaiSanPham + ")";
                    else
                        moTa = "(Tất cả sản phẩm)";

                    ReportParameter reportParameter = new ReportParameter("MoTaKetQuaHienThi", moTa);
                    reportViewer1.LocalReport.SetParameters(reportParameter);

                    reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                    reportViewer1.ZoomMode = ZoomMode.Percent;
                    reportViewer1.ZoomPercent = 100;
                    reportViewer1.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lọc dữ liệu: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}