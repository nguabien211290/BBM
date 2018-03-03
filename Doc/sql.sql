select  id,masp,tensp from [shop-sanpham] where masp like '%CV%' and tensp is NULL order by id desc


select top 21 id from [shop-sanpham] where masp like '%CM%' order by id desc



delete soft_Branches_Product_Stock where ProductId in(
select  id from [shop-sanpham] where tensp is NULL)

delete soft_Order_Child where ProductId in(
select  id from [shop-sanpham] where tensp is NULL)

delete soft_Channel_Product_Price where ProductId in(
select  id from [shop-sanpham] where tensp is NULL)

delete [shop-bienthe] where idsp in(
select  id from [shop-sanpham] where tensp is NULL) and id != 24107


delete [shop-sanpham] where id in(
select  id from [shop-sanpham] where tensp is NULL) and id !=12701


select * from donhang_ct where idpro in(
select id from [shop-bienthe] where idsp in(
select  id from [shop-sanpham] where tensp is NULL))




select  * from [shop-bienthe] where id=24107
select * from [shop-sanpham] where id=12701