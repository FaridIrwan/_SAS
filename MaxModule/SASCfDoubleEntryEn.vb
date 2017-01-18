Option Strict On
Option Explicit On

Public Class SASCfDoubleEntryEn

    Private _company_code As String
    Public Property company_code() As String
        Get
            Return _company_code
        End Get
        Set(ByVal value As String)
            _company_code = value
        End Set
    End Property

    Private _cf_batch_no As String
    Public Property cf_batch_no() As String
        Get
            Return _cf_batch_no
        End Get
        Set(ByVal value As String)
            _cf_batch_no = value
        End Set
    End Property

    Private _gl_account As String
    Public Property gl_account() As String
        Get
            Return _gl_account
        End Get
        Set(ByVal value As String)
            _gl_account = value
        End Set
    End Property

    Private _gl_description As String
    Public Property gl_description() As String
        Get
            Return _gl_description
        End Get
        Set(ByVal value As String)
            _gl_description = value
        End Set
    End Property

    Private _debit_amount As Decimal
    Public Property debit_amount() As Decimal
        Get
            Return _debit_amount
        End Get
        Set(ByVal value As Decimal)
            _debit_amount = value
        End Set
    End Property

    Private _credit_amount As Decimal
    Public Property credit_amount() As Decimal
        Get
            Return _credit_amount
        End Get
        Set(ByVal value As Decimal)
            _credit_amount = value
        End Set
    End Property

    Private _reference_no As String
    Public Property reference_no() As String
        Get
            Return _reference_no
        End Get
        Set(ByVal value As String)
            _reference_no = value
        End Set
    End Property

    Private _posting_type As String
    Public Property posting_type() As String
        Get
            Return _posting_type
        End Get
        Set(ByVal value As String)
            _posting_type = value
        End Set
    End Property

    Private _seq_no As Integer
    Public Property seq_no() As Integer
        Get
            Return _seq_no
        End Get
        Set(ByVal value As Integer)
            _seq_no = value
        End Set
    End Property

    Private _matric_no As String
    Public Property matric_no() As String
        Get
            Return _matric_no
        End Get
        Set(ByVal value As String)
            _matric_no = value
        End Set
    End Property

End Class
