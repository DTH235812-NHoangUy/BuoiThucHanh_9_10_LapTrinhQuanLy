namespace QuanLyBanHang.Reports
{
    partial class frmThongKeSanPham
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelTop = new Panel();
            btnLocKetQua = new Button();
            cboLoaiSanPham = new ComboBox();
            lblLoaiSanPham = new Label();
            cboHangSanXuat = new ComboBox();
            lblHangSanXuat = new Label();
            reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            panelTop.SuspendLayout();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.Controls.Add(btnLocKetQua);
            panelTop.Controls.Add(cboLoaiSanPham);
            panelTop.Controls.Add(lblLoaiSanPham);
            panelTop.Controls.Add(cboHangSanXuat);
            panelTop.Controls.Add(lblHangSanXuat);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(1235, 80);
            panelTop.TabIndex = 0;
            // 
            // btnLocKetQua
            // 
            btnLocKetQua.Location = new Point(917, 24);
            btnLocKetQua.Name = "btnLocKetQua";
            btnLocKetQua.Size = new Size(132, 32);
            btnLocKetQua.TabIndex = 4;
            btnLocKetQua.Text = "Lọc kết quả";
            btnLocKetQua.UseVisualStyleBackColor = true;
            btnLocKetQua.Click += btnLocKetQua_Click;
            // 
            // cboLoaiSanPham
            // 
            cboLoaiSanPham.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLoaiSanPham.FormattingEnabled = true;
            cboLoaiSanPham.Location = new Point(669, 26);
            cboLoaiSanPham.Name = "cboLoaiSanPham";
            cboLoaiSanPham.Size = new Size(220, 28);
            cboLoaiSanPham.TabIndex = 3;
            // 
            // lblLoaiSanPham
            // 
            lblLoaiSanPham.AutoSize = true;
            lblLoaiSanPham.Location = new Point(545, 30);
            lblLoaiSanPham.Name = "lblLoaiSanPham";
            lblLoaiSanPham.Size = new Size(113, 20);
            lblLoaiSanPham.TabIndex = 2;
            lblLoaiSanPham.Text = "Loại sản phẩm:";
            // 
            // cboHangSanXuat
            // 
            cboHangSanXuat.DropDownStyle = ComboBoxStyle.DropDownList;
            cboHangSanXuat.FormattingEnabled = true;
            cboHangSanXuat.Location = new Point(291, 26);
            cboHangSanXuat.Name = "cboHangSanXuat";
            cboHangSanXuat.Size = new Size(220, 28);
            cboHangSanXuat.TabIndex = 1;
            // 
            // lblHangSanXuat
            // 
            lblHangSanXuat.AutoSize = true;
            lblHangSanXuat.Location = new Point(164, 30);
            lblHangSanXuat.Name = "lblHangSanXuat";
            lblHangSanXuat.Size = new Size(111, 20);
            lblHangSanXuat.TabIndex = 0;
            lblHangSanXuat.Text = "Hãng sản xuất:";
            // 
            // reportViewer1
            // 
            reportViewer1.Dock = DockStyle.Fill;
            reportViewer1.Location = new Point(0, 80);
            reportViewer1.Name = "reportViewer1";
            reportViewer1.ServerReport.BearerToken = null;
            reportViewer1.Size = new Size(1235, 370);
            reportViewer1.TabIndex = 1;
            // 
            // frmThongKeSanPham
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1235, 450);
            Controls.Add(reportViewer1);
            Controls.Add(panelTop);
            Name = "frmThongKeSanPham";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Thống kê sản phẩm";
            Load += frmThongKeSanPham_Load;
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTop;
        private Label lblHangSanXuat;
        private ComboBox cboHangSanXuat;
        private Label lblLoaiSanPham;
        private ComboBox cboLoaiSanPham;
        private Button btnLocKetQua;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
    }
}