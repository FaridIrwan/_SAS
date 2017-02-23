Option Strict On
Option Explicit On

Public Class CfCbPayFlowEn

    Private _mypf_company As String
    Public Property mypf_company() As String
        Get
            Return _mypf_company
        End Get
        Set(ByVal value As String)
            _mypf_company = value
        End Set
    End Property

    Private _mypf_batchid As String
    Public Property mypf_batchid() As String
        Get
            Return _mypf_batchid
        End Get
        Set(ByVal value As String)
            _mypf_batchid = value
        End Set
    End Property

End Class
