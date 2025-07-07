using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MilkWayIndia.Models;
using MilkWayIndia.Entity;

namespace MilkWayIndia.Abstract
{
    public interface IBillPay
    {
        int SaveBillPayCircle(tblBillPayCircle model);
        List<tblBillPayCircle> GetAllBillPayCircle();
        tblBillPayCircle GetBillPayCircleByID(int ID);
        void UpdateBillPayCircleStatus(int ID);
        void DeleteBillPayCircle(int ID);
        Boolean ValidateCircleName(string Name);

        //Bill Pay Services
        List<tblBillPayService> GetAllBillPayService();
        int SaveBillPayService(tblBillPayService model);
        tblBillPayService GetBillPayServiceByID(int ID);
        void UpdateBillPayServiceStatus(int ID);
        void DeleteBillPayService(int ID);

        //Bill pay City
        int SaveBillPayCity(tblBillPayCity model);
        List<tblBillPayCity> GetAllBillPayCity();
        tblBillPayCity GetBillPayCityByID(int ID);
        void UpdateBillPayCityStatus(int ID);
        void DeleteBillPayCity(int ID);
        Boolean ValidateCityName(string Name);

        //Bill pay Operator
        List<tblBillPayOperator> GetAllBillPayOperator();
        int SaveBillPayOperator(tblBillPayOperator model);
        tblBillPayOperator GetBillPayOperatorByID(int ID);
        void UpdateBillPayOperatorStatus(int ID,string type);
        void DeleteBillPayOperator(int ID);
        Boolean ValidateOperatorName(string Name);

        //Bill pay Provider
        List<BillPayProviderVM> GetAllBillPayProvider();
        int SaveBillPayProvider(tblBillPayProvider model);
        int UpdateBillPayProvider(tblBillPayProvider model);
        tblBillPayProvider GetBillPayProviderByID(int ID);
        void UpdateBillPayProviderStatus(int ID);
        void DeleteBillPayProvider(int ID);
    }
}
