Imports System.Data
Imports HttpServerUtility
Imports System.Globalization
Public Module PF

    Dim context1 As System.Web.HttpContext = System.Web.HttpContext.Current
    Dim Server_Name As String = context1.Request.ServerVariables("SERVER_NAME")

    Function TitleCase(ByVal Name As String) As String
        Dim myTI As TextInfo = New CultureInfo("en-US", False).TextInfo
        TitleCase = myTI.ToLower(Name)
        TitleCase = myTI.ToTitleCase(TitleCase)
    End Function


    Function GetWindowsUser() As String
        Try
            Dim extendedUserName As String = Web.HttpContext.Current.User.Identity.Name.ToString
            Dim lcUser = Split(extendedUserName, "\")(UBound(Split(extendedUserName, "\")))
            GetWindowsUser = lcUser
        Catch ex As Exception
            GetWindowsUser = ""
        End Try
    End Function

    Function getUserName(ByVal CurrentUser As String) As String
        Dim SQL As String = String.Empty
        SQL = SQL + "Select E.USER_Name as USER_NAME from EBLINKs_users E "
        SQL = SQL + " where E.User_ID='" & CurrentUser & "'"
        'System.IO.File.WriteAllText("C:\DFSRoots\webfarm\IntraApps\TRsystem\App_Code\AAA.txt", SQL, System.Text.Encoding.UTF8)
        getUserName = DB.RetreiveScalar(EBDB, SQL)
    End Function


    Sub BoundLinkButton(ByRef e As DataListItemEventArgs)
        Dim L As LinkButton

        L = e.Item.FindControl("lnkbtnFileName")
        Dim R As DataRowView
        R = CType(e.Item.DataItem, DataRowView)
        L.Text = R.Item("FileName").ToString
        L.Text = Mid(L.Text, 6) 'remove Deal ID and '-' from file name

        Dim FileName As String
        FileName = R.Item("FILENAME")

        SetOnClientClick(L, GetAppPath() + "/TR_FileViewer.aspx?FileName=" + FileName, 600, 800)
    End Sub

    Sub ImplementDeleting(sender As Object)
        Dim img As ImageButton
        'Dim DL As System.Web.UI.WebControls.DataListItem
        img = CType(sender, ImageButton)
        Dim ATTACHMENTNUMBER As String = img.Attributes("AttachmentNo")
        DB.ExecuteNonQuery(EBDB, "Delete from TRSRY_ATTACHMENTS_TEMP where ATTACHMENTNUMBER='" & ATTACHMENTNUMBER & "'")

        'Add Event Log===================================================
        Dim Action As String = img.Attributes("AttachmentNo") & "  is Deleted"
        'PF.SaveEventLog(lblUserID.Text, lblStationID.Text, Action, Session.SessionID)
        '================================================================
    End Sub



    Sub BoundImageButton(ByRef e As DataListItemEventArgs)
        Dim M As ImageButton
        M = e.Item.FindControl("imgBtnDelete")

        Dim R As DataRowView
        R = CType(e.Item.DataItem, DataRowView)

        M.Attributes.Add("AttachmentNo", R.Item("ATTACHMENTNUMBER").ToString)

        'Add File Name in Attribute to use it later (if deleted)=======================
        Dim FileName As String = R.Item("FILENAME")
        FileName = Mid(FileName, 6)

        M.Attributes.Add("FileName", FileName)
        '===============================================================================
    End Sub



    Sub ShowTemporaryAttachmen(ByRef DataList2 As DataList,
                               ByVal DealID As String,
                               ByVal UserID As String,
                               ByVal Table As String)

        Dim dtAttachments As Data.DataTable
        dtAttachments = DB.GetDataTable(EBDB, "select * from TRSRY_ATTACHMENTS_TEMP where Upper(DEALID) = '" & DealID & "' and USER_='" & UserID & "' and Is_FX='" & IIf(Table = "FX", "Y", "N") & "'")

        'DataList1.DataSource = dtAttachments.DefaultView
        DataList2.DataSource = dtAttachments.DefaultView

        'DataList1.DataBind()
        DataList2.DataBind()
    End Sub

    Function ImplementUploading(ByVal FU As FileUpload,
                                ByVal UserID As String,
                                ByVal DealID As String,
                                ByVal Table As String) As Boolean
        ImplementUploading = False
        If FU.HasFile Then
            Dim lcFileName As String = FU.FileName
            Dim lcImportedFiles As String = ImportFiles(FU, UserID, DealID)
            Add_Attachment_Details(Split(lcImportedFiles, vbCrLf)(0), UserID, DealID, TABLE:=Table)

            ImplementUploading = True
        End If
    End Function

    Function ImportFiles(ByVal FU As FileUpload, ByVal UserID As String, ByVal DealID As String) As String
        Dim lcImportedFiles As String = ""
        GetAttachedFile(FU, lcImportedFiles, DealID)
        ImportFiles = lcImportedFiles
    End Function
    Function GetAttachedFile(ByVal FU As FileUpload, ByRef lcFiles As String, ByVal DealID As String) As String
        GetAttachedFile = ""
        If FU.HasFile Then
            Try
                Dim lcFile As String = DealID + "-" + FU.FileName
                Dim lcTargetFile As String = HttpContext.Current.Server.MapPath(GetAppPath() + "/Attachments/" + lcFile)
                'MsgBox(lcTargetFile)
                FU.SaveAs(lcTargetFile)
                lcFiles = lcFiles + lcTargetFile + vbCrLf
            Catch ex As Exception

            End Try
        Else
            'lstErrors.Add("Nothing to Import, please Browse for the file you need to import then click on the Import button")
        End If
    End Function



    Function Add_Attachment_Details(ByVal lcFileName As String,
                                    ByVal UserID As String,
                                    ByVal DealID As String,
                                    ByVal TABLE As String)
        Dim lcTheFile As String = IO.Path.GetFileName(lcFileName)
        Dim lcType As String = IO.Path.GetExtension(lcFileName)
        Dim lcAttachment_Number As String = getAttachmentNextNumber()

        Dim mr As Dictionary(Of String, String)
        mr = New Dictionary(Of String, String)
        mr.Add("DEALID", DealID)
        mr.Add("ATTACHMENTNUMBER", lcAttachment_Number)
        mr.Add("DATE_", Now.Date)
        mr.Add("TIME_", Now.TimeOfDay.ToString)
        mr.Add("USER_", UserID)
        mr.Add("DOCUMENTTYPE", lcType)
        mr.Add("FILENAME", lcTheFile)
        mr.Add("DESCRIPTION", "")
        mr.Add("IS_FX", IIf(TABLE = "FX", "Y", "N"))

        DB.InsertRecord(EBDB, "TRSRY_ATTACHMENTS_TEMP", mr)

    End Function

    Function getAttachmentNextNumber() As String
        Dim nestedSQL As String = ""
        Dim SQL As String = ""
        nestedSQL = nestedSQL + "Select Max(TO_NUMBER(ATTACHMENTNUMBER)) as ATTACHMENTNUMBER from TRSRY_ATTACHMENTS_TEMP "
        nestedSQL = nestedSQL + " union Select Max(TO_NUMBER(ATTACHMENTNUMBER)) as ATTACHMENTNUMBER from TRSRY_ATTACHMENTS"
        SQL = "Select Max(ATTACHMENTNUMBER) from (" & nestedSQL & ")"
        Dim NextID As String
        NextID = CStr(DB.RetreiveScalarInteger(EBDB, SQL))

        If NextID = "" Then
            NextID = "0"
        Else
            NextID = CInt(NextID) + 1
        End If

        NextID = NextID.PadLeft(8, "0")

        getAttachmentNextNumber = NextID
    End Function





    Function GetCurrentPageTitle(ByVal Page As Page) As String
        GetCurrentPageTitle = System.IO.Path.GetFileName(Page.Request.ServerVariables("SCRIPT_NAME"))
    End Function

    Function hexNumber(ByVal lcColor As Drawing.Color) As String
        Dim R As Integer = lcColor.R
        Dim G As Integer = lcColor.G
        Dim B As Integer = lcColor.B

        Dim hxR As String = Hex(R).ToString.PadLeft(2, "0")
        Dim hxG As String = Hex(G).ToString.PadLeft(2, "0")
        Dim hxB As String = Hex(B).ToString.PadLeft(2, "0")


        hexNumber = "#" & hxR & hxG & hxB
    End Function

    Sub AssingValueToLabel(ByRef P As Page,
                           ByVal R As DataRow,
                           ByVal FieldName As String,
                           ByVal LabelID As String,
                           ByRef TextFormat As TextFormat)

        Dim L As New Label
        L = P.FindControl(LabelID)

        Dim Value As String = R(FieldName).ToString
        Dim Currency As String = ""

        If TextFormat = TextFormat.NumberFormat_W_CURR Then
            Currency = R("Currency").ToString
        ElseIf TextFormat = TextFormat.DateFormat Then
            ' L.Text = Cdat

        End If

        AssingValueToLabel(L, Value, TextFormat, Currency)

    End Sub


    Sub AssingValueToLabel(ByVal R As GridViewRow,
                           ByVal FieldName As String,
                           ByVal LabelID As String,
                           ByRef TextFormat As TextFormat)
        Dim L As New Label
        L = R.FindControl(LabelID)

        Dim Value As String = R.DataItem(FieldName).ToString
        Dim Currency As String = ""

        If TextFormat = TextFormat.NumberFormat_W_CURR Then
            Currency = R.DataItem("Currency").ToString
        End If

        AssingValueToLabel(L, Value, TextFormat, Currency)

    End Sub

    'Sub AssingValueToLabel(ByVal R As DataRow,
    '                       ByVal FieldName As String,
    '                       ByRef L As Label,
    '                       ByRef TextFormat As TextFormat)

    '    Dim Value As String = R(FieldName).ToString

    '    AssingValueToLabel(L, Value, TextFormat)

    'End Sub




    Sub AssingValueToLabel(ByVal R As GridViewRow,
                           ByVal FieldName As String,
                           ByVal LabelID As String)

        Dim L As New Label
        L = R.FindControl(LabelID)
        Dim Value As String = R.DataItem(FieldName).ToString

        AssingValueToLabel(L, Value, TextFormat.TextFormat, "")

    End Sub




    Sub AssingValueToLabel(ByRef L As Label,
                           ByVal Value As String,
                           ByVal TextFormat As TextFormat,
                           ByVal Currency As String)
        If Value = "" Then
            L.Text = Value
        ElseIf TextFormat = TextFormat.TextFormat Or TextFormat = TextFormat.asItIs Then
            L.Text = Value
        Else
            If TextFormat = TextFormat.DateFormat Then
                L.Text = CDate(Value.ToString).ToString("dd MMM yyyy")
            ElseIf TextFormat = TextFormat.NumberFormat Or TextFormat = TextFormat.NumberFormatWith3Digits Then
                L.Text = CDbl(Value.ToString).ToString("###,###,##0.000")
            ElseIf TextFormat = TextFormat.NumberFormatWith2Digits Then
                L.Text = CDbl(Value.ToString).ToString("###,###,##0.00")
            ElseIf TextFormat = TextFormat.NumberFormat_W_CURR Then
                L.Text = CDbl(Value.ToString).ToString("###,###,##0.000 ") & Currency
            End If
        End If
    End Sub



    Function GetCurrentUser() As String
        GetCurrentUser = GetWindowsUser()
        'GetCurrentUser = "2288" ' Alaa
        'GetCurrentUser = "2317" 'Parween
        'GetCurrentUser = "1133"
        'GetCurrentUser = "2271"
        'GetCurrentUser = "2346"
        'GetCurrentUser = "2240"
        'GetCurrentUser = "2491"


        'for testing purpose only..should be removed from live========
        If (Server_Name.ToUpper = "LOCALHOST" Or Server_Name.ToUpper = "S0307") And GetWindowsUser() = PV.WorkAs Then
            GetCurrentUser = IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory & "\DEAL-CurrentUser.txt")
        End If

    End Function

    Function getCommentNextNumber() As String

        Dim SQL As String = ""
        SQL = "Select Max(COMMENTNUMBER) from TRSRY_DEALCOMMENTS"
        Dim NextID As String
        NextID = DB.RetreiveScalarInteger(EBDB, SQL)

        If NextID = "" Then
            NextID = "0"
        Else
            NextID = CInt(NextID) + 1
        End If

        NextID = NextID.PadLeft(8, "0")

        getCommentNextNumber = NextID
    End Function


    Public Function ConvertToRbg(ByVal HexColor As String) As Drawing.Color
        Dim Red As String
        Dim Green As String
        Dim Blue As String
        HexColor = Replace(HexColor, "#", "")
        Red = Val("&H" & Mid(HexColor, 1, 2))
        Green = Val("&H" & Mid(HexColor, 3, 2))
        Blue = Val("&H" & Mid(HexColor, 5, 2))

        Return Drawing.Color.FromArgb(Red, Green, Blue)
    End Function


    Function GetUserGroups(ByVal UserID As String) As String
        Dim Groups As String =
        DB.RetreiveScalar(EBDB, "Select USER_GROUPS from TRSRY_USERS where ID='" & UserID & "'")
        GetUserGroups = Groups
    End Function
    Sub ConvertToDataTable(ByRef xDT As Data.DataTable, ByVal A() As Data.DataRow)
        If A.Count > 0 Then
            xDT = Nothing
            xDT = A.CopyToDataTable
        Else
            xDT.Rows.Clear()
        End If

    End Sub
    Sub FilterByFields(ByVal FieldName As String, ByVal VAlues As String, ByRef XDT As Data.DataTable)
        Dim A() As Data.DataRow
        A = XDT.Select(FieldName & " Like '%" & VAlues & "%'")
        ConvertToDataTable(XDT, A)
    End Sub

    Function GetWorkStation() As String
        GetWorkStation = "L0006"
    End Function

    Sub SaveEventLog(ByVal USERID As String,
                     ByVal WORKSTATION As String,
                     ByVal Action As String,
                     ByVal SessionId As String,
                     ByVal PageTitle As String)

        Dim Dic As Dictionary(Of String, String)
        Dic = New Dictionary(Of String, String)

        Dic.Add("USERID", USERID)
        Dic.Add("WORKSTATION", WORKSTATION)
        Dic.Add("Action", Action)
        Dic.Add("SessionId", SessionId)
        Dic.Add("DateTime", Now.ToString)
        Dic.Add("FORMTITLE", PageTitle)

        InsertRecord(EBDB, "TRSRY_EVENTSLOG", Dic)

        Dic = Nothing
    End Sub

    Sub SaveToHistory(ByVal DealRef As String,
                      ByVal USERID As String,
                      ByVal FromState As String,
                      ByVal ToState As String,
                      ByVal ActionID As String,
                      ByVal StationID As String,
                      ByVal StationIP As String,
                      ByVal COMMENTS As String,
                      ByVal Parameters As String,
                      ByVal MSGID As String,
                      ByVal isFX As String)

        Dim Dic As Dictionary(Of String, String)
        Dic = New Dictionary(Of String, String)

        Dic.Add("DEALREF", DealRef)
        Dic.Add("DATE_", Now.Date.ToString)
        Dic.Add("TIME_", Now.TimeOfDay.ToString)
        Dic.Add("USER_", USERID)
        Dic.Add("FROMSTATE", FromState)
        Dic.Add("ACTION", ActionID)
        Dic.Add("TOSTATE", ToState)
        Dic.Add("WSID", StationID)
        Dic.Add("WSIP", StationIP)
        Dic.Add("COMMENTS", COMMENTS)
        Dic.Add("PARAMETERS", Parameters)
        Dic.Add("COMMENTID", MSGID)
        Dic.Add("IS_FX", isFX)

        InsertRecord(EBDB, "TRSRY_HISTORY", Dic)

    End Sub

    Sub SaveToHistory(ByVal DealRef As String,
                      ByVal USERID As String,
                      ByVal FromState As String,
                      ByVal ToState As String,
                      ByVal ActionID As String,
                      ByVal StationID As String,
                      ByVal StationIP As String,
                      ByVal COMMENTS As String,
                      ByVal isFx As String)

        Dim Dic As Dictionary(Of String, String)
        Dic = New Dictionary(Of String, String)

        Dic.Add("DEALREF", DealRef)
        Dic.Add("DATE_", Now.Date.ToString)
        Dic.Add("TIME_", Now.TimeOfDay.ToString)
        Dic.Add("USER_", USERID)
        Dic.Add("FROMSTATE", FromState)
        Dic.Add("ACTION", ActionID)
        Dic.Add("TOSTATE", ToState)
        Dic.Add("WSID", StationID)
        Dic.Add("WSIP", StationIP)
        Dic.Add("COMMENTS", COMMENTS)
        Dic.Add("IS_FX", isFx)

        InsertRecord(EBDB, "TRSRY_HISTORY", Dic)
    End Sub

    Public Function getListOfApproverForRepaymentView(ByVal dlTable As String,
                                            ByVal DealRef As String) As Data.DataTable
        getListOfApproverForRepaymentView = getListOfApprover(
            dlTable:=dlTable,
            DealRef:=DealRef,
            FromState:="1000",
            ToState:="9999")
    End Function

    Public Function getListOfApproverForDealSlipView(ByVal dlTable As String,
                                            ByVal DealRef As String) As Data.DataTable

        getListOfApproverForDealSlipView = getListOfApprover(
            dlTable:=dlTable,
            DealRef:=DealRef,
            FromState:="0000",
            ToState:="1000")

    End Function

    Function ReturnStatus(ByVal DR As DataRow) As String
        Dim lcToday As String = Now.ToString("yyyyMMdd")
        If CInt(DR("STATE").ToString) > 600 Then
            If lcToday > CDate(DR("MATURITY_DATE")).ToString("yyyyMMdd") Then
                ReturnStatus = "Matured"
            ElseIf lcToday <= CDate(DR("MATURITY_DATE")).ToString("yyyyMMdd") And lcToday >= CDate(DR("VALUE_DATE")).ToString("yyyyMMdd") Then
                ReturnStatus = "Waiting to be Matured"
            ElseIf lcToday < CDate(DR("VALUE_DATE")).ToString("yyyyMMdd") Then
                ReturnStatus = "Waiting for Value Date"
            Else
                ReturnStatus = DR("STATE_DESCRIPTION").ToString
            End If
        Else
            ReturnStatus = DR("STATE_DESCRIPTION").ToString
        End If
    End Function

    Public Function getListOfApprover(ByVal dlTable As String,
                                            ByVal DealRef As String,
                                      ByVal FromState As String,
                                      ByVal ToState As String) As Data.DataTable
        Dim isFX As String = "N"
        If dlTable = "FX" Then
            isFX = "Y"
            FromState = "0000"
            ToState = "1000"
        End If
        '$get list of approvers ======================================================================================
        Dim SQL3 As String = ""
        SQL3 = SQL3 + vbCrLf + " Select "
        SQL3 = SQL3 + vbCrLf + " ACT.STATE,USR.USER_NAME, "
        SQL3 = SQL3 + vbCrLf + " fSTT.DESCRIPTION As FromState, "
        SQL3 = SQL3 + vbCrLf + " ACT.DESCRIPTION As Action, "
        SQL3 = SQL3 + vbCrLf + " ACT.IMPLEMENTER as IMPLEMENTER,"
        SQL3 = SQL3 + vbCrLf + " tSTT.DESCRIPTION As ToState, "
        'SQL3 = SQL3 + " '' as Participle, "
        SQL3 = SQL3 + vbCrLf + " HS.DATE_ ,   "
        SQL3 = SQL3 + vbCrLf + " HS.Time_, "
        SQL3 = SQL3 + vbCrLf + " HS.TOSTATE as TOSTATE1 "
        SQL3 = SQL3 + vbCrLf + " From TRSRY_HISTORY HS "
        SQL3 = SQL3 + vbCrLf + " inner Join  ( Select FROMSTATE || '#' || ACTION || '#' || TOSTATE || max(DATE_ || Time_) as UniqueKey   From TRSRY_HISTORY Where TRSRY_HISTORY.FROMSTATE < TOSTATE And "
        SQL3 = SQL3 + vbCrLf + " DealRef = '" & DealRef & "' and IS_FX='" & isFX & "' Group by FROMSTATE || '#' || ACTION || '#' || TOSTATE Order by max(DATE_ || Time_)) TBL  "
        SQL3 = SQL3 + vbCrLf + " On TBL.UniqueKey = HS.FROMSTATE || '#' || HS.ACTION || '#' || HS.TOSTATE || HS.DATE_ || HS.Time_  "
        SQL3 = SQL3 + vbCrLf + " inner Join EBLINKS_USERS USR on HS.USER_ = USR.USER_ID  inner join TRSRY_STATESACTIONS ACT on HS.FROMSTATE = ACT.STATE  And HS.ACTION = ACT.ACTION   "
        SQL3 = SQL3 + vbCrLf + " inner join TRSRY_STATES fSTT on HS.FROMSTATE = fSTT.STATE  left join TRSRY_STATES tSTT on HS.TOSTATE = tSTT.STATE   where HS.FROMSTATE >='" & FromState & "' and HS.TOSTATE <='" & ToState & "'  "
        SQL3 = SQL3 + vbCrLf + " order by ACT.STATE asc "

        Dim DT3 As New DataTable
        DT3 = GetDataTable(EBDB, SQL3)
        DT3.TableName = "ApprovingDetails"


        For Each xDR1 As DataRow In DT3.Rows
            Dim lcDate As New Date
            lcDate = CDate(xDR1("DATE_"))
            xDR1("DATE_") = lcDate.ToString("d MMM yyyy")

            Dim lcTime As New Date
            lcTime = CDate(xDR1("Time_"))
            xDR1("Time_") = lcTime.ToString("HH:mm:ss")
        Next

        getListOfApprover = DT3
    End Function

    Public Enum TextFormat
        DateFormat = 0 '"yyyy MMM dd"
        NumberFormat = 1 '"###,###,###.###"
        TextFormat = 2
        NumberFormat_W_CURR = 3
        asItIs = 4
        NumberFormatWith2Digits = 5 '"###,###,###.##"
        NumberFormatWith3Digits = 1 '"###,###,###.###"

        NumberFormat_W_BHD = 6
        NumberFormat_W_USD = 7
    End Enum

End Module
