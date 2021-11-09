if exists (select name from sysdatabases where name = N'CinemaBooking')
drop database CinemaBooking
go
CREATE DATABASE CinemaBooking
go
USE CinemaBooking
go


-- Bảng khách hàng
CREATE TABLE khach_hang
(
	id INT identity(1,1) NOT NULL,
	ho_ten nVarchar(100) NOT NULL,
	username Varchar(100) UNIQUE,
	password Varchar(100) NOT NULL,
	sdt varchar(10) NOT NULL,
	email Varchar(100) UNIQUE,
	dia_chi nVarchar(200),
	gioi_tinh bit,	
	ngay_sinh DATETIME,
	cmnd varchar(9),
	create_at datetime,
	update_at datetime,
	confirmpassword Varchar(100)

	CONSTRAINT PK_KHang PRIMARY KEY(id),
)
GO

-- Bảng thể loại phim
CREATE TABLE the_loai_phim
(
	id INT identity(1,1) NOT NULL,
	ten_the_loai nvarchar(250),
	slug nvarchar(max),
	create_by nvarchar(250),
	create_at datetime,
	update_by nvarchar(250),
	update_at datetime ,

	CONSTRAINT PK_TLPhim PRIMARY KEY(id),
)
GO

-- Bảng diễn viên
CREATE TABLE dien_vien
(
	id INT identity(1,1) NOT NULL,
	ho_ten nvarchar(250),
	ngay_sinh datetime,
	mo_ta nvarchar(max),
	quoc_tich nvarchar(100),
	chieu_cao int,
	chi_tiet nvarchar(max),
	anh nvarchar(max),
	slug nvarchar(max),
	phim_da_tham_gia nvarchar(max),

	CONSTRAINT PK_DVien PRIMARY KEY(id),
)

GO

-- Bảng Đạo diễn
CREATE TABLE dao_dien
(
	id INT identity(1,1) NOT NULL,
	ho_ten nvarchar(250),
	ngay_sinh datetime,
	mo_ta nvarchar(max),
	quoc_tich nvarchar(100),
	chieu_cao int,
	chi_tiet nvarchar(max),
	anh nvarchar(max),
	slug nvarchar(max),
	phim_da_tham_gia nvarchar(max),

	CONSTRAINT PK_DDien PRIMARY KEY(id),
)

GO

-- Bảng phim
CREATE TABLE phim
(
	id INT identity(1,1) NOT NULL,
	ten_phim nvarchar(max) not null,
	gia decimal(18,0) CHECK (Gia>=0),
	chi_tiet ntext null,
	trailer nvarchar(max),
	slug nvarchar(max),
	anh nvarchar(max),
	status int,
	thoi_luong int,
	ngay_cong_chieu datetime,
	loai_phim_chieu int,
	create_by nvarchar(250),
	create_at datetime,
	update_by nvarchar(250),
	update_at datetime,
	dao_dien_id int,

	CONSTRAINT PK_Phim PRIMARY KEY(id),
	constraint Fk_DaoDien foreign key(dao_dien_id) references dao_dien(id),
)
GO

-- Bảng phòng chiếu
CREATE TABLE phong_chieu
(
	id INT identity(1,1) NOT NULL,
	so_luong_day int,
	so_luong_cot int,

	CONSTRAINT PK_PChieu PRIMARY KEY(id),
)
GO

-- Bảng suất chiếu
CREATE TABLE suat_chieu
(
	id INT identity(1,1) NOT NULL,
	gio_bat_dau time,
	gio_ket_thuc time,
	ngay_chieu datetime,
	phim_id int,
	phong_chieu_id int,

	CONSTRAINT PK_SChieu PRIMARY KEY(id),
	constraint Fk_PhimSC foreign key(phim_id) references phim(id),
	constraint Fk_PhimPC foreign key(phong_chieu_id) references phong_chieu(id),
)
GO

-- Bảng loại ghế
CREATE TABLE loai_ghe
(
	id INT identity(1,1) NOT NULL,
	ten_ghe nvarchar(250),
	phu_thu decimal(18,0),

	CONSTRAINT PK_LGhe PRIMARY KEY(id),
)
GO

-- Bảng ghế ngồi
CREATE TABLE ghe_ngoi
(
	id INT identity(1,1) NOT NULL,
	vi_tri_day int,
	vi_tri_cot int,
	da_chon bit,
	phong_chieu_id int,
	loai_ghe_id int,

	CONSTRAINT PK_Ghe PRIMARY KEY(id),
	constraint Fk_GhePC foreign key(phong_chieu_id) references phong_chieu(id),
	constraint Fk_LoaiGhe foreign key(loai_ghe_id) references loai_ghe(id),
)
GO

-- Bảng giá vé
CREATE TABLE gia_ve
(
	id INT identity(1,1) NOT NULL,
	ten_ve nvarchar(250),
	don_gia decimal(18,0),

	CONSTRAINT PK_GiaVe PRIMARY KEY(id),
)
GO

-- Bảng users
CREATE TABLE users
(
	id INT identity(1,1) NOT NULL,
	ho_ten nVarchar(100) NOT NULL,
	username Varchar(100) UNIQUE,
	password Varchar(100) NOT NULL,
	role int,
	sdt varchar(10),
	email Varchar(100) UNIQUE,
	dia_chi nVarchar(200),
	gioi_tinh bit,	
	ngay_sinh datetime,
	ngay_vao_lam datetime,
	cmnd varchar(9),
	dang_lam bit,
	create_by nvarchar(250),
	create_at datetime,
	update_by nvarchar(250),
	update_at datetime,

	CONSTRAINT PK_User PRIMARY KEY(id),
)
GO

-- Bảng vé bán
CREATE TABLE ve_ban
(
	id INT identity(1,1) NOT NULL,
	ngay_ban datetime,
	tong_tien decimal(18,0),
	suat_chieu_id int,
	gia_ve_id int,
	ghe_id int,
	users_id int,

	CONSTRAINT PK_Ve PRIMARY KEY(id),
	constraint Fk_VeSC foreign key(suat_chieu_id) references suat_chieu(id),
	constraint Fk_VeGia foreign key(gia_ve_id) references gia_ve(id),
	constraint Fk_VeGhe foreign key(ghe_id) references ghe_ngoi(id),
	constraint Fk_VeNV foreign key(users_id) references users(id),
)
GO







/*********************************************************************/


-- Bảng liên hệ
CREATE TABLE lien_he
(
	id INT identity(1,1) NOT NULL,
	ho_ten nvarchar(100),
	email varchar(100),
	tieu_de nvarchar(250),
	noi_dung nvarchar(max),
	tra_loi nvarchar(max),
	create_at datetime,
	update_by nvarchar(250),
	update_at datetime,

	CONSTRAINT PK_LienHe PRIMARY KEY(id),
)
GO

-- Bảng thông tin công ty
CREATE TABLE thong_tin_cong_ty
(
	id INT identity(1,1) NOT NULL,
	ten_rap nvarchar(250),
	dia_chi nvarchar(250),
	phong_chieu int,
	status int,
	create_by nvarchar(250),
	create_at datetime,
	update_by nvarchar(250),
	update_at datetime,

	CONSTRAINT PK_ThongTin PRIMARY KEY(id),
)
GO

-- Bảng Sự kiện
CREATE TABLE su_kien
(
	id INT identity(1,1) NOT NULL,
	tieu_de nvarchar(max),
	noi_dung nvarchar(max),
	slug nvarchar(max),
	anh nvarchar(max),
	status int,
	create_by nvarchar(250),
	create_at datetime,
	update_by nvarchar(250),
	update_at datetime,

	CONSTRAINT PK_Event PRIMARY KEY(id),
)

GO



/*------------------------------------------Thêm và xóa database----------------------------------------------------*/
-- Thêm ten_phong cho bản phòng chiếu
ALTER TABLE phong_chieu ADD ten_phong nvarchar(255);

-- Thêm status cho bảng liên hệ
ALTER TABLE lien_he ADD status int;


-- Bảng danh sách giữa phim và diễn viên
CREATE TABLE list_phim_dienvien(
	ID bigint IDENTITY(1,1) NOT NULL,
	id_phim int NOT NULL,
	id_dienvien int NOT NULL,

	CONSTRAINT PK_list_phim_dienvien PRIMARY KEY(ID),
	CONSTRAINT FK_list_phim_dienvien_dien_vien FOREIGN KEY (id_dienvien) REFERENCES dien_vien(id),
	CONSTRAINT FK_list_phim_dienvien_phim FOREIGN KEY (id_phim) REFERENCES phim([id])
)
GO
-- Bảng danh sách giữa phim và thể loại
CREATE TABLE list_phim_theloai(
	ID int IDENTITY(1,1) NOT NULL,
	id_phim int  NULL,
	id_theloai int NULL,

	CONSTRAINT PK_list_phim_theloai PRIMARY KEY(ID),
	CONSTRAINT FK_list_phim_theloai_the_loai_phim FOREIGN KEY (id_theloai) REFERENCES the_loai_phim(id),
	CONSTRAINT FK_list_phim_theloai_phim FOREIGN KEY (id_phim) REFERENCES phim(id)
)
GO

-- Thêm bảng movie rate
CREATE TABLE movie_rate(
	id int IDENTITY(1,1) NOT NULL,
	movie_id int,
	khachhang_id int,
	rate float,
	comment nvarchar(max),
	ten_khachhang nvarchar(255),
	created_at datetime,
	CONSTRAINT PK_MovieRate PRIMARY KEY(id),
)
GO


-- Điều chỉnh database: Thêm bảng orders, orderdetails, ghế ngồi
ALTER TABLE phim ADD anhbackground nvarchar(max);

ALTER TABLE ghe_ngoi ADD gia decimal(18,0);

ALTER TABLE ghe_ngoi ADD image nvarchar(max);


CREATE TABLE orders(
	id int IDENTITY(1,1) NOT NULL,
	id_khachhang int,
	ten_khach_hang nvarchar(255),
	ten_phim nvarchar(max),
	id_phim int,
	so_luong_ve int,
	code_ticket nvarchar(255),
	tong_tien decimal(18,0),
	the_loai_phim nvarchar(255),
	id_phong_chieu int,
	ten_phong_chieu nvarchar(255),
	
	time datetime,
	ngay_mua datetime,
	status int,

	CONSTRAINT PK_OrderID PRIMARY KEY(id),
	constraint Fk_KhachHangOrder foreign key(id_khachhang) references khach_hang(id),
	constraint Fk_PhimOrder foreign key(id_phim) references phim(id),
	constraint Fk_PhongChieuOrder foreign key(id_phong_chieu) references phong_chieu(id),
)

CREATE TABLE order_details(
	id int IDENTITY(1,1) NOT NULL,
	ten_ghe nvarchar(255),
	id_orders int,
	id_ghe int,
	gia_ve decimal(18,0),
	gia_ghe_ngoi decimal(18,0),

	CONSTRAINT PK_OrderDetailsID PRIMARY KEY(id),
	constraint Fk_Oder foreign key(id_orders) references orders(id),
	constraint Fk_Ghe foreign key(id_ghe) references ghe_ngoi(id),
)


-- them bâng dánh giá nội dung
-- Bảng đánh giá nội dung phim ( id : loại đánh giá, ten_rating; P, C13, C16, ...... )
CREATE TABLE content_rating(
	ID int NOT NULL, 
	ten_rating nvarchar(50) NOT NULL,
	CONSTRAINT PK_content_rating PRIMARY KEY(ID),
)

alter table dbo.phim
add idrating nvarchar(50) NULL


alter table dbo.phim
add id_content_rating int NULL
-- khóa ngoại cho phim và content rating
ALTER TABLE [dbo].[phim]  WITH CHECK ADD  CONSTRAINT [FK_phim_content_rating] FOREIGN KEY([id_content_rating])
REFERENCES [dbo].[content_rating] ([ID])
GO



-- fix bảng ghế ngồi
alter table ghe_ngoi
drop column vi_tri_day;
alter table ghe_ngoi
drop column vi_tri_cot;

alter table ghe_ngoi
add Row nvarchar(255);

alter table ghe_ngoi
add Col int;

-- thêm cột trong phim
-- dùng để lấy API dữ liệu về phim
alter table dbo.phim add idrating nvarchar(50) NULL 
--Đẻ dánh giá nội dung có phù hợp với độ tuổi
alter table dbo.phim add id_content_rating int NULL


alter table phim
add constraint Fk_DaoDien foreign key(dao_dien_id) references dao_dien(id)

alter table phim
drop constraint Fk_TheLoai 

alter table phim
drop column dien_vien_id;
alter table phim
drop column the_loai_phim_id;