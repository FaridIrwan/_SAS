﻿#Region "NameSpaces "

Imports MaxGeneric
Imports Microsoft.VisualBasic
Imports System.IO
Imports System.ComponentModel
Imports System.Data
Imports System.Collections.Generic

#End Region

Public Class Helper
#Region "Create Instances "
    'Create Instances - Start
    Private _WorkflowDAL As New HTS.SAS.DataAccessObjects.WorkflowDAL
    Private _AccountsDAL As New HTS.SAS.DataAccessObjects.AccountsDAL
    'Create Instances - Ended
#End Region

#Region "Generic Constants "

    Public Const RecordActive As String = "1"
    Public Const SessionTimeOut As Short = 20

#End Region

#Region "Query String Constants "

    Public Const QueryTaxId As String = "TaxId"

#End Region

#Region "Session Contants "

    Public Const UserSession As String = "User"
    Public Const MenuSession As String = "Menu"
    Public Const AliveSession As String = "Alive"
    Public Const UserGroupSession As String = "UserGroup"
    Public Const BranchCodeSession As String = "BranchCode"
    Public Const TransIDSession As String = "TranID"
    Public Const WorkflowSession As String = "WorkflowGroup"
    Public Const WorkflowRole As String = "WorkflowRole"
    Public Const UserIDSession As String = "UserID"

#End Region

#Region "Menu Constants "

    Public Const HomeMenu As String = "Home"
    Public Const SetUpMenu As String = "Setup"
    Public Const ProcessMenu As String = "Process"
    Public Const ReportsMenu As String = "Reports"
    Public Const DashboardMenu As String = "Dashboard"

#End Region

#Region "Menu Columns "

    Public Const MenuIdCol As String = "MenuName"
    Public Const PageUrlCol As String = "PageURL"
    Public Const ImageUrlCol As String = "ImageURL"
    Public Const MenuNameCol As String = "MenuId"

#End Region

#Region "Page Constants "

    Public Const SetupPage As String = "Setup.aspx"
    Public Const ProcessPage As String = "Process.aspx"
    Public Const ReportsPage As String = "Reports.aspx"
    Public Const LoginPage As String = "~/login.aspx"
    Public Const DashboardPage As String = "DashboardHome.aspx"
    Public Const HomePage As String = "Home.aspx"

#End Region

#Region "Image Constances "

    Public Const ReportsImg As String = "images/logimages/report.png"
    Public Const ProcessImg As String = "images/logimages/setting.png"
    Public Const SettingsImg As String = "images/logimages/settings.png"
    Public Const DashboardImg As String = "images/logimages/dashboard.png"
    Public Const HomeImg As String = "images/logimages/home.png"

#End Region

#Region "Decimal Format "
    Public Function DecimalFormat(ByVal Amount As Decimal) As String

        Return String.Format("{0:F}", Amount)

    End Function
#End Region

#Region "Dunning Letter General Properties "
    Public Function GetDunningLetterPath() As String

        Return clsGeneric.NullToString(ConfigurationManager.AppSettings("DUNNING_LETTER_PATH"))

    End Function
#End Region

#Region "Post to Workflow "

    'Purpose			: Post To Workflow
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 04/06/2015
    'Public Function PostToWorkflow(ByVal BatchCode As String,
    '    ByVal DoneBy As String, ByVal PageName As String) As Boolean
    Public Function PostToWorkflow(ByVal BatchCode As String,
        ByVal DoneBy As String, ByVal PageName As String, Optional type As String = "") As Boolean

        Try

            'Post To workflow for apporval - Start
            If _WorkflowDAL.Workflow(BatchCode, DoneBy, PageName) Then

                'If type = "StudentLoan" Then
                '    If _AccountsDAL.UpdatePostingStatusStudentLoan(BatchCode, DoneBy) Then
                '        Return True
                '    End If

                'Else
                '    If _AccountsDAL.UpdatePostingStatus(BatchCode, DoneBy) Then
                '        Return True
                '    End If

                'End If

                Return True

            End If
            'Post To workflow for apporval - Stop

            Return False

        Catch ex As Exception

            Call MaxModule.Helper.LogError(ex.Message)
            Return False

        End Try

    End Function
    '  PostToWorkflow("", "", "")
#End Region

#Region "Dunning Letter General Properties "
    Public Function SendDunningLetterPath() As String

        Return clsGeneric.NullToString(ConfigurationManager.AppSettings("SEND_DUNNING_LETTER"))

    End Function
#End Region

#Region "Sponsor Cover Letter General Properties "
    Public Function GetSponsorCoverLetterPath() As String

        Return clsGeneric.NullToString(ConfigurationManager.AppSettings("SPONSOR_COVER_LETTER_PATH"))

    End Function

    Public Function GetSponsorCoverLetterFolder() As String

        Return clsGeneric.NullToString(ConfigurationManager.AppSettings("SEND_SPONSOR_COVER_LETTER"))

    End Function

#End Region

#Region "ToDataTable"
    'added by Hafiz @ 15/12/2016
    'convert to datatable

    Public Function ToDataTable(Of T)(data As IList(Of T)) As DataTable

        Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(GetType(T))
        Dim table As New DataTable()

        For i As Integer = 0 To props.Count - 1
            Dim prop As PropertyDescriptor = props(i)
            table.Columns.Add(prop.Name, prop.PropertyType)
        Next

        Dim values As Object() = New Object(props.Count - 1) {}
        For Each item As T In data
            For i As Integer = 0 To values.Length - 1
                values(i) = props(i).GetValue(item)
            Next
            table.Rows.Add(values)
        Next

        Return table

    End Function

#End Region

End Class
