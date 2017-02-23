Imports HTS.SAS.Entities
Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.DataAccessObjects
Imports System.Data
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Linq

'added by Hafiz @ 21/02/2017

Partial Class GLFailedList
    Inherits System.Web.UI.Page

    Dim MenuId As Integer, Batchcode As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        MenuId = Request.QueryString("MenuId")

        If Not IsPostBack() Then
            Session("LoadGrid") = Nothing

            LoadTextBox()
            LoadGLFailedList()
            LoadGrid()
        End If

    End Sub

    Protected Sub LoadTextBox()
        Dim _MenuMasterEn As MenuMasterEn = New MenuDAL().GetMenuMasterList().FindAll(Function(x) x.MenuID = MenuId).FirstOrDefault()

        txtProcessName.Text = _MenuMasterEn.PageName
        txtTFailedTime.Text = DateTime.Now

    End Sub

    Protected Sub LoadGLFailedList()

        Try
            Batchcode = Request.QueryString("Batchcode")
            If String.IsNullOrEmpty(Batchcode) Then
                Throw New Exception("Batch number not found or empty.")
            End If

            If Not Session("List_Failed") Is Nothing Then
                Dim FailedList As List(Of WorkflowEn) = Session("List_Failed")
                Session("LoadGrid") = FailedList.GroupBy(Function(x) New With {Key x.ID, x.NAME, x.SOURCE}) _
                .Select(Function(y) New With {.ID = y.Key.ID, .NAME = y.Key.NAME, .SOURCE = y.Key.SOURCE}).OrderBy(Function(z) z.ID)
            Else
                Throw New Exception("No GL`s Failed List")
            End If

        Catch ex As Exception
            lblMsg.Visible = True
            lblMsg.Text = ex.Message.ToString()
        End Try

    End Sub

    Protected Sub LoadGrid()

        If Not Session("LoadGrid") Is Nothing Then
            gvUser.DataSource = Session("LoadGrid")
        Else
            gvUser.DataSource = Nothing
        End If
        gvUser.DataBind()

    End Sub

    Private Sub UpdateGrid()
        Dim cScript As String = ""

        Session("List_Failed") = Nothing

        cScript = "<SCRIPT LANGUAGE=""JavaScript"">window.opener.document.getElementById('ctl00_ContentPlaceHolder1_btnHiddenApp').click();self.close();</SCRIPT>"
        ClientScript.RegisterStartupScript(Me.GetType(), "ClosePopUp", cScript)
    End Sub

    Protected Sub ibtnClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnClose.Click
        UpdateGrid()
    End Sub

End Class
