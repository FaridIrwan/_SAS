using DataObjects;
using HTS.SAS.BusinessObjects;
using HTS.SAS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RptReceiptCollections : MenuAccess
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            lblMenuName.Text = this.MenuName;
            this.AssignControls();
            this.LoadUserRights();
        }
    }

    protected void ibtnCancel_Click(object sender, ImageClickEventArgs e)
    {
        this.ResetControls();
    }

    protected void AssignControls()
    {
        imgDteFrom.Attributes.Add("onclick", "return getDteFrom()");
        imgDteTo.Attributes.Add("onclick", "return getDteTo()");
        ibtnPrint.Attributes.Add("onclick", "return getDate()");
        ibtnView.Attributes.Add("onclick", "return getDate()");
        txtDteFrom.Attributes.Add("onkeyup", "return CheckFromDate()");
        txtDteTo.Attributes.Add("onkeyup", "return CheckToDate()");
        txtDteFrom.Text = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
        txtDteTo.Text = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
    }

    protected void LoadUserRights()
    {
        UsersBAL usersBal = new UsersBAL();
        UserRightsEn userRightsEn = new UserRightsEn();

        userRightsEn = usersBal.GetUserRights(Convert.ToInt32(Request.QueryString["Menuid"]), Convert.ToInt32(Session[Helper.UserGroupSession]));

        if (userRightsEn.IsAdd)
        {
            ibtnSave.Enabled = true;
        }
        else
        {
            ibtnNew.Enabled = false;
            ibtnNew.ImageUrl = "../images/gAdd.png";
            ibtnNew.ToolTip = "Access Denied";
            ibtnDelete.Enabled = false;
            ibtnDelete.ImageUrl = "../images/gdelete.png";
            ibtnDelete.ToolTip = "Access Denied";
            ibtnFirst.Enabled = false;
            ibtnLast.Enabled = false;
            ibtnPrevs.Enabled = false;
            ibtnNext.Enabled = false;
            ibtnFirst.ToolTip = "Access Denied";
            ibtnLast.ToolTip = "Access Denied";
            ibtnPrevs.ToolTip = "Access Denied";
            ibtnNext.ToolTip = "Access Denied";
            ibtnFirst.ImageUrl = "../images/gnew_first.png";
            ibtnLast.ImageUrl = "../images/gnew_last.png";
            ibtnPrevs.ImageUrl = "../images/gnew_Prev.png";
            ibtnNext.ImageUrl = "../images/gnew_next.png";
            ibtnSave.Enabled = false;
            ibtnSave.ImageUrl = "../images/gsave.png";
            ibtnSave.ToolTip = "Access Denied";
        }

        if (userRightsEn.IsEdit)
        {
            Session["EditFlag"] = true;
        }
        else
        {
            Session["EditFlag"] = false;
        }

        ibtnView.Enabled = userRightsEn.IsView;

        if (userRightsEn.IsView)
        {
            ibtnView.ImageUrl = "../images/find.png";
            ibtnView.Enabled = true;
        }
        else
        {
            ibtnView.ImageUrl = "../images/gfind.png";
            ibtnView.ToolTip = "Access Denied";
        }

        ibtnPrint.Enabled = userRightsEn.IsPrint;

        if (userRightsEn.IsPrint)
        {
            ibtnPrint.Enabled = true;
            ibtnPrint.ImageUrl = "../images/print.png";
            ibtnPrint.ToolTip = "Print";
        }
        else
        {
            ibtnPrint.Enabled = false;
            ibtnPrint.ImageUrl = "../images/gprint.png";
            ibtnPrint.ToolTip = "Access Denied";
        }

        if (userRightsEn.IsOthers)
        {
            ibtnOthers.Enabled = true;
            ibtnOthers.ImageUrl = "../images/others.png";
            ibtnOthers.ToolTip = "Others";
        }
        else
        {
            ibtnOthers.Enabled = false;
            ibtnOthers.ImageUrl = "../images/gothers.png";
            ibtnOthers.ToolTip = "Access Denied";
        }

        if (userRightsEn.IsPost)
        {
            ibtnPosting.Enabled = true;
            ibtnPosting.ImageUrl = "../images/posting.png";
            ibtnPosting.ToolTip = "Posting";
        }
        else
        {
            ibtnPosting.Enabled = false;
            ibtnPosting.ImageUrl = "../images/gposting.png";
            ibtnPosting.ToolTip = "Access Denied";
        }
    }

    protected void ResetControls()
    {
        ddlReportType.SelectedIndex = -1;
        txtDteFrom.Text = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
        txtDteTo.Text = String.Format("{0:dd/MM/yyyy}", DateTime.Now);
    }

}