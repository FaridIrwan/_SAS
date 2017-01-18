Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Configuration
Imports CrystalDecisions.CrystalReports.Engine

Partial Class RptTotalGLViewer
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
        'Modified by Hafiz @ 03/3/2016
        Try

            If Not IsPostBack Then
                Dim datecondition As String = Nothing
                Session("reportobject") = Nothing
                Session("reportobject1") = Nothing
                Dim datef As String = Request.QueryString("fdate")
                Dim datet As String = Request.QueryString("tdate")
                Dim str As String = Nothing

                If Request.QueryString("fdate") = "0" Or Request.QueryString("tdate") = "0" Or Request.QueryString("fdate") Is Nothing Or Request.QueryString("tdate") Is Nothing Then

                    datecondition = ""

                Else

                    Dim datelen As Integer = Len(datef)
                    Dim d1, m1, y1, d2, m2, y2 As String
                    d1 = Mid(datef, 1, 2)
                    m1 = Mid(datef, 4, 2)
                    y1 = Mid(datef, 7, 4)
                    Dim datefrom As String = y1 + "/" + m1 + "/" + d1
                    d2 = Mid(datet, 1, 2)
                    m2 = Mid(datet, 4, 2)
                    y2 = Mid(datet, 7, 4)
                    Dim dateto As String = y2 + "/" + m2 + "/" + d2

                    datecondition = " AND tbl.transdate between '" + datefrom + "' and '" + dateto + "'"

                End If

                'str = " select '" + datef + " - " + datet + "' dt,st.SAFT_Code,st.SAFT_Desc,st.SAFT_GLCode,"
                'str += " isnull(totalAmount,0) transAmount,sg.SAFT_FeeType from SAS_FeeTypes st left join (SELECT SAFT_Code,SAFT_GLCode,"
                'str += " SAFT_Desc,SAFT_FeeType,SAFT_Hostel,(isnull(Credit,0)-isnull(Debit,0)) totalAmount"
                'str += " FROM ( select sf.SAFT_Code,"
                'str += " sf.SAFT_GLCode, sf.SAFT_Desc, sf.SAFT_FeeType, sf.SAFT_Hostel, isnull(sum(sd.transamount), 0)"
                'str += " transAmount,sa.TransType from   SAS_FeeTypes  sf left join SAS_AccountsDetails sd on sd.RefCode = sf.SAFT_Code "
                'str += " left join SAS_Accounts sa on  sa.TransCode = sd.TransCode  " + datecondition + " group by sf.SAFT_Code,"
                'str += " sf.SAFT_GLCode, sf.SAFT_Desc, sf.SAFT_FeeType, sf.SAFT_Hostel,sa.TransType) AS SourceTable"
                'str += " PIVOT (SUM(transamount) FOR TransType IN ([Debit], [Credit])"
                'str += " ) AS PivotTable) sg on sg.SAFT_Code = st.SAFT_Code"

                'added by Hafiz @ 03/3/2016
                'modified by Noah @ 3/1/2017
                'to show those figures without the fee type configured
                str = " select '" + datef + " - " + datet + "' dt,sg.SAFT_Code,sg.SAFT_Desc,sg.SAFT_GLCode,"
                str += " COALESCE(totalAmount,0) transAmount,sg.SAFT_FeeType from"
                str += " ( SELECT SAFT_Code,SAFT_GLCode, SAFT_Desc,SAFT_FeeType,SAFT_Hostel,"
                str += " (SUM(CASE tbl.TransType WHEN 'Debit' THEN tbl.transAmount ELSE 0 END)) totalAmount"
                str += " FROM"
                str += " (select sf.SAFT_Code, sf.SAFT_GLCode, sf.SAFT_Desc, sf.SAFT_FeeType, sf.SAFT_Hostel,"
                str += " coalesce(sum(sd.transamount), 0) transAmount, sa.TransType, sa.transdate"
                str += " from SAS_Accounts sa LEFT JOIN SAS_AccountsDetails sd ON sa.transid = sd.transid"
                str += " LEFT JOIN SAS_FeeTypes sf on sd.RefCode = sf.SAFT_Code"
                str += " WHERE sa.category = 'AFC' OR (sa.category = 'Invoice' AND sa.subtype = 'Student') OR (sa.category = 'Debit Note' AND sa.subtype = 'Student')"
                str += " group by sf.SAFT_Code, sf.SAFT_GLCode, sf.SAFT_Desc, sf.SAFT_FeeType, sf.SAFT_Hostel, sa.TransType, sa.transdate) AS tbl"
                str += " WHERE 1=1"
                str += datecondition
                str += " group by SAFT_Code, SAFT_GLCode, SAFT_Desc, SAFT_FeeType, SAFT_Hostel) sg "

                'Author			: Anil Kumar - T-Melmax Sdn Bhd
                'Created Date	: 20/05/2015

                'DataSet Strating
                Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)
                _DataSet.Tables(0).TableName = "Table"
                'Report XML Loading

                Dim s As String = Server.MapPath("~/xml/totalGL.xml")
                _DataSet.WriteXml(s)

                'Report XML Ended

                'Records Checking

                If _DataSet.Tables(0).Rows.Count = 0 Then
                    Response.Write("No Record Found")

                Else

                    'Report Loading
                    'Dim subRpt As New ReportDocument
                    'subRpt.Load(Server.MapPath("RptBayaranPelajar.rpt"))
                    'subRpt.SetDataSource(ds1)
                    'MyReportDocument.Subreports("subRpt").Load(Server.MapPath("RptBayaranPelajar.rpt"))
                    'MyReportDocument.Subreports("subRpt").SetDataSource(ds1)

                    MyReportDocument.Load(Server.MapPath("~/GroupReport/RptTotalGL.rpt"))
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
        'MyReportDocument.Close()
        'MyReportDocument.Dispose()
    End Sub
End Class
