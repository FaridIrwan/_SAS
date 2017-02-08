Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Configuration
Imports CrystalDecisions.CrystalReports.Engine
Imports System.Collections.Generic
Imports HTS.SAS.Entities
Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.DataAccessObjects

Partial Class RptRefundAdvanceViewer
    Inherits System.Web.UI.Page
    Private MyReportDocument As New ReportDocument

#Region "Global Declarations "


    Private _ReportHelper As New ReportHelper

#End Region

#Region "Page Load Starting  "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        Try

            If Not IsPostBack Then

                Dim cond As String = Nothing
                Dim mode As String = Nothing
                Dim category As String = Nothing
                Dim str As String = Nothing
                Dim str2 As String = Nothing
                Dim voucher As String = Nothing
                Dim bobj As New StudentDAL
                'Dim eobj As New SponsorCoverLetterEn
                Dim ListObjects As New List(Of AccountsEn)
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
                mode = Request.QueryString("mode")
                voucher = Request.QueryString("voucher")
                Dim _result As New DataSet()
                If mode = "Advance" Then
                    category = "Student Advance"
                    If voucher = "All" Then
                        str = "select distinct '" + category + "' as category,sa.transdate,ss.sasi_matricno,ss.sasi_name,ss.sasi_icno,ss.sasi_accno, "
                        str += "'" + datef + "' as datefrom,'" + datet + "' as dateto,"
                        str += "sa.bankcode,sa.voucherno,sa.chequeno,sa.transamount from sas_studentloan sa inner join sas_student ss "
                        str += "on ss.sasi_matricno = sa.creditref "
                        str += "where sa.category = 'Loan' and sa.poststatus = 'Posted' and sa.transdate between'" + datefrom + "' and '" + dateto + "'"
                    Else
                        str = "select distinct '" + category + "' as category,sa.transdate,ss.sasi_matricno,ss.sasi_name,ss.sasi_icno,ss.sasi_accno, "
                        str += "'" + datef + "' as datefrom,'" + datet + "' as dateto,"
                        str += "sa.bankcode,sa.voucherno,sa.chequeno,sa.transamount from sas_studentloan sa inner join sas_student ss "
                        str += "on ss.sasi_matricno = sa.creditref "
                        str += "where sa.category = 'Loan' and sa.poststatus = 'Posted' and sa.transdate between'" + datefrom + "' and '" + dateto + "'"
                        str += " and sa.voucherno ='" + voucher + "'"
                    End If

                ElseIf mode = "Refund" Then
                    category = "Student Refund"
                    If voucher = "All" Then
                        str = "select distinct '" + category + "' as category,sa.transdate,ss.sasi_matricno,ss.sasi_name,ss.sasi_icno,ss.sasi_accno, "
                        str += "'" + datef + "' as datefrom,'" + datet + "' as dateto,"
                        str += "sa.bankcode,sa.voucherno,sa.chequeno,sa.transamount from sas_accounts sa inner join sas_student ss "
                        str += "on ss.sasi_matricno = sa.creditref "
                        str += "where sa.category = 'Refund' and sa.poststatus = 'Posted' and sa.transdate between'" + datefrom + "' and '" + dateto + "'"
                    Else
                        str = "select distinct '" + category + "' as category,sa.transdate,ss.sasi_matricno,ss.sasi_name,ss.sasi_icno,ss.sasi_accno, "
                        str += "'" + datef + "' as datefrom,'" + datet + "' as dateto,"
                        str += "sa.bankcode,sa.voucherno,sa.chequeno,sa.transamount from sas_accounts sa inner join sas_student ss "
                        str += "on ss.sasi_matricno = sa.creditref "
                        str += "where sa.category = 'Refund' and sa.poststatus = 'Posted' and sa.transdate between'" + datefrom + "' and '" + dateto + "'"
                        str += " and sa.voucherno ='" + voucher + "'"
                    End If
                End If


                'DataSet Strating
                Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)
                Dim s As String = Server.MapPath("~/xml/RefundAdvance.xml")
                _DataSet.WriteXml(s)

                'Report AFCReport Ended XML

                'Records Checking

                If _DataSet.Tables(0).Rows.Count = 0 Then
                    Response.Write("No Record Found")
                Else

                    'Report Loading 

                    MyReportDocument.Load(Server.MapPath("~/GroupReport/RptRefundAdvance.rpt"))

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
End Class
