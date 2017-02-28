Option Strict On
Option Explicit On

Public Class CfCbPayMyCbDetails

    Private _cbph_company As String
    Public Property cbph_company() As String
        Get
            Return _cbph_company
        End Get
        Set(ByVal value As String)
            _cbph_company = value
        End Set
    End Property

    Private _cbph_batchid As String
    Public Property cbph_batchid() As String
        Get
            Return _cbph_batchid
        End Get
        Set(ByVal value As String)
            _cbph_batchid = value
        End Set
    End Property

    Private _cbph_lineno As Integer
    Public Property cbph_lineno() As Integer
        Get
            Return _cbph_lineno
        End Get
        Set(ByVal value As Integer)
            _cbph_lineno = value
        End Set
    End Property

    Private _cbph_icno As String
    Public Property cbph_icno() As String
        Get
            Return _cbph_icno
        End Get
        Set(ByVal value As String)
            _cbph_icno = value
        End Set
    End Property

    Private _cbph_rocno As String = Helper.NullValue
    Public ReadOnly Property cbph_rocno() As String
        Get
            Return _cbph_rocno
        End Get
    End Property

    Private _cbph_acctno As String
    Public Property cbph_acctno() As String
        Get
            Return _cbph_acctno
        End Get
        Set(ByVal value As String)
            _cbph_acctno = value
        End Set
    End Property

    Private _cbph_recvbank As String
    Public Property cbph_recvbank() As String
        Get
            Return _cbph_recvbank
        End Get
        Set(ByVal value As String)
            _cbph_recvbank = value
        End Set
    End Property

    Private _cbph_email As String
    Public Property cbph_email() As String
        Get
            Return _cbph_email
        End Get
        Set(ByVal value As String)
            _cbph_email = value
        End Set
    End Property

    Private _cbph_mobile As String
    Public Property cbph_mobile() As String
        Get
            Return _cbph_mobile
        End Get
        Set(ByVal value As String)
            _cbph_mobile = value
        End Set
    End Property

    Private _cbph_prodcode As String = Helper.NullValue
    Public ReadOnly Property cbph_prodcode() As String
        Get
            Return _cbph_prodcode
        End Get
    End Property

    Private _cbph_telno As String = Helper.NullValue
    Public ReadOnly Property cbph_telno() As String
        Get
            Return _cbph_telno
        End Get
    End Property

    Private _cbph_comment As String
    Public Property cbph_comment() As String
        Get
            Return _cbph_comment
        End Get
        Set(value As String)
            _cbph_comment = value
        End Set
    End Property


End Class