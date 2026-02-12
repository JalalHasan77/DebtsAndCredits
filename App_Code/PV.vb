Public Module PV 'Public Values

    Dim _SQL As String

    Private glSelectedAuthorizer As New Dictionary(Of String, String)
    Private glSelectedCounterParty As New Dictionary(Of String, String())
    Private glSelectedAuthorizerName As New Dictionary(Of String, String)
    Private glGridViewPageIndex As New Dictionary(Of String, String)
    Private glDialogueResult As New Dictionary(Of String, String)
    Private glDealers As New Dictionary(Of String, String())
    Public Property ActiveSessionID As New GLOBALVARIABLES

    Public Property WorkAs As String

    Public Property CounterParty(ByVal WindowsUser As String) As String()
        Set(value As String())
            If glSelectedCounterParty.ContainsKey(WindowsUser) Then
                glSelectedCounterParty.Remove(WindowsUser)
            End If
            glSelectedCounterParty.Add(WindowsUser, value)
        End Set
        Get
            If glSelectedCounterParty.ContainsKey(WindowsUser) Then
                If glSelectedCounterParty(WindowsUser) Is Nothing Then
                    Return Nothing
                Else
                    Return glSelectedCounterParty(WindowsUser)
                End If
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public Property Dealers(ByVal WindowsUser As String) As String()
        Set(value As String())
            If glDealers.ContainsKey(WindowsUser) Then
                glDealers.Remove(WindowsUser)
            End If
            glDealers.Add(WindowsUser, value)
        End Set
        Get
            If glDealers.ContainsKey(WindowsUser) Then
                If glDealers(WindowsUser) Is Nothing Then
                    Return Nothing
                Else
                    Return glDealers(WindowsUser)
                End If
            Else
                Return Nothing
            End If
        End Get
    End Property


    Public Class GLOBALVARIABLES
        Private localStorage As New Dictionary(Of String, String)
        Default Public Property privateVAriable(ByVal WindowsUser As String) As String
            Set(value As String)

                If localStorage.ContainsKey(WindowsUser) Then
                    localStorage.Remove(WindowsUser)
                End If

                localStorage.Add(WindowsUser, value)

            End Set
            Get
                If localStorage.ContainsKey(WindowsUser) Then
                    If localStorage(WindowsUser) = "" Or localStorage(WindowsUser) Is Nothing Then
                        Return Nothing
                    Else
                        Return localStorage(WindowsUser)
                    End If
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Sub RemoveItem(ByVal WindowsUser As String)
            localStorage.Remove(WindowsUser)
        End Sub
    End Class

    Public Property DialogueResult(ByVal WindowsUser As String) As String
        Set(value As String)

            If glDialogueResult.ContainsKey(WindowsUser) Then
                glDialogueResult.Remove(WindowsUser)
            End If

            glDialogueResult.Add(WindowsUser, value)

        End Set
        Get
            If glDialogueResult.ContainsKey(WindowsUser) Then
                If glDialogueResult(WindowsUser) = "" Or glDialogueResult(WindowsUser) Is Nothing Then
                    Return Nothing
                Else
                    Return glDialogueResult(WindowsUser)
                End If
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public Property Authorizer(ByVal WindowsUser As String) As String
        Set(value As String)
            If glSelectedAuthorizer.ContainsKey(WindowsUser) Then
                glSelectedAuthorizer.Remove(WindowsUser)
            End If

            glSelectedAuthorizer.Add(WindowsUser, value)
        End Set
        Get
            If glSelectedAuthorizer.ContainsKey(WindowsUser) Then
                If glSelectedAuthorizer(WindowsUser) = "" Or glSelectedAuthorizer(WindowsUser) Is Nothing Then
                    Return Nothing
                Else
                    Return glSelectedAuthorizer(WindowsUser)
                End If
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public Property AuthorizerName(ByVal WindowsUser As String) As String
        Set(value As String)

            If glSelectedAuthorizerName.ContainsKey(WindowsUser) Then
                glSelectedAuthorizerName.Remove(WindowsUser)
            End If

            glSelectedAuthorizerName.Add(WindowsUser, value)

        End Set
        Get
            If glSelectedAuthorizerName.ContainsKey(WindowsUser) Then
                If glSelectedAuthorizerName(WindowsUser) = "" Or glSelectedAuthorizerName(WindowsUser) Is Nothing Then
                    Return Nothing
                Else
                    Return glSelectedAuthorizerName(WindowsUser)
                End If
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public Property GridViewPageIndex(ByVal WindowsUser As String) As String
        Set(value As String)

            If glGridViewPageIndex.ContainsKey(WindowsUser) Then
                glGridViewPageIndex.Remove(WindowsUser)
            End If

            glGridViewPageIndex.Add(WindowsUser, value)

        End Set
        Get
            If glGridViewPageIndex.ContainsKey(WindowsUser) Then
                If glGridViewPageIndex(WindowsUser) = "" Or glGridViewPageIndex(WindowsUser) Is Nothing Then
                    Return Nothing
                Else
                    Return glGridViewPageIndex(WindowsUser)
                End If
            Else
                Return Nothing
            End If
        End Get
    End Property
    Sub New()
        Dim SQL As String = String.Empty

        SQL = SQL + "Select Upper(CUS.CUST_B_NAME) as CUST_B_NAME,PRPS.BBTP_B_DESC, SUBSTR(PRPS.BBTP_B_DESC, -3, 3) || '-' || @TablePrefix_DEAL_NUM_3 as DealRef, "
        SQL = SQL + "  LMT.CPLM_BB_EXPOSURE_LMT_AMNT, LMT.CPLM_PB_EXPOSURE_LMT_AMNT , "
        SQL = SQL + "  @TablePrefix_AMNT as AMNT,"
        SQL = SQL + "  @TablePrefix_INT_RATE as INT_RATE,"
        SQL = SQL + "  @TablePrefix_INTEREST as INTEREST,"
        SQL = SQL + "  @TablePrefix_VALUE_DATE as VALUE_DATE, "
        SQL = SQL + "  @TablePrefix_MATURITY_DATE as MATURITY_DATE,"
        SQL = SQL + "  @TablePrefix_OPER_DATE as OPER_DATE,"
        SQL = SQL + "  @TablePrefix_DEAL_NUM_3 as DEAL_NUM_3 ,"
        SQL = SQL + " '@TableAlias' as xTable,"
        SQL = SQL + " @StatusValue as STATE,"
        SQL = SQL + " @StatusDesc as STATEDesc,"
        SQL = SQL + " '@isMarkedInMV' as isMarkedInMV,"
        SQL = SQL + " @color as BoxColor, "
        SQL = SQL + " '*' as RESTRICTEDTO,"
        SQL = SQL + " 'N' as ISDISABLED, "
        SQL = SQL + " (@TablePrefix_INTEREST + @TablePrefix_AMNT) as AmountatMaturity "
        SQL = SQL + " from @TableName MM "
        SQL = SQL + " inner join BBSD_CUSTOMERs CUS on MM.CUST_ID = CUS.CUST_ID "
        SQL = SQL + " inner join BBSD_TREASURY_PURPOSES PRPS on MM.BBTP_CODE = PRPS.BBTP_CODE "
        SQL = SQL + " inner join BBSD_COUNTERPARTY_LIMITS LMT on MM.CUST_ID = LMT.CUST_ID "
        SQL = SQL + " @JoinToTRSRY_DEALS "

        _SQL = SQL


        WorkAs = "2271"
    End Sub
    Public ReadOnly Property DealListSQL(ByVal TablePrefix As String,
                                         ByVal TableAlias As String,
                                         ByVal StatusValue As String,
                                         ByVal TableName As String,
                                         ByVal isMarkedInMV As String,
                                         ByVal StatusDesc As String,
                                         ByVal JoinTable As String,
                                         ByVal lcColor As String) As String
        Get

            Dim tempSQL As String = String.Empty
            tempSQL = _SQL

            tempSQL = tempSQL.Replace("@TablePrefix", TablePrefix)
            tempSQL = tempSQL.Replace("@TableAlias", TableAlias)
            tempSQL = tempSQL.Replace("@StatusValue", StatusValue)
            tempSQL = tempSQL.Replace("@TableName", TableName)
            tempSQL = tempSQL.Replace("@isMarkedInMV", isMarkedInMV)
            tempSQL = tempSQL.Replace("@StatusDesc", StatusDesc)
            tempSQL = tempSQL.Replace("@JoinToTRSRY_DEALS", JoinTable)
            tempSQL = tempSQL.Replace("@color", lcColor)


            Return tempSQL
        End Get
    End Property


    Public ReadOnly Property EBDBtoICBS As String
        Get
            EBDBtoICBS = ""
            Dim lcServerName As String = Get_Server_Name()
            If InStr(UCase(Left(lcServerName, 20)), "DRWEB") > 0 Then
                Return "EBDBXE"
            End If
            If InStr(UCase(Left(lcServerName, 20)), "S0305") > 0 Then
                Return "LIV"
            End If
            If InStr(UCase(Left(lcServerName, 20)), "S0307") > 0 Then
                Return "LIV"
            End If
            If InStr(UCase(Left(lcServerName, 20)), "LOCALHOST") > 0 Then
                Return "TST1"
            End If
            'Return "EBDBXE"
        End Get

    End Property

    Public ReadOnly Property ICBStoEBDB As String
        Get

            ICBStoEBDB = ""
            Dim lcServerName As String = Get_Server_Name()
            If InStr(UCase(Left(lcServerName, 20)), "DRWEB") > 0 Then
                Return "EBDBXE"
            End If
            If InStr(UCase(Left(lcServerName, 20)), "S0305") > 0 Then
                Return "EBDBXE"
            End If
            If InStr(UCase(Left(lcServerName, 20)), "S0307") > 0 Then
                Return "EB"
            End If
            If InStr(UCase(Left(lcServerName, 20)), "LOCALHOST") > 0 Then
                Return "EB"
            End If
            'Return "EBDBXE"
        End Get


    End Property


End Module

Public Enum SQLtype
    Update
    Insert
End Enum

Public Enum Emailtype
    RequestingAction
    EmailTracking
End Enum

