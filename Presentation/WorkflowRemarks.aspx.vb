Imports HTS.SAS.Entities
Imports HTS.SAS.BusinessObjects
Imports System.Data
Imports System.Collections.Generic
Imports HTS.SAS.DataAccessObjects

Partial Class WorkflowRemarks
    Inherits System.Web.UI.Page
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Dim str As String = Request.QueryString("BatchCode")
            Dim bsobj As New WorkflowDAL
            txtBatchCode.Text = str
            txtRemarks.Text = bsobj.GetWorkflowRemarks(str)
            If txtRemarks.Text = "" Then
                txtRemarks.Text = "No Remarks Available"
            End If
        End If
    End Sub
End Class
