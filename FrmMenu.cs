using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace CafePOSSystem
{
    public class FrmMenu : Form
    {
        private DataGridView dgvFoods, dgvCategories;
        private TextBox txtFoodName, txtCategoryName;
        private ComboBox cboCategory;
        private NumericUpDown nudPrice;
        private CheckBox chkSelling;
        private int _selectedFoodId = 0, _selectedCategoryId = 0;
        private Button btnNewFood, btnSaveFood, btnDeleteFood, btnClearFood;
        private Button btnNewCategory, btnSaveCategory, btnDeleteCategory, btnClearCategory;

        public FrmMenu()
        {
            Text = "Quản lý thực đơn và danh mục";
            StartPosition = FormStartPosition.CenterParent;
            Width = 1050; Height = 680; MinimumSize = new Size(900, 580);
            Font = new Font("Segoe UI", 9F);
            BuildUI();
            Load += (s, e) => { LoadCategories(); LoadFoods(); };
        }

        private void BuildUI()
        {
            TabControl tabs = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tabs);
            TabPage foodTab = new TabPage("Quản lý món");
            TabPage catTab = new TabPage("Quản lý danh mục");
            tabs.TabPages.Add(foodTab); tabs.TabPages.Add(catTab);

            var foodRoot = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(10) };
            foodRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
            foodRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
            foodTab.Controls.Add(foodRoot);

            var editor = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 2, RowCount = 5, Height = 235 };
            editor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
            editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foodRoot.Controls.Add(editor, 0, 0);
            editor.Controls.Add(new Label { Text = "Tên món", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
            txtFoodName = new TextBox { Dock = DockStyle.Fill, MaxLength = 150 };
            editor.Controls.Add(txtFoodName, 1, 0);
            editor.Controls.Add(new Label { Text = "Danh mục", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 1);
            cboCategory = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            editor.Controls.Add(cboCategory, 1, 1);
            editor.Controls.Add(new Label { Text = "Đơn giá (VNĐ)", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 2);
            nudPrice = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 0, Maximum = 100000000, DecimalPlaces = 0, ThousandsSeparator = true, Increment = 1000 };
            editor.Controls.Add(nudPrice, 1, 2);
            editor.Controls.Add(new Label { Text = "Trạng thái", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 3);
            chkSelling = new CheckBox { Text = "Đang bán", Checked = true, Dock = DockStyle.Fill };
            editor.Controls.Add(chkSelling, 1, 3);
            var foodButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            btnNewFood = new Button { Text = "Thêm mới", Width = 85 };
            btnSaveFood = new Button { Text = "Lưu", Width = 70 };
            btnDeleteFood = new Button { Text = "Xóa", Width = 70 };
            btnClearFood = new Button { Text = "Làm trống", Width = 90 };
            foodButtons.Controls.AddRange(new Control[] { btnNewFood, btnSaveFood, btnDeleteFood, btnClearFood });
            editor.Controls.Add(foodButtons, 0, 4); editor.SetColumnSpan(foodButtons, 2);

            dgvFoods = MakeGrid();
            foodRoot.Controls.Add(dgvFoods, 1, 0);
            foodRoot.SetRowSpan(dgvFoods, 2);
            btnNewFood.Click += (s, e) => ClearFoodForm();
            btnClearFood.Click += (s, e) => ClearFoodForm();
            btnSaveFood.Click += (s, e) => SaveFood();
            btnDeleteFood.Click += (s, e) => DeleteFood();
            dgvFoods.SelectionChanged += (s, e) => FillFoodForm();

            var catRoot = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(10) };
            catRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            catRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            catTab.Controls.Add(catRoot);
            var catEditor = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 2, RowCount = 3, Height = 150 };
            catEditor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            catEditor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            catRoot.Controls.Add(catEditor, 0, 0);
            catEditor.Controls.Add(new Label { Text = "Tên danh mục", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            txtCategoryName = new TextBox { Dock = DockStyle.Fill, MaxLength = 100 };
            catEditor.Controls.Add(txtCategoryName, 1, 0);
            var catButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            btnNewCategory = new Button { Text = "Thêm mới", Width = 85 };
            btnSaveCategory = new Button { Text = "Lưu", Width = 70 };
            btnDeleteCategory = new Button { Text = "Xóa", Width = 70 };
            btnClearCategory = new Button { Text = "Làm trống", Width = 90 };
            catButtons.Controls.AddRange(new Control[] { btnNewCategory, btnSaveCategory, btnDeleteCategory, btnClearCategory });
            catEditor.Controls.Add(catButtons, 0, 1); catEditor.SetColumnSpan(catButtons, 2);
            dgvCategories = MakeGrid();
            catRoot.Controls.Add(dgvCategories, 1, 0);
            catRoot.SetRowSpan(dgvCategories, 2);
            btnNewCategory.Click += (s, e) => ClearCategoryForm();
            btnClearCategory.Click += (s, e) => ClearCategoryForm();
            btnSaveCategory.Click += (s, e) => SaveCategory();
            btnDeleteCategory.Click += (s, e) => DeleteCategory();
            dgvCategories.SelectionChanged += (s, e) => FillCategoryForm();
        }

        private DataGridView MakeGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false, RowHeadersVisible = false, BackgroundColor = Color.White
            };
        }

        private void LoadCategories()
        {
            try
            {
                DataTable dt = Db.Query("SELECT MaDanhMuc, TenDanhMuc FROM dbo.DanhMuc ORDER BY TenDanhMuc");
                dgvCategories.DataSource = dt;
                if (dgvCategories.Columns.Contains("MaDanhMuc")) dgvCategories.Columns["MaDanhMuc"].HeaderText = "Mã danh mục";
                if (dgvCategories.Columns.Contains("TenDanhMuc")) dgvCategories.Columns["TenDanhMuc"].HeaderText = "Tên danh mục";
                cboCategory.DataSource = dt.Copy();
                cboCategory.DisplayMember = "TenDanhMuc";
                cboCategory.ValueMember = "MaDanhMuc";
            }
            catch (Exception ex) { Error(ex); }
        }

        private void LoadFoods()
        {
            try
            {
                dgvFoods.DataSource = Db.Query(@"SELECT m.MaMon, m.TenMon, m.MaDanhMuc, d.TenDanhMuc, m.DonGia, m.DangBan
                    FROM dbo.MonAn m INNER JOIN dbo.DanhMuc d ON m.MaDanhMuc=d.MaDanhMuc ORDER BY m.MaMon DESC");
                if (dgvFoods.Columns.Contains("MaMon")) dgvFoods.Columns["MaMon"].HeaderText = "Mã món";
                if (dgvFoods.Columns.Contains("TenMon")) dgvFoods.Columns["TenMon"].HeaderText = "Tên món";
                if (dgvFoods.Columns.Contains("MaDanhMuc")) dgvFoods.Columns["MaDanhMuc"].Visible = false;
                if (dgvFoods.Columns.Contains("TenDanhMuc")) dgvFoods.Columns["TenDanhMuc"].HeaderText = "Danh mục";
                if (dgvFoods.Columns.Contains("DonGia")) { dgvFoods.Columns["DonGia"].HeaderText = "Đơn giá"; dgvFoods.Columns["DonGia"].DefaultCellStyle.Format = "N0"; }
                if (dgvFoods.Columns.Contains("DangBan")) { dgvFoods.Columns["DangBan"].HeaderText = "Đang bán"; }
            }
            catch (Exception ex) { Error(ex); }
        }

        private void FillFoodForm()
        {
            if (dgvFoods.CurrentRow == null || dgvFoods.CurrentRow.IsNewRow) return;
            try
            {
                DataGridViewRow r = dgvFoods.CurrentRow;
                _selectedFoodId = Convert.ToInt32(r.Cells["MaMon"].Value);
                txtFoodName.Text = Convert.ToString(r.Cells["TenMon"].Value);
                nudPrice.Value = Math.Max(nudPrice.Minimum, Math.Min(nudPrice.Maximum, Convert.ToDecimal(r.Cells["DonGia"].Value)));
                chkSelling.Checked = Convert.ToBoolean(r.Cells["DangBan"].Value);
                if (r.Cells["MaDanhMuc"].Value != DBNull.Value) cboCategory.SelectedValue = Convert.ToInt32(r.Cells["MaDanhMuc"].Value);
            }
            catch { }
        }

        private void ClearFoodForm()
        {
            _selectedFoodId = 0; txtFoodName.Clear(); nudPrice.Value = 0; chkSelling.Checked = true;
            if (cboCategory.Items.Count > 0) cboCategory.SelectedIndex = 0;
            dgvFoods.ClearSelection();
        }

        private void SaveFood()
        {
            string name = txtFoodName.Text.Trim();
            if (name.Length == 0) { MessageBox.Show("Tên món không được để trống."); txtFoodName.Focus(); return; }
            if (cboCategory.SelectedValue == null || !(cboCategory.SelectedValue is int))
            { MessageBox.Show("Vui lòng tạo/chọn danh mục."); return; }
            try
            {
                if (_selectedFoodId == 0)
                    Db.Execute("INSERT INTO dbo.MonAn(TenMon,MaDanhMuc,DonGia,DangBan) VALUES(@name,@cat,@price,@selling)",
                        Db.NVarChar("@name", 150, name), Db.P("@cat", SqlDbType.Int, Convert.ToInt32(cboCategory.SelectedValue)),
                        Db.Decimal("@price", nudPrice.Value), Db.P("@selling", SqlDbType.Bit, chkSelling.Checked));
                else
                    Db.Execute("UPDATE dbo.MonAn SET TenMon=@name,MaDanhMuc=@cat,DonGia=@price,DangBan=@selling WHERE MaMon=@id",
                        Db.NVarChar("@name", 150, name), Db.P("@cat", SqlDbType.Int, Convert.ToInt32(cboCategory.SelectedValue)),
                        Db.Decimal("@price", nudPrice.Value), Db.P("@selling", SqlDbType.Bit, chkSelling.Checked),
                        Db.P("@id", SqlDbType.Int, _selectedFoodId));
                LoadFoods(); ClearFoodForm();
                MessageBox.Show("Đã lưu món ăn.");
            }
            catch (Exception ex) { Error(ex); }
        }

        private void DeleteFood()
        {
            if (_selectedFoodId == 0) { MessageBox.Show("Chọn món cần xóa."); return; }
            if (MessageBox.Show("Bạn chắc chắn muốn xóa món này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                Db.Execute("DELETE FROM dbo.MonAn WHERE MaMon=@id", Db.P("@id", SqlDbType.Int, _selectedFoodId));
                LoadFoods(); ClearFoodForm();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) MessageBox.Show("Không thể xóa món đã xuất hiện trong hóa đơn. Hãy bỏ chọn 'Đang bán' để ẩn món.");
                else Error(ex);
            }
            catch (Exception ex) { Error(ex); }
        }

        private void FillCategoryForm()
        {
            if (dgvCategories.CurrentRow == null || dgvCategories.CurrentRow.IsNewRow) return;
            try
            {
                _selectedCategoryId = Convert.ToInt32(dgvCategories.CurrentRow.Cells["MaDanhMuc"].Value);
                txtCategoryName.Text = Convert.ToString(dgvCategories.CurrentRow.Cells["TenDanhMuc"].Value);
            }
            catch { }
        }

        private void ClearCategoryForm()
        {
            _selectedCategoryId = 0; txtCategoryName.Clear(); dgvCategories.ClearSelection();
        }

        private void SaveCategory()
        {
            string name = txtCategoryName.Text.Trim();
            if (name.Length == 0) { MessageBox.Show("Tên danh mục không được để trống."); txtCategoryName.Focus(); return; }
            try
            {
                if (_selectedCategoryId == 0)
                    Db.Execute("INSERT INTO dbo.DanhMuc(TenDanhMuc) VALUES(@name)", Db.NVarChar("@name", 100, name));
                else
                    Db.Execute("UPDATE dbo.DanhMuc SET TenDanhMuc=@name WHERE MaDanhMuc=@id",
                        Db.NVarChar("@name", 100, name), Db.P("@id", SqlDbType.Int, _selectedCategoryId));
                LoadCategories(); LoadFoods(); ClearCategoryForm();
                MessageBox.Show("Đã lưu danh mục.");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601) MessageBox.Show("Tên danh mục đã tồn tại.");
                else Error(ex);
            }
            catch (Exception ex) { Error(ex); }
        }

        private void DeleteCategory()
        {
            if (_selectedCategoryId == 0) { MessageBox.Show("Chọn danh mục cần xóa."); return; }
            if (MessageBox.Show("Xóa danh mục này? Chỉ xóa được khi danh mục chưa có món ăn.", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                Db.Execute("DELETE FROM dbo.DanhMuc WHERE MaDanhMuc=@id", Db.P("@id", SqlDbType.Int, _selectedCategoryId));
                LoadCategories(); LoadFoods(); ClearCategoryForm();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) MessageBox.Show("Không thể xóa danh mục đang có món ăn. Hãy chuyển hoặc xóa các món thuộc danh mục trước.");
                else Error(ex);
            }
            catch (Exception ex) { Error(ex); }
        }

        private void Error(Exception ex)
        {
            MessageBox.Show("Lỗi: " + ex.Message, "Cafe POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
