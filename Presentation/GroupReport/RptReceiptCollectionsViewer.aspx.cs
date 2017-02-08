using CrystalDecisions.CrystalReports.Engine;
using DataObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RptReceiptCollectionsViewer : MenuAccess
{
    private ReportHelper reportHelper = new ReportHelper();
    private ReportDocument reportDocument = new ReportDocument();

    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder sbFilter = new StringBuilder();

        if (!Page.IsPostBack)
        {
            if (this.ReportType == "Student")
            {
                sbFilter.AppendFormat("SELECT '{0} - {1}' dtFilter, transid, transdate, transcode, bankcode, bankrecno, sabd_desc, sabd_accode, creditref, sasi_name as name, transamount, subtype FROM SAS_Accounts ", this.DateFrom, this.DateTo);
                sbFilter.Append("INNER JOIN SAS_Student ON SAS_Accounts.creditref = SAS_Student.sasi_matricno LEFT JOIN SAS_BankDetails ON SAS_Accounts.bankcode = SAS_BankDetails.sabd_code ");
                sbFilter.AppendFormat("WHERE category = 'Receipt' AND subtype = '{0}' AND sourcetype <> 'FER' AND poststatus = 'Posted' ", this.ReportType);
            }
            else
            {
                sbFilter.AppendFormat("SELECT '{0} - {1}' dtFilter, transid, transdate, transcode, bankcode, bankrecno, sabd_desc, sabd_accode, creditref, sasr_name as name, transamount, subtype FROM SAS_Accounts ", this.DateFrom, this.DateTo);
                sbFilter.Append("INNER JOIN SAS_Sponsor ON SAS_Accounts.creditref = SAS_Sponsor.sasr_code LEFT JOIN SAS_BankDetails ON SAS_Accounts.bankcode = SAS_BankDetails.sabd_code ");
                sbFilter.AppendFormat("WHERE category = 'Receipt' AND subtype = '{0}' AND sourcetype <> 'FER' AND poststatus = 'Posted' ", this.ReportType);
            }

            if (!string.IsNullOrWhiteSpace(this.DateFrom) && this.DateFrom != "0" &&
                !string.IsNullOrWhiteSpace(this.DateTo) && this.DateTo != "0")
            {
                sbFilter.AppendFormat(" AND SAS_Accounts.transdate BETWEEN '{0}' AND '{1}'", this.DateFromCondition, this.DateToCondition);
            }

            sbFilter.AppendFormat(" ORDER BY SAS_Accounts.transdate");

            DataSet dataSet = reportHelper.GetDataSet(sbFilter.ToString());
            dataSet.Tables[0].TableName = "SAS_Accounts";

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                Response.Write("No Record Found");
            }
            else
            {
                //Report Loading
                reportDocument.Load(Server.MapPath("~/GroupReport/RptReceiptCollections.rpt"));
                reportDocument.SetDataSource(dataSet);
                this.ReportDocument = reportDocument;
                CrystalReportViewer1.ReportSource = reportDocument;
                CrystalReportViewer1.DataBind();
                reportDocument.Refresh();

                //Report Ended
            }
        }
        else
        {
            reportDocument = this.ReportDocument;
            CrystalReportViewer1.ReportSource = reportDocument;
            CrystalReportViewer1.DataBind();
            reportDocument.Refresh();
        }
    }
}