using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QL_Cuahangxe.Models;

namespace QL_Cuahangxe.Models
{
    public class Giohang
    {
        QLBanXeGanMayEntities qlCuahangxeEntities = new QLBanXeGanMayEntities();

        public int iMaxe { get; set; }
        public string sTenxe { get; set; }
        public string sAnhbia { get; set; }
        public Double dDongia { get; set; }
        public int iSoluong { get; set; }
        public Double dThanhtien
        {
            get { return iSoluong * dDongia; }
        }

        public Giohang(int Maxe)
        {
            iMaxe = Maxe;
            XEGANMAY xe = qlCuahangxeEntities.XEGANMAYs.FirstOrDefault(n => n.MaXe == iMaxe);
            sTenxe = xe.TenXe;
            sAnhbia = xe.Anhbia;
            dDongia = Double.Parse(xe.Giaban.ToString());
            iSoluong = 1;
        }
    }
}