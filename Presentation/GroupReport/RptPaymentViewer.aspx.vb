Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Configuration
Imports CrystalDecisions.CrystalReports.Engine
Imports System.Collections.Generic
Imports HTS.SAS.Entities
Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.DataAccessObjects

Partial Class RptPaymentViewer
    Inherits System.Web.UI.Page
    Private MyReportDocument As New ReportDocument

#Region "Global Declarations "
    'Author			: Anil Kumar - T-Melmax Sdn Bhd
    'Created Date	: 20/05/2015

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
                mode = Request.QueryString("mode")
                voucher = Request.QueryString("voucher")
                Dim _result As New DataSet()
                If mode = "Allocation" Then
                    Dim listobjspon As New List(Of String)
                    Dim obj As String = Nothing

                    ListObjects = bobj.GetVoucher(mode, voucher)

                    _result.Tables.Add("VoucherAllocations")
                    _result.Tables("VoucherAllocations").Columns.Add("Name")
                    _result.Tables("VoucherAllocations").Columns.Add("PayeeName")
                    _result.Tables("VoucherAllocations").Columns.Add("voucher")
                    _result.Tables("VoucherAllocations").Columns.Add("Date")
                    _result.Tables("VoucherAllocations").Columns.Add("Desc")
                    _result.Tables("VoucherAllocations").Columns.Add("GLCode")
                    _result.Tables("VoucherAllocations").Columns.Add("Receiptno")
                    _result.Tables("VoucherAllocations").Columns.Add("AccountNo")
                    _result.Tables("VoucherAllocations").Columns.Add("category")
                    _result.Tables("VoucherAllocations").Columns.Add("ICNO")
                    _result.Tables("VoucherAllocations").Columns.Add("amount")
                    _result.Tables("VoucherAllocations").Columns.Add("ReceiptDate")

                    For Each item In ListObjects
                        Dim newRow As DataRow = _
                            _result.Tables("VoucherAllocations").NewRow()
                        newRow("Name") = "PAYEE NAME     :"
                        newRow("PayeeName") = item.PayeeName
                        newRow("voucher") = item.VoucherNo
                        newRow("Date") = item.BatchDate
                        newRow("Desc") = item.Description
                        newRow("GLCode") = item.GLCode
                        newRow("Receiptno") = item.TransactionCode
                        newRow("AccountNo") = ""
                        newRow("category") = "StudentPayment"
                        newRow("ICNO") = ""
                        newRow("amount") = item.TransactionAmount
                        newRow("ReceiptDate") = item.TransDate
                        _result.Tables("VoucherAllocations").Rows.Add(newRow)
                    Next


                ElseIf mode = "Refund" Then

                    category = "Refund"
                    ListObjects = bobj.GetVoucher(mode, voucher)
                    _result.Tables.Add("VoucherAllocations")
                    _result.Tables("VoucherAllocations").Columns.Add("Name")
                    _result.Tables("VoucherAllocations").Columns.Add("PayeeName")
                    _result.Tables("VoucherAllocations").Columns.Add("voucher")
                    _result.Tables("VoucherAllocations").Columns.Add("Date")
                    _result.Tables("VoucherAllocations").Columns.Add("Desc")
                    _result.Tables("VoucherAllocations").Columns.Add("GLCode")
                    _result.Tables("VoucherAllocations").Columns.Add("Receiptno")
                    _result.Tables("VoucherAllocations").Columns.Add("AccountNo")
                    _result.Tables("VoucherAllocations").Columns.Add("category")
                    _result.Tables("VoucherAllocations").Columns.Add("ICNO")
                    _result.Tables("VoucherAllocations").Columns.Add("amount")
                    _result.Tables("VoucherAllocations").Columns.Add("ReceiptDate")

                    For Each item In ListObjects
                        Dim newRow As DataRow = _
                            _result.Tables("VoucherAllocations").NewRow()
                        newRow("Name") = "STUDENT NAME     :"
                        newRow("PayeeName") = item.PayeeName
                        newRow("voucher") = item.VoucherNo
                        newRow("Date") = item.BatchDate
                        newRow("Desc") = item.Description
                        newRow("GLCode") = item.GLCode
                        newRow("Receiptno") = item.TransactionCode
                        newRow("AccountNo") = item.noAkaun
                        newRow("category") = item.Category
                        newRow("ICNO") = item.KodBank
                        newRow("amount") = item.TransactionAmount
                        newRow("ReceiptDate") = item.TransDate
                        _result.Tables("VoucherAllocations").Rows.Add(newRow)
                    Next
                ElseIf mode = "Payment" Then

                    category = "SponsorPayment"
                    ListObjects = bobj.GetVoucher(mode, voucher)
                    _result.Tables.Add("VoucherAllocations")
                    _result.Tables("VoucherAllocations").Columns.Add("Name")
                    _result.Tables("VoucherAllocations").Columns.Add("PayeeName")
                    _result.Tables("VoucherAllocations").Columns.Add("voucher")
                    _result.Tables("VoucherAllocations").Columns.Add("Date")
                    _result.Tables("VoucherAllocations").Columns.Add("Desc")
                    _result.Tables("VoucherAllocations").Columns.Add("GLCode")
                    _result.Tables("VoucherAllocations").Columns.Add("Receiptno")
                    _result.Tables("VoucherAllocations").Columns.Add("AccountNo")
                    _result.Tables("VoucherAllocations").Columns.Add("category")
                    _result.Tables("VoucherAllocations").Columns.Add("ICNO")
                    _result.Tables("VoucherAllocations").Columns.Add("amount")
                    _result.Tables("VoucherAllocations").Columns.Add("ReceiptDate")

                    For Each item In ListObjects
                        Dim newRow As DataRow = _
                            _result.Tables("VoucherAllocations").NewRow()
                        newRow("Name") = "PAYEE NAME     :"
                        newRow("PayeeName") = item.PayeeName
                        newRow("voucher") = item.VoucherNo
                        newRow("Date") = item.BatchDate
                        newRow("Desc") = item.Description
                        newRow("GLCode") = item.GLCode
                        newRow("Receiptno") = item.TransactionCode
                        newRow("AccountNo") = ""
                        newRow("category") = category
                        newRow("ICNO") = ""
                        newRow("amount") = item.TransactionAmount
                        newRow("ReceiptDate") = item.TransDate
                        _result.Tables("VoucherAllocations").Rows.Add(newRow)
                    Next
                ElseIf mode = "Advance" Then

                    category = "Advance"
                    ListObjects = bobj.GetVoucher(mode, voucher)
                    _result.Tables.Add("VoucherAllocations")
                    _result.Tables("VoucherAllocations").Columns.Add("Name")
                    _result.Tables("VoucherAllocations").Columns.Add("PayeeName")
                    _result.Tables("VoucherAllocations").Columns.Add("voucher")
                    _result.Tables("VoucherAllocations").Columns.Add("Date")
                    _result.Tables("VoucherAllocations").Columns.Add("Desc")
                    _result.Tables("VoucherAllocations").Columns.Add("GLCode")
                    _result.Tables("VoucherAllocations").Columns.Add("Receiptno")
                    _result.Tables("VoucherAllocations").Columns.Add("AccountNo")
                    _result.Tables("VoucherAllocations").Columns.Add("category")
                    _result.Tables("VoucherAllocations").Columns.Add("ICNO")
                    _result.Tables("VoucherAllocations").Columns.Add("amount")
                    _result.Tables("VoucherAllocations").Columns.Add("ReceiptDate")

                    For Each item In ListObjects
                        Dim newRow As DataRow = _
                            _result.Tables("VoucherAllocations").NewRow()
                        newRow("Name") = "STUDENT NAME     :"
                        newRow("PayeeName") = item.PayeeName
                        newRow("voucher") = item.VoucherNo
                        newRow("Date") = item.BatchDate
                        newRow("Desc") = item.Description
                        newRow("GLCode") = item.GLCode
                        newRow("Receiptno") = item.TransactionCode
                        newRow("AccountNo") = item.noAkaun
                        newRow("category") = category
                        newRow("ICNO") = item.KodBank
                        newRow("amount") = item.TransactionAmount
                        newRow("ReceiptDate") = item.BatchDate
                        _result.Tables("VoucherAllocations").Rows.Add(newRow)
                    Next
                End If

                'If Not Session("statusPrint") Is Nothing Then

                'Else

                'End If

                'str = "SELECT 'Status' = CASE WHEN Category = 'Payment' THEN 'Allocation' Else 'Refund' END,"
                'str += "'StatusPaymentMode' = CASE WHEN PaymentMode = 'CHQ' THEN 'CHEQUE' Else 'TELEGRAPHIC TRANSFER' END,"
                'str += "BankCode,Description, CreditRef, CreditRef1, BatchCode,CONVERT(VARCHAR(10), BatchDate, 103) BatchDate, CONVERT(VARCHAR(10), TransDate, 103) TransDate,"
                'str += "SubRef1, SAS_AccountsDetails.TransAmount,SAS_Accounts.VoucherNo "
                'str += ",SAS_Student.SASI_MatricNo,SAS_Student.SASI_Name,SAS_Student.SASI_PgId, SAS_Student.SASI_CurSem, SAS_AccountsDetails.TransAmount,SAS_Accounts.VoucherNo, SAS_Accounts.PocketAmount "
                'str += "FROM SAS_Accounts Inner Join SAS_AccountsDetails on SAS_AccountsDetails.transid = SAS_Accounts.TransID "
                'str += "INNER JOIN SAS_Student ON SAS_AccountsDetails.RefCode = SAS_Student.SASI_MatricNo"
                'str += " WHERE SAS_Accounts.Category='" & category & "' AND SAS_Accounts.PostStatus='" & Session("statusPrint") & _
                '       "' and SAS_Accounts.SubType ='Student'"
                'str += cond
               


              
                'DataSet Strating
                Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str, str2)
                Dim _voucherDataSet As DataSet = _result
                _DataSet.Tables.Add(_voucherDataSet.Tables(0).Copy())
                'Report AFCReport Loading XML
                '_DataSet.Tables(0).TableName = "StudentPayments"
                Dim s As String = Server.MapPath("~/xml/Payments.xml")
                _DataSet.WriteXml(s)

                'Report AFCReport Ended XML

                'Records Checking

                If _DataSet.Tables(0).Rows.Count = 0 Then
                    Response.Write("No Record Found")
                Else

                    'Report Loading 

                    MyReportDocument.Load(Server.MapPath("~/GroupReport/RptVoucher.rpt"))

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
