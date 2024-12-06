using QL_Cuahangxe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Data.Entity;
using Ganss.Xss;

namespace QL_Cuahangxe.Controllers
{
    public class AdminController : Controller
    {
        private QLBanXeGanMayEntities qLBanXeGanMayEntities;
        public AdminController()
        {
            qLBanXeGanMayEntities = new QLBanXeGanMayEntities();
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Xe(int ?page)
        {
            int pageSize = 5; 
            int pageNumber = (page ?? 1); 

            var Xeganmays = qLBanXeGanMayEntities.XEGANMAYs.OrderBy(s => s.MaXe).ToPagedList(pageNumber, pageSize);

            return View(Xeganmays);

        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            // Gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["username"];
            var matkhau = collection["password"];

            // Kiểm tra nếu thiếu tên đăng nhập
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }

            // Kiểm tra nếu thiếu mật khẩu
            if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }

            // Nếu cả hai trường đều có giá trị, kiểm tra tính hợp lệ của tài khoản
            if (!String.IsNullOrEmpty(tendn) && !String.IsNullOrEmpty(matkhau))
            {
                // Gán giá trị cho đối tượng được tạo mới (ad)
                var ad = qLBanXeGanMayEntities.Admins
                    .SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult ThemmoiXe()
        {
            // Lay ds tu table chu de, sap xep tang dan theo ten chu de, chon lay gia tri MaLX, hien thi thi TenLoaiXe
            ViewBag.MaLX = new SelectList(qLBanXeGanMayEntities.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
            ViewBag.MaNPP = new SelectList(qLBanXeGanMayEntities.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");
            return View();
        }

        [HttpPost]
        public ActionResult ThemmoiXe(XEGANMAY xEGANMAY, HttpPostedFileBase fileupload)
        {
            // Làm sạch HTML trong Mota để loại bỏ các thẻ nguy hiểm
            var sanitizer = new HtmlSanitizer();
            xEGANMAY.Mota = sanitizer.Sanitize(xEGANMAY.Mota);

            // Chuyển Mota thành kiểu text thuần (plain text) sau khi làm sạch HTML
            xEGANMAY.Mota = HttpUtility.HtmlDecode(xEGANMAY.Mota);

            // Kiểm tra xem fileupload có null không (người dùng có chọn ảnh hay không)
            if (fileupload != null && fileupload.ContentLength > 0)
            {
                // Lưu tên file, lưu ý bổ sung thư viện using System.IO;
                var fileName = Path.GetFileName(fileupload.FileName);

                // Lưu đường dẫn của file
                var path = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName);

                // Kiểm tra hình ảnh tồn tại chưa
                if (System.IO.File.Exists(path))
                {
                    ViewBag.Thongbao = "Hình ảnh đã tồn tại. Vui lòng chọn một tên khác.";
                    // Trả lại danh sách chọn MaLX và MaNPP
                    ViewBag.MaLX = new SelectList(qLBanXeGanMayEntities.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
                    ViewBag.MaNPP = new SelectList(qLBanXeGanMayEntities.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");
                    return View(xEGANMAY);
                }
                else
                {
                    try
                    {
                        // Lưu hình ảnh vào thư mục Hinhsanpham
                        fileupload.SaveAs(path);
                        xEGANMAY.Anhbia = fileName; // Gán tên ảnh cho thuộc tính Anhbia của đối tượng XE
                        ViewBag.Thongbao = "Lưu hình ảnh thành công.";
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi nếu có
                        ViewBag.Thongbao = "Có lỗi khi lưu hình ảnh: " + ex.Message;
                        // Trả lại danh sách chọn MaLX và MaNPP
                        ViewBag.MaLX = new SelectList(qLBanXeGanMayEntities.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
                        ViewBag.MaNPP = new SelectList(qLBanXeGanMayEntities.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");
                        return View(xEGANMAY);
                    }
                }
            }
            else
            {
                // Thông báo khi không chọn file
                ViewBag.Thongbao = "Vui lòng chọn một tệp để tải lên.";
                // Trả lại danh sách chọn MaLX và MaNPP
                ViewBag.MaLX = new SelectList(qLBanXeGanMayEntities.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
                ViewBag.MaNPP = new SelectList(qLBanXeGanMayEntities.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");
                return View(xEGANMAY);
            }

            // Thêm đối tượng XE vào database
            qLBanXeGanMayEntities.XEGANMAYs.Add(xEGANMAY);
            qLBanXeGanMayEntities.SaveChanges(); // Lưu thay đổi vào database

            // Thiết lập danh sách chọn cho các trường MaLX và MaNPP
            ViewBag.MaLX = new SelectList(qLBanXeGanMayEntities.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
            ViewBag.MaNPP = new SelectList(qLBanXeGanMayEntities.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");

            // Trả về View sau khi hoàn tất
            ViewBag.Thongbao = "Thêm xe mới thành công.";
            return RedirectToAction("Xe", "Admin");
        }

        public ActionResult ChitietXe(int id)
        {
            XEGANMAY xEGANMAY = qLBanXeGanMayEntities.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);
            ViewBag.MaXe = xEGANMAY.MaXe;
            if (xEGANMAY == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(xEGANMAY);
        }

        [HttpGet]
        public ActionResult Xoaxe(int id)
        {
            XEGANMAY xEGANMAY = qLBanXeGanMayEntities.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);
            if (xEGANMAY == null)
            {
                return HttpNotFound("Xe không tồn tại.");
            }

            return View(xEGANMAY);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Xacnhanxoa(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ID không hợp lệ.");
            }

            XEGANMAY xEGANMAY = qLBanXeGanMayEntities.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);

            if (xEGANMAY == null)
            {
                return HttpNotFound("Xe không tồn tại.");
            }

            try
            {
                // Đường dẫn thư mục chứa hình ảnh
                string imageFolderPath = Server.MapPath("~/Hinhsanpham/");
                string imagePath = Path.Combine(imageFolderPath, xEGANMAY.Anhbia); // `HinhAnh` là trường lưu tên file ảnh

                // Xóa hình ảnh nếu tồn tại
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // Xóa các bản ghi liên quan trong bảng SANXUATXE
               var sanxuatxe = qLBanXeGanMayEntities.SANXUATXEs.Where(v => v.MaXe == id).ToList();
                foreach (var item in sanxuatxe)
                {
                    qLBanXeGanMayEntities.SANXUATXEs.Remove(item);
                }

                // Xóa các bản ghi liên quan trong bảng CHITIETDONHANG
                var chitietdonhang = qLBanXeGanMayEntities.CHITIETDONTHANGs.Where(c => c.MaXe == id).ToList();
                foreach (var item in chitietdonhang)
                {
                    qLBanXeGanMayEntities.CHITIETDONTHANGs.Remove(item);
                }

                // Xóa bản ghi Xe
               qLBanXeGanMayEntities.XEGANMAYs.Remove(xEGANMAY);
                qLBanXeGanMayEntities.SaveChanges();
                return RedirectToAction("Sach", "Admin"); // Chuyển hướng về trang Xe.cshtml
            }
            catch (DbUpdateException)
            {
                ViewBag.Thongbao = "Không thể xóa sách này vì có dữ liệu liên quan.";
                return View("Xoasach", xEGANMAY);
            }
        }

        [HttpGet]
        public ActionResult Suaxe(int id)
        {
            var xEGANMAY = qLBanXeGanMayEntities.XEGANMAYs.Find(id);

            if (xEGANMAY == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.Ngaycapnhat = DateTime.Now.ToString("dd-mm-yyyy");
            ViewBag.MaLX = new SelectList(qLBanXeGanMayEntities.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
            ViewBag.MaNPP = new SelectList(qLBanXeGanMayEntities.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");
            return View(xEGANMAY);
        }

        [HttpPost]
        public ActionResult Suaxe(XEGANMAY xeganmay, HttpPostedFileBase fileUpload)
        {
            var xeHienTai = qLBanXeGanMayEntities.XEGANMAYs.Find(xeganmay.MaXe);
            if (xeHienTai == null)
            {
                ViewBag.Thongbao = "Xe không tồn tại.";
                return View(xeganmay);
            }

            // Xử lý ảnh (nếu có tải lên ảnh mới)
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                var fileName = Path.GetFileName(fileUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName);

                if (fileName != xeHienTai.Anhbia && System.IO.File.Exists(path))
                {
                    ViewBag.Thongbao = "Hình ảnh đã tồn tại. Vui lòng chọn một tên khác.";
                    return View(xeganmay);
                }
                else
                {
                    try
                    {
                        fileUpload.SaveAs(path);
                        xeHienTai.Anhbia = fileName;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Thongbao = "Có lỗi khi lưu hình ảnh: " + ex.Message;
                        return View(xeganmay);
                    }
                }
            }

            // Nếu không tải lên ảnh mới, giữ lại ảnh cũ
            else
            {
                xeHienTai.Anhbia = xeHienTai.Anhbia;
            }

            // Áp dụng các thay đổi từ form vào đối tượng hiện tại
            xeHienTai.TenXe = xeganmay.TenXe;
            xeHienTai.Giaban = xeganmay.Giaban;
            xeHienTai.Mota = xeganmay.Mota;
            xeHienTai.Ngaycapnhat = DateTime.Now;
            xeHienTai.Soluongton = xeganmay.Soluongton;
            xeHienTai.MaLX = xeganmay.MaLX;
            xeHienTai.MaNPP = xeganmay.MaNPP;

            // Lưu các thay đổi
            if (ModelState.IsValid)
            {
                try
                {
                    qLBanXeGanMayEntities.Entry(xeHienTai).State = EntityState.Modified;
                    qLBanXeGanMayEntities.SaveChanges();
                    ViewBag.Thongbao = "Cập nhật thông tin xe thành công.";
                    return RedirectToAction("Xe", "Admin");
                }
                catch (Exception ex)
                {
                    ViewBag.Thongbao = "Lỗi khi lưu vào cơ sở dữ liệu: " + ex.Message;
                    return View(xeganmay);
                }
            }

            // Thiết lập lại dropdown cho view
            ViewBag.MaLX = new SelectList(qLBanXeGanMayEntities.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe", xeganmay.MaLX);
            ViewBag.MaNPP = new SelectList(qLBanXeGanMayEntities.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP", xeganmay.MaNPP);
            return View(xeganmay);
        }


        public ActionResult Thongkexe()
        {
            // Lấy dữ liệu số lượng xe theo từng chủ đề
            var data = qLBanXeGanMayEntities.LOAIXEs
                .Select(cd => new
                {
                    TenLoaiXe = cd.TenLoaiXe,
                    SoLuong = cd.XEGANMAYs.Count() // Đếm số xe trong từng chủ đề
                })
                .ToList();

            // Truyền dữ liệu qua ViewBag để sử dụng trong view
            ViewBag.ChartData = data;

            return View();
        }


    }
}