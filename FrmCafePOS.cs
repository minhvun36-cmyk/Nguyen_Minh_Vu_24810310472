using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace CafePOSSystem
{
    public class FrmCafePOS : Form
    {
        private FlowLayoutPanel flpTables;
        private ComboBox cboCategory, cboFood;
        private NumericUpDown nudQuantity;
        private DataGridView dgvInvoice;
        private Label lblSelectedTable, lblTotal;
        private Button btnAdd, btnPay, btnTransfer, btnRefresh, btnMenu;
        private int _selectedTableId = 0;
        private int _currentInvoiceId = 0;
        private DataTable _invoiceTable;

        public FrmCafePOS()
        {
            Text = "Cafe POS System - Quản lý quán cafe";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1250;
            Height = 760;
            MinimumSize = new Size(1000, 650);
            Font = new Font("Segoe UI", 9F);
            BuildUI();
            Load += (s, e) => { LoadCategories(); LoadTables(); };
        }

        private void BuildUI()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(8) };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
            Controls.Add(root);

            var left = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
            root.Controls.Add(left, 0, 0);
            var lblTables = new Label { Text = "DANH SÁCH BÀN", Dock = DockStyle.Top, Height = 35, Font = new Font(Font, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
            left.Controls.Add(lblTables);
            flpTables = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(4) };
            left.Controls.Add(flpTables);
            var legend = new Label { Text = "■ Trống     ■ Có khách", Dock = DockStyle.Bottom, Height = 28, ForeColor = Color.DimGray };
            left.Controls.Add(legend);

            var right = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(8, 0, 0, 0) };
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 105));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            root.Controls.Add(right, 1, 0);

            lblSelectedTable = new Label { Text = "Chưa chọn bàn", Dock = DockStyle.Fill, Font = new Font(Font.FontFamily, 16, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
            right.Controls.Add(lblSelectedTable, 0, 0);

            var orderPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 2, Padding = new Padding(0, 4, 0, 4) };
            orderPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            orderPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
            orderPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 85));
            orderPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
            orderPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            orderPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            orderPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            right.Controls.Add(orderPanel, 0, 1);
            orderPanel.Controls.Add(new Label { Text = "Danh mục", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            orderPanel.Controls.Add(new Label { Text = "Món", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 1, 0);
            orderPanel.Controls.Add(new Label { Text = "Số lượng", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 2, 0);
            cboCategory = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cboFood = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            nudQuantity = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1, Maximum = 999, Value = 1 };
            btnAdd = new Button { Text = "Thêm món", Dock = DockStyle.Fill, BackColor = Color.LightSteelBlue };
            orderPanel.Controls.Add(cboCategory, 0, 1);
            orderPanel.Controls.Add(cboFood, 1, 1);
            orderPanel.Controls.Add(nudQuantity, 2, 1);
            orderPanel.Controls.Add(btnAdd, 3, 1);
            btnRefresh = new Button { Text = "Làm mới", Dock = DockStyle.Fill };
            orderPanel.Controls.Add(btnRefresh, 4, 1);
            cboCategory.SelectedIndexChanged += (s, e) => LoadFoods();
            btnAdd.Click += (s, e) => AddFood();
            btnRefresh.Click += (s, e) => { LoadTables(); if (_selectedTableId > 0) LoadInvoice(_selectedTableId); };

            dgvInvoice = new DataGridView
            {
                Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false, RowHeadersVisible = false, BackgroundColor = Color.White
            };
            dgvInvoice.Columns.Add("MaMon", "Mã món");
            dgvInvoice.Columns.Add("TenMon", "Tên món");
            dgvInvoice.Columns.Add("SoLuong", "Số lượng");
            dgvInvoice.Columns.Add("DonGia", "Đơn giá");
            dgvInvoice.Columns.Add("ThanhTien", "Thành tiền");
            dgvInvoice.Columns["MaMon"].Visible = false;
            dgvInvoice.Columns["TenMon"].FillWeight = 160;
            right.Controls.Add(dgvInvoice, 0, 2);

            var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 2, Padding = new Padding(0, 8, 0, 0) };
            bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));
            bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            bottom.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            bottom.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            right.Controls.Add(bottom, 0, 3);
            lblTotal = new Label { Text = "Tổng tiền: 0 đ", Dock = DockStyle.Fill, Font = new Font(Font.FontFamily, 14, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
            bottom.Controls.Add(lblTotal, 0, 0);
            btnTransfer = new Button { Text = "Chuyển bàn", Dock = DockStyle.Fill };
            btnPay = new Button { Text = "Thanh toán", Dock = DockStyle.Fill, BackColor = Color.PaleGreen, Font = new Font(Font, FontStyle.Bold) };
            btnMenu = new Button { Text = "Quản lý thực đơn", Dock = DockStyle.Fill };
            bottom.Controls.Add(btnTransfer, 1, 0);
            bottom.Controls.Add(btnPay, 2, 0);
            bottom.Controls.Add(btnMenu, 0, 1);
            bottom.SetColumnSpan(btnMenu, 2);
            var btnExit = new Button { Text = "Thoát", Dock = DockStyle.Fill };
            bottom.Controls.Add(btnExit, 2, 1);
            btnTransfer.Click += (s, e) => TransferTable();
            btnPay.Click += (s, e) => PayInvoice();
            btnMenu.Click += (s, e) => { using (var f = new FrmMenu()) f.ShowDialog(this); LoadCategories(); LoadFoods(); };
            btnExit.Click += (s, e) => Close();
        }

        private void LoadCategories()
        {
            try
            {
                DataTable dt = Db.Query("SELECT MaDanhMuc, TenDanhMuc FROM dbo.DanhMuc ORDER BY TenDanhMuc");
                cboCategory.SelectedIndexChanged -= CategoryChanged;
                cboCategory.DataSource = dt;
                cboCategory.DisplayMember = "TenDanhMuc";
                cboCategory.ValueMember = "MaDanhMuc";
                cboCategory.SelectedIndexChanged += CategoryChanged;
                LoadFoods();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void CategoryChanged(object sender, EventArgs e) { LoadFoods(); }

        private void LoadFoods()
        {
            if (cboFood == null) return;
            try
            {
                if (cboCategory.SelectedValue == null || !(cboCategory.SelectedValue is int))
                {
                    cboFood.DataSource = null;
                    return;
                }
                DataTable dt = Db.Query("SELECT MaMon, TenMon, DonGia FROM dbo.MonAn WHERE MaDanhMuc=@id AND DangBan=1 ORDER BY TenMon",
                    Db.P("@id", SqlDbType.Int, Convert.ToInt32(cboCategory.SelectedValue)));
                cboFood.DataSource = dt;
                cboFood.DisplayMember = "TenMon";
                cboFood.ValueMember = "MaMon";
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void LoadTables()
        {
            try
            {
                DataTable dt = Db.Query("SELECT MaBan, TenBan, TrangThai FROM dbo.BanAn ORDER BY MaBan");
                flpTables.Controls.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    int id = Convert.ToInt32(row["MaBan"]);
                    string name = row["TenBan"].ToString();
                    string status = row["TrangThai"].ToString();
                    Button b = new Button
                    {
                        Width = 105, Height = 75, Margin = new Padding(6), Tag = id,
                        Text = name + Environment.NewLine + status,
                        BackColor = status == "Trống" ? Color.Honeydew : Color.LightCoral,
                        FlatStyle = FlatStyle.Flat
                    };
                    b.FlatAppearance.BorderSize = 1;
                    b.Click += (s, e) => SelectTable(id, name);
                    flpTables.Controls.Add(b);
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void SelectTable(int id, string name)
        {
            _selectedTableId = id;
            lblSelectedTable.Text = "Đang chọn: " + name;
            LoadInvoice(id);
        }

        private void LoadInvoice(int tableId)
        {
            try
            {
                object inv = Db.Scalar("SELECT TOP 1 MaHoaDon FROM dbo.HoaDon WHERE MaBan=@id AND TrangThai=N'Chưa thanh toán' ORDER BY MaHoaDon DESC",
                    Db.P("@id", SqlDbType.Int, tableId));
                _currentInvoiceId = inv == null || inv == DBNull.Value ? 0 : Convert.ToInt32(inv);
                dgvInvoice.Rows.Clear();
                decimal total = 0;
                if (_currentInvoiceId > 0)
                {
                    _invoiceTable = Db.Query(@"SELECT ct.MaMon, m.TenMon, ct.SoLuong, ct.DonGia, ct.ThanhTien
                                               FROM dbo.ChiTietHoaDon ct INNER JOIN dbo.MonAn m ON ct.MaMon=m.MaMon
                                               WHERE ct.MaHoaDon=@id ORDER BY m.TenMon",
                        Db.P("@id", SqlDbType.Int, _currentInvoiceId));
                    foreach (DataRow row in _invoiceTable.Rows)
                    {
                        int i = dgvInvoice.Rows.Add(row["MaMon"], row["TenMon"], row["SoLuong"], row["DonGia"], row["ThanhTien"]);
                        total += Convert.ToDecimal(row["ThanhTien"]);
                        dgvInvoice.Rows[i].Cells["DonGia"].Value = Convert.ToDecimal(row["DonGia"]).ToString("N0");
                        dgvInvoice.Rows[i].Cells["ThanhTien"].Value = Convert.ToDecimal(row["ThanhTien"]).ToString("N0");
                    }
                }
                lblTotal.Text = "Tổng tiền: " + total.ToString("N0") + " đ";
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void AddFood()
        {
            if (_selectedTableId == 0) { MessageBox.Show("Vui lòng chọn bàn trước."); return; }
            if (cboFood.SelectedValue == null || !(cboFood.SelectedValue is int))
            { MessageBox.Show("Vui lòng chọn món đang bán."); return; }
            int foodId = Convert.ToInt32(cboFood.SelectedValue);
            int qty = Convert.ToInt32(nudQuantity.Value);
            using (SqlConnection conn = Db.GetConnection())
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    int invoiceId;
                    using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 1 MaHoaDon FROM dbo.HoaDon WITH (UPDLOCK,HOLDLOCK)
                        WHERE MaBan=@table AND TrangThai=N'Chưa thanh toán' ORDER BY MaHoaDon DESC", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@table", _selectedTableId);
                        object result = cmd.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            using (SqlCommand create = new SqlCommand(@"INSERT INTO dbo.HoaDon(MaBan) VALUES(@table); SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tran))
                            {
                                create.Parameters.AddWithValue("@table", _selectedTableId);
                                invoiceId = Convert.ToInt32(create.ExecuteScalar());
                            }
                        }
                        else invoiceId = Convert.ToInt32(result);
                    }

                    decimal price;
                    using (SqlCommand cmd = new SqlCommand("SELECT DonGia FROM dbo.MonAn WHERE MaMon=@food AND DangBan=1", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@food", foodId);
                        object p = cmd.ExecuteScalar();
                        if (p == null || p == DBNull.Value) throw new Exception("Món này không còn được bán.");
                        price = Convert.ToDecimal(p);
                    }
                    using (SqlCommand cmd = new SqlCommand(@"IF EXISTS(SELECT 1 FROM dbo.ChiTietHoaDon WHERE MaHoaDon=@inv AND MaMon=@food)
                        UPDATE dbo.ChiTietHoaDon SET SoLuong=SoLuong+@qty WHERE MaHoaDon=@inv AND MaMon=@food
                        ELSE INSERT INTO dbo.ChiTietHoaDon(MaHoaDon,MaMon,SoLuong,DonGia) VALUES(@inv,@food,@qty,@price)", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@inv", invoiceId);
                        cmd.Parameters.AddWithValue("@food", foodId);
                        cmd.Parameters.AddWithValue("@qty", qty);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand("UPDATE dbo.BanAn SET TrangThai=N'Có khách' WHERE MaBan=@id", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@id", _selectedTableId);
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                    _currentInvoiceId = invoiceId;
                    LoadTables();
                    LoadInvoice(_selectedTableId);
                }
                catch
                {
                    try { tran.Rollback(); } catch { }
                    throw;
                }
            }
            nudQuantity.Value = 1;
        }

        private void PayInvoice()
        {
            if (_selectedTableId == 0 || _currentInvoiceId == 0)
            { MessageBox.Show("Bàn này chưa có hóa đơn để thanh toán."); return; }
            if (dgvInvoice.Rows.Count == 0)
            { MessageBox.Show("Hóa đơn chưa có món."); return; }

            using (SqlConnection conn = Db.GetConnection())
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    decimal total;
                    using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(SoLuong*DonGia),0) FROM dbo.ChiTietHoaDon WHERE MaHoaDon=@id", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@id", _currentInvoiceId);
                        total = Convert.ToDecimal(cmd.ExecuteScalar());
                    }
                    using (SqlCommand cmd = new SqlCommand(@"UPDATE dbo.HoaDon SET TongTien=@total, TrangThai=N'Đã thanh toán', ThoiGianThanhToan=GETDATE()
                        WHERE MaHoaDon=@id AND TrangThai=N'Chưa thanh toán'", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@total", total);
                        cmd.Parameters.AddWithValue("@id", _currentInvoiceId);
                        if (cmd.ExecuteNonQuery() == 0) throw new Exception("Hóa đơn đã được thanh toán hoặc không còn tồn tại.");
                    }
                    using (SqlCommand cmd = new SqlCommand("UPDATE dbo.BanAn SET TrangThai=N'Trống' WHERE MaBan=@id", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@id", _selectedTableId);
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                    MessageBox.Show("Thanh toán thành công!\nMã hóa đơn: " + _currentInvoiceId + "\nTổng tiền: " + total.ToString("N0") + " đ",
                        "Thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadTables();
                    LoadInvoice(_selectedTableId);
                }
                catch (Exception ex)
                {
                    try { tran.Rollback(); } catch { }
                    ShowError(ex);
                }
            }
        }

        private void TransferTable()
        {
            if (_selectedTableId == 0 || _currentInvoiceId == 0)
            { MessageBox.Show("Hãy chọn bàn đang có hóa đơn chưa thanh toán."); return; }

            DataTable targets = Db.Query("SELECT MaBan, TenBan FROM dbo.BanAn WHERE TrangThai=N'Trống' AND MaBan<>@id ORDER BY MaBan",
                Db.P("@id", SqlDbType.Int, _selectedTableId));
            if (targets.Rows.Count == 0) { MessageBox.Show("Không còn bàn trống để chuyển."); return; }

            using (Form f = new Form { Text = "Chọn bàn đích", Width = 330, Height = 150, StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false })
            {
                ComboBox cb = new ComboBox { Left = 15, Top = 15, Width = 280, DropDownStyle = ComboBoxStyle.DropDownList, DataSource = targets, DisplayMember = "TenBan", ValueMember = "MaBan" };
                Button ok = new Button { Text = "Chuyển", Left = 120, Top = 55, Width = 80, DialogResult = DialogResult.OK };
                f.Controls.Add(cb); f.Controls.Add(ok); f.AcceptButton = ok;
                if (f.ShowDialog(this) != DialogResult.OK || cb.SelectedValue == null) return;
                int targetId = Convert.ToInt32(cb.SelectedValue);
                using (SqlConnection conn = Db.GetConnection())
                {
                    conn.Open();
                    SqlTransaction tran = conn.BeginTransaction();
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE dbo.HoaDon SET MaBan=@target WHERE MaHoaDon=@inv AND TrangThai=N'Chưa thanh toán'", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@target", targetId);
                            cmd.Parameters.AddWithValue("@inv", _currentInvoiceId);
                            if (cmd.ExecuteNonQuery() == 0) throw new Exception("Không thể chuyển hóa đơn này.");
                        }
                        using (SqlCommand cmd = new SqlCommand("UPDATE dbo.BanAn SET TrangThai=CASE WHEN MaBan=@source THEN N'Trống' WHEN MaBan=@target THEN N'Có khách' ELSE TrangThai END WHERE MaBan IN (@source,@target)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@source", _selectedTableId);
                            cmd.Parameters.AddWithValue("@target", targetId);
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        try { tran.Rollback(); } catch { }
                        throw;
                    }
                }
                _selectedTableId = targetId;
                LoadTables();
                SelectTable(targetId, targets.Select("MaBan=" + targetId)[0]["TenBan"].ToString());
            }
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show("Có lỗi xảy ra:\n" + ex.Message + "\n\nHãy kiểm tra kết nối SQL Server và đã chạy file CafePOSDB.sql chưa.",
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
