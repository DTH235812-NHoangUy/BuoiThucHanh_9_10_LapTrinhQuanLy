using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuanLyBanHang.Forms
{
    public partial class frmHangSanXuat : Form
    {
        QLBHDbContext context = new QLBHDbContext(); // Khởi tạo biến ngữ cảnh CSDL
        bool xuLyThem = false; // Kiểm tra có nhấn vào nút Thêm hay không?
        int id; // Lấy mã hãng sản xuất (dùng cho Sửa và Xóa)

        public frmHangSanXuat()
        {
            InitializeComponent();
        }

        private void BatTatChucNang(bool giaTri)
        {
            btnLuu.Enabled = giaTri;
            btnHuyBo.Enabled = giaTri;
            txtTenHangSanXuat.Enabled = giaTri;
            btnThem.Enabled = !giaTri;
            btnSua.Enabled = !giaTri;
            btnXoa.Enabled = !giaTri;
        }

        private void frmHangSanXuat_Load(object sender, EventArgs e)
        {
            BatTatChucNang(false);
            List<HangSanXuat> hsx = context.HangSanXuat.ToList();
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = hsx;

            txtTenHangSanXuat.DataBindings.Clear();
            txtTenHangSanXuat.DataBindings.Add("Text", bindingSource, "TenHangSanXuat", false, DataSourceUpdateMode.Never);
            dgvHangSanXuat.DataSource = bindingSource;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            xuLyThem = true;
            BatTatChucNang(true);
            txtTenHangSanXuat.DataBindings.Clear(); // Xóa ràng buộc dữ liệu cũ để nhập mới
            txtTenHangSanXuat.Text = ""; // Làm rỗng Textbox
            txtTenHangSanXuat.Focus(); // Đưa con trỏ chuột vào Textbox
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            xuLyThem = false;
            BatTatChucNang(true);
            if (dgvHangSanXuat.CurrentRow != null)
            {
                id = Convert.ToInt32(dgvHangSanXuat.CurrentRow.Cells["ID"].Value.ToString());
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenHangSanXuat.Text))
            {
                MessageBox.Show("Vui lòng nhập tên hãng sản xuất?", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (xuLyThem)
                {
                    HangSanXuat hsx = new HangSanXuat();
                    hsx.TenHangSanXuat = txtTenHangSanXuat.Text;
                    context.HangSanXuat.Add(hsx);
                    context.SaveChanges();
                }
                else
                {
                    HangSanXuat hsx = context.HangSanXuat.Find(id);
                    if (hsx != null)
                    {
                        hsx.TenHangSanXuat = txtTenHangSanXuat.Text;
                        context.HangSanXuat.Update(hsx);
                        context.SaveChanges();
                    }
                }
                // Sau khi lưu xong thì load lại dữ liệu
                frmHangSanXuat_Load(sender, e);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xác nhận xóa hãng sản xuất?", "Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (dgvHangSanXuat.CurrentRow != null)
                {
                    id = Convert.ToInt32(dgvHangSanXuat.CurrentRow.Cells["ID"].Value.ToString());
                    HangSanXuat hsx = context.HangSanXuat.Find(id);
                    if (hsx != null)
                    {
                        context.HangSanXuat.Remove(hsx);
                        context.SaveChanges();
                    }
                    frmHangSanXuat_Load(sender, e);
                }
            }
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            frmHangSanXuat_Load(sender, e);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form hiện tại
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Nhập dữ liệu Hãng Sản Xuất từ tập tin Excel";
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
                                // 1. Đọc dòng tiêu đề để tạo cột cho DataTable
                                if (firstRow)
                                {
                                    readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                                    foreach (IXLCell cell in row.Cells(readRange))
                                    {
                                        table.Columns.Add(cell.Value.ToString().Trim());
                                    }
                                    firstRow = false;
                                }
                                else // 2. Đọc các dòng nội dung
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
                                    // Kiểm tra nếu tên hãng sản xuất không trống
                                    string tenHang = r["TenHangSanXuat"]?.ToString();
                                    if (!string.IsNullOrEmpty(tenHang))
                                    {
                                        HangSanXuat hsx = new HangSanXuat();
                                        hsx.TenHangSanXuat = tenHang;

                                        // Thêm vào context (Entity Framework)
                                        context.HangSanXuat.Add(hsx);
                                    }
                                }

                                context.SaveChanges();
                                MessageBox.Show("Đã nhập thành công " + table.Rows.Count + " hãng sản xuất.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Gọi lại hàm Load để cập nhật GridView
                                frmHangSanXuat_Load(sender, e);
                            }

                            if (firstRow)
                                MessageBox.Show("Tập tin Excel rỗng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void btnXuat_Click(object sender, EventArgs e)

        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Xuất dữ liệu Hãng Sản Xuất ra tập tin Excel";
            saveFileDialog.Filter = "Tập tin Excel|*.xls;*.xlsx";
            // Định dạng tên file: HangSanXuat_Ngày_Tháng_Năm.xlsx
            saveFileDialog.FileName = "HangSanXuat_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable table = new DataTable();
                    // Thiết lập các cột cho DataTable dựa trên thuộc tính của lớp HangSanXuat
                    table.Columns.AddRange(new DataColumn[2] {
                new DataColumn("ID", typeof(int)),
                new DataColumn("TenHangSanXuat", typeof(string))
            });

                    // Lấy danh sách Hãng Sản Xuất từ database thông qua context
                    var danhSachHang = context.HangSanXuat.ToList();

                    if (danhSachHang != null)
                    {
                        foreach (var h in danhSachHang)
                        {
                            // Thêm dữ liệu vào dòng của DataTable
                            table.Rows.Add(h.ID, h.TenHangSanXuat);
                        }
                    }

                    // Sử dụng thư viện ClosedXML để tạo file Excel
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var sheet = wb.Worksheets.Add(table, "HangSanXuat");
                        // Tự động căn chỉnh độ rộng cột theo nội dung
                        sheet.Columns().AdjustToContents();

                        // Lưu file
                        wb.SaveAs(saveFileDialog.FileName);

                        MessageBox.Show("Đã xuất dữ liệu Hãng Sản Xuất ra tập tin Excel thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                Help.ShowHelp(this, "https://dth235812-nhoanguy.github.io/BuoiThucHanh_9_10_LapTrinhQuanLy/");
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
