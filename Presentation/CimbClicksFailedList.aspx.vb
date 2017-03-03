Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.Entities
Imports System.Data
Imports System.Collections.Generic
Partial Class CimbClicksFailedList
    Inherits System.Web.UI.Page

    Dim ListFailed As New List(Of FileHelper.CimbClicksEntity)

    Public Sub LoadGrid()
        Try
            ListFailed = Session("ListNotFoundStud")
        Catch ex As Exception
            LogError.Log("CimbClicksFailedList", "LoadGrid", ex.Message)
        End Try

        If Not ListFailed Is Nothing Then
            gvClicksFailed.DataSource = ListFailed
            gvClicksFailed.DataBind()
        Else
            Response.Write("No CIMB Failed Student List are found")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack() Then
            LoadGrid()
        End If
    End Sub

    Protected Sub gvClicksFailed_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvClicksFailed.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then

            If e.Row.Cells(0).Text = "&nbsp;" Then
                e.Row.Cells(0).Text = ""
            End If

        End If
    End Sub

    Private Sub UpdateGrid()
        Dim cScript As String = ""

        Session("ListNotFoundStud") = Nothing
        Session("NotFoundFlag") = False

        cScript = "<SCRIPT LANGUAGE=""JavaScript"">window.opener.document.getElementById('ctl00_ContentPlaceHolder1_btnHiddenApp').click();self.close();</SCRIPT>"
        ClientScript.RegisterStartupScript(Me.GetType(), "ClosePopUp", cScript)
    End Sub

    Protected Sub ibtnClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnClose.Click
        UpdateGrid()
    End Sub
End Class
