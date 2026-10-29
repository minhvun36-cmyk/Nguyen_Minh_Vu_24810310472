BAI TAP 5.5 - INVENTORY MANAGEMENT

1. Giai nen bai5.5.zip.
2. Mo InventoryDB.sql bang SQL Server Management Studio (SSMS), Execute de tao CSDL va du lieu mau.
   LUU Y: Script nay xoa va tao lai cac bang da co trong InventoryDB; khong chay lai neu can giu du lieu.
3. Mo InventoryManagement.sln bang Visual Studio 2022.
4. Mo App.config, sua Data Source=.\SQLEXPRESS cho dung ten SQL Server may ban.
5. Build > Build Solution, sau do nhan F5.

Chuc nang:
- Quan ly vat tu: them/sua/xoa/tim theo ma hoac ten, SQL parameter.
- Quan ly nha cung cap: them/sua/xoa/tim.
- Lap phieu NHAP/XUAT, chi tiet nhieu vat tu, cap nhat ton trong SqlTransaction.
- Xuat kho chi cap nhat khi SoLuongTon >= SoLuong; neu khong du hang transaction rollback.
- Bao cao vat tu ton duoi nguong va tong gia tri ton kho uoc tinh (ExecuteScalar).

Ghi chu ve de bai:
De bai noi 5 bang nhung chi mo ta cau truc 4 bang (NhaCungCap, VatTu, PhieuKho, ChiTietPhieuKho), nen project tao 4 bang theo dung phan mo ta. Tong gia tri ton kho duoc uoc tinh bang don gia nhap gan nhat do de bai khong dinh nghia bang gia von rieng.