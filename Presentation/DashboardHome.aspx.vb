#Region "NameSpaces "

Imports MaxGeneric
Imports System.Data
Imports System.Collections.Generic
Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.DataAccessObjects
Imports HTS.SAS.Entities
Imports System.Web
Imports System.Linq
Imports System.Globalization

#End Region
Partial Class Default4
    Inherits System.Web.UI.Page
#Region "Global Declarations "
    Private _WorkflowDAL As New HTS.SAS.DataAccessObjects.WorkflowDAL
    Private _AccountsDAL As New HTS.SAS.DataAccessObjects.AccountsDAL
    Private _AFCDAL As New HTS.SAS.DataAccessObjects.AFCDAL
    Private _MaxModule As New MaxModule.Workflow
#End Region
#Region "Page_Load "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            If Not Page.IsPostBack Then

                hidUserRole.Value = Session(Helper.WorkflowRole)
                hidUserId.Value = Session(Helper.UserIDSession)
                'Call Bind Grid
                Call BindGridd(hidUserRole.Value, hidUserId.Value)
                Call BindGrid(hidUserId.Value, hidUserRole.Value)
                Session("PageMode") = ""
                Session("ListObj") = Nothing
                'DisableRecordNavigator()
                OnView()
                'Menuname(CInt(Request.QueryString("Menuid")))

                'lblRemarks.Visible = False
                'txtRemarks.Visible = False
                If hidUserRole.Value = "R" Then
                    Session("Reviewer") = hidUserRole.Value
                ElseIf hidUserRole.Value = "A" Then
                    Session("Approver") = hidUserRole.Value
                End If

            End If

        Catch ex As Exception

            Call MaxModule.Helper.LogError(ex.Message)

        End Try

    End Sub

#End Region
    Private Sub BindGridd(ByVal WorkflowRole As String, ByVal UserId As String)
        Dim WorkFlowAccountDetails As DataSet = Nothing
        Dim Reviewer As Integer = 0
        Dim Approver As Integer = 0
        Dim listWF As New List(Of WorkflowEn)
        Dim listWFA As New List(Of WorkflowEn)
        Dim listWFA2 As New List(Of WorkflowEn)
        Dim batchcode As String = Nothing
        Try

            If UserId <> 0 Then

                'If WorkflowRole = "P" Then
                listWF = _WorkflowDAL.GetPendingWorkflowDetailsPreparer(UserId)
                'End If
                listWFA = _WorkflowDAL.GetPostedWorkflowDetails(UserId)
            End If

            'ElseIf WorkflowRole = "R" Then


            'End If


            If listWF.Count > 0 Then
                'Bind Data grid - Start
                dgPending.DataSource = listWF
                dgPending.DataBind()
                'Bind Data grid - Stop
            End If
            'if data available - Start
            ' Dim datee As String = "20/12/2016"
            ' Dim expenddt As Date = Date.ParseExact(datee, "dd/MM/yyyy",
            'CultureInfo.InvariantCulture)
            If listWFA.Count > 0 Then
                'Bind Data grid - Start
                For Each item In listWFA
                    If Not listWFA.Any(Function(x) item.AccountsEn.PostedDateTime.Date = Date.Today) Then

                    Else
                        listWFA2.Add(item)
                    End If
                Next
                dgDone.DataSource = listWFA2
                dgDone.DataBind()
                'Bind Data grid - Stop
            End If




        Catch ex As Exception

            MaxModule.Helper.LogError(ex.Message)

        End Try

    End Sub
#Region "Bind Grid "

    Private Sub BindGrid(ByVal Id As String, ByVal WorkflowRole As String)

        Dim lstHaveApprover As New List(Of WorkflowEn)
        Dim UserId As Integer = Convert.ToInt32(Id)
        'If WorkflowRole = "A" Then


        Try

            Dim Approver As Integer = 0
            Session("Approver") = Approver

            Dim listWFSetup As List(Of WorkflowSetupEn) = New WorkflowSetupDAL().GetApproverList().Where(Function(x) x.WorkflowApproverEn.UserId = CInt(Session(Helper.UserIDSession))).ToList()
            'Dim userid As Integer = listWFSetup.Select(Function(x) x.WorkflowApproverEn.UserId).FirstOrDefault()

            If UserId <> 0 Then
                Dim listWF As List(Of WorkflowEn) = _WorkflowDAL.GetWorkflowDetails(UserId)
                If listWF.Count > 0 Then

                    For Each obj As WorkflowEn In listWF

                        Dim _list As List(Of WorkflowSetupEn) = listWFSetup.Where(Function(x) x.WorkflowApproverEn.MenuId = obj.MenuMasterEn.MenuID).ToList()

                        If _list.Any(Function(x) x.WorkflowApproverEn.LowerLimit = 0 AndAlso x.WorkflowApproverEn.UpperLimit = 0) Then
                            lstHaveApprover.Add(obj)
                        Else

                            If _list.Any(Function(x) x.WorkflowApproverEn.LowerLimit = 0 AndAlso x.WorkflowApproverEn.UpperLimit <> 0) Then

                                If _list.Any(Function(x) obj.AccountsEn.TransactionAmount <= x.WorkflowApproverEn.UpperLimit) Then
                                    lstHaveApprover.Add(obj)
                                End If

                            ElseIf _list.Any(Function(x) x.WorkflowApproverEn.LowerLimit <> 0 AndAlso x.WorkflowApproverEn.UpperLimit = 0) Then

                                If _list.Any(Function(x) obj.AccountsEn.TransactionAmount >= x.WorkflowApproverEn.LowerLimit) Then
                                    lstHaveApprover.Add(obj)
                                End If

                            Else

                                If _list.Any(Function(x) obj.AccountsEn.TransactionAmount >= x.WorkflowApproverEn.LowerLimit AndAlso
                                                     obj.AccountsEn.TransactionAmount <= x.WorkflowApproverEn.UpperLimit) Then
                                    lstHaveApprover.Add(obj)
                                End If

                            End If

                        End If

                    Next
                End If
            End If

            If lstHaveApprover.Count > 0 Then
                'Bind Data grid - Start
                dgPending.DataSource = lstHaveApprover
                dgPending.DataBind()
                'Bind Data grid - Stop
            End If

        Catch ex As Exception

            MaxModule.Helper.LogError(ex.Message)

        End Try
        'End If

    End Sub

#End Region
    Protected Sub OnView()
        Dim link As HyperLink
        Dim str As String

        For Each dgItem1 As GridViewRow In dgDone.Rows

            link = dgItem1.Cells(5).Controls(1)
            str = dgItem1.Cells(0).Text
            link.Attributes.Add("onClick", "OpenWindow('about:blank')")


            link.NavigateUrl = "WorkflowRemarks.aspx?BatchCode=" + str + ""
            link.Target = "MyPopup"

        Next

    End Sub
End Class
