using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.WinForms;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuanLyBanHang.Reports
{
    public partial class frmInHoaDon : Form
    {
        private readonly QLBHDbContext context = new QLBHDbContext();

        private readonly QLBHDataSet.DanhSachHoaDon_ChiTietDataTable danhSachHoaDon_ChiTietDataTable
            = new QLBHDataSet.DanhSachHoaDon_ChiTietDataTable();

        private readonly int id;

        private readonly string reportsFolder = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Reports")
        );

        public frmInHoaDon(int maHoaDon = 0)
        {
            InitializeComponent();
            id = maHoaDon;
        }

        private void frmInHoaDon_Load(object sender, EventArgs e)
        {
            try
            {
                string reportPath = Path.Combine(reportsFolder, "rptInHoaDon.rdlc");

                if (!File.Exists(reportPath))
                {
                    MessageBox.Show(
                        "Không tìm thấy file report:\n" + reportPath,
                        "Lỗi đường dẫn report",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                var hoaDon = context.HoaDon
                    .Include(r => r.KhachHang)
                    .Include(r => r.HoaDon_ChiTiet)
                    .ThenInclude(ct => ct.SanPham)
                    .SingleOrDefault(r => r.ID == id);

                if (hoaDon == null)
                {
                    MessageBox.Show(
                        "Không tìm thấy hóa đơn với ID = " + id,
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    Close();
                    return;
                }

                var hoaDonChiTiet = hoaDon.HoaDon_ChiTiet
                    .Select(r => new DanhSachHoaDon_ChiTiet
                    {
                        ID = r.ID,
                        HoaDonID = r.HoaDonID,
                        SanPhamID = r.SanPhamID,
                        TenSanPham = r.SanPham != null ? r.SanPham.TenSanPham : "",
                        SoLuongBan = r.SoLuongBan,
                        DonGiaBan = r.DonGiaBan,
                        ThanhTien = Convert.ToInt32(r.SoLuongBan * r.DonGiaBan)
                    })
                    .ToList();

                danhSachHoaDon_ChiTietDataTable.Clear();

                foreach (var row in hoaDonChiTiet)
                {
                    danhSachHoaDon_ChiTietDataTable.AddDanhSachHoaDon_ChiTietRow(
                        row.ID,
                        row.HoaDonID,
                        row.SanPhamID,
                        row.TenSanPham,
                        row.SoLuongBan,
                        row.DonGiaBan,
                        row.ThanhTien
                    );
                }

                reportViewer1.Reset();

                ReportDataSource reportDataSource = new ReportDataSource
                {
                    Name = "DanhSachHoaDon_ChiTiet",
                    Value = danhSachHoaDon_ChiTietDataTable
                };

                reportViewer1.LocalReport.ReportPath = reportPath;
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(reportDataSource);

                IList<ReportParameter> param = new List<ReportParameter>
                {
                    new ReportParameter(
                        "NgayLap",
                        $"Ngày {hoaDon.NgayLap.Day} tháng {hoaDon.NgayLap.Month} năm {hoaDon.NgayLap.Year}"
                    ),
                    new ReportParameter("NguoiBan_Ten", "CÔNG TY TNHH LAZY ANT"),
                    new ReportParameter("NguoiBan_DiaChi", "Mỹ Phước, TP. Long Xuyên, An Giang"),
                    new ReportParameter("NguoiBan_MaSoThue", "1602162070"),
                    new ReportParameter("NguoiMua_Ten", hoaDon.KhachHang?.HoVaTen ?? ""),
                    new ReportParameter("NguoiMua_DiaChi", hoaDon.KhachHang?.DiaChi ?? ""),
                    new ReportParameter("NguoiMua_MaSoThue", ""),
                    new ReportParameter(
                        "TongTien",
                        hoaDon.HoaDon_ChiTiet.Sum(r => r.SoLuongBan * r.DonGiaBan).ToString()
                    )
                };

                reportViewer1.LocalReport.EnableExternalImages = true;
                reportViewer1.LocalReport.SetParameters(param);
                reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                ShowFullException("Lỗi frmInHoaDon_Load", ex);
            }
        }

        private void ShowFullException(string title, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(title);
            sb.AppendLine(new string('-', 60));

            int level = 0;
            Exception current = ex;

            while (current != null)
            {
                sb.AppendLine($"[Level {level}] {current.GetType().FullName}");
                sb.AppendLine(current.Message);
                sb.AppendLine();

                current = current.InnerException;
                level++;
            }

            MessageBox.Show(
                sb.ToString(),
                "Chi tiết lỗi report",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    public class DanhSachHoaDon_ChiTiet
    {
        public int ID { get; set; }
        public int HoaDonID { get; set; }
        public int SanPhamID { get; set; }
        public string TenSanPham { get; set; } = "";
        public short SoLuongBan { get; set; }
        public int DonGiaBan { get; set; }
        public int ThanhTien { get; set; }
    }
}