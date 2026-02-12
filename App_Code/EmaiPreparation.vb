Public Module EmaiPreparation
    Sub SendEmail(ByVal lcEmailtype As Emailtype,
                 ByVal DealRef As String,
                 ByVal DealTbl As String,
                 ByVal STATEID As String,
                 ByVal STATEDESC As String,
                 ByVal Restriction As String)

        'Decide Email-Template, and Email-Title according to Email Type ==================
        Dim EmailTemplate As String = ""
        Dim EmaiTitle As String = ""
        If lcEmailtype = Emailtype.RequestingAction Then
            EmailTemplate = "EmailNotification"
            EmaiTitle = "Tresury Deal No. " & DealRef & " is waiting your action"
        ElseIf lcEmailtype = Emailtype.EmailTracking Then
            EmailTemplate = "EmailTracking"
            EmaiTitle = "FYI: Tresury Deal No. " & DealRef & " is in [" & STATEDESC & "]"
        End If
        '=================================================================================

        'get Sender ID and Name ==========================================================
        Dim FromEmail As String = getFromEmail(PF.GetCurrentUser)
        Dim LastActor As String = getUserName(PF.GetCurrentUser)
        '=================================================================================

        Dim EmailBody As String = prepareEmailBody(EmailTemplate, DealRef, DealTbl, STATEDESC, LastActor)

        Dim ToEmails As New Collection
        ToEmails = getToEmails(STATEID, Restriction, lcEmailtype)

        SendEmailClass.SendingSMTPEmails(FromEmail, ToEmails, EmaiTitle, EmailBody)
    End Sub

    Function getFromEmail(ByVal CurrentUser As String) As String
        Dim SQL As String = String.Empty
        SQL = SQL + "Select E.USER_EMAIL as USER_EMAIL from EBLINKs_users E "
        SQL = SQL + " where E.User_ID='" & CurrentUser & "'"

        getFromEmail = DB.RetreiveScalar(EBDB, SQL)
    End Function

    Function getToEmails(ByVal StateID As String,
                         ByVal Restriction As String,
                         ByVal lcEmailtype As Emailtype) As Collection
        Dim SQL As String = String.Empty

        SQL = SQL + " Select distinct E.USER_ID,E.USER_EMAIL As USER_EMAIL from EBLINKs_users E "
        SQL = SQL + " inner Join TRSRY_STATES STT on STT.@EMAILTYPE Like '%' || E.USER_ID || '%' "
        SQL = SQL + " where STT.STATE = '" & StateID & "'"


        If lcEmailtype = Emailtype.RequestingAction Then
            'extract all users who can do an action in the deal

            SQL = SQL.Replace("@EMAILTYPE", "FYA")

            If Restriction.Trim <> "*" Then
                Restriction = Restriction.Replace("|", "','")
                SQL = SQL + " And E.USER_ID in ('" & Restriction & "')"
            End If


        ElseIf lcEmailtype = Emailtype.EmailTracking Then
            'extract all users who are in (Email Tracking) and not (can do an action)..because those who can do an action already received an email 
            'SQL = SQL + " Select distinct E.USER_ID,E.USER_EMAIL As USER_EMAIL from EBLINKs_users E "
            'SQL = SQL + " inner Join TRSRY_STATES STT on STT.FYI Like '%' || E.USER_ID || '%' "
            'SQL = SQL + " where STT.STATE = '" & StateID & "'"

            SQL = SQL.Replace("@EMAILTYPE", "FYI")

            'SQL = SQL + " And E.USER_ID Not in (" & getSQLofThoseInvolvedAndReceiveEmails(False, Restriction, StateID) & ") "
        End If


        Dim xDT As New Data.DataTable
        xDT = GetDataTable(EBDB, SQL)


        Dim cl As New Collection
        cl.Add("Jalal.Hasan@eskanbank.com")
        For Each DR As Data.DataRow In xDT.Rows
            If cl.Contains(DR("USER_EMAIL").ToString.Trim) = False Then
                cl.Add(DR("USER_EMAIL").ToString.Trim)
            End If
        Next

        getToEmails = cl
    End Function

    Function getSQLofThoseInvolvedAndReceiveEmails(ByVal includeEmails As Boolean,
                                                   ByVal Restriction As String,
                                                   ByVal StateID As String) As String
        Dim SQL As String = ""

        SQL = SQL + " Select distinct T.USERID from TRSRY_STATESACTIONSUSER T "
        SQL = SQL + " inner Join TRSRY_STATES S on S.FYA Like '%' || T.USERID || '%' "
        SQL = SQL + " where S.STATE = '" & StateID & "' and S.STATE='" & StateID & "' "

        If Restriction.Trim <> "*" Then
            Restriction = Restriction.Replace("|", "','")
            SQL = SQL + " And T.USERID in ('" & Restriction & "')"
        End If

        If includeEmails = True Then
            SQL = " Select distinct E.USER_ID,E.USER_EMAIL As USER_EMAIL from EBLINKs_users E where E.USER_ID In (" & SQL & ")"
        Else

        End If

        getSQLofThoseInvolvedAndReceiveEmails = SQL

    End Function
    Function GetFXTrans(ByVal DealRef As String) As Data.DataTable

        Dim SQL As String = String.Empty
        SQL = SQL + vbCrLf + "Select "
        SQL = SQL + vbCrLf + " MVMM_BEN_NAME, "
        SQL = SQL + vbCrLf + " MVMM_DEAL_NUM_3 as DEAL_REF,"
        SQL = SQL + vbCrLf + " MVMM_VALUE_DATE as VALUE_DATE,"
        SQL = SQL + vbCrLf + " MVMM_MATURITY_DATE as MATURITY_DATE,"
        SQL = SQL + vbCrLf + " MVMM_INT_RATE as INT_RATE,"
        SQL = SQL + vbCrLf + " MVMM_OPER_DATE as OPER_DATE,"
        SQL = SQL + vbCrLf + " MVMM_AMNT as AMNT,"
        SQL = SQL + vbCrLf + " MVMM_INTEREST as INTEREST,"
        SQL = SQL + vbCrLf + " MVMM_NBR_OF_DAYS as NBR_OF_DAYS, "
        SQL = SQL + vbCrLf + " MVMM_AMNT+MVMM_INTEREST as AMOUNTATMATURITYDATE, "
        SQL = SQL + vbCrLf + " MM.USER_ID, EB1.USER_NAME as MadeBy,"
        SQL = SQL + vbCrLf + " CURR.CURR_ISO as CurrencyName,"
        SQL = SQL + vbCrLf + " EXCHANGE_RATE, "
        SQL = SQL + vbCrLf + " AMOUNTSALE "
        SQL = SQL + vbCrLf + " from TRSRY_DEALS MM "
        SQL = SQL + vbCrLf + " inner join EBLINKS_USERS EB1 On MM.INSERTEDBY  Like '%' || EB1.USER_ID || '%' "
        SQL = SQL + vbCrLf + " inner join BBsc_currencies@" & PV.EBDBtoICBS & " CURR on CURR.CURR_CODE=MM.CURR_CODE "
        SQL = SQL + vbCrLf + " where MVMM_DEAL_NUM_3=" & DealRef & " and is_FX='Y'"

        Dim xDT As New Data.DataTable
        xDT = DT.DataTables.GetDataTable(EBDB, SQL)

        GetFXTrans = xDT
    End Function

    Function GetMMTrans(ByVal DealRef As String,
                        ByVal DealTbl As String) As Data.DataTable
        Dim SQL As String = String.Empty
        SQL = SQL + "Select CUS.CUST_ID, CUS.CUST_B_NAME,PRPS.BBTP_B_DESC, LMT.CPLM_BB_EXPOSURE_LMT_AMNT, LMT.CPLM_PB_EXPOSURE_LMT_AMNT , "
        SQL = SQL + " MVMM_DEAL_NUM_3 as DEAL_REF,"
        SQL = SQL + " MVMM_VALUE_DATE as VALUE_DATE,"
        SQL = SQL + " MVMM_MATURITY_DATE as MATURITY_DATE,"
        SQL = SQL + " MVMM_INT_RATE as INT_RATE,"
        SQL = SQL + " MVMM_OPER_DATE as OPER_DATE,"
        SQL = SQL + " MVMM_AMNT as AMNT,"
        SQL = SQL + " MVMM_INTEREST as INTEREST,"
        SQL = SQL + " MVMM_NBR_OF_DAYS as NBR_OF_DAYS, "
        SQL = SQL + " MVMM_AMNT+MVMM_INTEREST as AMOUNTATMATURITYDATE, "
        SQL = SQL + " MM.USER_ID, EB1.USER_NAME as MadeBy "
        SQL = SQL + " from BIMM_MV_MONEY_MARKET MM "
        SQL = SQL + " inner join EBLINKS_USERS@" & PV.ICBStoEBDB & " EB1 On MM.USER_ID  Like '%' || EB1.USER_ID || '%' "
        SQL = SQL + " inner join BBSD_CUSTOMERs CUS on MM.CUST_ID = CUS.CUST_ID "
        SQL = SQL + " inner join BBSD_TREASURY_PURPOSES PRPS on MM.BBTP_CODE = PRPS.BBTP_CODE "
        SQL = SQL + " inner join BBSD_COUNTERPARTY_LIMITS LMT on MM.CUST_ID = LMT.CUST_ID "
        SQL = SQL + " where MVMM_DEAL_NUM_3=" & DealRef

        If DealTbl = "MV" Then
            SQL = SQL.Replace("BIMM_HS_", "BIMM_MV_")
            SQL = SQL.Replace("MSMM_", "MVMM_")
        ElseIf DealTbl = "HS" Then
            SQL = SQL.Replace("BIMM_MV_", "BIMM_HS_")
            SQL = SQL.Replace("MVMM_", "HSMM_")
        End If

        Dim xDT As New Data.DataTable
        xDT = DT.DataTables.GetDataTable(ICBS, SQL)
        GetMMTrans = xDT
    End Function

    Function prepareEmailBody(
                  ByVal EmailTemplate As String,
                  ByVal DealRef As String,
                  ByVal DealTbl As String,
                  ByVal STATEDESC As String,
                  ByVal LastActor As String) As String

        Dim xDT As New Data.DataTable
        Dim allContent As String

        Select Case DealTbl
            Case "FX"
                xDT = GetFXTrans(DealRef:=DealRef)
                allContent = IO.File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory & "\EmailsTemplate\" & EmailTemplate & "FX.html")
            Case "MV", "HS"
                xDT = GetMMTrans(DealTbl:=DealTbl, DealRef:=DealRef)
                allContent = IO.File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory & "\EmailsTemplate\" & EmailTemplate & ".html")
            Case Else
        End Select





        Dim AllContentArr As String()
        AllContentArr = Split(allContent, "@")

        Dim Dic As New Dictionary(Of String, String)
        For Each DC As Data.DataColumn In xDT.Columns
            Dic.Add(DC.ColumnName, xDT.Rows(0)(DC.ColumnName).ToString)
        Next
        Dic.Add("STAGE", STATEDESC)
        Dic.Add("ServerName", DB.Get_Server_Name())
        Dic.Add("LastActionBy", LastActor)



        For I As Integer = 1 To AllContentArr.Length - 1
            Dim Str As String = AllContentArr(I)
            Dim Key As String
            Key = Str.Substring(0, InStr(Str, "|") - 1).Trim
            Dim lcFormat As String = Str.Substring(InStr(Str, "|"), -1 + InStr(Str, "#") - InStr(Str, "|")).Trim
            'MsgBox(Key & vbTab & lcFormat & vbTab & Dic(Key))

            Select Case lcFormat.ToUpper

                Case "Currency".ToUpper

                    AllContentArr(I) = AllContentArr(I).Replace(Key & "|" & lcFormat & "#", CDbl(Dic(Key)).ToString("###,###,###.###").Trim)
                Case "Integer3digits".ToUpper
                    AllContentArr(I) = AllContentArr(I).Replace(Key & "|" & lcFormat & "#", CInt(Dic(Key)).ToString("000").Trim)
                Case "Integer".ToUpper
                    AllContentArr(I) = AllContentArr(I).Replace(Key & "|" & lcFormat & "#", CInt(Dic(Key)).ToString.Trim)
                Case "Date".ToUpper
                    AllContentArr(I) = AllContentArr(I).Replace(Key & "|" & lcFormat & "#", CDate(Dic(Key)).ToString("yyyy-MMM-dd").Trim)
                Case "Percentage".ToUpper
                    AllContentArr(I) = AllContentArr(I).Replace(Key & "|" & lcFormat & "#", CDbl(Dic(Key)) & "%".Trim)
                Case "text".ToUpper
                    AllContentArr(I) = AllContentArr(I).Replace(Key & "|" & lcFormat & "#", Dic(Key).Trim)

            End Select


        Next

        prepareEmailBody = Join(AllContentArr, "")

        IO.File.WriteAllText("\\HQPROFILES\HQUP$\2271\Desktop\EmailBody.html", prepareEmailBody)
    End Function
End Module
