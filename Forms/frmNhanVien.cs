using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BC = BCrypt.Net.BCrypt;

namespace QuanLyBanHang.Forms
{
    public partial class frmNhanVien : Form
    {
        QLBHDbContext context = new QLBHDbContext();
        bool xuLyThem = false;
        int id;

        private readonly BindingSource bindingSource = new BindingSource();

        public frmNhanVien()
        {
            InitializeComponent();
        }

        private void BatTatChucNang(bool giaTri)
        {
            btnLuu.Enabled = giaTri;
            btnHuybo.Enabled = giaTri;
            txtHoVaTen.Enabled = giaTri;
            txtDienThoai.Enabled = giaTri;
            txtDiaChi.Enabled = giaTri;
            txtTenDangNhap.Enabled = giaTri;
            txtMatKhau.Enabled = giaTri;
            cboQuyenHan.Enabled = giaTri;

            btnThem.Enabled = !giaTri;
            btnSua.Enabled = !giaTri;
            btnXoa.Enabled = !giaTri;
            btnTimKiem.Enabled = !giaTri;
            btnNhap.Enabled = !giaTri;
            btnXuat.Enabled = !giaTri;
        }

        private void LoadData(IEnumerable<NhanVien> items)
        {
            bindingSource.DataSource = items.ToList();

            txtHoVaTen.DataBindings.Clear();
            txtHoVaTen.DataBindings.Add("Text", bindingSource, "HoVaTen", true, DataSourceUpdateMode.OnPropertyChanged);

            txtDienThoai.DataBindings.Clear();
            txtDienThoai.DataBindings.Add("Text", bindingSource, "DienThoai", true, DataSourceUpdateMode.OnPropertyChanged);

            txtDiaChi.DataBindings.Clear();
            txtDiaChi.DataBindings.Add("Text", bindingSource, "DiaChi", true, DataSourceUpdateMode.OnPropertyChanged);

            txtTenDangNhap.DataBindings.Clear();
            txtTenDangNhap.DataBindings.Add("Text", bindingSource, "TenDangNhap", true, DataSourceUpdateMode.OnPropertyChanged);

            cboQuyenHan.DataBindings.Clear();
            cboQuyenHan.DataBindings.Add("SelectedIndex", bindingSource, "QuyenHan", true, DataSourceUpdateMode.OnPropertyChanged);

            dataGridView.DataSource = bindingSource;
        }

        private void frmNhanVien_Load(object sender, EventArgs e)
        {
            BatTatChucNang(false);
            dataGridView.AutoGenerateColumns = false;

            cboQuyenHan.Items.Clear();
            cboQuyenHan.Items.Add("Quản lý");
            cboQuyenHan.Items.Add("Nhân viên");

            LoadData(context.NhanVien.ToList());
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            xuLyThem = true;
            BatTatChucNang(true);
            txtHoVaTen.Clear();
            txtDienThoai.Clear();
            txtDiaChi.Clear();
            txtTenDangNhap.Clear();
            txtMatKhau.Clear();
            cboQuyenHan.SelectedIndex = -1;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            xuLyThem = false;
            BatTatChucNang(true);

            if (bindingSource.Current is NhanVien nv)
            {
                id = nv.ID;
                txtHoVaTen.Text = nv.HoVaTen;
                txtDienThoai.Text = nv.DienThoai;
                txtDiaChi.Text = nv.DiaChi;
                txtTenDangNhap.Text = nv.TenDangNhap;
                cboQuyenHan.SelectedIndex = nv.QuyenHan ? 0 : 1;
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoVaTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên nhân viên?", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập?", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cboQuyenHan.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn quyền hạn cho nhân viên?", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (xuLyThem)
            {
                if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu?", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                NhanVien nv = new NhanVien
                {
                    HoVaTen = txtHoVaTen.Text,
                    DienThoai = txtDienThoai.Text,
                    DiaChi = txtDiaChi.Text,
                    TenDangNhap = txtTenDangNhap.Text,
                    MatKhau = BC.HashPassword(txtMatKhau.Text),
                    QuyenHan = cboQuyenHan.SelectedIndex == 0
                };
                context.NhanVien.Add(nv);
            }
            else
            {
                NhanVien nv = context.NhanVien.Find(id);
                if (nv != null)
                {
                    nv.HoVaTen = txtHoVaTen.Text;
                    nv.DienThoai = txtDienThoai.Text;
                    nv.DiaChi = txtDiaChi.Text;
                    nv.TenDangNhap = txtTenDangNhap.Text;
                    nv.QuyenHan = cboQuyenHan.SelectedIndex == 0;

                    if (!string.IsNullOrEmpty(txtMatKhau.Text))
                        nv.MatKhau = BC.HashPassword(txtMatKhau.Text);
                }
            }

            context.SaveChanges();
            frmNhanVien_Load(sender, e);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current is NhanVien nv)
            {
                if (MessageBox.Show($"Xác nhận xóa nhân viên {nv.HoVaTen}?",
                                    "Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        context.NhanVien.Remove(nv);
                        context.SaveChanges();

                        // Reset lại ID sau khi xóa
                        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('NhanVien', RESEED, 0)");

                        MessageBox.Show("Xóa nhân viên thành công và đã reset lại ID!",
                                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        frmNhanVien_Load(sender, e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa : " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void btnHuybo_Click(object sender, EventArgs e)
        {
            frmNhanVien_Load(sender, e);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string searchTerm = Microsoft.VisualBasic.Interaction.InputBox("Nhập từ khóa tìm kiếm (họ tên, điện thoại, địa chỉ):", "Tìm kiếm nhân viên", "");

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadData(context.NhanVien.ToList());
                BatTatChucNang(false);
                return;
            }

            string lower = searchTerm.ToLower();

            var results = context.NhanVien
                .Where(k =>
                    (k.HoVaTen ?? "").ToLower().Contains(lower) ||
                    (k.DienThoai ?? "").ToLower().Contains(lower) ||
                    (k.DiaChi ?? "").ToLower().Contains(lower))
                .ToList();

            if (results.Count == 0)
            {
                MessageBox.Show("Không tìm thấy kết quả phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(context.NhanVien.ToList());
            }
            else
            {
                LoadData(results);
            }

            BatTatChucNang(false);
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {

            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Nhập dữ liệu Nhân Viên từ tập tin Excel";
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
                                        table.Columns.Add(cell.Value.ToString().Trim());
                                    firstRow = false;
                                }
                                else // 2. Đọc nội dung dữ liệu
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
                                    NhanVien nv = new NhanVien();

                                    // Gán dữ liệu dựa trên tên cột trong file Excel
                                    nv.HoVaTen = r["HoVaTen"].ToString();
                                    nv.DienThoai = r["DienThoai"]?.ToString();
                                    nv.DiaChi = r["DiaChi"]?.ToString();
                                    nv.TenDangNhap = r["TenDangNhap"].ToString();

                                    // Đối với mật khẩu, bạn nên xử lý mã hóa nếu cần
                                    nv.MatKhau = r["MatKhau"].ToString();

                                    // Xử lý kiểu bool cho Quyền hạn (Excel có thể là True/False hoặc 1/0)
                                    bool quyen;
                                    if (bool.TryParse(r["QuyenHan"].ToString(), out quyen))
                                        nv.QuyenHan = quyen;
                                    else
                                        nv.QuyenHan = false; // Mặc định nếu lỗi

                                    context.NhanVien.Add(nv);
                                }

                                context.SaveChanges();
                                MessageBox.Show("Đã nhập thành công " + table.Rows.Count + " nhân viên.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Cập nhật lại GridView hiển thị nhân viên
                                frmNhanVien_Load(sender, e);
                            }

                            if (firstRow)
                                MessageBox.Show("Tập tin Excel rỗng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }
        private void btnXuat_Click(object sender, EventArgs e)
        {

            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Xuất dữ liệu Nhân Viên ra tập tin Excel";
                saveFileDialog.Filter = "Tập tin Excel|*.xls;*.xlsx";
                // Tên file mặc định: NhanVien_Ngày_Tháng_Năm.xlsx
                saveFileDialog.FileName = "NhanVien_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        DataTable table = new DataTable();
                        // Thiết lập 7 cột tương ứng với các thuộc tính của lớp NhanVien
                        table.Columns.AddRange(new DataColumn[7] {
                new DataColumn("ID", typeof(int)),
                new DataColumn("HoVaTen", typeof(string)),
                new DataColumn("DienThoai", typeof(string)),
                new DataColumn("DiaChi", typeof(string)),
                new DataColumn("TenDangNhap", typeof(string)),
                new DataColumn("MatKhau", typeof(string)),
                new DataColumn("QuyenHan", typeof(bool))
            });

                        // Lấy danh sách nhân viên từ Database qua context
                        var danhSachNhanVien = context.NhanVien.ToList();

                        if (danhSachNhanVien != null)
                        {
                            foreach (var nv in danhSachNhanVien)
                            {
                                // Thêm dữ liệu từng dòng vào DataTable
                                table.Rows.Add(
                                    nv.ID,
                                    nv.HoVaTen,
                                    nv.DienThoai,
                                    nv.DiaChi,
                                    nv.TenDangNhap,
                                    nv.MatKhau,
                                    nv.QuyenHan
                                );
                            }
                        }

                        // Sử dụng thư viện ClosedXML (XLWorkbook)
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            var sheet = wb.Worksheets.Add(table, "NhanVien");
                            // Tự động căn chỉnh độ rộng cột theo nội dung
                            sheet.Columns().AdjustToContents();

                            wb.SaveAs(saveFileDialog.FileName);
                            MessageBox.Show("Đã xuất dữ liệu Nhân Viên ra tập tin Excel thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
