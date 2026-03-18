namespace QuanLyBanHang
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Khởi tạo HelpProvider
            HelpProvider helpProvider = new HelpProvider();

            // Dán link GitHub Pages của bạn vào đây
            helpProvider.HelpNamespace = "https://dth235812-nhoanguy.github.io/BuoiThucHanh9_10_LapTrinhQuanLy/";

            // Cài đặt cho phép Form hiện tại sử dụng HelpProvider này
            helpProvider.SetShowHelp(this, true);
        }
    }
}