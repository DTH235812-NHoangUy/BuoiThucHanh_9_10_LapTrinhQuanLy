using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyBanHang.Forms
{
    public partial class frmHoaDon : Form
    {
        // Khởi tạo ngữ cảnh CSDL
        QLBHDbContext context = new QLBHDbContext();
        int id;

        public frmHoaDon()
        {
            InitializeComponent();

            // Đăng ký các sự kiện
            this.Load += frmHoaDon_Load;
            this.btnLapHoaDon.Click += btnLapHoaDon_Click;
            this.btnSua.Click += btnSua_Click;
            this.btnXoa.Click += btnXoa_Click;
            this.btnThoat.Click += btnThoat_Click;
            this.dgvHoaDon.CellContentClick += dgvHoaDon_CellContentClick;
            this.btnNhap.Click += btnNhap_Click;
            this.btnXuat.Click += btnXuat_Click;
        }

        private void frmHoaDon_Load(object sender, EventArgs e) => LoadData();

        private void LoadData()
        {
            try
            {
                dgvHoaDon.AutoGenerateColumns = false;
                var hd = context.HoaDon
                    .Include(x => x.NhanVien)
                    .Include(x => x.KhachHang)
                    .Include(x => x.HoaDon_ChiTiet)
                    .Select(r => new
                    {
                        ID = r.ID,
                        // Đã lấy mã nhân viên và mã khách hàng
                        MaNhanVien = r.NhanVienID,
                        MaKhachHang = r.KhachHangID,
                        NgayLap = r.NgayLap,
                        TongTien = r.HoaDon_ChiTiet.Sum(ct => (double)ct.SoLuongBan * ct.DonGiaBan),
                        XemChiTiet = "Xem chi tiết"
                    }).ToList();

                dgvHoaDon.DataSource = hd;

                // [ĐÃ SỬA] Ép buộc gán DataPropertyName theo đúng thứ tự cột trên giao diện
                if (dgvHoaDon.Columns.Count >= 6)
                {
                    dgvHoaDon.Columns[0].DataPropertyName = "ID";           // Cột 1: ID
                    dgvHoaDon.Columns[1].DataPropertyName = "MaNhanVien";   // Cột 2: Mã nhân viên
                    dgvHoaDon.Columns[2].DataPropertyName = "MaKhachHang";  // Cột 3: Mã khách hàng
                    dgvHoaDon.Columns[3].DataPropertyName = "NgayLap";      // Cột 4: Ngày lập
                    dgvHoaDon.Columns[4].DataPropertyName = "TongTien";     // Cột 5: Tổng tiền
                    dgvHoaDon.Columns[5].DataPropertyName = "XemChiTiet";   // Cột 6: Xem chi tiết
                }

                // Định dạng hiển thị số tiền cho cột Tổng tiền (cột số 4 tính từ 0)
                if (dgvHoaDon.Columns.Count >= 5)
                {
                    dgvHoaDon.Columns[4].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi");
            }
        }

        #region XỬ LÝ NHẬP EXCEL
        private void btnNhap_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Excel Files|*.xlsx;*.xls" };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (XLWorkbook workbook = new XLWorkbook(stream))
                        {
                            var hdSheet = workbook.Worksheet("HoaDon");
                            var hdctSheet = workbook.Worksheet("HoaDon_ChiTiet");

                            DataTable dtHoaDon = WorksheetToDataTable(hdSheet);
                            DataTable dtChiTiet = WorksheetToDataTable(hdctSheet);

                            foreach (DataRow r in dtHoaDon.Rows)
                            {
                                string tenNV = r["NhanVien"].ToString().Trim();
                                string tenKH = r["KhachHang"].ToString().Trim();
                                string excelID = r["ID"].ToString();

                                var nv = context.NhanVien.FirstOrDefault(x => x.HoVaTen == tenNV);
                                var kh = context.KhachHang.FirstOrDefault(x => x.HoVaTen == tenKH);

                                if (nv == null || kh == null) continue;

                                HoaDon hd = new HoaDon
                                {
                                    NhanVienID = nv.ID,
                                    KhachHangID = kh.ID,
                                    NgayLap = DateTime.TryParse(r["NgayLap"].ToString(), out var d) ? d : DateTime.Now
                                };

                                var chiTiets = dtChiTiet.AsEnumerable().Where(x => x["ID"].ToString() == excelID);
                                foreach (var ct in chiTiets)
                                {
                                    var sp = context.SanPham.FirstOrDefault(x => x.TenSanPham == ct["TenSanPham"].ToString().Trim());
                                    if (sp != null)
                                    {
                                        hd.HoaDon_ChiTiet.Add(new HoaDon_ChiTiet
                                        {
                                            SanPhamID = sp.ID,
                                            SoLuongBan = (short)Convert.ToInt32(ct["SoLuongBan"]),
                                            DonGiaBan = Convert.ToInt32(ct["DonGiaBan"])
                                        });
                                    }
                                }
                                context.HoaDon.Add(hd);
                            }
                            context.SaveChanges();
                            MessageBox.Show("Nhập dữ liệu thành công!");
                            LoadData();
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi nhập: " + ex.Message); }
            }
        }
        #endregion

        #region XỬ LÝ XUẤT EXCEL
        private void btnXuat_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel Files|*.xlsx", FileName = "BaoCaoHoaDon.xlsx" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var hdData = context.HoaDon.Select(h => new {
                            ID = h.ID,
                            NhanVien = h.NhanVien.HoVaTen,
                            KhachHang = h.KhachHang.HoVaTen,
                            NgayLap = h.NgayLap,
                            TongTien = h.HoaDon_ChiTiet.Sum(c => (double)c.SoLuongBan * c.DonGiaBan)
                        }).ToList();
                        wb.Worksheets.Add(ToDataTable(hdData), "HoaDon");

                        var ctData = context.HoaDon_ChiTiet.Select(c => new {
                            ID = c.HoaDonID,
                            TenSanPham = c.SanPham.TenSanPham,
                            SoLuongBan = c.SoLuongBan,
                            DonGiaBan = c.DonGiaBan
                        }).ToList();
                        wb.Worksheets.Add(ToDataTable(ctData), "HoaDon_ChiTiet");

                        wb.SaveAs(sfd.FileName);
                        MessageBox.Show("Xuất file thành công!");
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi xuất: " + ex.Message); }
            }
        }
        #endregion

        // Helper
        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dt = new DataTable(typeof(T).Name);
            var props = typeof(T).GetProperties();
            foreach (var prop in props) dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++) values[i] = props[i].GetValue(item, null);
                dt.Rows.Add(values);
            }
            return dt;
        }

        private DataTable WorksheetToDataTable(IXLWorksheet worksheet)
        {
            DataTable dt = new DataTable();
            var range = worksheet.RangeUsed();
            bool firstRow = true;
            foreach (var row in range.Rows())
            {
                if (firstRow)
                {
                    foreach (var cell in row.Cells()) dt.Columns.Add(cell.Value.ToString().Trim());
                    firstRow = false;
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < dt.Columns.Count; i++) dr[i] = row.Cell(i + 1).Value.ToString().Trim();
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        private void btnThoat_Click(object sender, EventArgs e) => this.Close();

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvHoaDon.CurrentRow != null && int.TryParse(dgvHoaDon.CurrentRow.Cells[0].Value?.ToString(), out id))
            {
                if (MessageBox.Show("Xác nhận xóa?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var h = context.HoaDon.Include(x => x.HoaDon_ChiTiet).FirstOrDefault(x => x.ID == id);
                    if (h != null) { context.HoaDon.Remove(h); context.SaveChanges(); LoadData(); }
                }
            }
        }

        private void btnLapHoaDon_Click(object sender, EventArgs e)
        {
            using (frmHoaDon_ChiTiet f = new frmHoaDon_ChiTiet()) { f.ShowDialog(); LoadData(); }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvHoaDon.CurrentRow != null && int.TryParse(dgvHoaDon.CurrentRow.Cells[0].Value?.ToString(), out id))
            {
                using (frmHoaDon_ChiTiet f = new frmHoaDon_ChiTiet(id)) { f.ShowDialog(); LoadData(); }
            }
        }

        private void dgvHoaDon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra click vào cột Xem chi tiết (cột số 5)
            if (e.RowIndex >= 0 && e.ColumnIndex == 5)
            {
                id = Convert.ToInt32(dgvHoaDon.Rows[e.RowIndex].Cells[0].Value);
                using (frmHoaDon_ChiTiet f = new frmHoaDon_ChiTiet(id)) { f.ShowDialog(); LoadData(); }
            }
        }
    }
}