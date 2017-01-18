Option Strict On
Option Explicit On

Public Class CfGstDistEn

    Private _utgd_company As String
    Public Property utgd_company() As String
        Get
            Return _utgd_company
        End Get
        Set(ByVal value As String)
            _utgd_company = value
        End Set
    End Property

    Private _utgd_module As String
    Public Property utgd_module() As String
        Get
            Return _utgd_module
        End Get
        Set(ByVal value As String)
            _utgd_module = value
        End Set
    End Property

    Private _utgd_itemtype As String
    Public Property utgd_itemtype() As String
        Get
            Return _utgd_itemtype
        End Get
        Set(ByVal value As String)
            _utgd_itemtype = value
        End Set
    End Property

    Private _utgd_batchid As String
    Public Property utgd_batchid() As String
        Get
            Return _utgd_batchid
        End Get
        Set(ByVal value As String)
            _utgd_batchid = value
        End Set
    End Property

    Private _utgd_code As String
    Public Property utgd_code() As String
        Get
            Return _utgd_code
        End Get
        Set(ByVal value As String)
            _utgd_code = value
        End Set
    End Property

    Private _utgd_itemref As String
    Public Property utgd_itemref() As String
        Get
            Return _utgd_itemref
        End Get
        Set(ByVal value As String)
            _utgd_itemref = value
        End Set
    End Property

    'Private _utgd_itemrefline As Integer = 0
    'Public Property utgd_itemrefline() As Integer
    '    Get
    '        Return _utgd_itemrefline
    '    End Get
    '    Set(ByVal value As Integer)
    '        _utgd_itemrefline = value
    '    End Set
    'End Property

    Private _utgd_itemrefline As Integer = 1
    Public ReadOnly Property utgd_itemrefline() As Integer
        Get
            Return _utgd_itemrefline
        End Get
    End Property

    Private _utgd_seqno As Integer = 0
    Public Property utgd_seqno() As Integer
        Get
            Return _utgd_seqno
        End Get
        Set(ByVal value As Integer)
            _utgd_seqno = value
        End Set
    End Property

    Private _utgd_desc As String
    Public Property utgd_desc() As String
        Get
            Return _utgd_desc
        End Get
        Set(ByVal value As String)
            _utgd_desc = value
        End Set
    End Property

    Private _utgd_qty As Double = 0.0
    Public Property utgd_qty() As Double
        Get
            Return _utgd_qty
        End Get
        Set(ByVal value As Double)
            _utgd_qty = value
        End Set
    End Property

    Private _utgd_taxtype As String
    Public Property utgd_taxtype() As String
        Get
            Return _utgd_taxtype
        End Get
        Set(ByVal value As String)
            _utgd_taxtype = value
        End Set
    End Property

    Private _utgd_taxcode As String
    Public Property utgd_taxcode() As String
        Get
            Return _utgd_taxcode
        End Get
        Set(ByVal value As String)
            _utgd_taxcode = value
        End Set
    End Property

    Private _utgd_amount As Double = 0.0
    Public Property utgd_amount() As Double
        Get
            Return _utgd_amount
        End Get
        Set(ByVal value As Double)
            _utgd_amount = value
        End Set
    End Property

    Private _utgd_gstamount As Double = 0.0
    Public Property utgd_gstamount() As Double
        Get
            Return _utgd_gstamount
        End Get
        Set(ByVal value As Double)
            _utgd_gstamount = value
        End Set
    End Property

    Private _utgd_batchdate As String
    Public Property utgd_batchdate() As String
        Get
            Return _utgd_batchdate
        End Get
        Set(ByVal value As String)
            _utgd_batchdate = value
        End Set
    End Property

    Private _utgd_invopenamt As Double = 0.0
    Public Property utgd_invopenamt() As Double
        Get
            Return _utgd_invopenamt
        End Get
        Set(ByVal value As Double)
            _utgd_invopenamt = value
        End Set
    End Property

    Private _utgd_curcode As String
    Public Property utgd_curcode() As String
        Get
            Return _utgd_curcode
        End Get
        Set(ByVal value As String)
            _utgd_curcode = value
        End Set
    End Property

    Private _utgd_leqfor As String
    Public Property utgd_leqfor() As String
        Get
            Return _utgd_leqfor
        End Get
        Set(ByVal value As String)
            _utgd_leqfor = value
        End Set
    End Property

    Private _utgd_currate As Double = 0.0
    Public Property utgd_currate() As Double
        Get
            Return _utgd_currate
        End Get
        Set(ByVal value As Double)
            _utgd_currate = value
        End Set
    End Property

    Private _utgd_lclamount As Double = 0.0
    Public Property utgd_lclamount() As Double
        Get
            Return _utgd_lclamount
        End Get
        Set(ByVal value As Double)
            _utgd_lclamount = value
        End Set
    End Property

    Private _utgd_invpaid As String
    Public Property utgd_invpaid() As String
        Get
            Return _utgd_invpaid
        End Get
        Set(ByVal value As String)
            _utgd_invpaid = value
        End Set
    End Property

    Private _utgd_gstpaid As String
    Public Property utgd_gstpaid() As String
        Get
            Return _utgd_gstpaid
        End Get
        Set(ByVal value As String)
            _utgd_gstpaid = value
        End Set
    End Property

    Private _utgd_gstclaim As String
    Public Property utgd_gstclaim() As String
        Get
            Return _utgd_gstclaim
        End Get
        Set(ByVal value As String)
            _utgd_gstclaim = value
        End Set
    End Property

    Private _utgd_posted As String
    Public Property utgd_posted() As String
        Get
            Return _utgd_posted
        End Get
        Set(ByVal value As String)
            _utgd_posted = value
        End Set
    End Property

End Class