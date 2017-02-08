Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.Entities
Imports System.Data
Imports System.Collections.Generic
Partial Class RptRefundAdvance
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack() Then
            
            ibtnFDate.Attributes.Add("onClick", "return getibtnFDate()")
            ibtnTodate.Attributes.Add("onClick", "return getDateto()")
           

            ibtnPrint.Attributes.Add("onclick", "return getDate()")

            'ibtnView.Attributes.Add("onclick", "return getDate()")
            txtFrom.Attributes.Add("OnKeyup", "return CheckFromDate()")
            txtTodate.Attributes.Add("OnKeyup", "return CheckToDate()")
            txtFrom.ReadOnly = True
            txtTodate.ReadOnly = True
            ibtnFDate.Visible = False
            ibtnTodate.Visible = False
            dates()
        End If
        Session("sortby") = Nothing
    End Sub
    Private Sub dates()
        txtFrom.Text = Format(Date.Now, "dd/MM/yyyy")
        txtTodate.Text = Format(Date.Now, "dd/MM/yyyy")
    End Sub
    Protected Sub ChkDateRange_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        If ChkDateRange.Checked = True Then
            txtFrom.ReadOnly = False
            txtTodate.ReadOnly = False
            ibtnFDate.Visible = True
            ibtnTodate.Visible = True
        Else
            txtFrom.ReadOnly = True
            txtTodate.ReadOnly = True
            ibtnFDate.Visible = False
            ibtnTodate.Visible = False
            dates()
        End If
    End Sub
    Protected Sub ddltype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddltype.SelectedIndexChanged

        Dim report As String
        'Dim type As String
        report = ddltype.SelectedValue

        If report = "-1" Then
            Response.Redirect("/GroupReport/RptRefundAdvance.aspx")
            Session("report") = Nothing
        ElseIf report = "0" Then
            Session("report") = report
        ElseIf report = "1" Then
            Session("report") = report
        End If
    End Sub
    Protected Sub rdbSelect_checkedchanged(sender As Object, e As EventArgs)
        Dim radiodate As String
        If rdbSelect.Checked = True Then
            rdbAll.Checked = False
            rdbSelect.Checked = True
            'rdbAll.Enabled = False
            'radiodate = rdbSelect.Checked
            'Session("radiodate") = radiodate
        ElseIf rdbSelect.Checked = False Then
            'RadioButton2.Checked = False
            rdbAll.Checked = True
            rdbSelect.Checked = False
            'Session("radiodate") = Nothing
        End If
    End Sub
    Protected Sub rdbAll_checkedchanged(sender As Object, e As EventArgs)
        Dim radioyear As String
        If rdbAll.Checked = True Then
            rdbAll.Checked = True
            rdbSelect.Checked = False
            'rdbAll.Enabled = False
            'radiodate = rdbSelect.Checked
            'Session("radiodate") = radiodate
        ElseIf rdbAll.Checked = False Then
            'RadioButton2.Checked = False
            rdbAll.Checked = False
            rdbSelect.Checked = True
            'Session("radiodate") = Nothing
        End If
    End Sub
    Protected Sub ibtnCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnCancel.Click
        Response.Redirect("/GroupReport/RptRefundAdvance.aspx")
    End Sub
End Class
