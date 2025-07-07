using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Abstract;
using MilkWayIndia.Concrete;
using MilkWayIndia.Entity;
using MilkWayIndia.Models;

namespace MilkWayIndia.Controllers
{
    public class AttributeController : Controller
    {
        Dictionary<string, object> res = new Dictionary<string, object>();
        private IAttribute _AttributeRepo;
        Vendor objvendor = new Vendor();
        Product _product = new Product();

        public AttributeController()
        {
            this._AttributeRepo = new AttributeRepository();
        }
        // GET: Attribute
        public ActionResult Index()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            return View();
        }

        public PartialViewResult GetAttributeList()
        {
            var attribute = _AttributeRepo.GetAllAttribute();
            return PartialView("_List", attribute);
        }

        public ActionResult Create()
        {
            return PartialView("_CreateOrUpdate");
        }

        [HttpPost]
        public ActionResult Create(tbl_Attributes model)
        {
            try
            {
                var attribute = _AttributeRepo.SaveAttribute(model);
                if (attribute == 1)
                {
                    res["status"] = "1";
                    res["message"] = "Successfully save attribute...";
                }
            }
            catch (Exception ex)
            {
                res["message"] = ex.Message;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id)
        {
            var attribute = _AttributeRepo.GetAttributeByID(Id);
            if (attribute != null)
                return PartialView("_CreateOrUpdate", attribute);
            return PartialView("_CreateOrUpdate");
        }

        public void PopulateDropdown()
        {
            ViewBag.Vendor = objvendor.getVendorList(null);
            var attribute = _AttributeRepo.GetAllAttribute();
            ViewBag.lstAttribute = new SelectList(attribute, "ID", "Name");
        }

        public ActionResult AssignProduct(string ProductID)
        {
            //if (string.IsNullOrEmpty(ProductID))
            //    return Redirect("/Product/ProductList");
            ProductAssignVM model = new ProductAssignVM();
            model.ProductID = Convert.ToInt32(ProductID);
            try
            {
                var product = _product.BindProuct(model.ProductID);
                if (product.Rows.Count > 0)
                {
                    ViewBag.ProductName = product.Rows[0]["ProductName"].ToString();
                    if (!string.IsNullOrEmpty(product.Rows[0]["Price"].ToString()))
                        model.MRPPrice = Convert.ToDecimal(product.Rows[0]["Price"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["SalePrice"].ToString()))
                        model.SellPrice = Convert.ToDecimal(product.Rows[0]["SalePrice"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["PurchasePrice"].ToString()))
                        model.PurchasePrice = Convert.ToDecimal(product.Rows[0]["PurchasePrice"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["DiscountAmount"].ToString()))
                        model.DiscountPrice = Convert.ToDecimal(product.Rows[0]["DiscountAmount"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["Profit"].ToString()))
                        model.Profit = Convert.ToDecimal(product.Rows[0]["Profit"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["CGST"].ToString()))
                        model.CGST = Convert.ToDecimal(product.Rows[0]["CGST"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["SGST"].ToString()))
                        model.SGST = Convert.ToDecimal(product.Rows[0]["SGST"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["IGST"].ToString()))
                        model.IGST = Convert.ToDecimal(product.Rows[0]["IGST"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["CategoryId"].ToString()))
                        model.ParentcatId = Convert.ToInt32(product.Rows[0]["CategoryId"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["SubcatId"].ToString()))
                        model.SubcatId = Convert.ToInt32(product.Rows[0]["SubcatId"].ToString());

                    if (!string.IsNullOrEmpty(product.Rows[0]["B2BProfit"].ToString()))
                        model.B2BProfit = Convert.ToInt32(product.Rows[0]["B2BProfit"].ToString());
                    else
                        model.B2BProfit = 0;

                    if (!string.IsNullOrEmpty(product.Rows[0]["B2BSalePrice"].ToString()))
                        model.B2BSellPrice = Convert.ToInt32(product.Rows[0]["B2BSalePrice"].ToString());
                    else
                        model.B2BSellPrice = 0;

                }
                else
                    return Redirect("/Product/ProductList");
            }
            catch
            {
                return Redirect("/Product/ProductList");
            }
            PopulateDropdown();
            return View(model);
        }

        [HttpPost]
        public ActionResult AssignProduct(ProductAssignVM model, FormCollection frm, string[] chkSector)
        {
            var vendorId = frm["ddlVendor"];

            var SectorId = frm["txtSectorid1"];
            Vendor objVendor = new Vendor();
            if (chkSector != null)
            {
                foreach (var item in chkSector)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var product = _product.BindProuct(model.ProductID);
                        if (product.Rows.Count > 0)
                        objvendor.CategoryId = Convert.ToInt32(product.Rows[0]["CategoryId"].ToString());
                        //objvendor.VendorId = Convert.ToInt32(vendorId);
                        objvendor.ProductId = model.ProductID;
                        //objvendor.SectorId = Convert.ToInt32(item);
                        objvendor.VendorCatId = Convert.ToInt32(item);
                        DataTable dtList = new DataTable();
                        dtList = objVendor.getVendorid(objvendor.VendorCatId);

                        if (dtList.Rows.Count > 0)
                        {
                            

                            objvendor.VendorId = Convert.ToInt32(dtList.Rows[0].ItemArray[0].ToString());
                        }
                        // objvendor.att
                        objvendor.AttributeId = model.AttributeID;
                        //var vendorAssignID = objvendor.InsertSectorProduct(objvendor);

                        //new code here
                        int vendorAssignID = 0;
                        string delimStr = ",";
                        char[] delimiter = delimStr.ToCharArray();
                        string a = "";
                        foreach (string s in SectorId.Split(delimiter))
                        {


                            objvendor.SectorId= Convert.ToInt32(s);
                           

                            if (!string.IsNullOrEmpty(objvendor.SectorId.ToString()))
                            {

                                DataTable dtList1 = new DataTable();
                                dtList1 = objVendor.getVendorcatsector(objvendor.VendorCatId, objvendor.SectorId);
                                if(dtList1.Rows.Count>0)
                                {
                                    vendorAssignID = objvendor.InsertSectorProduct1(objvendor);
                                }
                                
                            }

                        }


                         



                       tbl_Product_Attributes attribute = new tbl_Product_Attributes();
                       // Vendor attribute = new Vendor();
                        
                        attribute.ProductID = model.ProductID;
                        attribute.VendorProductAssignID = vendorAssignID;
                        attribute.AttributeID = model.AttributeID;
                        attribute.MRPPrice = model.MRPPrice;
                        attribute.PurchasePrice = model.PurchasePrice;
                        attribute.DiscountPrice = model.DiscountPrice;
                        attribute.CGST = model.CGST;
                        attribute.SGST = model.SGST;
                        attribute.IGST = model.IGST;
                        attribute.Profit = model.Profit;
                        attribute.SellPrice = model.SellPrice;
                        attribute.IsActive = true;
                        attribute.IsDeleted = false;
                        attribute.VendorId = objvendor.VendorId;
                        attribute.VendorCatId = objvendor.VendorCatId;

                        attribute.B2BProfit = model.B2BProfit;

                        attribute.B2BSellPrice = model.B2BSellPrice;
                        // attribute.b2

                       

                        if(vendorAssignID>0)
                        {
                            var id = _AttributeRepo.SaveProductAttribute(attribute);

                            //int 
                            res["status"] = "1";
                        }
                        
                    }
                }
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetProductList(string ProductID)
        {
            Product objprod = new Product();
            DataTable product = _product.GetProductAssignAttribute(Convert.ToInt32(ProductID));
            ViewBag.lstProduct = product;
            if(product.Rows.Count>0)
            {
                string htmlstring = "";
                string attribute = "";
                decimal profit = 0;
                string sector = "";
                //string Id = "";
                string VendorId = "0";
                for (int i = 0; i < product.Rows.Count; i++)
                {
                    int j = i + 1;
                    htmlstring += "<tr>";
                    htmlstring += "<td>" + j + "</td>";
                    htmlstring += "<td>" + product.Rows[i]["VendorName"].ToString() + "</td>";

                    //if(attribute== product.Rows[i]["AttributeId"].ToString() && profit== Convert.ToDecimal(product.Rows[i]["Profit"]) && VendorId == product.Rows[i]["VendorId"].ToString())
                    //{
                    //    sector += " [" + product.Rows[i]["SectorName"].ToString()+"]";
                    //}
                    DataTable dtList = objprod.GetProductAssignAttributeVendorsector(Convert.ToInt32(product.Rows[i]["VendorId"]), Convert.ToInt32(product.Rows[i]["ProductId"]), Convert.ToInt32(product.Rows[i]["AttributeId"]), product.Rows[i]["Profit"].ToString());
                    if (dtList.Rows.Count > 0)
                    {
                        //Id = dtList.Rows[0]["Id"].ToString();
                        sector = "";
                        for (int k = 0; k < dtList.Rows.Count; k++)
                        {
                            sector += " [" +dtList.Rows[k]["SectorName"].ToString()+"]";
                        }
                    }
                    htmlstring += "<td>" + sector + "</td>";
                    htmlstring += "<td>" + product.Rows[i]["AttributeName"].ToString() + "</td>";
                    htmlstring += "<td>" + product.Rows[i]["MRPPrice"].ToString() + "</td>";
                    htmlstring += "<td>" + product.Rows[i]["PurchasePrice"].ToString() + "</td>";
                    htmlstring += "<td>" + product.Rows[i]["DiscountPrice"].ToString() + "</td>";
                    htmlstring += "<td>" + product.Rows[i]["CGST"].ToString() + "</td>";
                    htmlstring += "<td>" + product.Rows[i]["SGST"].ToString() + "</td>";
                    htmlstring += "<td>" + product.Rows[i]["IGST"].ToString() + "</td>";
                    htmlstring += "<td>" + product.Rows[i]["Profit"].ToString() + "</td>";
                    htmlstring += "<td>" + product.Rows[i]["SellPrice"].ToString() + "</td>";
                    htmlstring += "<td>" + product.Rows[i]["B2BProfit"].ToString() + "</td>";
                    htmlstring += "<td>" + product.Rows[i]["B2BSellPrice"].ToString() + "</td>";
                    htmlstring += "<td><a href=\"/Attribute/EditAttribute/"+ product.Rows[i]["ID"].ToString()+"\"  class=\"btn-bootstrap-dialog\" ><i class=\"fa fa-edit\"></i></a></td>";
                    htmlstring += "</tr>";
                    attribute = product.Rows[i]["AttributeId"].ToString();
                    profit =Convert.ToDecimal(product.Rows[i]["Profit"]);
                    VendorId = product.Rows[i]["VendorId"].ToString();

                }

                ViewBag.AssignProduct = htmlstring;
            }

            


            return PartialView("_AssignProductList");
        }

        public ActionResult DeleteProduct(int? ID)
        {
            var product = _AttributeRepo.DeleteProduct(ID);
            return Redirect("/Attribute/AssignProduct?ProductID=" + product);
        }


        public PartialViewResult GetSectorListMsgWise(string cH)
        {
            Vendor objprod = new Vendor();

            string sid = cH;

            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            int id = 0;
            DataTable dt1 = new DataTable();
            foreach (string s in sid.Split(delimiter))
                {
                DataTable dtProduct = objprod.GetSectorAssignedMsg(s);
                dt1.Merge(dtProduct);
                dtProduct.Clear();

            }


                    
            List<VendorSectorVM> product = new List<VendorSectorVM>();
            if (dt1.Rows.Count > 0)
            {
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    VendorSectorVM p = new VendorSectorVM();
                    if (!string.IsNullOrEmpty(dt1.Rows[i]["Name"].ToString()))
                        p.Text = dt1.Rows[i]["Name"].ToString();
                    if (!string.IsNullOrEmpty(dt1.Rows[i]["VendorCatname"].ToString()))
                        p.Text1 = dt1.Rows[i]["VendorCatname"].ToString();
                    if (!string.IsNullOrEmpty(dt1.Rows[i]["SectorName"].ToString()))
                        p.Text2 = dt1.Rows[i]["SectorName"].ToString();
                    if (!string.IsNullOrEmpty(dt1.Rows[i]["Id"].ToString()))
                        p.Text3 = dt1.Rows[i]["Id"].ToString();

                    product.Add(p);
                }
            }
            VendorSectorListVM _list = new VendorSectorListVM();
            _list.VendorSector = product;
            //ViewBag.lstProduct = dtProduct;
            return PartialView("AssignVendorSectorPartial", _list);
        }

        public PartialViewResult GetSectorListVendorWise(string cH,string AttributeId1,string ProductID1)
        {
            Vendor objprod = new Vendor();

            string sid = cH;
            int atid = Convert.ToInt32(AttributeId1);
            int pid = Convert.ToInt32(ProductID1);
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            int id = 0;
            DataTable dt1 = new DataTable();
            foreach (string s in sid.Split(delimiter))
            {
                DataTable dtProduct = objprod.GetSectorAssignedVendor(s,atid,pid);
                dt1.Merge(dtProduct);
                dtProduct.Clear();

            }

            string ch = "";

            List<VendorSectorVM> product = new List<VendorSectorVM>();
            if (dt1.Rows.Count > 0)
            {

                
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    VendorSectorVM p = new VendorSectorVM();
                    if (!string.IsNullOrEmpty(dt1.Rows[i]["statename"].ToString()))
                        p.Text = dt1.Rows[i]["statename"].ToString();
                    if (!string.IsNullOrEmpty(dt1.Rows[i]["Cityname"].ToString()))
                        p.Text1 = dt1.Rows[i]["Cityname"].ToString();

                    if (!string.IsNullOrEmpty(dt1.Rows[i]["VendorCatname"].ToString()))
                        p.Text2 = dt1.Rows[i]["VendorCatname"].ToString();
                    if (!string.IsNullOrEmpty(dt1.Rows[i]["SectorName"].ToString()))
                        p.Text3 = dt1.Rows[i]["SectorName"].ToString();
                    if (!string.IsNullOrEmpty(dt1.Rows[i]["Id"].ToString()))
                        p.Text4 = dt1.Rows[i]["Id"].ToString();

                    if (ch == "")
                    {
                        ch = dt1.Rows[i]["Id"].ToString();

                    }
                    else
                    {
                        ch = ch + "," + dt1.Rows[i]["Id"].ToString();
                    }

                    if (!string.IsNullOrEmpty(dt1.Rows[i]["VendorName"].ToString()))
                        p.Text5 = dt1.Rows[i]["VendorName"].ToString();

                    product.Add(p);
                }

                //ViewBag.Sectorid1 = ch;
            }
            VendorSectorListVM _list = new VendorSectorListVM();
            _list.VendorSector = product;
            //ViewBag.lstProduct = dtProduct;
            return PartialView("AssignVendorSectorPartial1", _list);
        }


        [HttpGet]
        public ActionResult EditAttribute(int? ID)
        {
            ProductAssignVM model = new ProductAssignVM();
            model.ID = Convert.ToInt32(ID);
            //model.ProductID = Convert.ToInt32(ID);
            try
            {
                var product = _product.BindProuctAttribute(model.ID);
                if (product.Rows.Count > 0)
                {
                    ViewBag.ProductName = product.Rows[0]["ProductName"].ToString();
                    model.AttributeID1 = Convert.ToInt32(product.Rows[0]["AttributeID"].ToString());
                    model.AttributeID = Convert.ToInt32(product.Rows[0]["AttributeID"].ToString());
                    model.ProductID=Convert.ToInt32(product.Rows[0]["ProductID"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["Name"].ToString()))
                        model.AttributeName = product.Rows[0]["Name"].ToString();
                    if (!string.IsNullOrEmpty(product.Rows[0]["MRPPrice"].ToString()))
                        model.MRPPrice = Convert.ToDecimal(product.Rows[0]["MRPPrice"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["SellPrice"].ToString()))
                        model.SellPrice = Convert.ToDecimal(product.Rows[0]["SellPrice"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["PurchasePrice"].ToString()))
                        model.PurchasePrice = Convert.ToDecimal(product.Rows[0]["PurchasePrice"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["DiscountPrice"].ToString()))
                        model.DiscountPrice = Convert.ToDecimal(product.Rows[0]["DiscountPrice"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["Profit"].ToString()))
                        model.Profit = Convert.ToDecimal(product.Rows[0]["Profit"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["CGST"].ToString()))
                        model.CGST = Convert.ToDecimal(product.Rows[0]["CGST"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["SGST"].ToString()))
                        model.SGST = Convert.ToDecimal(product.Rows[0]["SGST"].ToString());
                    if (!string.IsNullOrEmpty(product.Rows[0]["IGST"].ToString()))
                        model.IGST = Convert.ToDecimal(product.Rows[0]["IGST"].ToString());
                  

                    if (!string.IsNullOrEmpty(product.Rows[0]["B2BProfit"].ToString()))
                        model.B2BProfit = Convert.ToDecimal(product.Rows[0]["B2BProfit"].ToString());
                    else
                        model.B2BProfit = 0;

                    if (!string.IsNullOrEmpty(product.Rows[0]["B2BSellPrice"].ToString()))
                        model.B2BSellPrice = Convert.ToDecimal(product.Rows[0]["B2BSellPrice"].ToString());
                    else
                        model.B2BSellPrice = 0;


                    ViewBag.Active = Convert.ToBoolean(product.Rows[0]["IsActive"]);

                    //Get Vendor List



                }
                else
                    return Redirect("/Product/ProductList");
            }
            catch
            {
                return Redirect("/Product/ProductList");
            }
            PopulateDropdown();
            return View(model);
        }



        [HttpPost]
        public ActionResult EditAttribute(ProductAssignVM model, FormCollection frm, string[] chkVendor)
        {
            Product objprod = new Product();

            var SectorId = frm["txtSectorid1"];
            Vendor objVendor = new Vendor();
            int id =Convert.ToInt32( model.ID);
            int attribute = Convert.ToInt32(model.AttributeID);
            int attributenew = Convert.ToInt32(model.AttributeID1);
            int proid= Convert.ToInt32(model.ProductID);



            objVendor.AttributeId = attribute;
            objVendor.AttributeId1 = attributenew;
            objVendor.ProductId = proid;


            //

            decimal MRPPrice =Convert.ToDecimal(model.MRPPrice);
            decimal PurchasePrice = Convert.ToDecimal(model.PurchasePrice);
            decimal DiscountPrice = Convert.ToDecimal(model.DiscountPrice);
           
            decimal Profit = Convert.ToDecimal(model.Profit);
            decimal SellPrice = Convert.ToDecimal(model.SellPrice);
            //

            objVendor.MRPPrice = model.MRPPrice;
            objVendor.PurchasePrice = model.PurchasePrice;
            objVendor.DiscountPrice = model.DiscountPrice;
            objVendor.CGST = model.CGST;
            objVendor.SGST = model.SGST;
            objVendor.IGST = model.IGST;
            objVendor.Profit = model.Profit;
            objVendor.SellPrice = model.SellPrice;
            
            objVendor.IsDeleted = false;
           

            objVendor.B2BProfit = model.B2BProfit;

            objVendor.B2BSellPrice = model.B2BSellPrice;
            
            string active = Request["IsActive"];
            if (active == "on")
            {
                bool add = Convert.ToBoolean(true);
                objVendor.IsActive = Convert.ToBoolean(add);
            }
            else
            {
                if (!string.IsNullOrEmpty(active))
                {
                    bool add = Convert.ToBoolean(Request["IsActive"].Split(',')[0]);
                    objVendor.IsActive = Convert.ToBoolean(add);
                }
            }
            var fdate = Request["datepicker"];
            if (!string.IsNullOrEmpty(fdate.ToString()))
            {
                objVendor.updatedon = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
            }

            int update1 = 0, update2 = 0;
            //if(attribute!=attributenew)
            //{

               
                if (chkVendor != null)
                {
                    foreach (var item in chkVendor)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {


                            objVendor.VendorCatId = Convert.ToInt32(item);


                            string delimStr = ",";
                            char[] delimiter = delimStr.ToCharArray();
                            string a = "";
                            foreach (string s in SectorId.Split(delimiter))
                            {


                                objVendor.SectorId = Convert.ToInt32(s);


                                if (!string.IsNullOrEmpty(objVendor.SectorId.ToString()))
                                {

                                    DataTable dtList1 = new DataTable();
                                    dtList1 = objVendor.getVendorcatsector(objVendor.VendorCatId, objVendor.SectorId);

                                    if(dtList1.Rows.Count>0)
                                    {
                                        update1= objVendor.updateVendorwiseProduct(objVendor);
                                    }
                                //order detail and ordertransaction Price update


                                //check MRP and SalePrice Changed or Not
                                DataTable ProductInTransaction = objVendor.ProductInTransactiondatewise(objVendor,proid);
                                if (ProductInTransaction.Rows.Count > 0)
                                {

                                    for (int i = 0; i < ProductInTransaction.Rows.Count; i++)
                                    {
                                        int orderid = Convert.ToInt32(ProductInTransaction.Rows[i].ItemArray[1].ToString());
                                        int qty = Convert.ToInt32(ProductInTransaction.Rows[i].ItemArray[3].ToString());

                                        double samount = Convert.ToDouble(qty) * Convert.ToDouble(objVendor.SellPrice);

                                        double pamount = Convert.ToDouble(qty) * Convert.ToDouble(objVendor.PurchasePrice);
                                        double mamount = Convert.ToDouble(qty) * Convert.ToDouble(objVendor.MRPPrice);

                                        double profit = Convert.ToDouble(qty) * Convert.ToDouble(objVendor.Profit);
                                        double cgst = Convert.ToDouble(qty) * Convert.ToDouble(objVendor.CGST);
                                        double sgst = Convert.ToDouble(qty) * Convert.ToDouble(objVendor.SGST);
                                        double igst = Convert.ToDouble(qty) * Convert.ToDouble(objVendor.IGST);
                                        double disamount = Convert.ToDouble(qty) * Convert.ToDouble(objVendor.DiscountPrice);

                                        int od = objVendor.UpdatePriceFutureOrder(orderid, samount, pamount, mamount, Convert.ToDouble(objVendor.SellPrice), profit, cgst, sgst, igst, disamount, objVendor.AttributeId1);


                                    }


                                }



                            }

                        }

                        DataTable dtCheckPrice = objprod.BindProuctAttributewise(Convert.ToInt32(objVendor.ProductId), Convert.ToInt32(objVendor.AttributeId), Convert.ToInt32(objVendor.VendorCatId));
                        if (dtCheckPrice.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCheckPrice.Rows[0]["MRPPrice"].ToString()))
                                MRPPrice = Convert.ToDecimal(dtCheckPrice.Rows[0]["MRPPrice"]);
                            if (!string.IsNullOrEmpty(dtCheckPrice.Rows[0]["SellPrice"].ToString()))
                                SellPrice = Convert.ToDecimal(dtCheckPrice.Rows[0]["SellPrice"]);

                            if (!string.IsNullOrEmpty(dtCheckPrice.Rows[0]["PurchasePrice"].ToString()))
                                PurchasePrice = Convert.ToDecimal(dtCheckPrice.Rows[0]["PurchasePrice"]);
                        }

                        if (Convert.ToDecimal(MRPPrice) != Convert.ToDecimal(objVendor.MRPPrice) || Convert.ToDecimal(SellPrice) != Convert.ToDecimal(objVendor.SellPrice) || Convert.ToDecimal(PurchasePrice) != Convert.ToDecimal(objVendor.PurchasePrice))
                        {
                            //SaveProductAttributePriceList
                            update2 = objVendor.InsertVendorwiseProductAttribute(objVendor);
                        }


                            update2 = objVendor.updateVendorwiseProductAttribute(objVendor);
                            res["status"] = "1";

                    }
                    }
                }



            //}
            if(attribute==attributenew)
            {

            }
            PopulateDropdown();

            ViewBag.SucMsg = "Attribute Updated Successfully";
            //return View(model);

            return Json(res, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult AttributePriceChangeList(string ProductID)
        {
            Product objproduct = new Product();
            objproduct.Id =Convert.ToInt32(ProductID);

            DataTable product = objproduct.BindProuctPriceChangeList(objproduct.Id);
            
            ViewBag.ProductList = product;
            return View();
        }

            public JsonResult Status(int ID)
        {
            // _VendorRepo.UpdateVendorCatSubcatStatus
            var userInfo = _AttributeRepo.UpdateProductAttributeStatus(ID);
            if (userInfo != null)
            {
                if (userInfo.IsActive == false)
                {
                    userInfo.IsActive = false;
                    res["class"] = "label-danger";
                    res["name"] = "InActive";
                }
                else
                {
                    userInfo.IsActive = true;
                    res["class"] = "label-success";
                    res["name"] = "Active";
                }
                res["id"] = ID.ToString();
                res["status"] = "1";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }


    }
}