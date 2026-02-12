Public Module OpenForms
    Public Function SetOnClientClick(bt As Web.UI.WebControls.Button, ByVal lcUrl As String, Optional ByVal lnHeight As Integer = 600, Optional ByVal lnWidth As Integer = 800, Optional lcTitle As String = "", Optional SidePannel As Boolean = False) As String
        'lcUrl = lcUrl.Replace("/\", "/")
        'lcUrl = lcUrl.Replace("\/", "/")
        'lcUrl = lcUrl.Replace("//", "/")
        'lcUrl = lcUrl.Replace("\\", "\")


        Dim cc As String = Replace(bt.Page.ClientScript.GetPostBackEventReference(bt, "").ToString(), "'", Chr(34))
        If SidePannel Then
            bt.OnClientClick = CreateSidePannel(lcUrl, lnWidth, lcTitle, cc)
        Else
            bt.OnClientClick = GetMntDialogJQ(lcUrl, lnHeight, lnWidth, lcTitle, cc)
        End If
    End Function

    Public Function SetOnClientClick(bt As ImageButton, ByVal lcUrl As String, Optional ByVal lnHeight As Integer = 600, Optional ByVal lnWidth As Integer = 800, Optional lcTitle As String = "", Optional SidePannel As Boolean = False) As String
        'lcUrl = lcUrl.Replace("/\", "/")
        'lcUrl = lcUrl.Replace("\/", "/")
        'lcUrl = lcUrl.Replace("//", "/")
        'lcUrl = lcUrl.Replace("\\", "\")

        Dim cc As String = Replace(bt.Page.ClientScript.GetPostBackEventReference(bt, "").ToString(), "'", Chr(34))
        If SidePannel Then
            bt.OnClientClick = CreateSidePannel(lcUrl, lnWidth, lcTitle, cc)
        Else
            bt.OnClientClick = GetMntDialogJQ(lcUrl, lnHeight, lnWidth, lcTitle, cc)
        End If
    End Function

    Public Function SetOnClientClick(bt As Web.UI.WebControls.LinkButton, ByVal lcUrl As String, Optional ByVal lnHeight As Integer = 600, Optional ByVal lnWidth As Integer = 800, Optional lcTitle As String = "", Optional SidePannel As Boolean = False) As String
        'lcUrl = lcUrl.Replace("/\", "/")
        'lcUrl = lcUrl.Replace("\/", "/")
        'lcUrl = lcUrl.Replace("//", "/")
        'lcUrl = lcUrl.Replace("\\", "\")


        Dim cc As String = Replace(bt.Page.ClientScript.GetPostBackEventReference(bt, "").ToString(), "'", Chr(34))
        If SidePannel Then
            bt.OnClientClick = CreateSidePannel(lcUrl, lnWidth, lcTitle, cc)
        Else
            bt.OnClientClick = GetMntDialogJQ(lcUrl, lnHeight, lnWidth, lcTitle, cc)
        End If
    End Function

    Public Function CreateSidePannel(ByVal lcUrl As String, Optional ByVal lnWidth As Integer = 800, Optional lcTitle As String = "", Optional callBackCode As String = "") As String
        Dim lcScript As String = ""
        Dim lcDialogId As String = Guid.NewGuid().ToString
        CreateSidePannel = lcScript
        Dim lcWidth As String = CStr(lnWidth)
        lcScript = ""
        lcUrl = Replace(lcUrl, "[AND]", "&")
        If InStr(lcUrl, "?") > 0 Then
            lcUrl = lcUrl + "&DIALOG_ID=" + lcDialogId
        Else
            lcUrl = lcUrl + "?DIALOG_ID=" + lcDialogId
        End If
        lcScript = lcScript + "return CreateSidePannel('" + lcTitle + "','" + lcUrl + "'," + lcWidth + ",'" + callBackCode + "','" + lcDialogId + "')"
        CreateSidePannel = lcScript
    End Function

    Public Function GetMntDialogJQ(ByVal lcUrl As String, Optional ByVal lnHeight As Integer = 600, Optional ByVal lnWidth As Integer = 800, Optional lcTitle As String = "", Optional callBackCode As String = "") As String
        Dim lcScript As String = ""
        Dim lcDialogId As String = Guid.NewGuid().ToString
        GetMntDialogJQ = lcScript
        Dim lcHeight As String = CStr(lnHeight)
        Dim lcWidth As String = CStr(lnWidth)
        lcScript = ""
        lcUrl = Replace(lcUrl, "[AND]", "&")
        If InStr(lcUrl, "?") > 0 Then
            lcUrl = lcUrl + "&DIALOG_ID=" + lcDialogId
        Else
            lcUrl = lcUrl + "?DIALOG_ID=" + lcDialogId
        End If
        lcScript = lcScript + "return ShowInDialog('" + lcTitle + "','" + lcUrl + "'," + lcHeight + "," + lcWidth + ",'" + callBackCode + "','" + lcDialogId + "')"
        GetMntDialogJQ = lcScript
    End Function

    Function GetAppPath() As String
        Dim lcPath As String = HttpRuntime.AppDomainAppVirtualPath
        GetAppPath = lcPath
        If lcPath = "\" Or lcPath = "/" Then
            GetAppPath = ""
        End If
    End Function

    Function Get_server_Url(ByRef p As System.Web.UI.Page) As String
        Dim laServerUrl As String() = Split(p.Request.Url.ToString, "/")
        Dim lcServerUrl As String = laServerUrl(0) + "//" + laServerUrl(2) + GetAppPath()
        Get_server_Url = lcServerUrl
    End Function

    Sub AddBootStrapLinks(ByRef p As Web.UI.Page)
        Dim laServerUrl As String() = Split(p.Request.Url.ToString, "/")
        Dim lcServerUrl As String = laServerUrl(0) + "//" + laServerUrl(2) + GetAppPath()


        Dim css As HtmlLink = New HtmlLink()
        css.Href = lcServerUrl + "/bootstrap/bootstrap.min.css"
        css.Attributes("rel") = "stylesheet"
        css.Attributes("type") = "text/css"
        css.Attributes("media") = "all"
        css.Attributes("integrity") = "sha384-gH2yIJqKdNHPEq0n4Mqa/HGKIhSkIHeL5AyhkYV8i59U5AR6csBvApHHNl/vI1Bx"
        css.Attributes("crossorigin") = "anonymous"
        p.Header.Controls.Add(css)

        Dim js As HtmlGenericControl = New HtmlGenericControl("script")
        js.Attributes("type") = "text/javascript"
        js.Attributes("src") = lcServerUrl + "/bootstrap/bootstrap.bundle.min.js"
        js.Attributes("integrity") = "sha384-A3rJD856KowSb7dwlZdYEkO39Gagi7vIsF0jrRAoQmDKKtQBHUuLZ9AsSv4jD4Xa"
        js.Attributes("crossorigin") = "anonymous"
        p.Header.Controls.Add(js)

    End Sub

    Sub AddJQueryLinks(ByRef p As Web.UI.Page,
                   Optional llAddFontsOwsome As Boolean = False,
                   Optional AddjsDialogWithoutDatePicker As Boolean = False)

        Dim laServerUrl As String() = Split(p.Request.Url.ToString, "/")
        Dim lcServerUrl As String = laServerUrl(0) + "//" + laServerUrl(2) + GetAppPath()

        '==by Jalal
        AddClientScript(p, lcServerUrl + "/JQ/jquery-2.1.0.min.js")
        '================================
        AddClientCss(p, lcServerUrl + "/JQ/jquery-ui.css")
        AddClientScript(p, lcServerUrl + "/JQ/jquery.js")
        AddClientScript(p, lcServerUrl + "/JQ/jquery-ui.js")

        If AddjsDialogWithoutDatePicker Then
            AddClientScript(p, lcServerUrl + "/JQ/Dialogs/jsDialog2.js")
        Else
            AddClientScript(p, lcServerUrl + "/JQ/Dialogs/jsDialog.js")
        End If

        AddClientScript(p, lcServerUrl + "/JQ/jquery.blockUI.js")

        AddClientCss(p, lcServerUrl + "/JQ/toastr/toastr.css")
        AddClientCss(p, lcServerUrl + "/css/Forms.css")
        AddClientScript(p, lcServerUrl + "/JQ/toastr/toastr.js")
        AddClientScript(p, lcServerUrl + "/JQ/Masks/masks.js")
        If llAddFontsOwsome = True Then
            AddClientCss(p, lcServerUrl + "/css/font-awesome-4.7.0/css/font-awesome.min.css")
        End If



    End Sub


    Sub AddClientCss(ByVal ThePage As Web.UI.Page, ByVal CssFile As String)
        Dim css As HtmlLink = New HtmlLink()
        css.Href = CssFile
        css.Attributes("rel") = "stylesheet"
        css.Attributes("type") = "text/css"
        css.Attributes("media") = "all"
        ThePage.Header.Controls.Add(css)
    End Sub

    Sub AddClientScript(ByVal ThePage As Web.UI.Page, ByVal ScriptFile As String)
        Dim js As HtmlGenericControl = New HtmlGenericControl("script")
        js.Attributes("type") = "text/javascript"
        js.Attributes("src") = ScriptFile
        ThePage.Header.Controls.Add(js)
    End Sub

    Sub AppendClientScript(ByVal ThePage As Web.UI.Page, ByVal ScriptFile As String)
        Dim js As HtmlGenericControl = New HtmlGenericControl("script")
        js.Attributes("type") = "text/javascript"
        js.Attributes("src") = ScriptFile
    End Sub

    Public Function CloseDialogJQ(p As Web.UI.Page, Optional lcDialogID As String = "") As String
        If lcDialogID = "" Then
            Dim context As System.Web.HttpContext = System.Web.HttpContext.Current
            lcDialogID = context.Request("DIALOG_ID")
        End If
        If lcDialogID <> "" Then
            Dim CSM As ClientScriptManager = p.ClientScript
            CSM.RegisterStartupScript(p.GetType, Guid.NewGuid().ToString(), "<script language=""JavaScript"">CloseDialog('" + lcDialogID + "');</script>")
        Else
            Dim CSM As ClientScriptManager = p.ClientScript
            CSM.RegisterStartupScript(p.GetType, Guid.NewGuid().ToString(), "<script language=""JavaScript"">GoBack();</script>")
        End If
        CloseDialogJQ = ""
    End Function

    Function AddAppPath(ByVal lcPath As String) As String
        If Left(lcPath, 1) = "/" Then
            AddAppPath = GetAppPath() + lcPath
        Else
            AddAppPath = lcPath
        End If
    End Function
End Module
