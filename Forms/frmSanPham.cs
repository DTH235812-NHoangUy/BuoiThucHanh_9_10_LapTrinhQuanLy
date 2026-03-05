using ClosedXML.Excel;
using QuanLyBanHang.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QuanLyBanHang.Forms
{
    public partial class frmSanPham : Form
    {
        // 1. Khai báo biến
        QLBHDbContext context = new QLBHDbContext();
        bool xuLyThem = false;
        int id;
        string imagesFolder = Path.Combine(Application.StartupPath, "Images");

        public frmSanPham()
        {
            InitializeComponent();

            // Tạo thư mục Images nếu chưa có
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            // [QUAN TRỌNG NHẤT] Chặn lỗi FormatException của DataGridView
            dgvSanPham.DataError += dgvSanPham_DataError;
        }

        private void dgvSanPham_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Không cho hiện bảng thông báo lỗi màu đỏ khi đổ String vào cột Image
            e.ThrowException = false;
        }

        private void BatTatChucNang(bool giaTri)
        {
            btnLuu.Enabled = giaTri;
            btnHuyBo.Enabled = giaTri;
            cboHangSanXuat.Enabled = giaTri;
            cboLoaiSanPham.Enabled = giaTri;
            txtTenSanPham.Enabled = giaTri;
            numSoLuong.Enabled = giaTri;
            numDonGia.Enabled = giaTri;
            txtMoTa.Enabled = giaTri;
            btnDoiAnh.Enabled = giaTri;

            btnThem.Enabled = !giaTri;
            btnSua.Enabled = !giaTri;
            btnXoa.Enabled = !giaTri;
            btnTimKiem.Enabled = !giaTri;
            btnNhap.Enabled = !giaTri;
            btnXuat.Enabled = !giaTri;
        }

        private void LoadData()
        {
            dgvSanPham.AutoGenerateColumns = false;

            // Lấy dữ liệu từ DB thông qua DTO DanhSachSanPham
            var ds = context.SanPham.Select(r => new DanhSachSanPham
            {
                ID = r.ID,
                LoaiSanPhamID = r.LoaiSanPhamID,
                TenLoai = r.LoaiSanPham.TenLoai,
                HangSanXuatID = r.HangSanXuatID,
                TenHangSanXuat = r.HangSanXuat.TenHangSanXuat,
                TenSanPham = r.TenSanPham,
                SoLuong = r.SoLuong,
                DonGia = r.DonGia,
                HinhAnh = r.HinhAnh, // Chỉ là chuỗi tên file
                MoTa = r.MoTa
            }).ToList();

            BindingSource bs = new BindingSource { DataSource = ds };

            // Clear Bindings cũ
            txtTenSanPham.DataBindings.Clear();
            txtMoTa.DataBindings.Clear();
            numSoLuong.DataBindings.Clear();
            numDonGia.DataBindings.Clear();
            cboLoaiSanPham.DataBindings.Clear();
            cboHangSanXuat.DataBindings.Clear();
            picHinhAnh.DataBindings.Clear();

            // Thiết lập Bindings mới
            txtTenSanPham.DataBindings.Add("Text", bs, "TenSanPham", true, DataSourceUpdateMode.Never);
            txtMoTa.DataBindings.Add("Text", bs, "MoTa", true, DataSourceUpdateMode.Never);
            numSoLuong.DataBindings.Add("Value", bs, "SoLuong", true, DataSourceUpdateMode.Never);
            numDonGia.DataBindings.Add("Value", bs, "DonGia", true, DataSourceUpdateMode.Never);
            cboLoaiSanPham.DataBindings.Add("SelectedValue", bs, "LoaiSanPhamID", true, DataSourceUpdateMode.Never);
            cboHangSanXuat.DataBindings.Add("SelectedValue", bs, "HangSanXuatID", true, DataSourceUpdateMode.Never);

            // Binding ImageLocation cho PictureBox
            Binding imgBind = new Binding("ImageLocation", bs, "HinhAnh", true);
            imgBind.Format += (s, ev) =>
            {
                if (ev.Value != null)
                    ev.Value = Path.Combine(imagesFolder, ev.Value.ToString());
            };
            picHinhAnh.DataBindings.Add(imgBind);

            dgvSanPham.DataSource = bs;
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            // Load danh mục
            cboLoaiSanPham.DataSource = context.LoaiSanPham.ToList();
            cboLoaiSanPham.ValueMember = "ID";
            cboLoaiSanPham.DisplayMember = "TenLoai";

            cboHangSanXuat.DataSource = context.HangSanXuat.ToList();
            cboHangSanXuat.ValueMember = "ID";
            cboHangSanXuat.DisplayMember = "TenHangSanXuat";

            LoadData();
            BatTatChucNang(false);
        }

        private void dgvSanPham_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // [XỬ LÝ HIỂN THỊ ẢNH TRÊN GRID]
            // Tên cột phải trùng với Name bạn đặt trong Designer (vd: colHinhAnh)
            if (dgvSanPham.Columns[e.ColumnIndex].Name == "colHinhAnh" && e.Value != null)
            {
                string fullPath = Path.Combine(imagesFolder, e.Value.ToString());
                if (File.Exists(fullPath))
                {
                    try
                    {
                        // Dùng MemoryStream để không làm khóa file ảnh
                        byte[] bytes = File.ReadAllBytes(fullPath);
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            Image img = Image.FromStream(ms);
                            e.Value = new Bitmap(img, 40, 40); // Tạo hình thu nhỏ 40x40
                        }
                        e.FormattingApplied = true;
                    }
                    catch { e.Value = null; }
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            xuLyThem = true;
            BatTatChucNang(true);
            txtTenSanPham.Clear();
            txtMoTa.Clear();
            numSoLuong.Value = 0;
            numDonGia.Value = 0;
            picHinhAnh.ImageLocation = null;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvSanPham.CurrentRow == null) return;
            id = Convert.ToInt32(dgvSanPham.CurrentRow.Cells["colID"].Value);
            xuLyThem = false;
            BatTatChucNang(true);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string fileNameOnly = !string.IsNullOrEmpty(picHinhAnh.ImageLocation)
                                  ? Path.GetFileName(picHinhAnh.ImageLocation) : null;

            if (xuLyThem)
            {
                SanPham sp = new SanPham
                {
                    TenSanPham = txtTenSanPham.Text,
                    HangSanXuatID = (int)cboHangSanXuat.SelectedValue,
                    LoaiSanPhamID = (int)cboLoaiSanPham.SelectedValue,
                    DonGia = (int)numDonGia.Value,
                    SoLuong = (int)numSoLuong.Value,
                    MoTa = txtMoTa.Text,
                    HinhAnh = fileNameOnly
                };
                context.SanPham.Add(sp);
            }
            else
            {
                var sp = context.SanPham.Find(id);
                if (sp != null)
                {
                    sp.TenSanPham = txtTenSanPham.Text;
                    sp.HangSanXuatID = (int)cboHangSanXuat.SelectedValue;
                    sp.LoaiSanPhamID = (int)cboLoaiSanPham.SelectedValue;
                    sp.DonGia = (int)numDonGia.Value;
                    sp.SoLuong = (int)numSoLuong.Value;
                    sp.MoTa = txtMoTa.Text;
                    if (!string.IsNullOrEmpty(fileNameOnly)) sp.HinhAnh = fileNameOnly;
                }
            }
            context.SaveChanges();
            LoadData();
            BatTatChucNang(false);
        }

        private void btnDoiAnh_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Images|*.jpg;*.png;*.jpeg";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string ext = Path.GetExtension(ofd.FileName);
                    string newFileName = Guid.NewGuid().ToString() + ext; // Tên file ngẫu nhiên
                    string destPath = Path.Combine(imagesFolder, newFileName);

                    File.Copy(ofd.FileName, destPath, true);
                    picHinhAnh.ImageLocation = destPath;
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSanPham.CurrentRow == null) return;
            if (MessageBox.Show("Xóa sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                id = Convert.ToInt32(dgvSanPham.CurrentRow.Cells["colID"].Value);
                var sp = context.SanPham.Find(id);
                if (sp != null)
                {
                    context.SanPham.Remove(sp);
                    context.SaveChanges();
                }
                LoadData();
            }
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            BatTatChucNang(false);
            LoadData();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNhap_Click(object sender, EventArgs e)

        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Nhập dữ liệu Sản Phẩm từ tập tin Excel";
            openFileDialog.Filter = "Tập tin Excel|*.xls;*.xlsx";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable table = new DataTable();
                    using (XLWorkbook workbook = new XLWorkbook(openFileDialog.FileName))
                    {
                        IXLWorksheet worksheet = workbook.Worksheet(1);
                        bool firstRow = true;
                        string readRange = "1:1";

                        foreach (IXLRow row in worksheet.RowsUsed())
                        {
                            if (firstRow)
                            {
                                readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                                foreach (IXLCell cell in row.Cells(readRange))
                                    table.Columns.Add(cell.Value.ToString().Trim());
                                firstRow = false;
                            }
                            else
                            {
                                table.Rows.Add();
                                int cellIndex = 0;
                                foreach (IXLCell cell in row.Cells(readRange))
                                {
                                    table.Rows[table.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                                    cellIndex++;
                                }
                            }
                        }

                        if (table.Rows.Count > 0)
                        {
                            foreach (DataRow r in table.Rows)
                            {
                                SanPham sp = new SanPham();

                                // Các trường bắt buộc
                                sp.TenSanPham = r["TenSanPham"].ToString();
                                sp.HangSanXuatID = int.Parse(r["HangSanXuatID"].ToString());
                                sp.LoaiSanPhamID = int.Parse(r["LoaiSanPhamID"].ToString());
                                sp.DonGia = int.Parse(r["DonGia"].ToString());
                                sp.SoLuong = int.Parse(r["SoLuong"].ToString());

                                // Các trường có thể rỗng (nullable)
                                sp.HinhAnh = r["HinhAnh"]?.ToString();
                                sp.MoTa = r["MoTa"]?.ToString();

                                context.SanPham.Add(sp);
                            }

                            context.SaveChanges();
                            MessageBox.Show("Đã nhập thành công " + table.Rows.Count + " sản phẩm.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Cập nhật lại giao diện (Thay tên hàm Load tương ứng của bạn)
                            frmSanPham_Load(sender, e);
                        }

                        if (firstRow)
                            MessageBox.Show("Tập tin Excel rỗng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi dữ liệu: Hãy đảm bảo HangSanXuatID và LoaiSanPhamID là số nguyên hợp lệ.\n Chi tiết: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnXuat_Click(object sender, EventArgs e)
       
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Xuất dữ liệu Sản Phẩm ra tập tin Excel";
            saveFileDialog.Filter = "Tập tin Excel|*.xls;*.xlsx";
            saveFileDialog.FileName = "SanPham_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable table = new DataTable();
                    // Thiết lập các cột dựa trên class DanhSachSanPham (10 cột)
                    table.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID", typeof(int)),
                new DataColumn("TenSanPham", typeof(string)),
                new DataColumn("TenHangSanXuat", typeof(string)),
                new DataColumn("TenLoai", typeof(string)),
                new DataColumn("DonGia", typeof(int)),
                new DataColumn("SoLuong", typeof(int)),
                new DataColumn("HinhAnh", typeof(string)),
                new DataColumn("MoTa", typeof(string)),
                new DataColumn("HangSanXuatID", typeof(int)),
                new DataColumn("LoaiSanPhamID", typeof(int))
            });

                    // Truy vấn kết hợp (Join) để lấy tên Hãng và tên Loại
                    var danhSach = context.SanPham.Select(p => new DanhSachSanPham
                    {
                        ID = p.ID,
                        TenSanPham = p.TenSanPham,
                        TenHangSanXuat = p.HangSanXuat.TenHangSanXuat, // Lấy từ bảng liên kết
                        TenLoai = p.LoaiSanPham.TenLoai,               // Lấy từ bảng liên kết
                        DonGia = p.DonGia,
                        SoLuong = p.SoLuong,
                        HinhAnh = p.HinhAnh,
                        MoTa = p.MoTa,
                        HangSanXuatID = p.HangSanXuatID,
                        LoaiSanPhamID = p.LoaiSanPhamID
                    }).ToList();

                    if (danhSach != null)
                    {
                        foreach (var p in danhSach)
                        {
                            table.Rows.Add(
                                p.ID,
                                p.TenSanPham,
                                p.TenHangSanXuat,
                                p.TenLoai,
                                p.DonGia,
                                p.SoLuong,
                                p.HinhAnh,
                                p.MoTa,
                                p.HangSanXuatID,
                                p.LoaiSanPhamID
                            );
                        }
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var sheet = wb.Worksheets.Add(table, "SanPham");
                        sheet.Columns().AdjustToContents();
                        wb.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Đã xuất dữ liệu Sản Phẩm ra Excel thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}