/*
  CAFE POS SYSTEM - SQL Server
  CHU Y: Script nay xoa database CafePOSDB neu da ton tai.
  Neu muon giu du lieu cu, bo phan DROP DATABASE va chay cac lenh CREATE/INSERT phu hop.
*/
USE master;
GO
IF DB_ID(N'CafePOSDB') IS NOT NULL
BEGIN
    ALTER DATABASE CafePOSDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CafePOSDB;
END
GO
CREATE DATABASE CafePOSDB;
GO
USE CafePOSDB;
GO

CREATE TABLE dbo.BanAn (
    MaBan INT IDENTITY(1,1) PRIMARY KEY,
    TenBan NVARCHAR(50) NOT NULL UNIQUE,
    TrangThai NVARCHAR(20) NOT NULL CONSTRAINT DF_BanAn_TrangThai DEFAULT N'Trống',
    CONSTRAINT CK_BanAn_TrangThai CHECK (TrangThai IN (N'Trống', N'Có khách'))
);
GO

CREATE TABLE dbo.DanhMuc (
    MaDanhMuc INT IDENTITY(1,1) PRIMARY KEY,
    TenDanhMuc NVARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE dbo.MonAn (
    MaMon INT IDENTITY(1,1) PRIMARY KEY,
    TenMon NVARCHAR(150) NOT NULL,
    MaDanhMuc INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    DangBan BIT NOT NULL CONSTRAINT DF_MonAn_DangBan DEFAULT 1,
    CONSTRAINT FK_MonAn_DanhMuc FOREIGN KEY (MaDanhMuc) REFERENCES dbo.DanhMuc(MaDanhMuc),
    CONSTRAINT CK_MonAn_DonGia CHECK (DonGia >= 0)
);
GO

CREATE TABLE dbo.HoaDon (
    MaHoaDon INT IDENTITY(1,1) PRIMARY KEY,
    MaBan INT NOT NULL,
    ThoiGianTao DATETIME NOT NULL CONSTRAINT DF_HoaDon_ThoiGianTao DEFAULT GETDATE(),
    ThoiGianThanhToan DATETIME NULL,
    TongTien DECIMAL(18,2) NOT NULL CONSTRAINT DF_HoaDon_TongTien DEFAULT 0,
    TrangThai NVARCHAR(20) NOT NULL CONSTRAINT DF_HoaDon_TrangThai DEFAULT N'Chưa thanh toán',
    CONSTRAINT FK_HoaDon_BanAn FOREIGN KEY (MaBan) REFERENCES dbo.BanAn(MaBan),
    CONSTRAINT CK_HoaDon_TrangThai CHECK (TrangThai IN (N'Chưa thanh toán', N'Đã thanh toán')),
    CONSTRAINT CK_HoaDon_TongTien CHECK (TongTien >= 0)
);
GO

CREATE TABLE dbo.ChiTietHoaDon (
    MaHoaDon INT NOT NULL,
    MaMon INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    ThanhTien AS (CONVERT(DECIMAL(18,2), SoLuong * DonGia)) PERSISTED,
    CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY (MaHoaDon, MaMon),
    CONSTRAINT FK_CTHD_HoaDon FOREIGN KEY (MaHoaDon) REFERENCES dbo.HoaDon(MaHoaDon),
    CONSTRAINT FK_CTHD_MonAn FOREIGN KEY (MaMon) REFERENCES dbo.MonAn(MaMon),
    CONSTRAINT CK_CTHD_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CK_CTHD_DonGia CHECK (DonGia >= 0)
);
GO

INSERT INTO dbo.BanAn(TenBan, TrangThai)
VALUES (N'Bàn 01',N'Trống'),(N'Bàn 02',N'Trống'),(N'Bàn 03',N'Trống'),
       (N'Bàn 04',N'Trống'),(N'Bàn 05',N'Trống'),(N'Bàn 06',N'Trống'),
       (N'Bàn 07',N'Trống'),(N'Bàn 08',N'Trống'),(N'Bàn 09',N'Trống'),
       (N'Bàn 10',N'Trống');
GO
INSERT INTO dbo.DanhMuc(TenDanhMuc)
VALUES (N'Cà phê'),(N'Trà'),(N'Nước ép');
GO
INSERT INTO dbo.MonAn(TenMon,MaDanhMuc,DonGia,DangBan)
VALUES
(N'Cà phê đen',1,20000,1),
(N'Cà phê sữa',1,25000,1),
(N'Bạc xỉu',1,30000,1),
(N'Trà đào',2,30000,1),
(N'Trà chanh',2,20000,1),
(N'Nước cam',3,35000,1),
(N'Nước ép dưa hấu',3,30000,1);
GO
