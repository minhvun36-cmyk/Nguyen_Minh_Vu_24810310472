/* Chay file nay trong SQL Server Management Studio (SSMS) */
IF DB_ID(N'HotelDB') IS NULL CREATE DATABASE HotelDB;
GO
USE HotelDB;
GO

IF OBJECT_ID('dbo.SuDungDichVu','U') IS NOT NULL DROP TABLE dbo.SuDungDichVu;
IF OBJECT_ID('dbo.PhieuDatPhong','U') IS NOT NULL DROP TABLE dbo.PhieuDatPhong;
IF OBJECT_ID('dbo.DichVu','U') IS NOT NULL DROP TABLE dbo.DichVu;
IF OBJECT_ID('dbo.KhachHang','U') IS NOT NULL DROP TABLE dbo.KhachHang;
IF OBJECT_ID('dbo.Phong','U') IS NOT NULL DROP TABLE dbo.Phong;
GO

CREATE TABLE Phong(
    SoPhong VARCHAR(10) NOT NULL PRIMARY KEY,
    LoaiPhong NVARCHAR(50) NOT NULL,
    GiaTheoGio DECIMAL(18,2) NOT NULL CHECK (GiaTheoGio >= 0),
    TrangThai NVARCHAR(30) NOT NULL CHECK (TrangThai IN (N'Trống',N'Đang ở',N'Đang dọn dẹp'))
);

CREATE TABLE KhachHang(
    MaKH INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    CCCD VARCHAR(20) NOT NULL UNIQUE,
    SoDienThoai VARCHAR(15) NULL
);

CREATE TABLE DichVu(
    MaDV INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenDV NVARCHAR(100) NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0)
);

CREATE TABLE PhieuDatPhong(
    MaPhieu INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SoPhong VARCHAR(10) NOT NULL,
    MaKH INT NOT NULL,
    NgayNhanPhong DATETIME NOT NULL DEFAULT GETDATE(),
    NgayTraPhong DATETIME NULL,
    TongTienThanhToan DECIMAL(18,2) NULL,
    CONSTRAINT FK_Phieu_Phong FOREIGN KEY(SoPhong) REFERENCES Phong(SoPhong),
    CONSTRAINT FK_Phieu_KhachHang FOREIGN KEY(MaKH) REFERENCES KhachHang(MaKH)
);

CREATE TABLE SuDungDichVu(
    MaPhieu INT NOT NULL,
    MaDV INT NOT NULL,
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    ThoiGian DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_SDDV_Phieu FOREIGN KEY(MaPhieu) REFERENCES PhieuDatPhong(MaPhieu),
    CONSTRAINT FK_SDDV_DichVu FOREIGN KEY(MaDV) REFERENCES DichVu(MaDV)
);
GO

INSERT INTO Phong(SoPhong,LoaiPhong,GiaTheoGio,TrangThai) VALUES
('P101',N'Đơn',50000,N'Trống'),
('P102',N'Đôi',80000,N'Trống'),
('P103',N'Đơn',50000,N'Trống'),
('P201',N'VIP',150000,N'Trống'),
('P202',N'Đôi',80000,N'Đang dọn dẹp');

INSERT INTO DichVu(TenDV,DonGia) VALUES
(N'Nước suối',10000),(N'Nước ngọt',15000),(N'Giặt là',30000),(N'Đồ ăn',50000);

INSERT INTO KhachHang(HoTen,CCCD,SoDienThoai) VALUES
(N'Nguyễn Văn An','001234567890','0912345678');
GO
