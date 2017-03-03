Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Configuration
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Partial Class RptStudentLedgerViewer
    Inherits System.Web.UI.Page
    Private MyReportDocument As New ReportDocument

#Region "Global Declarations "
    'Author			: Anil Kumar - T-Melmax Sdn Bhd
    'Created Date	: 20/05/2015

    Private _ReportHelper As New ReportHelper

#End Region

#Region "Page Load Starting  "

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        'Author			: Anil Kumar - T-Melmax Sdn Bhd
        'Created Date	: 20/05/2015

        Try

            If Not IsPostBack Then

                Dim sortbyvalue As String = Nothing
                Dim status As String = Nothing
                Dim datecondition As String = Nothing
                Dim str As String = Nothing

                If Session("sortby") = Nothing Then

                    sortbyvalue = " Order By SASI_PgId, SASI_Name, Sb_TransDate "

                Else

                    sortbyvalue = " Order By " + Session("sortby") + ", Sb_TransDate "

                End If

                If Session("status") = Nothing Or Session("status") = -1 Then

                    status = "%"

                Else

                    status = Session("status")

                End If
                str = Session("studentCode")
                Dim GetSponsorSQL As String = Nothing
                Dim Category As String = Session("LedgerType").ToString()
                If Session("studentCode") <> Nothing Then



                    'If Category = "Student Ledger" Then

                    str = "select ss.SASI_Name,ss.SASI_ICNo,ss.sasi_MatricNo,ss.SASI_PgId,ss.SASI_CurSemYr,"
                    'str += " ss.SART_Code || ss.SASI_FloorNo || ss.SABK_Code || ss.SAKO_Code Hostel,"
                    str += " ss.SASI_Intake,ss.SASI_CurSem,sa.Category, (select sk.sako_description from"
                    str += " sas_kolej sk where sk.sako_code=ss.SAKO_Code) as Hostel, ss.SASS_Code , ss.SASc_Code,"
                    str += " to_char(sa.transdate, 'DD/MM/YYYY') Sb_TransDate, CASE WHEN sa.Category = 'SPA' "
                    str += " THEN SUBSTRING(sa.Description, 9, 17) ::text || ' | ' || "
                    str += " ( SELECT Description FROM sas_accounts WHERE"
                    str += " batchcode = sa.BatchCode and category = 'Allocation' and subtype = 'Sponsor')::text"
                    str += " ELSE sa.Description END Description, CASE WHEN"
                    str += " sa.Category = 'Receipt' THEN CASE WHEN sa.Description LIKE 'CIMB CLICKS%' THEN "
                    str += " sa.TransCode Else CASE WHEN SA.Subcategory='Loan' THEN SA.TransCode ELSE"
                    str += " SA.BankRecNo END End Else SA.TransCode END TransCode, CASE WHEN sa.category ="
                    str += " 'Credit Note' or sa.category = 'Debit Note' or sa.category = 'Invoice' then CASE"
                    str += " WHEN sa.transtype='Credit' THEN sa.transamount ELSE 0 End Else CASE WHEN"
                    str += " sa.transtype='Credit' THEN CASE WHEN SA.Category='Loan' THEN 0 ELSE sa.transamount "
                    str += " END ELSE 0 End End as Credit, CASE when sa.category = 'Credit Note' or sa.category ="
                    str += " 'Debit Note' or sa.category = 'Invoice' THEN CASE when sa.transtype = 'Debit' then"
                    str += " sa.transamount Else 0 End Else CASE WHEN sa.TransType = 'Debit' THEN sa.TransAmount"
                    str += " Else CASE WHEN SA.Category='Loan' THEN sa.transamount ELSE 0 END End END as Debit,"
                    str += " sa.transamount,sa.postedtimestamp FROM SAS_Student ss   inner join sas_accounts sa"
                    str += " on sa.creditref=ss.sasi_MatricNo  and sa.PostStatus = 'Posted' and sa.SubType ="
                    str += " 'Student' left join sas_accountsdetails de on sa.transid = de.transid where"
                    str += " sa.transtype in  ('Debit','Credit')"
                    str += " and sa.creditref = '" + Session("studentCode") + "' AND sa.TransAmount > 0 "
                    str += " group by ss.sasi_name,ss.SASI_ICNo,ss.sasi_MatricNo,ss.SASI_PgId,ss.SASI_CurSemYr,sa.Category,SA.Subcategory,ss.SASI_Intake,ss.SASI_CurSem,ss.SART_Code,"
                    str += " ss.SASI_FloorNo, ss.SABK_Code, ss.SAKO_Code, ss.SASS_Code, ss.SASc_Code, sa.transcode, sa.transdate, sa.description, SA.transtype, sa.bankrecno, sa.batchcode, sa.TransAmount,sa.postedtimestamp"
                    'ElseIf Category = "Loan Ledger" Then
                    str += " UNION"
                    str += " select ss.SASI_Name, ss.SASI_ICNo,ss.sasi_MatricNo,ss.SASI_PgId,ss.SASI_CurSemYr,"
                    'str += " ss.SART_Code || ss.SASI_FloorNo || ss.SABK_Code || ss.SAKO_Code Hostel,"
                    str += " ss.SASI_Intake,ss.SASI_CurSem,lo.Category, (select sk.sako_description from"
                    str += " sas_kolej sk where sk.sako_code=ss.SAKO_Code) as Hostel, ss.SASS_Code , ss.SASc_Code,"

                    'str += " CASE WHEN lo.TransType = 'Credit' THEN lo.TransAmount"
                    'str += " ELSE 0 END Debit ,"
                    'str += " CASE WHEN lo.TransType = 'Debit' THEN lo.TransAmount "
                    'str += " ELSE 0 END Credit ,"

                    str += " to_char(lo.transdate, 'DD/MM/YYYY') Sb_TransDate, lo.description,lo.transcode,CASE"
                    str += " WHEN lo.Category = 'Receipt' THEN  CASE WHEN lo.TransType = 'Credit' THEN"
                    str += " lo.TransAmount ELSE 0 End Else CASE WHEN lo.TransType = 'Debit' THEN lo.TransAmount"
                    str += " ELSE 0 End END Credit,CASE WHEN lo.Category = 'Receipt' THEN  CASE WHEN lo.TransType"
                    str += " = 'Debit' THEN lo.TransAmount ELSE 0 End Else CASE WHEN lo.TransType = 'Credit' "
                    str += " THEN lo.TransAmount ELSE 0 End END Debit,"
                    str += " lo.TransAmount,lo.postedtimestamp FROM  SAS_StudentLoan lo left join sas_student"
                    str += " ss on lo.creditref = ss.sasi_matricno WHERE lo.transtype in  ('Debit','Credit') and"
                    str += " lo.CreditRef = '" + Session("studentCode") + "'  AND lo.SubType = 'Student' and lo.PostStatus ='Posted'" ' and lo.Transstatus = 'Closed'
                    str += " ORDER BY postedtimestamp"

                    GetSponsorSQL = "select spon.sasr_code, spon.sasr_name, sspon.sasi_matricno from SAS_STUDENTSPON sspon inner join sas_sponsor spon on sspon.sass_sponsor = spon.sasr_code where sasi_matricno='" + Session("studentCode") + "'"
                End If

                'Author			: Anil Kumar - T-Melmax Sdn Bhd
                'Created Date	: 20/05/2015

                'DataSet Strating
                Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)
                _DataSet.Tables(0).TableName = "Table"

                Dim _result As New DataSet()
                _result.Tables.Add("Category")
                _result.Tables("Category").Columns.Add("Type")
                Dim newRow As DataRow = _result.Tables("Category").NewRow()
                newRow("Type") = Category
                _result.Tables("Category").Rows.Add(newRow)

                Dim _SponDataSet As DataSet = _ReportHelper.GetDataSet(GetSponsorSQL)
                _SponDataSet.Tables(0).TableName = "StudentSpon"

                _DataSet.Tables.Add(_result.Tables(0).Copy())
                _DataSet.Tables.Add(_SponDataSet.Tables(0).Copy())

                'Report XML Loading

                Dim s As String = Server.MapPath("~/xml/StudentLedger.xml")

                _DataSet.WriteXml(s)

                'Report XML Ended

                'Records Checking

                If _DataSet.Tables(0).Rows.Count = 0 Then
                    Response.Write("No Record Found")

                Else

                    'Report Loading 
                    MyReportDocument.Load(Server.MapPath("~/GroupReport/RptStudentLedger.rpt"))


                    MyReportDocument.SetDataSource(_DataSet)
                    Session("reportobject") = MyReportDocument
                    CrystalReportViewer1.ReportSource = MyReportDocument
                    CrystalReportViewer1.DataBind()
                    MyReportDocument.Refresh()

                    'Report Ended
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
        ' Session("studentCode") = Nothing
        ' Session("status") = Nothing
        'MyReportDocument.Close()
        'MyReportDocument.Dispose()
    End Sub
End Class
