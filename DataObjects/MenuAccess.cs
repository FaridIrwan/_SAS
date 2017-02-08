using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using HTS.SAS.BusinessObjects;
using HTS.SAS.Entities;
using CrystalDecisions.CrystalReports.Engine;

namespace DataObjects
{
    public class MenuAccess : Page
    {
        private string mnuName = string.Empty;
        private string dteFrom = string.Empty;
        private string dteTo = string.Empty;
        private string day, month, year;
        private string rptType = string.Empty;
        private ReportDocument rptDocument;

        public string MenuName
        {
            get
            {
                if (Request.QueryString["Menuid"] != null)
                {
                    mnuName = this.GetMenuName(Convert.ToInt32(Request.QueryString["Menuid"].ToString()));
                }

                return mnuName;
            }
        }

        protected string GetMenuName(int menuId)
        {
            MenuEn menuEn = new MenuEn();
            MenuBAL menuBal = new MenuBAL();

            menuEn.MenuId = menuId;
            menuEn = menuBal.GetMenus(menuEn);

            return menuEn.MenuName;
        }

        protected string DateFrom
        {
            get
            {
                if (Request.QueryString["fdate"] != null)
                {
                    dteFrom = Request.QueryString["fdate"].ToString();
                }

                return dteFrom;
            }
        }

        protected string DateTo
        {
            get
            {
                if (Request.QueryString["tdate"] != null)
                {
                    dteTo = Request.QueryString["tdate"].ToString();
                }

                return dteTo;
            }
        }

        protected string DateFromCondition
        {
            get
            {
                day = DateFrom.Substring(0, 2);
                month = DateFrom.Substring(3, 2);
                year = DateFrom.Substring(6, 4);

                return year + "/" + month + "/" + day;
            }
        }

        protected string DateToCondition
        {
            get
            {
                day = DateTo.Substring(0, 2);
                month = DateTo.Substring(3, 2);
                year = DateTo.Substring(6, 4);

                return year + "/" + month + "/" + day;
            }
        }

        protected string ReportType
        {
            get
            {
                if (Request.QueryString["ReportType"] != null)
                {
                    rptType = Request.QueryString["ReportType"].ToString();
                }

                return rptType;
            }
        }

        protected ReportDocument ReportDocument
        {
            get
            {
                if (Session["reportobject"] != null)
                {
                    rptDocument = Session["reportobject"] as ReportDocument;
                }

                return rptDocument;
            }

            set
            {
                Session["reportobject"] = value;
            }
        }
    }
}
