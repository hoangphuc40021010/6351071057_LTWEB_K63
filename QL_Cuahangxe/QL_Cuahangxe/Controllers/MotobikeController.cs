using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QL_Cuahangxe.Models;

namespace QL_Cuahangxe.Controllers
{
    public class MotobikeController : Controller
    {
        // Khởi tạo đối tượng của context
        private  QLBanXeGanMayEntities qlCuahangxeEntities = new QLBanXeGanMayEntities();

        // Lấy 5 xe gắn máy mới nhất
        private List<XEGANMAY> LayXeganmoi(int count)
        {
            return qlCuahangxeEntities.XEGANMAYs.OrderByDescending(x => x.Ngaycapnhat).Take(count).ToList();
        }

        // Lấy danh sách loại xe
        private List<LOAIXE> LayLoaixe()
        {
            return qlCuahangxeEntities.LOAIXEs.ToList();
        }

        // Lấy danh sách hãng sản xuất
        private List<HANGSANXUAT> LayHangsanxuat()
        {
            return qlCuahangxeEntities.HANGSANXUATs.ToList();
        }

        // Phương thức Index hiển thị danh sách
        public ActionResult Index()
        {
            var xeganmaymoi = LayXeganmoi(5); // Lấy 5 xe gắn máy mới nhất
            var loaixeList = LayLoaixe(); // Lấy danh sách loại xe
            var hangsanxuatList = LayHangsanxuat(); // Lấy danh sách hãng sản xuất

            var viewModel = new MotobikeVM
            {
                Xeganmays = xeganmaymoi,
                Loaixes = loaixeList,
                Hangsanxuats = hangsanxuatList
            };

            return View(viewModel);
        }

        // Phương thức chi tiết cho từng xe gắn máy
        public ActionResult Details(int id)
        {
            var xeganmay = qlCuahangxeEntities.XEGANMAYs.FirstOrDefault(x => x.MaXe == id);

            if (xeganmay == null)
            {
                return HttpNotFound(); // Trả về lỗi nếu không tìm thấy xe gắn máy
            }

            return View(xeganmay); // Truyền đối tượng xe gắn máy vào view chi tiết
        }

        // Phương thức lọc xe theo loại
        public ActionResult FilterByCategory(int id)
        {
            var xeTheoLoai = qlCuahangxeEntities.XEGANMAYs.Where(x => x.MaLX == id).ToList();
            var viewModel = new MotobikeVM
            {
                Xeganmays = xeTheoLoai,
                Loaixes = LayLoaixe(),
                Hangsanxuats = LayHangsanxuat()
            };

            return View("Index", viewModel); // Render lại view Index với kết quả lọc
        }

        // Phương thức lọc xe theo hãng sản xuất
        public ActionResult FilterByManufacturer(int id)
        {
            var xeTheoHang = qlCuahangxeEntities.XEGANMAYs.Where(x => x.MaNPP == id).ToList();
            var viewModel = new MotobikeVM
            {
                Xeganmays = xeTheoHang,
                Loaixes = LayLoaixe(),
                Hangsanxuats = LayHangsanxuat()
            };

            return View("Index", viewModel); // Render lại view Index với kết quả lọc
        }
    }
}
