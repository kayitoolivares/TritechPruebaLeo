create database tritech_prueba_Leo
go;
use tritech_prueba_Leo
go;
create table producto(
noParte varchar(30) primary key,
nombre varchar(50),
descripcion varchar(300),
precio float,
cantidad int
)


 
insert into producto values ('54401F4200','manubrio','detalles del producto',5,2000)
insert into producto values ('55501f4300','portavasos','detalles del producto',15,2000)
insert into producto values ('46501f4100','tablero','detalles del producto',50,2000)
go;
create table contenedor(
id int identity primary key,
tipo varchar(50)
)

create table movimiento(
id int identity primary key,
tipo varchar(10),
estatus char
)

drop table movimientoDetalle
create table movimientoDetalle(
id int identity primary key,
id_movimiento int,
noParte_producto varchar(30),
id_contenedor int,
piezasXContenedor int,
contenedorXPallet int,
TotalPiezas int,
constraint fk_movimientoDetalle_movimiento foreign key (id_movimiento) references movimiento(id),
constraint fk_movimientoDetalle_producto foreign key (noParte_producto) references producto(noParte),
constraint fk_movimientoDetalle_contenedor foreign key (id_contenedor) references contenedor(id)
)


create view movimientodetalleView as
select m.id,m.id_movimiento ,
m.noParte_producto ,m.id_contenedor ,m.piezasXContenedor ,m.contenedorXPallet, c.tipo as tipoContenedor,m.TotalPiezas from movimientoDetalle m inner join contenedor c
on m.id_contenedor=c.id
go;

--exec sp_getProducto
create procedure sp_getProducto
as begin
select * from producto
end
go;

create procedure sp_Producto_save
@noParte varchar(30),
@nombre varchar(50),
@descripcion varchar(300),
@precio float,
@cantidad int
as begin

if ((select count(*) from producto where noParte=@noParte)>0)
begin
update producto set nombre=@nombre,descripcion=@descripcion,precio=@precio,cantidad=@cantidad where noParte=@noParte
end else
begin
insert into producto values (@noParte,@nombre,@descripcion,@precio,@cantidad)
end
end
go;

create procedure sp_getMovimiento
as begin
select * from movimiento
end
go;

create procedure sp_Movimiento_save
@id int,
@tipo varchar(50),
@estatus char
as begin

if ((select count(*) from movimiento where id=@id)>0)
begin
update movimiento set @estatus=@estatus  where  id=@id
end else
begin
insert into movimiento values (@tipo,@estatus)
end
end
go;
--exec sp_getContenedores

create procedure sp_getContenedores
as begin
select * from contenedor
end
go;

insert into contenedor values ('CNT90')
insert into contenedor values ('CNT80')
insert into contenedor values ('RCK80')
insert into contenedor values ('RCK70')
go;

create procedure sp_saveDetalle
@id_movimiento int,
@noParte_producto varchar(30),
@id_contenedor int,
@piezasXContenedor int,
@contenedorXPallet int
as begin
declare @total int
if @contenedorXPallet=0
begin
set @total=@piezasXContenedor
end else
begin
set @total=@piezasXContenedor*@contenedorXPallet
end
insert into movimientoDetalle values (@id_movimiento ,@noParte_producto ,@id_contenedor ,@piezasXContenedor ,@contenedorXPallet,@total )
end

go;

--exec sp_getDetalle 1
create  procedure sp_getDetalle
@Idmovimiento int
as begin
select * from movimientodetalleView where id_movimiento=@Idmovimiento
end
go;

create procedure sp_deleteDetalle
@id int
as
begin
declare @estatus char
select @estatus=estatus from movimiento where id in (select id_movimiento from movimientoDetalle where id=@id)
if @estatus='a'
begin
delete movimientoDetalle where id=@id
end
end
go;

create procedure sp_Afectar
@id int
as begin
declare @estatus char
declare @tipo varchar(10)
select @estatus=estatus,@tipo=tipo from movimiento where   id=@id
if @estatus='a'
begin
if @tipo='salida'
begin

update
p
set 
p.cantidad=p.cantidad-m.TotalPiezas

from movimientoDetalle as m inner join producto as p on p.noParte=m.noParte_producto
where m.id_movimiento=@id
end
else begin

update
p
set 
p.cantidad=p.cantidad+m.TotalPiezas

from movimientoDetalle as m inner join producto as p on p.noParte=m.noParte_producto
where m.id_movimiento=@id
end

update movimiento set estatus='b' where id=@id
end
end
go;

