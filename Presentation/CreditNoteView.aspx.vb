Imports System.Collections.Generic
Imports HTS.SAS.Entities
Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.DataAccessObjects
Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports SQLPowerQueryManager.PowerQueryManager
Imports System.Linq
Imports System.ComponentModel

Partial Class CreditNoteView
    Inherits System.Web.UI.Page
#Region "Global Declarations "
    Private _WorkflowDAL As New HTS.SAS.DataAccessObjects.WorkflowDAL
    Private _AccountsDAL As New HTS.SAS.DataAccessObjects.AccountsDAL
    Private _Helper As New Helper
    Private _GstSetupDal As New HTS.SAS.DataAccessObjects.GSTSetupDAL
    Private sumAmt As Double = 0
    Dim sumGST As Double = 0
#End Region
    ''Private LogErrors As LogError
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            If Not Page.IsPostBack Then

                DirectCast(Master.FindControl("Panel1"), System.Web.UI.WebControls.Panel).Visible = False
                DirectCast(Master.FindControl("td"), System.Web.UI.HtmlControls.HtmlTableCell).Visible = False

                Dim TransID As String = MaxGeneric.clsGeneric.NullToString(Request.QueryString("TransID"))
                'Dim Category As String = MaxGeneric.clsGeneric.NullToString(Request.QueryString("Category"))
                'Dim Type As String = MaxGeneric.clsGeneric.NullToString(Request.QueryString("SubType"))
                'added by Hafiz @ 31/3/2016
                Dim MatricNo As String = MaxGeneric.clsGeneric.NullToString(Request.QueryString("MatricNo"))
                Dim docno As String = MaxGeneric.clsGeneric.NullToString(Request.QueryString("docno"))
                'added by Mona @ 30/6/2016
                Dim StudentCurrentSem As String = MaxGeneric.clsGeneric.NullToString(Request.QueryString("CurSem"))

                'Call Bind Grid
                'Call BindGrid(TransID, Category, Type)
                addIntake()
                Call BindGrid(docno)
                'While loading the page make the CFlag as null
                Session("PageMode") = ""
                'Loading User Rights
                ' LoadUserRights()

                'while loading list object make it nothing
                'Session("ListObj") = Nothing
                ' DisableRecordNavigator()
                'load PageName
                '  Menuname(CInt(Request.QueryString("Menuid")))
            End If

        Catch ex As Exception

            Call MaxModule.Helper.LogError(ex.Message)

        End Try
    End Sub
    Private Enum dgViewCell As Integer
        CheckBox = 0
        MatricNo = 1
        StudentName = 2
        ReferenceCode = 3
        Description = 4
        Fee_Amount = 5
        TransactionAmount = 6
        Fee_Code = 7
        Actual_Fee_Amount = 8
        GSTAmount = 9
        Total_Fee_Amount = 10
        Priority = 11
        TaxId = 12
        Transid = 13
    End Enum

    

#Region "Methods"
    ''' <summary>
    ''' Method to Fill the Intake DropDown
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub addIntake()
        Dim eIntake As New SemesterSetupEn
        Dim bIntake As New SemesterSetupBAL
        Dim listIntake As New List(Of SemesterSetupEn)
        ddlIntake.Items.Clear()
        ddlIntake.Items.Add(New ListItem("---Select---", "-1"))
        'ddlIntake.Items.Add(New ListItem("All", "1"))
        ddlIntake.DataTextField = "SemisterSetupCode"
        ddlIntake.DataValueField = "SemisterSetupCode"
        eIntake.SemisterSetupCode = "%"

        Try
            listIntake = bIntake.GetListSemesterCode(eIntake)
        Catch ex As Exception
            LogError.Log("BatchInvoice", "addIntake", ex.Message)
        End Try
        ddlIntake.DataSource = listIntake
        ddlIntake.DataBind()
        'Session("faculty") = listfac
    End Sub
    
    Private Sub dates()
        'Dim GBFormat As System.Globalization.CultureInfo
        'GBFormat = New System.Globalization.CultureInfo("en-GB")
        txtBatchDate.Text = Format(Date.Now, "dd/MM/yyyy")
        txtInvoiceDate.Text = Format(Date.Now, "dd/MM/yyyy")
        txtDuedate.Text = Format(DateAdd(DateInterval.Day, 30, Date.Now), "dd/MM/yyyy")
    End Sub
    
    'Public Sub BindGrid()
    '    Dim eob As New AccountsEn
    '    Dim obj As New AccountsBAL
    '    Dim ListObjects As New List(Of AccountsEn)

    '    eob.PostStatus = "Posted"
    '    eob.SubType = "Student"
    '    Try
    '        ListObjects = obj.GetTransactions(eob)
    '    Catch ex As Exception
    '        LogError.Log("BatchInvoice", "LoadListObjects", ex.Message)
    '    End Try

    'End Sub
    Private Sub BindGrid(ByVal docno As String)

        'Create Instances
        Dim WorkFlowAccountDetails As DataSet = Nothing
        Dim ProgStudentDetails As DataSet = Nothing
        Dim StudentFeeDetails As DataSet = Nothing
        Dim ListTranctionDetails As New List(Of AccountsDetailsEn)
        Dim StudentCount As Integer = 0
        Dim BatchAmount As Decimal = 0
        Dim obje As New AccountsBAL
        Dim eob As New AccountsEn
        Dim item As New AccountsEn
        Dim objProcess As New AccountsDetailsBAL
        Dim m_no As String = Nothing
        Try

           
            eob.TransactionCode = docno
            item = obje.GetItemByTransCode(eob)

            If item.BatchCode <> "" Then
                txtBatchNo.Text = item.BatchCode
                txtBatchDate.Text = item.BatchDate
                txtDesc.Text = item.Description
                txtInvoiceDate.Text = item.TransDate
                txtDuedate.Text = item.DueDate
                txttotalpaid.Text = item.PaidAmount
                txtstatuss.Text = item.TransStatus
                If item.CreditRef <> "" Then
                    m_no = item.CreditRef
                Else
                    m_no = ""
                End If
                If item.BatchIntake <> "" Or item.BatchIntake IsNot Nothing Then
                    ddlIntake.SelectedValue = item.BatchIntake
                Else
                    ddlIntake.SelectedIndex = -1
                End If
                If item.PostStatus = "Posted" Then
                    lblStatus.Value = "Posted"
                    ibtnStatus.ImageUrl = "images/Posted.gif"
                End If
                btnBatchInvoice.Text = item.Category
                ListTranctionDetails = objProcess.GetStudentAccountsDetailsByBatchCode(item, m_no)
                MultiView1.SetActiveView(View1)
                'dgView.Visible = True
                dgView.DataSource = ListTranctionDetails
                dgView.DataBind()
            End If

        Catch ex As Exception

            MaxModule.Helper.LogError(ex.Message)

        End Try

    End Sub
    Public Function GSTFeeAmt(ByVal FeeAmt As Double, ByVal gst As Double, ByVal TaxId As Integer) As String
        Dim TaxMode As Integer = 0
        Try
            TaxMode = _GstSetupDal.GetGstDetails(MaxGeneric.clsGeneric.NullToInteger(TaxId)).Tables(0).Rows(0)(3).ToString()
        Catch

        End Try
        Dim FeeAmout As Double = 0
        If (TaxMode = Generic._TaxMode.Inclusive) Then
            FeeAmout = MaxGeneric.clsGeneric.NullToDecimal(FeeAmt)
        ElseIf (TaxMode = Generic._TaxMode.Exclusive) Then
            FeeAmout = MaxGeneric.clsGeneric.NullToDecimal(FeeAmt) - gst
        End If
        Return FeeAmout
    End Function
    Public Function GSTActAmt(ByVal Amt As Double, ByVal gst As Double, ByVal TaxId As Integer) As String
        Dim ActAmout As Double = 0
        Dim TaxMode As Integer = 0
        If Not TaxId = Nothing Then
            If TaxId <> 0 Then
                TaxMode = _GstSetupDal.GetGstDetails(MaxGeneric.clsGeneric.NullToInteger(TaxId)).Tables(0).Rows(0)(3).ToString()
            End If
            If (TaxMode = Generic._TaxMode.Inclusive) Then
                ActAmout = MaxGeneric.clsGeneric.NullToDecimal(Amt) - gst
            ElseIf (TaxMode = Generic._TaxMode.Exclusive) Then
                ActAmout = MaxGeneric.clsGeneric.NullToDecimal(Amt) - gst
            End If
        End If
        Return String.Format("{0:F}", ActAmout)

    End Function
    Protected Sub btnHidden_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        'If Not Session("liststu") Is Nothing Then
        '    addStudent()
        'End If
        'If Not Session("eobj") Is Nothing Then
        '    addFeeType()
        'End If
    End Sub
    Protected Sub btnHiddenApp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnHiddenApp.Click

    End Sub
    Protected Sub dgView_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgView.ItemDataBound
        Dim txtAmount As TextBox
        Dim GSTAmt As Double = 0
        Dim TaxId As Integer = 0
        Dim TaxMode As Integer = 0
        Dim chk As CheckBox
        Dim CBSelected As New List(Of Integer)

        TaxId = Session("TaxId")
        'If Session("CBSelected") IsNot Nothing Then
        '    CBSelected = Session("CBSelected")
        'End If

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.SelectedItem
                chk = e.Item.Cells(dgViewCell.CheckBox).Controls(1)

                txtAmount = CType(e.Item.FindControl("txtFeeAmt"), TextBox)
                txtAmount.Attributes.Add("onKeyPress", "checkValue();")
                txtAmount.Text = String.Format("{0:F}", CDbl(txtAmount.Text))

              
                        sumAmt = sumAmt + CDbl(e.Item.Cells(dgViewCell.TransactionAmount).Text)
                        sumGST = sumGST + CDbl(e.Item.Cells(dgViewCell.GSTAmount).Text)

            Case ListItemType.Footer

                e.Item.Cells(dgViewCell.TransactionAmount).Text = sumAmt.ToString
                e.Item.Cells(dgViewCell.Total_Fee_Amount).Text = String.Format("{0:F}", sumAmt)
                e.Item.Cells(dgViewCell.GSTAmount).Text = String.Format("{0:F}", sumGST)

                'txtTotalFeeAmt.Text = String.Format("{0:F}", sumAmt)
                txtTotal.Text = String.Format("{0:F}", sumAmt)

        End Select

        'If chkSelectedView.Checked = True Then
        '    Select Case e.Item.ItemType
        '        Case ListItemType.Item, ListItemType.AlternatingItem
        '            chk = CType(e.Item.FindControl("chkview"), CheckBox)
        '            chk.Checked = True
        '    End Select
        '    'End If
        'End If

    End Sub
#End Region
    End Class   