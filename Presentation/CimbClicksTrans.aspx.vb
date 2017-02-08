#Region "NameSpaces "

Imports System.IO
Imports System.Data
Imports MaxGeneric
Imports HTS.SAS.Entities
Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.DataAccessObjects
Imports System.Collections.Generic
Imports System.Linq

#End Region

Partial Class CimbClicksTrans
    Inherits System.Web.UI.Page

#Region "File Paths "

    Private ReadOnly Property GetUploadFilePath As String
        Get
            Return clsGeneric.NullToString(
                ConfigurationManager.AppSettings("CIMB_CLICKS_UPLOAD_PATH"))
        End Get
    End Property

#End Region

#Region "Display Message "

    Public Sub DisplayMessage(ByVal MessageToDisplay As String)

        lblMsg.Text = String.Empty
        lblMsg.Text = MessageToDisplay

    End Sub

#End Region

#Region "Menuname"

    Private Sub Menuname(ByVal MenuId As Integer)
        Dim eobj As New MenuEn
        Dim bobj As New MenuBAL
        eobj.MenuId = MenuId
        Try
            eobj = bobj.GetMenus(eobj)
        Catch ex As Exception
            lblMsg.Text = ex.Message
            LogError.Log("Payments", "Menuname", ex.Message)
        End Try
        lblMenuName.Text = eobj.MenuName
    End Sub

#End Region

#Region "Page Load "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Create Instances - Start
        Dim _BankProfileEn As New BankProfileEn
        Dim _BankProfileBAL As New BankProfileBAL
        Dim ListBankProfileEn As New List(Of BankProfileEn)
        'Create Instances - Stop

        Try
            'if page is not post back - Start
            If Not Page.IsPostBack Then

                Menuname(CInt(Request.QueryString("Menuid")))
                MenuId.Value = GetMenuId()
                ibtnSave.Attributes.Add("onclick", "return validate()")
                ibtnPosting.Attributes.Add("onClick", "return getpostconfirm()")
                LoadUserRights()
                DisableRecordNavigator()
                ClearControls()

            End If
            'if page is not post back - Stop

            If Not Session("CheckApproverList") Is Nothing Then
                SendToApproval()
            End If

        Catch ex As Exception

            'Log Error
            Call DisplayMessage(ex.Message)
            Call MaxModule.Helper.LogError(ex.Message)

        End Try

    End Sub

#End Region

#Region "SendToApproval"

    Protected Sub SendToApproval()

        Try
            If Not Session("listWF") Is Nothing Then
                Dim list As List(Of WorkflowSetupEn) = Session("listWF")
                If list.Count > 0 Then

                    If New WorkflowDAL().Workflow(Trim(lblBatchCode.Text), Session("User"), Me.ToString()) Then

                        If Session("listWF").count > 0 Then
                            WorkflowApproverList(Trim(lblBatchCode.Text), Session("listWF"))
                        End If

                        Call DisplayMessage("Record Posted Successfully for Approval")
                    Else
                        Call DisplayMessage("Record Already Posted")
                    End If

                Else
                    Throw New Exception("Posting to workflow failed caused NO approver selected.")
                End If

            End If

        Catch ex As Exception
            Call DisplayMessage(ex.Message.ToString())
        Finally
            Session("listWF") = Nothing
            Session("CheckApproverList") = Nothing
        End Try

    End Sub

#End Region

#Region "WorkflowApproverList"

    Protected Sub WorkflowApproverList(ByVal batchcode As String, ByVal lisWF As List(Of WorkflowSetupEn))

        Dim result As Boolean = False

        Try
            For Each wfEn As WorkflowSetupEn In lisWF

                result = New WorkflowDAL().InsertWFApprovalList(batchcode, wfEn.MenuMasterEn.PageName,
                                                                wfEn.UsersEn.UserName, wfEn.UserGroupsEn.UserGroupName)

            Next
        Catch ex As Exception
            Call DisplayMessage(ex.Message.ToString())
            LogError.Log("CimbClicksTrans", "InsertWFApprovalList", ex.Message)
        End Try

    End Sub

#End Region

#Region "Methods"

    ''' <summary>
    ''' Method to Load UserRights
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadUserRights()
        Dim obj As New UsersBAL
        Dim eobj As New UserRightsEn

        Try
            eobj = obj.GetUserRights(CInt(Request.QueryString("Menuid")), CInt(Session("UserGroup")))
        Catch ex As Exception
            LogError.Log("UniversityFund", "LoadUserRights", ex.Message)
        End Try

        'Rights for Add
        If eobj.IsAdd = True Then
            'ibtnSave.Enabled = True
            ibtnNew.ImageUrl = "images/add.png"
            ibtnNew.Enabled = True
        Else
            ibtnNew.ImageUrl = "images/gadd.png"
            ibtnNew.Enabled = False
            ibtnNew.ToolTip = "Access Denied"
        End If

        'Rights for Edit
        If eobj.IsEdit = True Then
            ibtnSave.Enabled = True
            ibtnSave.ImageUrl = "images/save.png"
            ibtnSave.ToolTip = "Edit"
            If eobj.IsAdd = False Then
                ibtnSave.Enabled = False
                ibtnSave.ImageUrl = "images/gsave.png"
                ibtnSave.ToolTip = "Access Denied"
            End If
            Session("EditFlag") = True
        Else
            Session("EditFlag") = False
            ibtnSave.Enabled = False
            ibtnSave.ImageUrl = "images/gsave.png"
        End If

        'Rights for View
        ibtnView.Enabled = eobj.IsView
        If eobj.IsView = True Then
            ibtnView.ImageUrl = "images/find.png"
            ibtnView.Enabled = True
        Else
            ibtnView.ImageUrl = "images/gfind.png"
            ibtnView.ToolTip = "Access Denied"
        End If

        'Rights for Delete
        If eobj.IsDelete = True Then
            ibtnDelete.ImageUrl = "images/delete.png"
            ibtnDelete.Enabled = True
        Else
            ibtnDelete.ImageUrl = "images/gdelete.png"
            ibtnDelete.ToolTip = "Access Denied"
            ibtnDelete.Enabled = False
        End If

        'Rights for Print
        ibtnPrint.Enabled = eobj.IsPrint
        If eobj.IsPrint = True Then
            ibtnPrint.Enabled = True
            ibtnPrint.ImageUrl = "images/print.png"
            ibtnPrint.ToolTip = "Print"
        Else
            ibtnPrint.Enabled = False
            ibtnPrint.ImageUrl = "images/gprint.png"
            ibtnPrint.ToolTip = "Access Denied"
        End If

        'Others
        If eobj.IsOthers = True Then
            ibtnOthers.Enabled = True
            ibtnOthers.ImageUrl = "images/others.png"
            ibtnOthers.ToolTip = "Others"
        Else
            ibtnOthers.Enabled = False
            ibtnOthers.ImageUrl = "images/gothers.png"
            ibtnOthers.ToolTip = "Access Denied"
        End If

        'Rights for Post
        If eobj.IsPost = True Then
            ibtnPosting.Enabled = True
            ibtnPosting.ImageUrl = "images/posting.png"
            ibtnPosting.ToolTip = "Posting"
        Else
            ibtnPosting.Enabled = False
            ibtnPosting.ImageUrl = "images/gposting.png"
            ibtnPosting.ToolTip = "Access Denied"
        End If

    End Sub

    ''' <summary>
    ''' Method to Enable or Disable Navigation Buttons
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisableRecordNavigator()
        Dim flag As Boolean
        If Session("ListObj") Is Nothing Then
            flag = False
            txtRecNo.Text = ""
            lblCount.Text = ""
        Else
            flag = True
        End If
        ibtnFirst.Enabled = flag
        ibtnLast.Enabled = flag
        ibtnPrevs.Enabled = flag
        ibtnNext.Enabled = flag
        If flag = False Then
            ibtnFirst.ImageUrl = "images/gnew_first.png"
            ibtnLast.ImageUrl = "images/gnew_last.png"
            ibtnPrevs.ImageUrl = "images/gnew_Prev.png"
            ibtnNext.ImageUrl = "images/gnew_next.png"
        Else
            ibtnFirst.ImageUrl = "images/new_last.png"
            ibtnLast.ImageUrl = "images/new_first.png"
            ibtnPrevs.ImageUrl = "images/new_Prev.png"
            ibtnNext.ImageUrl = "images/new_next.png"

        End If
    End Sub

#End Region

#Region "File Upload "
    'modified by Hafiz @ 04/6/2016

    Protected Sub File_Upload(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpload.Click

        'Create Instances
        Dim _FileHelper As New FileHelper

        'Variable Declarations - Start
        Dim BankCode As String = Nothing, TotalRecords As Integer = 0, HeaderNo As String = Nothing
        Dim UploadedClicksFile As String = Nothing, TransDate As String = Nothing, TotalAmount As Decimal = 0
        'Variable Declarations - Stop

        Try
            'Get Bank Code
            BankCode = ddlBankCode.SelectedValue

            'Get Uploaded File - Start
            UploadedClicksFile = UploadFile.FileName
            UploadedClicksFile = GetUploadFilePath & Path.GetFileName(UploadedClicksFile)
            'Get Uploaded File - Stop

            'Save File
            UploadFile.SaveAs(UploadedClicksFile)

            'Check file uploaded - Start
            'Modified by Hafiz Roslan @ 11-1-2016
            If _FileHelper.IsClicksFileUploaded(UploadedClicksFile, HeaderNo) Then
                Call DisplayMessage(UploadFile.FileName & " - File Uploaded Previously")
                lblFileUpload.Text = ""
                Exit Sub
            End If
            'Check file uploaded - Stop   

            'if file uploaded Successfully - Start
            If _FileHelper.UploadCimbClicksFile(UploadedClicksFile,
                dgClicksTransactions, TotalAmount, TransDate, TotalRecords, BankCode) Then

                'Show Panel
                pnlDisplay.Visible = True
                'Added By Zoya @8/03/2016
                _summry.Visible = True
                'End Added By Zoya @8/03/2016
                batch_code.Visible = False

                'Display Error Message
                Call DisplayMessage("File Uploaded Successfully")

                'Added By Zoya @7/03/2016
                'Dispaly a Message if the student Name Does not Exist
                Dim StudentName As String

                For Each dgitem In dgClicksTransactions.Items
                    StudentName = dgitem.Cells(0).Text
                    If StudentName = "&nbsp;" Then
                        StudentNameMsg.Text = "Some of The Student Records not Exist"
                    End If
                Next
                'End Added By Zoya @7/03/2016

                'Added by Hafiz Roslan @ 12/01/2016
                'For the File Upload related matters
                UploadFile.Visible = False
                lblFileUpload.Text = UploadFile.FileName

                'Display File Details
                Call TextFileToLabel(UploadedClicksFile, TotalAmount, TotalRecords, HeaderNo)

                hdnTransDate.Value = TransDate

                'Reset Controls
                btnUpload.Visible = False
                ibtnSave.Enabled = True
                ibtnSave.ImageUrl = "images/save.png"
                ibtnSave.ToolTip = "Save"
                ibtnPosting.Enabled = False
                ibtnPosting.ImageUrl = "~/images/gposting.png"

                PnlView.Visible = False
            Else
                'Show Panel
                pnlDisplay.Visible = False

                'Display Error Message
                Call DisplayMessage("File Upload Failed")

                lblFileUpload.Text = UploadFile.FileName

                'Reset Controls
                btnUpload.Visible = False

            End If
            'if file uploaded Successfully - Stop

        Catch ex As Exception

            'Log Error
            Call MaxModule.Helper.LogError(ex.Message)
        Finally

        End Try

    End Sub

#End Region

#Region "TextFileToLabel "

    Private Sub TextFileToLabel(ByVal UploadedClicksFile As String,
        ByVal TotalAmount As Decimal, ByVal TotalRecords As Integer, ByVal HeaderNo As String)

        lblFileName.Text = UploadedClicksFile
        lblTotalStudent.Text = TotalRecords
        lblTotalAmount.Text = clsGeneric.SetCurrencyFormat(TotalAmount)
        'Added by Hafiz Roslan @ 12/01/2016
        hidHeaderNo.Value = HeaderNo
        'Assign header no
    End Sub

#End Region

#Region "On Save "
    'updated by Hafiz @ 19/7/2016

    Protected Sub File_Save(ByVal sender As Object, ByVal e As System.EventArgs) Handles ibtnSave.Click

        'Create Instances
        Dim _FileHelper As New FileHelper

        'Variable Declarations
        Dim TotalRecords As Integer = 0
        Dim UploadedClicksFile As String = Nothing, TotalAmount As Decimal = 0, HeaderNo As String = Nothing
        Dim BatchCode As String = Nothing, TransDate As String = Nothing

        Try

            'Get Values - Start
            TotalAmount = lblTotalAmount.Text
            TotalRecords = lblTotalStudent.Text
            UploadedClicksFile = Path.GetFileName(lblFileName.Text)
            'Get Values - Stop

            'Added by Hafiz Roslan @ 12/01/2016
            'Assign value to headerno
            HeaderNo = hidHeaderNo.Value
            TransDate = hdnTransDate.Value

            'Save Data - Start
            If _FileHelper.InsertClicksTransToAccounts(dgClicksTransactions,
                Session(Helper.UserSession), lblTotalAmount.Text, lblFileName.Text,
                ddlBankCode.SelectedValue, HeaderNo, BatchCode, TransDate) Then

                Call DisplayMessage("Records Saved Successfully")

                batch_code.Visible = True

                lblBatchCode.ForeColor = Drawing.Color.Red
                lblBatchCode.Text = BatchCode
                lblFileUpload.Text = UploadedClicksFile
                StudentNameMsg.Text = String.Empty

                'Track File Details - Start
                Call _FileHelper.TrackClicksFileDetails(UploadedClicksFile,
                    TotalAmount, TotalRecords, ddlBankCode.SelectedValue, HeaderNo, BatchCode)
                'Track File Details - Stop

                ibtnPosting.Enabled = True
                ibtnPosting.ImageUrl = "~/images/posting.png"
                ibtnPosting.ToolTip = "Posting"

            Else
                Call DisplayMessage("Records Failed to Save")

                lblFileUpload.Text = UploadedClicksFile

            End If
            'Save Data - Stop

        Catch ex As Exception

            'Log Error
            Call MaxModule.Helper.LogError(ex.Message)

        End Try

    End Sub

#End Region

    '#Region "On Post "
    '    'modified by Hafiz @ 19/7/2016

    '    Protected Sub File_Post(ByVal sender As Object, ByVal e As System.EventArgs) Handles ibtnPosting.Click

    '        'Create Instances - Start
    '        Dim Workflow As New WorkflowDAL()
    '        Dim _AccountsDAL As New AccountsDAL()
    '        Dim _DataGridItem As DataGridItem = Nothing
    '        'Create Instances - Stop

    '        'Variable declarations
    '        Dim BatchCode As String = Nothing

    '        Try

    '            BatchCode = lblBatchCode.Text

    '            If Not Workflow.Workflow(BatchCode, Session("User"), Me.ToString()) Then
    '                Call DisplayMessage("Posting to workflow failed.")
    '                Exit Sub
    '            Else
    '                Call DisplayMessage("Record Posted Successfully for Approval")
    '            End If

    '            'commented by Hafiz @ 19/7/2016
    '            'Loop thro the Data Grid Items - Start
    '            'For Each _DataGridItem In dgClicksTransactions.Items

    '            '    'get batch Code
    '            '    BatchCode = _DataGridItem.Cells(6).Text

    '            '    'Post To workflow for apporval - Start
    '            '    'If Not Workflow.Workflow(BatchCode, Session(Helper.UserSession), Me.ToString()) Then
    '            '    If Not Workflow.Workflow(BatchCode, Session("User"), Me.ToString()) Then

    '            '        'Update Accounts Details - Start
    '            '        'If Not _AccountsDAL.UpdatePostingStatus(BatchCode, Session(Helper.UserSession)) Then
    '            '        Call DisplayMessage("Posting to workflow failed.")
    '            '        Exit Sub
    '            '        'End If
    '            '        'Update Accounts Details - Stop

    '            '    End If
    '            '    'Post To workflow for apporval - Stop

    '            'Next
    '            ''Loop thro the Data Grid Items - Stop

    '            'Call DisplayMessage("Record Posted Successfully for Approval")

    '        Catch ex As Exception

    '            'Log Error
    '            Call MaxModule.Helper.LogError(ex.Message)

    '        End Try

    '    End Sub

    '#End Region

#Region "On Clear"
    Protected Sub File_Cancel(ByVal sender As Object, ByVal e As System.EventArgs) Handles ibtnCancel.Click
        Call ClearControls()
    End Sub
#End Region

#Region "On New"
    Protected Sub File_New(ByVal sender As Object, ByVal e As System.EventArgs) Handles ibtnNew.Click
        Call ClearControls()
    End Sub
#End Region

#Region "Clear Controls"
    Public Sub ClearControls()
        Call BindBankCode()
        Call BindCimbList()
    End Sub
#End Region

#Region "BindBankCode"
    Public Sub BindBankCode()
        'Create Instances - Start
        Dim _BankProfileEn As New BankProfileEn
        Dim _BankProfileBAL As New BankProfileBAL
        Dim ListBankProfileEn As New List(Of BankProfileEn)
        'Create Instances - Stop

        'Set Values - Start
        _BankProfileEn.Status = True
        _BankProfileEn.ACCode = String.Empty
        _BankProfileEn.GLCode = String.Empty
        _BankProfileEn.Description = String.Empty
        _BankProfileEn.BankDetailsCode = String.Empty
        'Set Values - Stop

        'Get Bank Codes
        ListBankProfileEn = _BankProfileBAL.GetBankProfileList(_BankProfileEn)

        'Populate Drop Down List - Start
        ddlBankCode.Items.Clear()
        ddlBankCode.Items.Add(New ListItem("---Select---", "-1"))
        ddlBankCode.DataTextField = "Description"
        ddlBankCode.DataValueField = "BankDetailsCode"
        ddlBankCode.DataSource = ListBankProfileEn
        ddlBankCode.DataBind()
        'Populate Drop Down List - Stop

        'clear Controls - Start
        dgClicksTransactions.DataSource = Nothing
        dgClicksTransactions.DataBind()
        _summry.Visible = False
        pnlDisplay.Visible = False
        PnlAdd.Visible = True
        batch_code.Visible = False
        lblMsg.Text = ""
        lblFileName.Text = ""
        lblTotalAmount.Text = ""
        lblTotalStudent.Text = ""
        lblFileUpload.Text = ""
        ibtnPosting.Enabled = False
        ibtnPosting.ImageUrl = "~/images/gposting.png"
        UploadFile.Visible = True

        'Added by Zoya @7/03/2016
        StudentNameMsg.Text = ""
        btnUpload.Visible = True
        'End Added by Zoya @7/03/2016

        'clear controls - End
    End Sub
#End Region

#Region "BindCimbList"
    'added by Hafiz @ 09/12/2016

    Public Sub BindCimbList()

        Try
             PnlView.Visible = True

            Dim _listEN As List(Of CIMBclicksEn) = New AccountsDAL().GetCimbClicksList()
            If _listEN.Count > 0 Then
                dgView.DataSource = _listEN.OrderBy(Function(x) x.BatchCode)
                dgView.DataBind()

                For Each dg As DataGridItem In dgView.Items

                    Dim link As HyperLink = DirectCast(dg.FindControl("View"), HyperLink)
                    Dim lb As LinkButton = DirectCast(dg.Cells(0).FindControl("imgBtn1"), LinkButton)

                    link.Attributes.Add("onClick", "OpenWindow('about:blank')")

                    link.Target = "MyPopup"

                    If dg.Cells(5).Text = "Posted" Then
                        link.NavigateUrl = "Receipts.aspx?Menuid=17&IsView=1&IsPosted=1&BatchCode=" + Trim(lb.Text) + ";St&IsCimbClicks=1"
                        lb.ForeColor = Drawing.Color.Red
                        lb.Enabled = False
                    ElseIf dg.Cells(5).Text = "Ready" Then
                        link.NavigateUrl = "Receipts.aspx?Menuid=17&IsView=1&BatchCode=" + Trim(lb.Text) + ";St&IsCimbClicks=1"
                    End If
                Next
            Else
                Throw New Exception("CIMB Details Is Empty. Add New to Proceed.")
            End If

        Catch ex As Exception
            MaxModule.Helper.LogError(ex.Message)
        End Try

    End Sub

#End Region

#Region "dgView_ItemCommand"
    'added by Hafiz @ 10/12/2016

    Protected Sub dgView_ItemCommand(source As Object, e As DataGridCommandEventArgs) Handles dgView.ItemCommand

        Dim dgItem As DataGridItem = DirectCast(DirectCast(e.CommandSource, LinkButton).NamingContainer, DataGridItem)
        Dim lb As LinkButton = TryCast(dgItem.Cells(0).FindControl("imgBtn1"), LinkButton)
        Dim dsInput As DataSet = New AccountsDAL().GetCIMBclicksDetails(lb.Text)

        If dsInput.Tables(0).Rows.Count = 0 Then
            Throw New Exception("No Records")
        Else
            'build datatable for dgClicksTransactions - START
            Dim dt1 As DataTable = New DataTable()
            Dim dc1 As DataColumn = New DataColumn()
            Dim dr1 As DataRow

            'build column
            dc1 = New DataColumn("STUDENT_NAME")
            dt1.Columns.Add(dc1)

            dc1 = New DataColumn("MATRIC_NO")
            dt1.Columns.Add(dc1)

            dc1 = New DataColumn("IC_NO")
            dt1.Columns.Add(dc1)

            dc1 = New DataColumn("RECEIPT_NO")
            dt1.Columns.Add(dc1)

            dc1 = New DataColumn("RECEIPT_DATE")
            dt1.Columns.Add(dc1)

            dc1 = New DataColumn("PAID_AMOUNT")
            dt1.Columns.Add(dc1)

            'push data into row - START
            For Each row As DataRow In dsInput.Tables(0).Rows

                dr1 = dt1.NewRow()
                dr1("STUDENT_NAME") = row("SASI_Name")
                dr1("MATRIC_NO") = row("CreditRef")
                dr1("IC_NO") = row("SASI_IcNo")
                dr1("RECEIPT_NO") = row("SubRef1")
                dr1("RECEIPT_DATE") = Format(row("ReceiptDate"), "dd/MM/yyyy")
                dr1("PAID_AMOUNT") = row("TransAmount")

                dt1.Rows.Add(dr1)

            Next row
            'push data into row - END

            dgClicksTransactions.DataSource = dt1
            dgClicksTransactions.DataBind()
            'build datatable for dgClicksTransactions - END

            TranStatus.Value = Trim(dsInput.Tables(0).Rows(0).Item("poststatus"))

            'populate info into summary - START
            HideSomeItemWhenView()

            lblFileName.Text = dsInput.Tables(0).Rows(0).Item("file_in_name")
            lblTotalAmount.Text = dsInput.Tables(0).Rows(0).Item("file_amount")
            lblTotalStudent.Text = dsInput.Tables(0).Rows(0).Item("file_transactions")
            lblBatchCode.ForeColor = Drawing.Color.Red
            lblBatchCode.Text = dsInput.Tables(0).Rows(0).Item("batchcode")
            'populate info into summary - END

            CheckWorkflowStatus(Trim(dsInput.Tables(0).Rows(0).Item("poststatus")), Trim(dsInput.Tables(0).Rows(0).Item("batchcode")))

        End If

    End Sub

#End Region

#Region "HideSomeItemWhenView"

    Protected Sub HideSomeItemWhenView()
        ibtnSave.Enabled = False
        ibtnSave.ImageUrl = "images/gsave.png"
        ibtnSave.ToolTip = "Access Denied"
        ibtnPosting.Enabled = True
        ibtnPosting.ImageUrl = "images/posting.png"
        ibtnPosting.ToolTip = "Posting"
        batch_code.Visible = True

        PnlAdd.Visible = False
        pnlDisplay.Visible = True
        _summry.Visible = True
        GridPanel.Visible = True
        PnlView.Visible = False
    End Sub

#End Region

#Region "GetApprovalDetails"

    Protected Function GetMenuId() As Integer

        Dim MenuId As Integer = New MenuDAL().GetMenuMasterList().Where(Function(x) x.PageName = "CIMB Clicks File Upload").Select(Function(y) y.MenuID).FirstOrDefault()
        Return MenuId

    End Function

#End Region

#Region "For popup AddApprover() close"

    Protected Sub btnHiddenApp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnHiddenApp.Click

    End Sub

#End Region

#Region "CheckWorkflowStatus"

    Protected Sub CheckWorkflowStatus(ByVal poststatus As String, ByVal BatchCode As String)

        If poststatus = "Ready" Then

            Dim _list As List(Of WorkflowEn) = New WorkflowDAL().GetList().Where(Function(x) x.BatchCode = BatchCode).ToList()
            If _list.Count > 0 Then

                If _list.Where(Function(x) x.WorkflowStatus = 1).Count > 0 Then
                    Call DisplayMessage("Record Pending For Approval.")
                ElseIf _list.Where(Function(x) x.WorkflowStatus = 3).Count > 0 Then
                    Call DisplayMessage("Record Rejected By Approval. [Reason:" & _list.Where(Function(x) x.WorkflowStatus = 3).Select(Function(y) y.WorkflowRemarks).FirstOrDefault() & "]")
                Else
                    Call DisplayMessage("")
                End If

            End If

        End If

    End Sub

#End Region
End Class
