Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Configuration
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.Collections.Generic

Partial Class RptCollectionAnalysisViewer
    Inherits System.Web.UI.Page
    Private MyReportDocument As New ReportDocument

#Region "Global Declarations "

    Private _ReportHelper As New ReportHelper

#End Region

#Region "Page Load Starting  "

    'Author			: Anil Kumar - T-Melmax Sdn Bhd
    'Purpose		: Get the AFCReport Report
    'Created Date	: 20/05/2015
    'Modified by Hafiz @ 10/3/2016
    'Modified by Hafiz @ 26/4/2016

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Try
            If Not IsPostBack Then
                Dim _ReportHelper As New ReportHelper
                Dim lsthideunhide As New List(Of String)
                Dim Knockoffcredit As String = Nothing
                Dim Knockoffdebit As String = Nothing
                Dim sortbyvalue As String = Nothing
                Dim status As String = Nothing
                Dim program As String = Nothing
                Dim faculty As String = Nothing
                Dim sponsor As String = Nothing
                Dim datecondition As String = Nothing
                Dim datecondition2 As String = Nothing
                Dim str As String = Nothing
                sortbyvalue = " Order By a.SASI_PgId,a.SASI_Name"

                If Request.QueryString("fdate") = "0" Or Request.QueryString("tdate") = "0" Then

                    datecondition = ""
                    datecondition2 = ""

                Else

                    Dim datef As String = Request.QueryString("fdate")
                    Dim datet As String = Request.QueryString("tdate")
                    Dim d1, m1, y1, d2, m2, y2 As String
                    d1 = Mid(datef, 1, 2)
                    m1 = Mid(datef, 4, 2)
                    y1 = Mid(datef, 7, 4)
                    Dim datefrom As String = y1 + "/" + m1 + "/" + d1
                    d2 = Mid(datet, 1, 2)
                    m2 = Mid(datet, 4, 2)
                    y2 = Mid(datet, 7, 4)
                    Dim dateto As String = y2 + "/" + m2 + "/" + d2

                    datecondition = " and SAS_Accounts.TransDate between '" + datefrom + "' and '" + dateto + "'"
                    datecondition2 = " and TransDate between '" + datefrom + "' and '" + dateto + "'"

                End If

                If Session("type") = "1" Then

                    If Session("sponsor") Is Nothing Then

                        sponsor = "%"

                    Else

                        sponsor = Session("sponsor")

                    End If


                    If Not sponsor = "%" Then

                        str = "select count(distinct creditref)as bil,creditref,creditref1,sum(allocateamount) as total, "
                        str += "(select sasr_name from sas_sponsor where sasr_code = creditref1)as sponsor,"
                        str += "(select sasi_name from sas_student where sasi_matricno = creditref) as studname"
                        str += " from sas_accounts where Category IN ('SPA') AND TransType = 'Credit' AND Description LIKE '%Sponsor Allocation%' and PostStatus = 'Posted'"
                        str += " and creditref1 like '" + sponsor + "'" + datecondition2 + ""
                        str += " group by creditref,creditref1"
                    Else

                        str = "select count(distinct creditref)as bil,creditref1,sum(allocateamount) as total, "
                        str += "(select sasr_name from sas_sponsor where sasr_code = creditref1)as sponsor from sas_accounts where PostStatus = 'Posted' and SubType = 'Student'"
                        str += "and Category IN ('SPA') AND TransType = 'Credit' AND Description LIKE '%Sponsor Allocation%' "
                        str += "" + datecondition2 + ""
                        str += " group by creditref1"
                    End If

                    Dim _result As New DataSet()
                    'DataSet Strating
                    Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)
                    _DataSet.Tables(0).TableName = "Table"

                    'Report CollectionAnalysis Loading XML
                    Dim s As String = Server.MapPath("~/xml/CollectionAnalysys.xml")
                    _DataSet.WriteXml(s)

                    'Records Checking
                    If _DataSet.Tables(0).Rows.Count = 0 Then
                        Response.Write("No Record Found")

                    Else

                        'Report Loading
                        If Session("type") = "1" Then

                            If Session("sponsor") Is Nothing Then

                                sponsor = "%"

                            Else

                                sponsor = Session("sponsor")

                            End If


                            If Not sponsor = "%" Then
                                MyReportDocument.Load(Server.MapPath("~/GroupReport/RptCollectionSponsorDetails.rpt"))
                            Else
                                MyReportDocument.Load(Server.MapPath("~/GroupReport/RptCollectionSponsor.rpt"))
                            End If
                        End If

                        MyReportDocument.SetDataSource(_DataSet)
                        Session("reportobject") = MyReportDocument
                        CrystalReportViewer1.ReportSource = MyReportDocument
                        CrystalReportViewer1.DataBind()
                        MyReportDocument.Refresh()
                        'Report Ended

                    End If

                ElseIf Session("type") = "2" Then
                    Dim Invoice As String = Nothing
                    Dim dateinvoice As String = Nothing
                    Dim amtinvoice As String = Nothing
                    Dim receipt As String = Nothing
                    Dim datereceipt As String = Nothing
                    Dim amtreceipt As String = Nothing
                    If Not Session("invoice") Is Nothing Then
                        Invoice = "Show"
                    Else
                        Invoice = "Hide"
                    End If

                    If Not Session("dateinvoice") Is Nothing Then
                        dateinvoice = "Show"
                    Else
                        dateinvoice = "Hide"
                    End If

                    If Not Session("amtinvoice") Is Nothing Then
                        amtinvoice = "Show"
                    Else
                        amtinvoice = "Hide"
                    End If

                    If Not Session("receipt") Is Nothing Then
                        receipt = "Show"
                    Else
                        receipt = "Hide"
                    End If

                    If Not Session("datereceipt") Is Nothing Then
                        datereceipt = "Show"
                    Else
                        datereceipt = "Hide"
                    End If

                    If Not Session("amtreceipt") Is Nothing Then
                        amtreceipt = "Show"
                    Else
                        amtreceipt = "Hide"
                    End If

                    If Session("invoice") Is Nothing And Session("dateinvoice") Is Nothing And Session("amtinvoice") Is Nothing And Session("receipt") Is Nothing And Session("datereceipt") Is Nothing And Session("amtreceipt") Is Nothing Then
                        Invoice = "Show"
                        dateinvoice = "Show"
                        amtinvoice = "Show"
                        receipt = "Show"
                        datereceipt = "Show"
                        amtreceipt = "Show"
                    End If
                    'Knockoffcredit = "select distinct sa.transid,sa.creditref,sa.transdate,sa.transcode,sa.category,sa.transamount, "
                    'Knockoffcredit += "(select sasi_name from sas_student where sasi_matricno = sa.creditref) as studname,"
                    'Knockoffcredit += "sad.inv_no,sum(sad.paidamount) as paidamt from sas_accounts sa inner join sas_accountsdetails sad"
                    'Knockoffcredit += " on sad.transid = sa.transid where sa.category in ('Invoice','Debit Note','AFC') and sa.subtype = 'Student'"
                    'Knockoffcredit += " and sa.poststatus = 'Posted' and sa.description not LIKE '%Sponsor Pocket%' "
                    'Knockoffcredit += "" + datecondition2 + ""
                    'Knockoffcredit += " group by sa.transid,sa.creditref,sa.transdate,sa.transcode,sa.category,sa.transamount,sad.inv_no"
                    'Knockoffcredit += " order by creditref"

                    Knockoffdebit = "select distinct sa.transid,sa.creditref,sa.transdate,CASE WHEN sa.Category = 'Receipt' THEN CASE WHEN sa.SourceType = 'FER' THEN sa.TransCode "
                    Knockoffdebit += "ELSE CASE WHEN sa.Description LIKE 'CIMB CLICKS%' THEN sa.TransCode "
                    Knockoffdebit += "ELSE CASE WHEN sa.Subcategory='Loan' THEN sa.TransCode "
                    Knockoffdebit += "ELSE sa.BankRecNo END "
                    Knockoffdebit += "END END "
                    Knockoffdebit += "ELSE "
                    Knockoffdebit += "CASE WHEN Category = 'Loan' THEN BatchCode "
                    Knockoffdebit += "ELSE sa.TransCode "
                    Knockoffdebit += "END "
                    Knockoffdebit += "END TransCode,"
                    Knockoffdebit += "sa.category, (select sum(paidamount) from sas_accountsdetails where transcode = sad.inv_no) as debitamt,"
                    Knockoffdebit += "(select sasi_name from sas_student where sasi_matricno = sa.creditref) as studname,sad.inv_no,sum(sad.paidamount) as paidamt,"
                    Knockoffdebit += " (select transdate from sas_accounts where transcode = inv_no) as debitdate from sas_accounts sa"
                    Knockoffdebit += " inner join sas_accountsdetails sad on sad.transid = sa.transid where sa.category in ('Receipt','Credit Note','SPA') and sa.subtype = 'Student'"
                    Knockoffdebit += " and sa.poststatus = 'Posted' and sa.description not LIKE '%Sponsor Pocket%' and sad.inv_no not in ('') "
                    Knockoffdebit += "" + datecondition2 + ""
                    Knockoffdebit += " and sad.inv_no in (select transcode from sas_accounts where category in ('Invoice','Debit Note','AFC') and subtype = 'Student'"
                    Knockoffdebit += " and poststatus = 'Posted' and description not LIKE '%Sponsor Pocket%'"
                    Knockoffdebit += "" + datecondition2 + ""
                    Knockoffdebit += " )group by sa.transid,sa.creditref,sa.transdate,sa.transcode,sa.category,sa.transamount,sad.inv_no"
                    Knockoffdebit += " order by creditref,inv_no"

                    sortbyvalue = "select '" + Invoice + "' as invoice, '" + dateinvoice + "' as dateinvoice, '" + amtinvoice + "' as amtinvoice, "
                    sortbyvalue += "'" + receipt + "' as receipt,'" + datereceipt + "' as datereceipt, '" + amtreceipt + "' as amtreceipt"
                    sortbyvalue += " from sas_universityfund"


                    Dim _DataSet As DataSet = _ReportHelper.GetDataSet(Knockoffdebit, sortbyvalue)
                    _DataSet.Tables(0).TableName = "Knockoffdebit"
                    _DataSet.Tables(1).TableName = "sortbyvalue"

                    'Report CollectionAnalysis Loading XML
                    Dim s As String = Server.MapPath("~/xml/CollectionStudent.xml")
                    _DataSet.WriteXml(s)

                    'Records Checking
                    If _DataSet.Tables(0).Rows.Count = 0 Then
                        Response.Write("No Record Found")

                    Else

                        'Report Loading
                        MyReportDocument.Load(Server.MapPath("~/GroupReport/RptCollectionStudent.rpt"))
                        MyReportDocument.SetDataSource(_DataSet)
                        Session("reportobject") = MyReportDocument
                        CrystalReportViewer1.ReportSource = MyReportDocument
                        CrystalReportViewer1.DataBind()
                        MyReportDocument.Refresh()
                        'Report Ended

                    End If
                    End If

            Else

                    'Report Loading
                    MyReportDocument = Session("reportobject")
                    CrystalReportViewer1.ReportSource = MyReportDocument
                    CrystalReportViewer1.DataBind()
                    MyReportDocument.Refresh()
                    'Report Ended

            End If

        Catch ex As Exception

            Response.Write(ex.Message)

        End Try

    End Sub

#End Region

    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        'MyReportDocument.Close()
        'MyReportDocument.Dispose()
    End Sub

End Class
