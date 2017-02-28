#Region "Name Spaces "

Imports MaxGeneric
Imports System.Reflection
Imports System.Data.Common

#End Region

Public Class CfPosting

#Region "Global Declarations "

    'Create Instances - Start
    Private _CfCommon As New CfCommon()
    Private _CfGeneric As New CfGeneric()
    Private _DataBaseProvider As New DatabaseProvider()
    'Create Instances - Stop

#End Region

#Region "Execute Sql Statement "

    'Purpose			: To Execute the Sql Statement(s)
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 31/05/2015
    Private Function ExecuteSqlStatement(ByVal SqlStatement As String) As Boolean

        Try

            If _DataBaseProvider.ExecuteSqlStatement(Helper.
                GetFinancialsDataBaseType, Helper.FinancialsConnectionString,
                SqlStatement) > -1 Then
                Return True
            End If

            Return False

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

    'added by Hafiz @ 29/11/2016
    'connect through SAS DB
    Private Function ExecuteSql(ByVal SqlStatement As String) As Boolean
        Try

            If _DataBaseProvider.ExecuteSqlStatement(Helper.GetDataBaseType, Helper.NpgSqlConnectionString, SqlStatement) > -1 Then
                Return True
            End If

            Return False

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try
    End Function

#End Region

#Region "Get Data Reader "

    'Purpose			: To Get Sql Statement as Data Reader
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 31/05/2015
    Private Function GetDataReader(ByVal SqlStatement As String) As IDataReader

        Try

            Return _DataBaseProvider.ExecuteReader(Helper.GetDataBaseType,
                Helper.GetConnectionString, SqlStatement).CreateDataReader()

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return Nothing

        End Try

    End Function

#End Region

#Region "Cash Book Payment "

    'Purpose			: To Post Cash Book Payment
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 29/05/2015
    Public Function CashBookPayment(ByVal BatchCode As String, ByRef CompanyCode As String, ByRef CashBookNo As String) As Boolean

        'Variable Declarations - Start
        'Dim CashBookNo As String = Nothing
        Dim CfCbPayBatchSql As String = Nothing
        'Variable Declarations - Stop

        Try
            'Get Company Code
            CompanyCode = _CfCommon.GetCompanyCode()

            'Get Auto Number
            CashBookNo = _CfCommon.GetAutoNumber(CfGeneric.PostingType.CashBookPayment)

            'Post Cash Book Payment Batch - Start
            If Not CashBookPaymentBatch(BatchCode, CompanyCode, CashBookNo) Then
                'Roll Back Transactions
                Call RollBackCashBookPayment(BatchCode, CashBookNo)

                Return False
            Else
                'Increment Auto No
                Call _CfCommon.UpdateAutoNumber(CfGeneric.PostingType.CashBookPayment)

                Return True
            End If
            'Post Cash Book Payment Batch - Stop

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

#End Region

#Region "Cash Book Payment Batch "

    'Purpose			: To Post Cash Book Payment Batch
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 29/05/2015
    'Modified by Hafiz @ 30/11/2016

    Private Function CashBookPaymentBatch(ByVal BatchCode As String, ByVal CompanyCode As String, ByVal CashBookNo As String) As Boolean

        Dim _CfCbPayBatchEn As New CfCbPayBatchEn(), SqlStatement As String = Nothing, CfCbPayBatchSql As String = Nothing, result As Boolean = False

        Try
            'Buils Sql Statement - Start
            SqlStatement = "SELECT SUM(SAS_Accounts.Batchtotal) AS Batch_Total,"
            SqlStatement &= "SAS_Accounts.Subtype AS Sub_Type,"
            SqlStatement &= "SAS_Accounts.Paymentmode AS Payment_Mode,"
            SqlStatement &= "SAS_Accounts.BankCode AS Bank_Code,"
            SqlStatement &= "SAS_Workflow.User_Id AS Posted_By,"
            SqlStatement &= "SAS_Workflow.Date_Time AS Posted_Date "
            SqlStatement &= "FROM SAS_Accounts "
            SqlStatement &= "INNER JOIN SAS_Workflow ON SAS_Accounts.Batchcode = SAS_Workflow.Batch_Code "
            SqlStatement &= "WHERE SAS_Accounts.Batchcode = " & clsGeneric.AddQuotes(BatchCode) & " "
            SqlStatement &= "GROUP BY SAS_Accounts.Paymentmode,SAS_Accounts.SubType,SAS_Accounts.BankCode,SAS_Accounts.Batchcode,SAS_Workflow.User_Id,SAS_Workflow.Date_Time"
            'Buils Sql Statement - Stop

            Using BatchDetails As IDataReader = GetDataReader(SqlStatement)

                'if batch details available - Start
                While BatchDetails.Read

                    'Set Values - Start
                    _CfCbPayBatchEn.cbpb_batchid = CashBookNo
                    _CfCbPayBatchEn.cbpb_company = CompanyCode
                    _CfCbPayBatchEn.cbpb_who = BatchDetails(Helper.PostedByCol)
                    _CfCbPayBatchEn.cbpb_bank = BatchDetails(Helper.BankCodeCol)
                    _CfCbPayBatchEn.cbpb_type = IIf(BatchDetails(Helper.PaymentModeCol).Equals("EFT"), "DDR", BatchDetails(Helper.PaymentModeCol))
                    _CfCbPayBatchEn.cbpb_usrctltot = BatchDetails(Helper.BatchTotalCol)
                    _CfCbPayBatchEn.cbpb_batchtot = BatchDetails(Helper.BatchTotalCol)
                    _CfCbPayBatchEn.cbpb_batchdate = CfGeneric.DateConversion(BatchDetails(Helper.PostedDateCol))
                    'Set Values - Stop

                    'Build Sql Statement - Start
                    Call _CfGeneric.BuildSqlStatement(_CfCbPayBatchEn, CfCbPayBatchSql, CfGeneric.CfCbPayBatchTbl)
                    'Build Sql Statement - Stop

                    'if posted successfully - Start
                    If ExecuteSqlStatement(CfCbPayBatchSql) Then

                        'Clear Sql Statement
                        CfCbPayBatchSql = String.Empty

                        'Build Cash Book Payment Header - Start
                        If CashBookPaymentHeader(BatchCode, CompanyCode, CashBookNo, _CfCbPayBatchEn.cbpb_type, BatchDetails(Helper.CategorySubTypeCol)) Then
                            result = True
                        End If
                        'Build Cash Book Payment Header - Stop

                    End If
                    'if posted successfully - Stop

                End While
                'if batch details available - Stop

                BatchDetails.Close()

            End Using

            Return result

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

#End Region

#Region "Cash Book Payment Header "

    'Purpose			: To Post Cash Book Payment Header
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 31/05/2015
    'Modified by Hafiz @ 30/11/2016

    Private Function CashBookPaymentHeader(ByVal BatchCode As String, ByVal CompanyCode As String, ByVal CashBookNo As String,
                                           ByVal PayMode As String, ByVal SubType As String) As Boolean

        Dim LineNo As Integer = 0, MagicNo As Integer = 0, TransDate As String = Nothing
        Dim SqlStatement As String = Nothing, CfCbPayHeaderSql As String = Nothing, result As Boolean = False
        Dim Payee_Name As String = Nothing, InnerJoin As String = Nothing, GroupBy As String = Nothing

        Try
            If SubType = "Sponsor" Then

                Payee_Name = "CASE WHEN SAS_Accounts.Paymentmode='CHQ' THEN COALESCE(SAS_Accounts.PayeeName,'') "
                Payee_Name += "ELSE COALESCE(SAS_Sponsor.SASR_Name,'') END AS Payee_Name,"

                InnerJoin = "INNER JOIN SAS_Sponsor ON SAS_Accounts.CreditRef = SAS_Sponsor.SASR_Code "
                GroupBy = "SAS_Sponsor.SASR_Name"
            Else

                Payee_Name = "CASE WHEN SAS_Accounts.Paymentmode='CHQ' THEN COALESCE(SAS_Accounts.PayeeName,'') "
                Payee_Name += "ELSE COALESCE(SAS_Student.SASI_Name,'') END AS Payee_Name,"

                InnerJoin = "INNER JOIN SAS_Student ON SAS_Accounts.CreditRef = SAS_Student.SASI_MatricNo "
                GroupBy = "SAS_Student.SASI_name"
            End If

            'Buils Sql Statement - Start
            SqlStatement = "SELECT ROW_NUMBER() OVER (ORDER BY SAS_Accounts.Paymentmode) AS Row_No, "
            SqlStatement &= "CASE WHEN SAS_Accounts.Paymentmode='CHQ' THEN SAS_Accounts.Transamount "
            SqlStatement &= "ELSE CASE WHEN SAS_Accounts.Category='Payment' THEN SAS_Accounts.Transamount "
            SqlStatement &= "ELSE SUM(SAS_Accounts.Transamount) END END AS Trans_Amount,"
            SqlStatement &= "CASE WHEN SAS_Accounts.Voucherno = '' THEN CASE WHEN SAS_Accounts.Transcode = '' THEN "
            SqlStatement &= "SUBSTRING(SAS_Accounts.Transtempcode FROM 2 FOR Length(SAS_Accounts.Transtempcode)) ELSE "
            SqlStatement &= "SAS_Accounts.Transcode END ELSE SAS_Accounts.Voucherno END AS Trans_Code,"
            SqlStatement &= "SAS_Accounts.Paymentmode AS Payment_Mode,"
            SqlStatement &= Payee_Name
            SqlStatement &= "SAS_Workflow.Date_Time AS Posted_Date,"
            SqlStatement &= "SAS_Accounts.CreditRef AS Matric_No,"
            SqlStatement &= "CASE WHEN SAS_Accounts.Paymentmode = 'EFT' THEN "
            SqlStatement &= "CASE WHEN SAS_Accounts.Subtype = 'Student' THEN (SELECT SASI_Bank FROM SAS_Student WHERE SASI_MatricNo = SAS_Accounts.CreditRef) "
            SqlStatement &= "ELSE SAS_Accounts.Bankcode END "
            SqlStatement &= "ELSE SAS_Accounts.Bankcode END AS Bank_Code,"
            SqlStatement &= "CASE WHEN SAS_Accounts.Paymentmode = 'EFT' THEN "
            SqlStatement &= "CASE WHEN SAS_Accounts.Subtype = 'Student' THEN (SELECT SASB_Desc FROM SAS_Studentbank WHERE SASB_Code = (SELECT SASI_Bank FROM SAS_Student WHERE "
            SqlStatement &= "SASI_MatricNo = SAS_Accounts.CreditRef )) "
            SqlStatement &= "ELSE '' END "
            SqlStatement &= "ELSE '' END AS Bank_Name,"
            SqlStatement &= "CASE WHEN SAS_Accounts.Paymentmode = 'EFT' THEN "
            SqlStatement &= "CASE WHEN SAS_Accounts.Paymentmode = 'EFT' THEN (SELECT SASI_Accno FROM SAS_Student WHERE SASI_MatricNo = SAS_Accounts.CreditRef ) "
            SqlStatement &= "ELSE '' END "
            SqlStatement &= "ELSE '' END AS Bank_Acct "
            SqlStatement &= "FROM  SAS_Accounts "
            SqlStatement &= InnerJoin
            SqlStatement &= "INNER JOIN SAS_Workflow ON SAS_Accounts.Batchcode = SAS_Workflow.Batch_Code "
            SqlStatement &= "WHERE SAS_Accounts.Batchcode = " & clsGeneric.AddQuotes(BatchCode) & " "
            SqlStatement &= "GROUP BY SAS_Accounts.Category,SAS_Accounts.Transamount,SAS_Accounts.TransId,SAS_Accounts.Voucherno,SAS_Accounts.Paymentmode,"
            SqlStatement &= "SAS_Accounts.PayeeName,SAS_Accounts.Bankcode,SAS_Accounts.CreditRef,SAS_Workflow.Date_Time," & GroupBy
            'Buils Sql Statement - Stop

            Using HeaderDetails As IDataReader = GetDataReader(SqlStatement)

                'if batch details available - Start
                While HeaderDetails.Read

                    LineNo = HeaderDetails(Helper.LineNoCol)
                    MagicNo = _CfCommon.GetPaymentHeaderMagicNo()
                    TransDate = CfGeneric.DateConversion(HeaderDetails(Helper.PostedDateCol))

                    Dim _CfCbPayHeaderEn As New CfCbPayHeaderEn()
                    _CfCbPayHeaderEn.cbph_lineno = LineNo
                    _CfCbPayHeaderEn.cbph_payee = HeaderDetails(Helper.PayeeNameCol)
                    _CfCbPayHeaderEn.cbph_voucher = HeaderDetails(Helper.TransCodeCol)
                    _CfCbPayHeaderEn.cbph_amount = HeaderDetails(Helper.TransAmountCol)
                    _CfCbPayHeaderEn.cbph_lclamount = HeaderDetails(Helper.TransAmountCol)
                    _CfCbPayHeaderEn.cbph_batchid = CashBookNo
                    _CfCbPayHeaderEn.cbph_company = CompanyCode
                    _CfCbPayHeaderEn.cbph_magic = MagicNo
                    _CfCbPayHeaderEn.cbph_bankcode = clsGeneric.NullToString(HeaderDetails(Helper.BankCodeCol))
                    _CfCbPayHeaderEn.cbph_bankname = clsGeneric.NullToString(HeaderDetails(Helper.BankNameCol))
                    _CfCbPayHeaderEn.cbph_bankacct = clsGeneric.NullToString(HeaderDetails(Helper.BankAcctCol))

                    'Build Sql Statement - Start
                    If _CfGeneric.BuildSqlStatement(_CfCbPayHeaderEn, CfCbPayHeaderSql, CfGeneric.CfCbPayHeaderTbl) Then

                        'if record successfully posted - Start
                        If ExecuteSqlStatement(CfCbPayHeaderSql) Then

                            'Clear Sql Statement
                            CfCbPayHeaderSql = String.Empty

                            'Post Cash Book Payment Details - Start
                            If CashBookPaymentDetail(MagicNo, LineNo, TransDate, HeaderDetails(Helper.TransCodeCol), HeaderDetails(Helper.TransAmountCol)) Then

                                'modified by Hafiz @ 20&28/02/2017
                                'CBP WF - Start
                                Call CBPinterfaceWorkFlow(CompanyCode, CashBookNo)

                                If PayMode.Contains("DDR") Then
                                    Call CBPaymentModeMY(_CfCbPayHeaderEn, HeaderDetails(Helper.MatricNoCol), BatchCode)
                                End If
                                'CBP WF - End

                                'Cash Book Payment Gl Distribution - Start
                                If CashBookGlDist("CBP", BatchCode, LineNo, CashBookNo, CompanyCode, HeaderDetails(Helper.TransAmountCol), clsGeneric.NullToString(HeaderDetails(Helper.BankCodeCol)),
                                                  HeaderDetails(Helper.MatricNoCol), _CfCbPayHeaderEn.cbph_lineno, SubType) Then
                                    result = True
                                End If
                                'Cash Book Payment Gl Distribution - Stop
                            Else
                                result = False
                            End If
                            'Post Cash Book Payment Details - Stop

                        Else
                            result = False
                        End If
                        'if record successfully posted - Stop

                    End If
                    'Build Sql Statement - Stop

                End While
                'if batch details available - Stop

                HeaderDetails.Close()

            End Using

            Return result

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

#End Region

#Region "Cash Book Payment Details "

    'Purpose			: To Post Cash Book Payment Details
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 31/05/2015
    'Modified by Hafiz @ 30/11/2016

    Private Function CashBookPaymentDetail(ByVal MagicNo As Integer, ByVal LineNo As Integer, ByVal TransDate As String,
                                           ByVal TransCode As String, ByVal TransAmount As Decimal) As Boolean

        Dim _CfCbPayDetailsEn As New CfCbPayDetailsEn(), CfCbPayDetailsSql As String = Nothing

        Try
            _CfCbPayDetailsEn.cbpr_magic = MagicNo
            _CfCbPayDetailsEn.cbpr_lineno = LineNo
            _CfCbPayDetailsEn.cbpr_itemdate = TransDate
            _CfCbPayDetailsEn.cbpr_itemrefno = TransCode
            _CfCbPayDetailsEn.cbpr_rmtamount = TransAmount
            _CfCbPayDetailsEn.cbpr_itemamount = TransAmount

            If _CfGeneric.BuildSqlStatement(_CfCbPayDetailsEn, CfCbPayDetailsSql, CfGeneric.CfCbPayDetailsTbl) Then
                Return ExecuteSqlStatement(CfCbPayDetailsSql)
            End If

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

#End Region

#Region "Roll Back Cash Book Payment "

    'Purpose			: To Roll Back Cash Book Payment 
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 31/05/2015
    Private Sub RollBackCashBookPayment(ByVal BatchCode As String, ByVal CashBookNo As String)

        'Variable Declarations
        Dim SqlStatement As String = Nothing

        Try

            'Build Delete Sql Statement for Cash Book Payment - Start
            SqlStatement = "DELETE FROM " & CfGeneric.CfCbGlDistTbl & " WHERE "
            SqlStatement &= "gldi_batchid = " & clsGeneric.AddQuotes(CashBookNo) & ";"
            SqlStatement &= " DELETE FROM " & CfGeneric.CfCbPayDetailsTbl & " WHERE cbpr_magic "
            SqlStatement &= " IN (SELECT cbph_magic FROM " & CfGeneric.CfCbPayHeaderTbl & " WHERE "
            SqlStatement &= " cbph_batchid = " & clsGeneric.AddQuotes(CashBookNo) & ");"
            SqlStatement &= " DELETE FROM " & CfGeneric.CfCbPayHeaderTbl & " WHERE "
            SqlStatement &= " cbph_batchid = " & clsGeneric.AddQuotes(CashBookNo) & ";"
            SqlStatement &= " DELETE FROM " & CfGeneric.CfCbPayBatchTbl & " WHERE "
            SqlStatement &= " cbpb_batchid = " & clsGeneric.AddQuotes(CashBookNo) & ";"
            'Build Delete Sql Statement for Cash Book Payment - Stop

            'Execute Sql Statement
            Call ExecuteSqlStatement(SqlStatement)

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

        End Try

    End Sub

#End Region

#Region "CBPaymentModeMY"
    'added by Hafiz @ 28/02/2017
    'Payment Mode DDR(EFT)

    Public Function CBPaymentModeMY(ByVal CbPayHeader As Object, ByVal MatricNo As String, ByVal BatchCode As String) As Boolean

        Dim _CfCbPayMyCbDetSql As String = Nothing
        Dim _CfCbPayMyCbDetails As New CfCbPayMyCbDetails(), CbPayEn As New CfCbPayMyCbDetails()

        Try

            For Each prop As PropertyInfo In CbPayHeader.GetType().GetProperties()
                If prop.CanRead Then
                    Select Case prop.Name
                        Case "cbph_company"
                            _CfCbPayMyCbDetails.cbph_company = prop.GetValue(CbPayHeader, Nothing)
                        Case "cbph_batchid"
                            _CfCbPayMyCbDetails.cbph_batchid = prop.GetValue(CbPayHeader, Nothing)
                        Case "cbph_lineno"
                            _CfCbPayMyCbDetails.cbph_lineno = prop.GetValue(CbPayHeader, Nothing)
                    End Select
                End If
            Next

            If GetMyCbDetails(MatricNo, BatchCode, CbPayEn) Then
                _CfCbPayMyCbDetails.cbph_icno = CbPayEn.cbph_icno
                _CfCbPayMyCbDetails.cbph_acctno = CbPayEn.cbph_acctno
                _CfCbPayMyCbDetails.cbph_recvbank = CbPayEn.cbph_recvbank
                _CfCbPayMyCbDetails.cbph_email = CbPayEn.cbph_email
                _CfCbPayMyCbDetails.cbph_mobile = CbPayEn.cbph_mobile
                _CfCbPayMyCbDetails.cbph_comment = CbPayEn.cbph_comment
            End If

            If _CfGeneric.BuildSqlStatement(_CfCbPayMyCbDetails, _CfCbPayMyCbDetSql, CfGeneric.CfCbPayMyCbDetTbl) Then
                Return ExecuteSqlStatement(_CfCbPayMyCbDetSql)
            End If

        Catch ex As Exception

            Call Helper.LogError("Entity Class: " & CbPayHeader.ToString() & vbCrLf & ex.Message)
            Return False

        End Try

    End Function

    'construct query for CBP Details
    Public Function GetMyCbDetails(ByVal MatricNo As String, ByVal BatchCode As String, ByRef CbPayEn As CfCbPayMyCbDetails) As Boolean

        Dim SqlStatement As String = Nothing

        Try
            SqlStatement = "SELECT COALESCE(SS.SASI_IcNo,'') AS IC_NO, COALESCE(SS.SASI_AccNo,'')AS ACC_NO, COALESCE(SS.SASI_Bank,'') AS BANK,"
            SqlStatement &= "COALESCE(SS.SASI_Email,'') AS EMAIL,COALESCE(SS.SASI_Hp,'') AS MOBILE, COALESCE(SA.Description,'') AS DESC "
            SqlStatement &= "FROM SAS_Accounts SA "
            SqlStatement &= "INNER JOIN SAS_Student SS ON SS.SASI_MatricNo=SA.CreditRef "
            SqlStatement &= "WHERE SA.CreditRef = " & clsGeneric.AddQuotes(MatricNo) & " "
            SqlStatement &= "AND SA.BatchCode=" & clsGeneric.AddQuotes(BatchCode) & " "

            Using _DataReader As IDataReader = _DataBaseProvider.ExecuteReader(Helper.GetDataBaseType, Helper.GetConnectionString, SqlStatement).CreateDataReader()

                While _DataReader.Read
                    CbPayEn.cbph_icno = _DataReader("IC_NO")
                    CbPayEn.cbph_acctno = _DataReader("ACC_NO")
                    CbPayEn.cbph_recvbank = _DataReader("BANK")
                    CbPayEn.cbph_email = _DataReader("EMAIL")
                    CbPayEn.cbph_mobile = _DataReader("MOBILE")
                    CbPayEn.cbph_comment = _DataReader("DESC")
                End While

                _DataReader.Close()

            End Using

            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function

#End Region

#Region "CBPinterfaceWorkFlow"

    'added by Hafiz @ 22/02/2017
    'CBP interface workflow

    Protected Function CBPinterfaceWorkFlow(ByVal CompanyCode As String, ByVal CashBookNo As String) As Boolean

        Dim _CfCbPayFlowEn As New CfCbPayFlowEn(), CfCbPayFlowSql As String = Nothing

        Try
            _CfCbPayFlowEn.mypf_company = CompanyCode
            _CfCbPayFlowEn.mypf_batchid = CashBookNo

            If _CfGeneric.BuildSqlStatement(_CfCbPayFlowEn, CfCbPayFlowSql, CfGeneric.CfCbPayFlowTbl) Then
                Return ExecuteSqlStatement(CfCbPayFlowSql)
            End If

        Catch ex As Exception
            Call Helper.LogError(ex.Message)
            Return False
        End Try

    End Function

#End Region

#Region "Cash Book Receipt "

    'Purpose			: To Post Cash Book Receipt
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 31/05/2015
    Public Function CashBookReceipt(ByVal BatchCode As String, ByRef CompanyCode As String, ByRef CashBookNo As String) As Boolean

        'Variable Declarations - Start
        'Dim CashBookNo As String = Nothing
        Dim CfCbPayBatchSql As String = Nothing
        'Variable Declarations - Stop

        Try

            'Get Company Code
            CompanyCode = _CfCommon.GetCompanyCode()

            'Get Auto Number
            CashBookNo = _CfCommon.GetAutoNumber(CfGeneric.PostingType.CashBookReceipt)

            'Post Cash Book Receipt Batch - Start
            If Not CashBookReceiptBatch(BatchCode, CompanyCode, CashBookNo) Then

                'Roll Back Transactions
                Call RollBackCashBookReceipt(BatchCode, CashBookNo)

                Return False
            Else
                'Increment Auto No
                Call _CfCommon.UpdateAutoNumber(CfGeneric.PostingType.CashBookReceipt)

                Return True
            End If
            'Post Cash Book Receipt Batch - Stop

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

#End Region

#Region "Cash Book Receipt Batch "

    'Purpose			: To Post Cash Book Receipt
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 29/05/2015
    'Modified by Hafiz @ 30/11/2016

    Private Function CashBookReceiptBatch(ByVal BatchCode As String, ByVal CompanyCode As String, ByVal CashBookNo As String) As Boolean

        'Variable Declarations
        Dim _CfCbRecBatchEn As New CfCbRecBatchEn(), SqlStatement As String = Nothing, CfCbRecBatchSql As String = Nothing, result As Boolean = False

        Try

            'Buils Sql Statement - Start
            SqlStatement &= "SELECT DISTINCT A.BankCode AS Bank_Code,A.Subtype AS Sub_Type,"
            SqlStatement &= "(SELECT SUM(Transamount) FROM SAS_Accounts WHERE Batchcode = A.Batchcode) AS Batch_Total,"
            SqlStatement &= "CASE WHEN A.Description LIKE 'CIMB CLICKS%' THEN "
            SqlStatement &= "(SELECT date_time FROM SAS_Clicks_Filedetails where BatchCode = " & clsGeneric.AddQuotes(BatchCode) & ") "
            SqlStatement &= "ELSE A.TransDate END AS Batch_Date,"
            SqlStatement &= "SAS_Workflow.Date_Time AS Posted_Date,SAS_Workflow.User_id AS Posted_By "
            SqlStatement &= "FROM SAS_Accounts A INNER JOIN SAS_Workflow ON A.Batchcode = SAS_Workflow.Batch_Code "
            SqlStatement &= "WHERE A.Batchcode = " & clsGeneric.AddQuotes(BatchCode)
            'Buils Sql Statement - Stop

            'get Batch Details
            Using BatchDetails As IDataReader = GetDataReader(SqlStatement)

                'if batch details available - Start
                While BatchDetails.Read()

                    'Set Values - Start
                    _CfCbRecBatchEn.cbrb_batchid = CashBookNo
                    _CfCbRecBatchEn.cbrb_company = CompanyCode
                    _CfCbRecBatchEn.cbrb_who = BatchDetails(Helper.PostedByCol)
                    _CfCbRecBatchEn.cbrb_bank = BatchDetails(Helper.BankCodeCol)
                    _CfCbRecBatchEn.cbrb_batchtot = BatchDetails(Helper.BatchTotalCol)
                    _CfCbRecBatchEn.cbrb_batchdate = CfGeneric.DateConversion(BatchDetails(Helper.BatchDateCol))
                    '_CfCbRecBatchEn.cbrb_postdate = CfGeneric.DateConversion(BatchDetails(Helper.PostedDateCol))
                    _CfCbRecBatchEn.cbrb_usrctltot = BatchDetails(Helper.BatchTotalCol)
                    'Set Values - Stop

                    'Build Sql Statement - Start
                    Call _CfGeneric.BuildSqlStatement(_CfCbRecBatchEn, CfCbRecBatchSql, CfGeneric.CfCbRecBatchTbl)
                    'Build Sql Statement - Stop

                    'if posted successfully - Start
                    If ExecuteSqlStatement(CfCbRecBatchSql) Then

                        'Build Cash Book Payment Header - Start
                        If CashBookReceiptHeader(BatchCode, CompanyCode, CashBookNo, BatchDetails(Helper.CategorySubTypeCol)) Then
                            result = True
                        End If
                        'Build Cash Book Payment Header - Stop

                    End If
                    'if posted successfully - Stop

                End While
                'if batch details available - Stop

                BatchDetails.Close()

            End Using

            Return result

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

#End Region

#Region "Cash Book Receipt Header "

    'Purpose			: To Post Cash Book Receipt Header
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 01/06/2015
    'Modified by Hafiz @ 30/11/2016

    Private Function CashBookReceiptHeader(ByVal BatchCode As String, ByVal CompanyCode As String, ByVal CashBookNo As String,
                                           ByVal SubType As String) As Boolean

        Dim SqlStatement As String = Nothing, CfCbRecHeaderSql As String = Nothing, result As Boolean = False
        Dim Payee_Name As String = Nothing, InnerJoin As String = Nothing

        Try
            If SubType = "Sponsor" Then
                Payee_Name = "SAS_Sponsor.SASR_Name AS Payee_Name,"
                InnerJoin = "INNER JOIN SAS_Sponsor ON SAS_Accounts.CreditRef = SAS_Sponsor.SASR_Code "
            Else
                Payee_Name = "SAS_Student.SASI_Name AS Payee_Name,"
                InnerJoin = "INNER JOIN SAS_Student ON SAS_Accounts.CreditRef = SAS_Student.SASI_MatricNo "
            End If

            'Buils Sql Statement - Start
            SqlStatement = "SELECT ROW_NUMBER() OVER (ORDER BY SAS_Accounts.TransID) AS Row_No,"
            SqlStatement &= "CASE WHEN SAS_Accounts.Description LIKE 'CIMB CLICKS%' THEN SAS_Accounts.SubRef1 "
            SqlStatement &= "ELSE SAS_Accounts.BankRecNo END AS Bank_Slip_No,"
            SqlStatement &= "SAS_Accounts.BatchCode As Batch_Code,"
            SqlStatement &= "CASE WHEN SAS_Accounts.Description LIKE 'CIMB CLICKS%' THEN 'CIMB Clicks Upload' "
            SqlStatement &= "ELSE 'Bank Receipt Manual' END AS Description,"
            SqlStatement &= Payee_Name
            SqlStatement &= "SAS_Accounts.TransAmount AS Trans_Amount,"
            SqlStatement &= "SAS_Accounts.BankCode AS Bank_Code,"
            SqlStatement &= "SUBSTRING(SAS_Accounts.PaymentMode,1,3) AS Payment_Mode,"
            SqlStatement &= "SAS_Accounts.CreditRef AS Matric_No "
            SqlStatement &= "FROM SAS_Accounts "
            SqlStatement &= "LEFT JOIN SAS_AccountsDetails ON SAS_Accounts.TransID = SAS_AccountsDetails.TransID "
            SqlStatement &= InnerJoin
            SqlStatement &= "WHERE SAS_Accounts.BatchCode = " & clsGeneric.AddQuotes(BatchCode)
            'Buils Sql Statement - Stop

            Using HeaderDetails As IDataReader = GetDataReader(SqlStatement)

                'if batch details available - Start
                While HeaderDetails.Read()

                    Dim _CfCbRecHeaderEn As New CfCbRecHeaderEn()
                    _CfCbRecHeaderEn.cbrh_lineno = HeaderDetails(Helper.LineNoCol)
                    _CfCbRecHeaderEn.cbrh_refno = HeaderDetails(Helper.BatchCodeCol)
                    _CfCbRecHeaderEn.cbrh_payer = HeaderDetails(Helper.PayeeNameCol).Replace("'", "''")
                    _CfCbRecHeaderEn.cbrh_rcptid = HeaderDetails(Helper.BankSlipNoCol)
                    _CfCbRecHeaderEn.cbrh_frombank = HeaderDetails(Helper.BankCodeCol)
                    _CfCbRecHeaderEn.cbrh_amount = HeaderDetails(Helper.TransAmountCol)
                    _CfCbRecHeaderEn.cbrh_batchid = CashBookNo
                    _CfCbRecHeaderEn.cbrh_company = CompanyCode
                    _CfCbRecHeaderEn.cbrh_cashtype = HeaderDetails(Helper.PaymentModeCol)
                    _CfCbRecHeaderEn.cbrh_lclamount = HeaderDetails(Helper.TransAmountCol)
                    _CfCbRecHeaderEn.cbrh_desc = HeaderDetails(Helper.DescriptionCol)
                    _CfCbRecHeaderEn.cbrh_magic = _CfCommon.GetReceiptHeaderMagicNo()

                    'Build Sql Statement - Start
                    If _CfGeneric.BuildSqlStatement(_CfCbRecHeaderEn, CfCbRecHeaderSql, CfGeneric.CfCbRecHeaderTbl) Then

                        'if record successfully posted - Start
                        If ExecuteSqlStatement(CfCbRecHeaderSql) Then

                            'Clear Sql Statement
                            CfCbRecHeaderSql = String.Empty

                            'Cash Book Payment Gl Distribution - Start
                            If CashBookGlDist("CBR", HeaderDetails(Helper.BatchCodeCol), HeaderDetails(Helper.LineNoCol), CashBookNo, CompanyCode, HeaderDetails(Helper.TransAmountCol),
                                             HeaderDetails(Helper.BankCodeCol), HeaderDetails(Helper.MatricNoCol), _CfCbRecHeaderEn.cbrh_lineno, SubType) Then
                                result = True
                            End If
                            'Cash Book Payment Gl Distribution - Stop
                        Else
                            result = False
                        End If
                        'if record successfully posted - Stop

                    End If
                    'Build Sql Statement - Stop

                End While
                'if batch details available - Stop

                HeaderDetails.Close()

            End Using

            Return result

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

#End Region

#Region "Roll Back Cash Book Receipt "

    'Purpose			: To Roll Back Cash Book Receipt 
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 01/06/2015
    Private Sub RollBackCashBookReceipt(ByVal BatchCode As String, ByVal CashBookNo As String)

        'Variable Declarations
        Dim SqlStatement As String = Nothing

        Try

            'Build Delete Sql Statement for Cash Book Payment - Start
            SqlStatement = "DELETE FROM " & CfGeneric.CfCbGlDistTbl & " WHERE "
            SqlStatement &= "gldi_batchid = " & clsGeneric.AddQuotes(CashBookNo) & ";"
            SqlStatement &= " DELETE FROM " & CfGeneric.CfCbRecHeaderTbl & " WHERE "
            SqlStatement &= " cbrh_batchid = " & clsGeneric.AddQuotes(CashBookNo) & ";"
            SqlStatement &= " DELETE FROM " & CfGeneric.CfCbRecBatchTbl & " WHERE "
            SqlStatement &= " cbrb_batchid = " & clsGeneric.AddQuotes(CashBookNo) & ";"
            'Build Delete Sql Statement for Cash Book Payment - Stop

            'Execute Sql Statement
            Call ExecuteSqlStatement(SqlStatement)

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

        End Try

    End Sub

#End Region

#Region "Cash Book Gl Disturbtion "

    'Purpose			: To Post Cash Book Payment GL Distribution
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 31/05/2015
    'Modified By Hafiz @ 01/12/2016

    Private Function CashBookGlDist(ByVal CB_Type As String, ByVal BatchCode As String, ByVal LineNo As Integer, ByVal CashBookNo As String,
        ByVal CompanyCode As String, ByVal TransAmount As Decimal, ByVal BankCode As String, ByVal MatricNo As String, ByVal Counter As Integer,
        Optional ByVal SubType As String = Nothing) As Boolean

        'Create Instances
        Dim _CfCbGlDistEn As CfCbGlDistEn, TempLineNo As Integer = 0, CheckBookType As String = Nothing, result As Boolean = False, ItemType As String = Nothing

        'Variable Declarations - Start
        Dim SqlStatement As String = Nothing, Glcode As String = Nothing
        Dim CfCbGlDistSql As String = Nothing, LedgerType As String = Nothing
        'Variable Declarations - Stop

        Try
            Glcode = _CfCommon.GetGlCode(BankCode, MatricNo, CfGeneric.GlType.BankCode, CB_Type, SubType)
            LedgerType = _CfCommon.GetLedgerType(Glcode)

            If LineNo <> 0 Then
                LineNo = 1
            End If

            If CB_Type = "CBR" Then
                CheckBookType = "CashBook Receipt"
                ItemType = "CSH"
            ElseIf CB_Type = "CBP" Then
                CheckBookType = "CashBook Payment"
                ItemType = "PAY"
            End If

            _CfCbGlDistEn = New CfCbGlDistEn
            _CfCbGlDistEn.gldi_glac = Glcode
            _CfCbGlDistEn.gldi_itemtype = ItemType
            _CfCbGlDistEn.gldi_seqno = LineNo
            _CfCbGlDistEn.gldi_itemref = Counter
            _CfCbGlDistEn.gldi_batchid = CashBookNo
            _CfCbGlDistEn.gldi_code = CashBookNo
            _CfCbGlDistEn.gldi_amount = TransAmount
            _CfCbGlDistEn.gldi_modify = "C"
            _CfCbGlDistEn.gldi_company = CompanyCode
            _CfCbGlDistEn.gldi_lclamount = TransAmount
            _CfCbGlDistEn.gldi_serial = _CfCommon.GetGlDistNextSerialNo()
            _CfCbGlDistEn.gldi_desc = _CfCommon.GetGLDescription(Glcode, LedgerType)

            'Build Sql Statement - Start
            If _CfGeneric.BuildSqlStatement(_CfCbGlDistEn, CfCbGlDistSql, CfGeneric.CfCbGlDistTbl) Then

                'if Successful - Start
                If ExecuteSqlStatement(CfCbGlDistSql) Then

                    'Clear Sql Statement
                    CfCbGlDistSql = String.Empty

                    'store at the sas db environment - start
                    SASCF_InsertDDLine(_CfCbGlDistEn, CheckBookType, "Debit", BatchCode, MatricNo)
                    'store at the sas db environment - end

                    Glcode = _CfCommon.GetGlCode(BankCode, MatricNo, CfGeneric.GlType.StudentProgram, CB_Type, SubType)
                    LedgerType = _CfCommon.GetLedgerType(Glcode)

                    _CfCbGlDistEn = New CfCbGlDistEn
                    _CfCbGlDistEn.gldi_glac = Glcode
                    _CfCbGlDistEn.gldi_itemtype = ItemType
                    _CfCbGlDistEn.gldi_seqno = LineNo + 1
                    _CfCbGlDistEn.gldi_itemref = Counter
                    _CfCbGlDistEn.gldi_batchid = CashBookNo
                    _CfCbGlDistEn.gldi_code = CashBookNo
                    _CfCbGlDistEn.gldi_company = CompanyCode
                    _CfCbGlDistEn.gldi_amount = TransAmount * -1
                    _CfCbGlDistEn.gldi_modify = String.Empty
                    _CfCbGlDistEn.gldi_lclamount = TransAmount * -1
                    _CfCbGlDistEn.gldi_serial = _CfCommon.GetGlDistNextSerialNo()
                    _CfCbGlDistEn.gldi_desc = _CfCommon.GetGLDescription(Glcode, LedgerType)

                    'Build Sql Statement - Start
                    If _CfGeneric.BuildSqlStatement(_CfCbGlDistEn, CfCbGlDistSql, CfGeneric.CfCbGlDistTbl) Then

                        If Not ExecuteSqlStatement(CfCbGlDistSql) Then
                            result = False
                        End If
                        'Clear Sql Statement
                        CfCbGlDistSql = String.Empty

                        'store at the sas db environment - start
                        SASCF_InsertDDLine(_CfCbGlDistEn, CheckBookType, "Credit", BatchCode, MatricNo)
                        'store at the sas db environment - end

                        result = True

                    End If
                    'Build Sql Statement - Stop
                Else
                    result = False
                End If
                'if Successful - Stop

            End If
            'Build Sql Statement - Stop

            Return result

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

#End Region

#Region "Multiple Journel "

    'Purpose			: To Post Mulitple Journel
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 01/06/2015
    'Modified by Hafiz @ 25/11/2016

    Public Function MultipleJournel(ByVal CategoryType As String, ByVal CategorySubType As String,
        ByVal BatchCode As String, ByVal TransactionType As String, ByVal Description As String,
         ByVal PostedBy As String, ByRef CompanyCode As String, ByRef JournelNo As String) As Boolean

        'Variable Declarations - Start
        Dim CfMjHeaderSql As String = Nothing
        'Variable Declarations - Stop

        Try

            'Get Company Code
            CompanyCode = _CfCommon.GetCompanyCode()

            'Get Auto Number
            JournelNo = _CfCommon.GetAutoNumber(CfGeneric.PostingType.MultipleJournel)

            'Post to Multiple Journel Header - Start
            If MultipleJournelHeader(BatchCode, CompanyCode,
                JournelNo, CategoryType, CategorySubType, Description, PostedBy, TransactionType) Then

                'Increment Auto No
                Call _CfCommon.UpdateAutoNumber(CfGeneric.PostingType.MultipleJournel)

                Return True

            Else

                'Roll Back Transactions
                Call RollBackMultipleJournal(BatchCode, JournelNo)

                Return False

            End If
            'Post to Multiple Journel Header - Stop

            Return False

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

#End Region

#Region "Multiple Journel Header "

    'Purpose			: To Post Mulitple Journel Header
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 01/06/2015
    'Modified by Hafiz @ 24/11/2016

    Private Function MultipleJournelHeader(ByVal BatchCode As String,
        ByVal CompanyCode As String, ByVal JournalNo As String,
        ByVal CategoryType As String, ByVal CategorySubType As String, ByVal Description As String,
        ByVal PostedBy As String, ByVal TransactionType As String) As Boolean

        'Create Instances - Start
        Dim _CfMjHeaderEn As New CfMjHeaderEn()
        'Create Instances - Stop

        'Variable Declarations - Start
        Dim TotalRecords As Integer = 0
        Dim CfMjHeaderSql As String = Nothing, HeaderDescription As String = Nothing
        'Variable Declarations - Stop

        Try

            'Set Header Description
            If CategoryType = CfGeneric.CategoryTypeAfc Then
                'HeaderDescription = CategoryType & " For Sem " & Description.Substring(12) & "-" & Description.Substring(4, 4) & "/" & Description.Substring(8, 4)
                HeaderDescription = CategoryType & " " & Description.Substring(13)
            ElseIf CategoryType = CfGeneric.CategoryTypeInvoice Or CategoryType = CfGeneric.CategoryTypeDebitNote Or CategoryType = CfGeneric.CategoryTypeCreditNote Then
                HeaderDescription = Trim(Description)
            ElseIf CategoryType = CfGeneric.CategoryTypeAllocation Then
                HeaderDescription = "Sponsor Allocation - Batch " & BatchCode
            Else
                HeaderDescription = CategoryType & " Batch " & BatchCode & " From SAS"
            End If

            'Set Values - Start
            _CfMjHeaderEn.mjjh_who = PostedBy
            _CfMjHeaderEn.mjjh_jnl = JournalNo
            _CfMjHeaderEn.mjjh_company = CompanyCode
            _CfMjHeaderEn.mjjh_desc = HeaderDescription
            _CfMjHeaderEn.mjjh_entdate = CfGeneric.DateConversion(Now)
            _CfMjHeaderEn.mjjh_jnldate = CfGeneric.DateConversion(Now)
            'Set Values - Stop

            'Build Sql Statement - Start
            If _CfGeneric.BuildSqlStatement(_CfMjHeaderEn,
                CfMjHeaderSql, CfGeneric.CfMjHeaderTbl) Then

                'if header successfully posted - Start
                If ExecuteSqlStatement(CfMjHeaderSql) Then

                    'If Line Debit Details Successful - Start
                    If MultipleJournelLineDebit(BatchCode, CompanyCode,
                        JournalNo, TotalRecords, TransactionType, CategoryType, CategorySubType) Then

                        'If Line Credit Details Successful - Start
                        If Not MultipleJournelLineCredit(BatchCode, CompanyCode,
                            JournalNo, TotalRecords, TransactionType, CategoryType, CategorySubType) Then
                            Return False
                        End If
                        'If Line Credit Details Successful - Stop

                    End If
                    'If Line Debit Details Successful - Stop

                End If
                'if header successfully posted - Stop

            End If
            'Build Sql Statement - Stop

            Return True

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

#End Region

#Region "Multiple Journel Line - Debit "

    'Purpose			: To Post Mulitple Journel Line - Debit
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 01/06/2015
    'Modified by Hafiz @ 12/1/2017

    Private Function MultipleJournelLineDebit(ByVal BatchCode As String, ByVal CompanyCode As String, ByVal JournalNo As String,
        ByRef TotalRecords As Integer, ByVal TransactionType As String, Optional ByVal CategoryType As String = Nothing,
        Optional ByVal CategorySubType As String = Nothing) As Boolean

        'Create Instances - Start
        Dim _CfMJLineEn As New CfMjLineEn()
        Dim LineDetails As IDataReader = Nothing
        'Create Instances - Stop

        'Variable Declarations - Start
        Dim LineNo As Integer = 0
        Dim SqlStatement As String = Nothing, GlCode As String = Nothing, GlDescription As String = Nothing
        Dim TransactionAmount As Decimal = 0, LedgerType As String = Nothing, CfMjLineSql As String = Nothing
        'Variable Declarations - Stop

        Try

            If CategorySubType = "Sponsor" Then

                Select Case CategoryType

                    Case CfGeneric.CategoryTypeCreditNote
                        SqlStatement = "SELECT ROW_NUMBER() OVER (ORDER BY SA.GLcode) AS Row_No,"
                        SqlStatement &= "SA.GLcode AS GL_Code,"
                        SqlStatement &= "SUM(SA.TransAmount) AS Trans_Amount "
                        SqlStatement &= "FROM SAS_Accounts SA "
                        SqlStatement &= "INNER JOIN SAS_Sponsor SPN ON SPN.SASR_Code= SA.CreditRef "
                        SqlStatement &= "WHERE SA.Batchcode = " & clsGeneric.AddQuotes(BatchCode) & " "
                        SqlStatement &= "GROUP BY SA.GLcode"

                    Case Else
                        SqlStatement = "SELECT ROW_NUMBER() OVER (ORDER BY SAS_Sponsor.SASR_Glaccount) AS Row_No,"
                        SqlStatement &= "SAS_Sponsor.SASR_Glaccount AS GL_Code,"
                        SqlStatement &= "SUM(SAS_Accounts.TransAmount) AS Trans_Amount "
                        SqlStatement &= "FROM SAS_Accounts "
                        SqlStatement &= "INNER JOIN SAS_Sponsor ON SAS_Accounts.CreditRef = SAS_Sponsor.SASR_Code "
                        SqlStatement &= "WHERE SAS_Accounts.Batchcode = " & clsGeneric.AddQuotes(BatchCode) & " "
                        SqlStatement &= "GROUP BY SAS_Sponsor.SASR_Glaccount"

                End Select

            ElseIf CategorySubType = "Student" Then

                Select Case CategoryType

                    Case CfGeneric.CategoryTypeLoan
                        SqlStatement = "SELECT ROW_NUMBER() OVER (ORDER BY SAS_Universityfund.SAUF_Glcode) AS Row_No,"
                        SqlStatement &= "SAS_Universityfund.SAUF_Glcode AS GL_Code,"
                        SqlStatement &= "SUM(SAS_StudentLoan.TransAmount) AS Trans_Amount "
                        SqlStatement &= "FROM SAS_Accounts "
                        SqlStatement &= "INNER JOIN SAS_StudentLoan ON SAS_Accounts.Batchcode=SAS_StudentLoan.Batchcode "
                        SqlStatement &= "INNER JOIN SAS_Universityfund ON SAS_Accounts.SubRef1=SAS_Universityfund.SAUF_Code "
                        SqlStatement &= "WHERE SAS_Accounts.Batchcode = " & clsGeneric.AddQuotes(BatchCode) & " "
                        SqlStatement &= "GROUP BY SAS_Universityfund.SAUF_Glcode"

                    Case CfGeneric.CategoryTypeCreditNote
                        SqlStatement = "SELECT ROW_NUMBER() OVER (ORDER BY GL_Code) AS Row_No,GL_Code,Trans_Amount "
                        SqlStatement &= "FROM ("
                        SqlStatement &= "SELECT CASE WHEN FGL.GL_Account IS NULL THEN "
                        SqlStatement &= "CASE WHEN KGL.GL_Account IS NULL THEN (SELECT GL_Account FROM SAS_Kolej_GLaccount WHERE SAFT_Code=SAS_AccountsDetails.RefCode "
                        SqlStatement &= "AND SAKO_Code=(SELECT SASI_KokoCode FROM SAS_Student WHERE SASI_MatricNo=SAS_Accounts.CreditRef)) "
                        SqlStatement &= "ELSE KGL.GL_Account END "
                        SqlStatement &= "ELSE FGL.GL_Account END AS GL_Code,"
                        SqlStatement &= "SUM(SAS_AccountsDetails.TransAmount) AS Trans_Amount "
                        SqlStatement &= "FROM SAS_Accounts "
                        SqlStatement &= "INNER JOIN SAS_AccountsDetails ON SAS_Accounts.TransId = SAS_AccountsDetails.TransId "
                        SqlStatement &= "INNER JOIN SAS_Student ON SAS_Accounts.CreditRef = SAS_Student.SASI_MatricNo "
                        SqlStatement &= "LEFT JOIN SAS_Faculty_GLaccount FGL ON SAS_AccountsDetails.RefCode = FGL.SAFT_Code AND FGL.SAFC_Code = SAS_Student.SASI_Faculty "
                        SqlStatement &= "LEFT JOIN SAS_Kolej_GLaccount KGL ON SAS_AccountsDetails.RefCode = KGL.SAFT_Code AND KGL.SAKO_Code = SAS_Student.SAKO_Code "
                        SqlStatement &= "WHERE SAS_Accounts.Batchcode = " & clsGeneric.AddQuotes(BatchCode) & " "
                        SqlStatement &= "GROUP BY GL_Code ) a "

                    Case Else
                        SqlStatement = "Select ROW_NUMBER() OVER (ORDER BY SAS_Program.SAPG_TI) AS Row_No,"
                        SqlStatement &= "SAS_Program.SAPG_TI AS GL_Code,SUM(SAS_AccountsDetails.TransAmount) AS Trans_Amount"
                        SqlStatement &= " FROM SAS_Accounts INNER JOIN SAS_AccountsDetails ON"
                        SqlStatement &= " SAS_Accounts.TransId = SAS_AccountsDetails.TransId INNER JOIN"
                        SqlStatement &= " SAS_Student ON SAS_Accounts.CreditRef = SAS_Student.SASI_MatricNo"
                        SqlStatement &= " INNER JOIN SAS_Program ON SAS_Student.SASI_PgId = SAS_Program.SAPG_Code"
                        SqlStatement &= " WHERE SAS_Accounts.batchcode = " & clsGeneric.AddQuotes(BatchCode)
                        SqlStatement &= " GROUP BY SAS_Program.SAPG_TI"

                End Select

            End If

            'Get Line Details - Start
            LineDetails = _DataBaseProvider.ExecuteReader(Helper.GetDataBaseType,
                Helper.GetConnectionString, SqlStatement).CreateDataReader()
            'Get Line Details - Stop

            'Loop thro the Data Reader - Start
            While LineDetails.Read

                'Get Values - Start
                GlCode = IIf(IsDBNull(LineDetails(Helper.GlCodeCol)), String.Empty, LineDetails(Helper.GlCodeCol))
                LedgerType = _CfCommon.GetLedgerType(GlCode)
                LineNo = LineDetails(Helper.LineNoCol)
                TransactionAmount = LineDetails(Helper.TransAmountCol)
                GlDescription = IIf(IsDBNull(_CfCommon.GetGLDescription(GlCode, LedgerType)), String.Empty, _CfCommon.GetGLDescription(GlCode, LedgerType))
                'Get Values - Stop

                'Set Values - Start
                _CfMJLineEn.mjjl_lineno = LineNo
                _CfMJLineEn.mjjl_jnl = JournalNo
                _CfMJLineEn.mjjl_account = GlCode
                _CfMJLineEn.mjjl_ledger = LedgerType
                _CfMJLineEn.mjjl_desc = GlDescription
                _CfMJLineEn.mjjl_company = CompanyCode
                _CfMJLineEn.mjjl_amount = TransactionAmount
                _CfMJLineEn.mjjl_lclamount = TransactionAmount
                '_CfMJLineEn.mjjl_reference = BatchCode.Substring(4, (BatchCode.Length - 4))
                _CfMJLineEn.mjjl_reference = BatchCode
                'Set Values - Stop

                'Build Sql Statement - Start
                If _CfGeneric.BuildSqlStatement(_CfMJLineEn,
                    CfMjLineSql, CfGeneric.CfMjLineTbl) Then

                    'if posting not successful - Start
                    If Not ExecuteSqlStatement(CfMjLineSql) Then
                        Return False
                    End If
                    CfMjLineSql = String.Empty
                    'if posting not successful - Stop

                    'store at the sas db environment - start
                    SASCF_InsertDDLine(_CfMJLineEn, "Multiple Journal", "Debit", BatchCode)
                    'store at the sas db environment - end

                End If
                'Build Sql Statement - Stop


                'Increment Row Count
                TotalRecords = TotalRecords + 1

            End While
            'Loop thro the Data Reader - Stop

            Return True

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

#End Region

#Region "Multiple Journel Line - Credit "

    'Purpose			: To Post Mulitple Journel Line - Debit
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 01/06/2015
    'Modified by Hafiz @ 12/1/2017

    Private Function MultipleJournelLineCredit(ByVal BatchCode As String, ByVal CompanyCode As String, ByVal JournalNo As String,
        ByVal TotalRecords As Integer, ByVal TransactionType As String, Optional ByVal CategoryType As String = Nothing,
        Optional ByVal CategorySubType As String = Nothing) As Boolean

        'Create Instances - Start
        Dim _CfMJLineEn As New CfMjLineEn()
        Dim LineDetails As IDataReader = Nothing
        'Create Instances - Stop

        'Variable Declarations - Start
        Dim LineNo As Integer = 0
        Dim SqlStatement As String = Nothing, GlCode As String = Nothing, GlDescription As String = Nothing
        Dim TransactionAmount As Decimal = 0, LedgerType As String = Nothing, CfMjLineSql As String = Nothing
        'Variable Declarations - Stop

        Try

            If CategorySubType = "Sponsor" Then

                Select Case CategoryType

                    Case CfGeneric.CategoryTypeDebitNote
                        SqlStatement = "SELECT ROW_NUMBER() OVER (ORDER BY SA.GLcode) AS Row_No,"
                        SqlStatement &= "SA.GLcode AS GL_Code,"
                        SqlStatement &= "SUM(SA.TransAmount) AS Trans_Amount "
                        SqlStatement &= "FROM SAS_Accounts SA "
                        SqlStatement &= "INNER JOIN SAS_Sponsor SPN ON SPN.SASR_Code= SA.CreditRef "
                        SqlStatement &= "WHERE SA.Batchcode = " & clsGeneric.AddQuotes(BatchCode) & " "
                        SqlStatement &= "GROUP BY SA.GLcode"

                    Case CfGeneric.CategoryTypeAllocation
                        SqlStatement = "SELECT ROW_NUMBER() OVER (ORDER BY SP.SAPG_TI) AS Row_No,"
                        SqlStatement &= "SP.SAPG_TI AS GL_Code,"
                        SqlStatement &= "SUM(SA.TransAmount) AS Trans_Amount "
                        SqlStatement &= "FROM SAS_Accounts SA "
                        SqlStatement &= "INNER JOIN SAS_Student SS ON SS.SASI_MatricNo=SA.CreditRef "
                        SqlStatement &= "INNER JOIN SAS_Program SP ON SP.SAPG_Code=SS.SASI_PgId "
                        SqlStatement &= "WHERE SA.BatchCode = " & clsGeneric.AddQuotes(BatchCode) & " "
                        SqlStatement &= "AND SA.Category='SPA' "
                        SqlStatement &= "GROUP BY SP.SAPG_TI"

                    Case Else
                        SqlStatement = "SELECT ROW_NUMBER() OVER (ORDER BY SAS_Sponsor.SASR_Glaccount) AS Row_No,"
                        SqlStatement &= "SAS_Sponsor.SASR_Glaccount AS GL_Code,"
                        SqlStatement &= "SUM(SAS_Accounts.TransAmount) AS Trans_Amount "
                        SqlStatement &= "FROM SAS_Accounts "
                        SqlStatement &= "INNER JOIN SAS_Sponsor ON SAS_Accounts.CreditRef = SAS_Sponsor.SASR_Code "
                        SqlStatement &= "WHERE SAS_Accounts.Batchcode = " & clsGeneric.AddQuotes(BatchCode) & " "
                        SqlStatement &= "GROUP BY SAS_Sponsor.SASR_Glaccount"

                End Select


            ElseIf CategorySubType = "Student" Then

                Select Case CategoryType

                    Case CfGeneric.CategoryTypeLoan
                        SqlStatement = "SELECT ROW_NUMBER() OVER (ORDER BY SAS_Universityfund.SAUF_Glcode) AS Row_No,"
                        SqlStatement &= "SAS_Universityfund.SAUF_Glcode AS GL_Code,"
                        SqlStatement &= "SUM(SAS_StudentLoan.TransAmount) AS Trans_Amount "
                        SqlStatement &= "FROM SAS_Accounts "
                        SqlStatement &= "INNER JOIN SAS_StudentLoan ON SAS_Accounts.Batchcode=SAS_StudentLoan.Batchcode "
                        SqlStatement &= "INNER JOIN SAS_Universityfund ON SAS_Accounts.SubRef1=SAS_Universityfund.SAUF_Code "
                        SqlStatement &= "WHERE SAS_Accounts.Batchcode = " & clsGeneric.AddQuotes(BatchCode) & " "
                        SqlStatement &= "GROUP BY SAS_Universityfund.SAUF_Glcode"

                    Case CfGeneric.CategoryTypeCreditNote
                        SqlStatement = "Select ROW_NUMBER() OVER (ORDER BY SAS_Program.SAPG_TI) AS Row_No,"
                        SqlStatement &= "SAS_Program.SAPG_TI AS GL_Code,SUM(SAS_AccountsDetails.TransAmount) AS Trans_Amount"
                        SqlStatement &= " FROM SAS_Accounts INNER JOIN SAS_AccountsDetails ON"
                        SqlStatement &= " SAS_Accounts.TransId = SAS_AccountsDetails.TransId INNER JOIN"
                        SqlStatement &= " SAS_Student ON SAS_Accounts.CreditRef = SAS_Student.SASI_MatricNo"
                        SqlStatement &= " INNER JOIN SAS_Program ON SAS_Student.SASI_PgId = SAS_Program.SAPG_Code"
                        SqlStatement &= " WHERE SAS_Accounts.batchcode = " & clsGeneric.AddQuotes(BatchCode)
                        SqlStatement &= " GROUP BY SAS_Program.SAPG_TI"

                    Case Else
                        SqlStatement = "SELECT ROW_NUMBER() OVER (ORDER BY GL_Code) AS Row_No,GL_Code,Trans_Amount "
                        SqlStatement &= "FROM ("
                        SqlStatement &= "SELECT CASE WHEN FGL.GL_Account IS NULL THEN "
                        SqlStatement &= "CASE WHEN KGL.GL_Account IS NULL THEN (SELECT GL_Account FROM SAS_Kolej_GLaccount WHERE SAFT_Code=SAS_AccountsDetails.RefCode "
                        SqlStatement &= "AND SAKO_Code=(SELECT SASI_KokoCode FROM SAS_Student WHERE SASI_MatricNo=SAS_Accounts.CreditRef)) "
                        SqlStatement &= "ELSE KGL.GL_Account END "
                        SqlStatement &= "ELSE FGL.GL_Account END AS GL_Code,"
                        SqlStatement &= "SUM(SAS_AccountsDetails.TransAmount) AS Trans_Amount "
                        SqlStatement &= "FROM SAS_Accounts "
                        SqlStatement &= "INNER JOIN SAS_AccountsDetails ON SAS_Accounts.TransId = SAS_AccountsDetails.TransId "
                        SqlStatement &= "INNER JOIN SAS_Student ON SAS_Accounts.CreditRef = SAS_Student.SASI_MatricNo "
                        SqlStatement &= "LEFT JOIN SAS_Faculty_GLaccount FGL ON SAS_AccountsDetails.RefCode = FGL.SAFT_Code AND FGL.SAFC_Code = SAS_Student.SASI_Faculty "
                        SqlStatement &= "LEFT JOIN SAS_Kolej_GLaccount KGL ON SAS_AccountsDetails.RefCode = KGL.SAFT_Code AND KGL.SAKO_Code = SAS_Student.SAKO_Code "
                        SqlStatement &= "WHERE SAS_Accounts.Batchcode = " & clsGeneric.AddQuotes(BatchCode) & " "
                        SqlStatement &= "GROUP BY GL_Code ) a "

                End Select

            End If

            'Get Line Details - Start
            LineDetails = _DataBaseProvider.ExecuteReader(Helper.GetDataBaseType,
                Helper.GetConnectionString, SqlStatement).CreateDataReader()
            'Get Line Details - Stop

            'Loop thro the Data Reader - Start
            While LineDetails.Read

                'Get Values - Start
                LineNo = LineDetails(Helper.LineNoCol)
                GlCode = IIf(IsDBNull(LineDetails(Helper.GlCodeCol)), String.Empty, LineDetails(Helper.GlCodeCol))
                LedgerType = _CfCommon.GetLedgerType(GlCode)
                TransactionAmount = LineDetails(Helper.TransAmountCol) * -1
                GlDescription = IIf(IsDBNull(_CfCommon.GetGLDescription(GlCode, LedgerType)), String.Empty, _CfCommon.GetGLDescription(GlCode, LedgerType))
                'Get Values - Stop

                'Set Values - Start
                _CfMJLineEn.mjjl_jnl = JournalNo
                _CfMJLineEn.mjjl_account = GlCode
                _CfMJLineEn.mjjl_ledger = LedgerType
                _CfMJLineEn.mjjl_desc = GlDescription
                _CfMJLineEn.mjjl_company = CompanyCode
                _CfMJLineEn.mjjl_amount = TransactionAmount
                _CfMJLineEn.mjjl_lclamount = TransactionAmount
                _CfMJLineEn.mjjl_lineno = LineNo + TotalRecords
                '_CfMJLineEn.mjjl_reference = BatchCode.Substring(4, (BatchCode.Length - 4))
                _CfMJLineEn.mjjl_reference = BatchCode
                'Set Values - Stop

                'Build Sql Statement - Start
                If _CfGeneric.BuildSqlStatement(_CfMJLineEn,
                    CfMjLineSql, CfGeneric.CfMjLineTbl) Then

                    'if posting not successful - Start
                    If Not ExecuteSqlStatement(CfMjLineSql) Then
                        Return False
                    End If
                    CfMjLineSql = String.Empty
                    'if posting not successful - Stop

                    'store at the sas db environment - start
                    SASCF_InsertDDLine(_CfMJLineEn, "Multiple Journal", "Credit", BatchCode)
                    'store at the sas db environment - end

                End If
                'Build Sql Statement - Stop

            End While
            'Loop thro the Data Reader - Stop

            Return True

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

            Return False

        End Try

    End Function

#End Region

#Region "Roll Back Multiple Journel "

    'Purpose			: To Roll Back Multiple Journal 
    'Author			    : Sujith Sharatchandran - T-Melmax Sdn Bhd
    'Created Date		: 01/06/2015
    Private Sub RollBackMultipleJournal(ByVal BatchCode As String, ByVal JournalNo As String)

        'Variable Declarations
        Dim SqlStatement As String = Nothing

        Try

            'Build Delete Sql Statement for Multiple Journal - Start
            SqlStatement = "DELETE FROM " & CfGeneric.CfMjLineTbl & " WHERE "
            SqlStatement &= "mjjl_jnl = " & clsGeneric.AddQuotes(JournalNo) & ";"
            SqlStatement &= " DELETE FROM " & CfGeneric.CfMjHeaderTbl & " WHERE "
            SqlStatement &= " mjjh_jnl = " & clsGeneric.AddQuotes(JournalNo) & ";"
            'Build Delete Sql Statement for Multiple Journal - Stop

            'Execute Sql Statement
            Call ExecuteSqlStatement(SqlStatement)

        Catch ex As Exception

            'log error
            Call Helper.LogError(ex.Message)

        End Try

    End Sub

#End Region

#Region "MJGstDist"
    'added by Hafiz @ 25/11/2016

    Public Function MJGstDist(ByVal _Module As String, ByVal BatchCode As String, ByVal CompanyCode As String, ByVal ReferenceNo As String) As Boolean

        Dim _CfGstDistEn As New CfGstDistEn(), _DataReader As IDataReader = Nothing
        Dim SqlStatement As String = Nothing, Desc As String = Nothing, ConstSql As String = Nothing, JoinSql As String = Nothing

        Try
            SqlStatement = "SELECT ROW_NUMBER() OVER() AS Row_No, CASE WHEN SAS_Accounts.Category='Refund' THEN SAS_Accounts.Voucherno "
            SqlStatement &= "WHEN SAS_Accounts.Category='Receipt' THEN "
            SqlStatement &= "CASE WHEN SAS_Accounts.Transcode = '' THEN SUBSTRING(SAS_Accounts.Transtempcode FROM 2 FOR Length(SAS_Accounts.Transtempcode)) "
            SqlStatement &= "ELSE SAS_Accounts.Transcode END "
            SqlStatement &= "WHEN SAS_Accounts.SubType='Sponsor' OR SAS_Accounts.Category='Loan' THEN CASE WHEN SAS_Accounts.Transcode = '' THEN "
            SqlStatement &= "SUBSTRING(SAS_Accounts.Transtempcode FROM 2 FOR Length(SAS_Accounts.Transtempcode)) "
            SqlStatement &= "ELSE SAS_Accounts.Transcode END "
            SqlStatement &= "ELSE CASE WHEN SAS_Accountsdetails.Transcode = '' THEN "
            SqlStatement &= "SUBSTRING(SAS_Accountsdetails.Transtempcode FROM 2 FOR Length(SAS_Accountsdetails.Transtempcode)) ELSE SAS_Accountsdetails.Transcode "
            SqlStatement &= "END END AS Transcode, SAS_Accounts.Description AS Description, "
            SqlStatement &= "CASE WHEN SAS_Accounts.Category = 'Receipt' OR SAS_Accounts.Category='Refund' "
            SqlStatement &= "OR SAS_Accounts.SubType='Sponsor' OR SAS_Accounts.Category='Loan' THEN SUM(SAS_Accounts.Transamount) "
            SqlStatement &= "ELSE SUM(SAS_Accountsdetails.Transamount) END AS Transamount,"
            SqlStatement &= "CASE WHEN SAS_Accounts.SubType='Sponsor' THEN COALESCE(SAS_Accounts.TaxCode,'ES') "
            SqlStatement &= "ELSE CASE WHEN SAS_Accounts.Category='Receipt' THEN 'ES' "
            SqlStatement &= "ELSE COALESCE(SAS_Accountsdetails.TaxCode,'ES') END END AS Taxcode,"
            SqlStatement &= "CASE WHEN SAS_Accounts.SubType='Sponsor' THEN COALESCE(SAS_Accounts.TaxAmount,0.0) "
            SqlStatement &= "ELSE COALESCE(SAS_Accountsdetails.TaxAmount,0.0) END AS Taxamount "
            SqlStatement &= "FROM SAS_Accounts "
            SqlStatement &= "LEFT JOIN SAS_Accountsdetails ON SAS_Accounts.Transid = SAS_Accountsdetails.Transid "
            SqlStatement &= "WHERE SAS_Accounts.Batchcode = " & clsGeneric.AddQuotes(BatchCode) & " "
            SqlStatement &= "AND SAS_Accounts.Category NOT IN ('SPA','STA') "
            SqlStatement &= "GROUP BY SAS_Accounts.Voucherno,SAS_Accounts.Transcode,SAS_Accountsdetails.Transcode,SAS_Accounts.Transtempcode,SAS_Accountsdetails.Transtempcode,SAS_Accounts.Description,"
            SqlStatement &= "SAS_Accounts.Category,SAS_Accounts.SubType,SAS_Accounts.TaxCode,SAS_Accountsdetails.TaxCode,SAS_Accounts.Taxamount,SAS_Accountsdetails.Taxamount"

            _DataReader = _DataBaseProvider.ExecuteReader(Helper.GetDataBaseType, Helper.GetConnectionString, SqlStatement).CreateDataReader()

            While _DataReader.Read

                _CfGstDistEn.utgd_company = CompanyCode
                _CfGstDistEn.utgd_module = _Module
                _CfGstDistEn.utgd_itemtype = "CSH"
                _CfGstDistEn.utgd_batchid = ReferenceNo
                _CfGstDistEn.utgd_code = ReferenceNo
                '_CfGstDistEn.utgd_itemref = _DataReader("Transcode")
                _CfGstDistEn.utgd_itemref = _DataReader(Helper.LineNoCol)
                _CfGstDistEn.utgd_seqno = _DataReader(Helper.LineNoCol)
                _CfGstDistEn.utgd_desc = _DataReader("Description")
                _CfGstDistEn.utgd_qty = 1.0
                _CfGstDistEn.utgd_taxtype = "GST-SUPP"
                _CfGstDistEn.utgd_taxcode = IIf(Not IsDBNull(_DataReader("Taxcode")), _DataReader("Taxcode"), String.Empty)
                _CfGstDistEn.utgd_amount = _DataReader("Transamount")
                _CfGstDistEn.utgd_gstamount = IIf(Not IsDBNull(_DataReader("Taxamount")), _DataReader("Taxamount"), 0.0)
                _CfGstDistEn.utgd_batchdate = CfGeneric.DateConversion(DateTime.Now)
                _CfGstDistEn.utgd_invopenamt = 0.0
                _CfGstDistEn.utgd_curcode = "MYR"
                _CfGstDistEn.utgd_leqfor = "000"
                _CfGstDistEn.utgd_currate = 1.0
                _CfGstDistEn.utgd_lclamount = _DataReader("Transamount")
                _CfGstDistEn.utgd_invpaid = "N"
                _CfGstDistEn.utgd_gstpaid = "N"
                _CfGstDistEn.utgd_gstclaim = "N"
                _CfGstDistEn.utgd_posted = "N"

                If _CfGeneric.BuildSqlStatement(_CfGstDistEn, ConstSql, CfGeneric.CfGSTDist) Then
                    If Not ExecuteSqlStatement(ConstSql) Then
                        Return False
                    End If
                End If
                ConstSql = String.Empty

            End While

            Return True

        Catch ex As Exception
            Call Helper.LogError(ex.Message)
            Return False
        End Try

    End Function

#End Region

#Region "SASCF_InsertDDLine"
    'added by Hafiz @ 28/11/2016
    'insert Debit Line into SAS DB environment

    Public Sub SASCF_InsertDDLine(ByVal obj As Object, ByVal Posting_Type As String, ByVal Trans_Type As String, Optional ByVal BatchCode As String = Nothing,
                                  Optional ByVal MatricNo As String = Nothing)

        Dim _SASCfDoubleEntryEn As New SASCfDoubleEntryEn()
        Dim SqlStatement As String = Nothing, count As Integer = 0, sql As String = Nothing

        If Posting_Type = "Multiple Journal" Then

            Dim argEn As CfMjLineEn = CType(obj, CfMjLineEn)

            SqlStatement = "SELECT COUNT(*) AS count FROM SAS_CF_DoubleEntry WHERE posting_type = 'Multiple Journal' AND cf_batch_no = '" + argEn.mjjl_jnl + "' "
            SqlStatement &= "AND gl_account = '" + argEn.mjjl_account + "' AND seq_no  = " & argEn.mjjl_lineno

            If Trans_Type = "Debit" Then
                _SASCfDoubleEntryEn.debit_amount = argEn.mjjl_amount
                _SASCfDoubleEntryEn.credit_amount = 0.0
            ElseIf Trans_Type = "Credit" Then
                _SASCfDoubleEntryEn.debit_amount = 0.0
                _SASCfDoubleEntryEn.credit_amount = argEn.mjjl_amount
            End If

            If Not String.IsNullOrEmpty(BatchCode) Then
                _SASCfDoubleEntryEn.reference_no = BatchCode
            End If

            _SASCfDoubleEntryEn.company_code = argEn.mjjl_company
            _SASCfDoubleEntryEn.cf_batch_no = argEn.mjjl_jnl
            _SASCfDoubleEntryEn.gl_account = argEn.mjjl_account
            _SASCfDoubleEntryEn.gl_description = argEn.mjjl_desc
            _SASCfDoubleEntryEn.posting_type = "Multiple Journal"
            _SASCfDoubleEntryEn.seq_no = argEn.mjjl_lineno
            _SASCfDoubleEntryEn.matric_no = ""

        ElseIf Posting_Type = "CashBook Receipt" Or Posting_Type = "CashBook Payment" Then

            Dim argEn As CfCbGlDistEn = CType(obj, CfCbGlDistEn)

            SqlStatement = "SELECT COUNT(*) AS count FROM SAS_CF_DoubleEntry WHERE posting_type = '" + Posting_Type + "' "
            SqlStatement &= "AND cf_batch_no = '" + argEn.gldi_batchid + "' AND gl_account = '" + argEn.gldi_glac + "' AND matric_no = '" + argEn.gldi_itemref + "' "
            SqlStatement &= "AND seq_no = " & argEn.gldi_seqno

            If Trans_Type = "Debit" Then
                _SASCfDoubleEntryEn.debit_amount = argEn.gldi_amount
                _SASCfDoubleEntryEn.credit_amount = 0.0
            ElseIf Trans_Type = "Credit" Then
                _SASCfDoubleEntryEn.debit_amount = 0.0
                _SASCfDoubleEntryEn.credit_amount = argEn.gldi_amount
            End If

            If Not String.IsNullOrEmpty(BatchCode) Then
                _SASCfDoubleEntryEn.reference_no = BatchCode
            End If

            _SASCfDoubleEntryEn.company_code = argEn.gldi_company
            _SASCfDoubleEntryEn.cf_batch_no = argEn.gldi_batchid
            _SASCfDoubleEntryEn.gl_account = argEn.gldi_glac
            _SASCfDoubleEntryEn.gl_description = argEn.gldi_desc
            _SASCfDoubleEntryEn.posting_type = Posting_Type
            _SASCfDoubleEntryEn.seq_no = argEn.gldi_seqno
            _SASCfDoubleEntryEn.matric_no = IIf(String.IsNullOrEmpty(MatricNo), "", MatricNo)

        End If

        Using _DataReader As IDataReader = _DataBaseProvider.ExecuteReader(Helper.GetDataBaseType, Helper.GetConnectionString, SqlStatement).CreateDataReader()

            Try
                While _DataReader.Read
                    count = _DataReader("count")

                    If count > 0 Then
                        Throw New Exception("Record Already Existed.")
                    Else

                        If _CfGeneric.BuildSqlStatement(_SASCfDoubleEntryEn, sql, CfGeneric.SASCfDoubleEntryTbl) Then
                            If Not ExecuteSql(sql) Then
                                Throw New Exception("Insert Failed.")
                            End If
                            sql = String.Empty
                        End If

                    End If

                End While

                _DataReader.Close()

            Catch ex As Exception
                Call Helper.LogError(ex.Message)
            End Try

        End Using

    End Sub

#End Region

End Class