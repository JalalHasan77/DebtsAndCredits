Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Public Module VendorPopupHelper

    Public Enum PopupPlacement
        Center
        RightSide
    End Enum

    Public Sub RegisterVendorPopup(ByVal page As Page,
                                   ByVal triggerControl As WebControl,
                                   ByVal popupPageUrl As String,
                                   ByVal popupWidth As Integer,
                                   ByVal popupHeight As Integer,
                                   ByVal placement As PopupPlacement,
                                   ByVal selectedVendorValueField As HiddenField,
                                   ByVal selectedVendorTextField As HiddenField,
                                   ByVal displayTextBox As TextBox,
                                   Optional ByVal popupTitle As String = "Select Vendor")

        RegisterVendorPopupStyles(page)
        RegisterVendorPopupMarkup(page, popupWidth, popupHeight, placement, popupTitle)
        RegisterVendorPopupScript(page,
                                  popupPageUrl,
                                  triggerControl.UniqueID,
                                  selectedVendorValueField.ClientID,
                                  selectedVendorTextField.ClientID,
                                  displayTextBox.ClientID)

        triggerControl.Attributes("onclick") = "openVendorDialog(); return false;"
    End Sub

    Private Sub RegisterVendorPopupStyles(ByVal page As Page)
        If page.Items("VendorPopupStylesRegistered") IsNot Nothing Then Exit Sub

        Dim css As New StringBuilder()
        css.AppendLine("<style type=""text/css"">")
        css.AppendLine("#vendorModalOverlay {")
        css.AppendLine("    display: none;")
        css.AppendLine("    position: fixed;")
        css.AppendLine("    top: 0;")
        css.AppendLine("    left: 0;")
        css.AppendLine("    width: 100%;")
        css.AppendLine("    height: 100%;")
        css.AppendLine("    background-color: rgba(0, 0, 0, 0.55);")
        css.AppendLine("    z-index: 1000;")
        css.AppendLine("}")
        css.AppendLine("#vendorModalDialog {")
        css.AppendLine("    position: absolute;")
        css.AppendLine("    max-width: 95vw;")
        css.AppendLine("    max-height: 95vh;")
        css.AppendLine("    background-color: #ffffff;")
        css.AppendLine("    border-radius: 8px;")
        css.AppendLine("    box-shadow: 0 8px 30px rgba(0,0,0,0.3);")
        css.AppendLine("    overflow: hidden;")
        css.AppendLine("    display: flex;")
        css.AppendLine("    flex-direction: column;")
        css.AppendLine("}")
        css.AppendLine(".vendor-modal-header {")
        css.AppendLine("    display: flex;")
        css.AppendLine("    align-items: center;")
        css.AppendLine("    justify-content: space-between;")
        css.AppendLine("    padding: 15px 15px 10px 15px;")
        css.AppendLine("    border-bottom: 1px solid #e5e5e5;")
        css.AppendLine("}")
        css.AppendLine(".vendor-modal-title {")
        css.AppendLine("    font-family: Arial, sans-serif;")
        css.AppendLine("    font-size: 20px;")
        css.AppendLine("    font-weight: bold;")
        css.AppendLine("    color: #333;")
        css.AppendLine("}")
        css.AppendLine(".btn-close-x {")
        css.AppendLine("    background: transparent;")
        css.AppendLine("    border: none;")
        css.AppendLine("    color: #666;")
        css.AppendLine("    cursor: pointer;")
        css.AppendLine("    font-size: 24px;")
        css.AppendLine("    line-height: 1;")
        css.AppendLine("    padding: 0 4px;")
        css.AppendLine("}")
        css.AppendLine(".btn-close-x:hover {")
        css.AppendLine("    color: #cc0000;")
        css.AppendLine("}")
        css.AppendLine("#vendorPopupFrame {")
        css.AppendLine("    width: 100%;")
        css.AppendLine("    height: 100%;")
        css.AppendLine("    min-height: 0;")
        css.AppendLine("    border: none;")
        css.AppendLine("    flex: 1 1 auto;")
        css.AppendLine("}")
        css.AppendLine(".vendor-modal-footer {")
        css.AppendLine("    padding: 12px 15px;")
        css.AppendLine("    text-align: right;")
        css.AppendLine("    border-top: 1px solid #e5e5e5;")
        css.AppendLine("}")
        css.AppendLine(".btn-close {")
        css.AppendLine("    padding: 8px 18px;")
        css.AppendLine("    background: #cc0000;")
        css.AppendLine("    color: #fff;")
        css.AppendLine("    border: none;")
        css.AppendLine("    border-radius: 4px;")
        css.AppendLine("    cursor: pointer;")
        css.AppendLine("    font-size: 14px;")
        css.AppendLine("}")
        css.AppendLine(".btn-close:hover {")
        css.AppendLine("    background: #a80000;")
        css.AppendLine("}")
        css.AppendLine("</style>")

        If page.Header IsNot Nothing Then
            page.Header.Controls.Add(New LiteralControl(css.ToString()))
        ElseIf page.Form IsNot Nothing Then
            page.Form.Controls.Add(New LiteralControl(css.ToString()))
        End If

        page.Items("VendorPopupStylesRegistered") = True
    End Sub

    Private Sub RegisterVendorPopupMarkup(ByVal page As Page,
                                          ByVal popupWidth As Integer,
                                          ByVal popupHeight As Integer,
                                          ByVal placement As PopupPlacement,
                                          ByVal popupTitle As String)

        If page.Items("VendorPopupMarkupRegistered") IsNot Nothing Then Exit Sub

        Dim popupTop As String = "50%"
        Dim popupLeft As String = "50%"
        Dim popupRight As String = "auto"
        Dim popupTransform As String = "translate(-50%, -50%)"

        If placement = PopupPlacement.RightSide Then
            popupTop = "20px"
            popupLeft = "auto"
            popupRight = "20px"
            popupTransform = "none"
        End If

        Dim popupMarkup As New StringBuilder()
        popupMarkup.AppendLine("<div id=""vendorModalOverlay"" onclick=""closeVendorDialog();"">")
        popupMarkup.AppendLine("    <div id=""vendorModalDialog"" role=""dialog"" aria-modal=""true"" aria-labelledby=""vendorModalTitle"" onclick=""if (event.stopPropagation) event.stopPropagation(); event.cancelBubble = true;"" style=""width:" & popupWidth & "px;height:" & popupHeight & "px;top:" & popupTop & ";left:" & popupLeft & ";right:" & popupRight & ";transform:" & popupTransform & ";"">")
        popupMarkup.AppendLine("        <div class=""vendor-modal-header"">")
        popupMarkup.AppendLine("            <span id=""vendorModalTitle"" class=""vendor-modal-title"">" & page.Server.HtmlEncode(popupTitle) & "</span>")
        popupMarkup.AppendLine("            <button type=""button"" class=""btn-close-x"" onclick=""closeVendorDialog(); return false;"" aria-label=""Close popup"">&#10005;</button>")
        popupMarkup.AppendLine("        </div>")
        popupMarkup.AppendLine("        <iframe id=""vendorPopupFrame"" src=""about:blank""></iframe>")
        popupMarkup.AppendLine("        <div class=""vendor-modal-footer"">")
        popupMarkup.AppendLine("            <button type=""button"" class=""btn-close"" onclick=""closeVendorDialog(); return false;"">Cancel</button>")
        popupMarkup.AppendLine("        </div>")
        popupMarkup.AppendLine("    </div>")
        popupMarkup.AppendLine("</div>")

        If page.Form IsNot Nothing Then
            page.Form.Controls.Add(New LiteralControl(popupMarkup.ToString()))
        End If

        page.Items("VendorPopupMarkupRegistered") = True
    End Sub

    Private Sub RegisterVendorPopupScript(ByVal page As Page,
                                          ByVal popupPageUrl As String,
                                          ByVal postBackUniqueId As String,
                                          ByVal selectedVendorValueClientId As String,
                                          ByVal selectedVendorTextClientId As String,
                                          ByVal displayTextBoxClientId As String)

        If page.Items("VendorPopupScriptRegistered") IsNot Nothing Then Exit Sub

        Dim resolvedUrl As String = popupPageUrl
        If Not String.IsNullOrEmpty(popupPageUrl) AndAlso popupPageUrl.StartsWith("~") Then
            resolvedUrl = VirtualPathUtility.ToAbsolute(popupPageUrl)
        End If

        Dim js As New StringBuilder()
        js.AppendLine("function openVendorDialog() {")
        js.AppendLine("    document.getElementById('vendorPopupFrame').src = '" & JsEncode(resolvedUrl) & "';")
        js.AppendLine("    document.getElementById('vendorModalOverlay').style.display = 'block';")
        js.AppendLine("    document.body.style.overflow = 'hidden';")
        js.AppendLine("}")
        js.AppendLine("")
        js.AppendLine("function closeVendorDialog() {")
        js.AppendLine("    var overlay = document.getElementById('vendorModalOverlay');")
        js.AppendLine("    var frame = document.getElementById('vendorPopupFrame');")
        js.AppendLine("    if (overlay) overlay.style.display = 'none';")
        js.AppendLine("    if (frame) frame.src = 'about:blank';")
        js.AppendLine("    document.body.style.overflow = '';")
        js.AppendLine("}")
        js.AppendLine("")
        js.AppendLine("function receiveVendorValue(selectedValue, displayText) {")
        js.AppendLine("    var vendorText = displayText || selectedValue || '';")
        js.AppendLine("    document.getElementById('" & JsEncode(selectedVendorValueClientId) & "').value = selectedValue || '';")
        js.AppendLine("    document.getElementById('" & JsEncode(selectedVendorTextClientId) & "').value = vendorText;")
        js.AppendLine("    document.getElementById('" & JsEncode(displayTextBoxClientId) & "').value = vendorText;")
        js.AppendLine("    closeVendorDialog();")
        js.AppendLine("    __doPostBack('" & JsEncode(postBackUniqueId) & "', '');")
        js.AppendLine("}")

        If ScriptManager.GetCurrent(page) IsNot Nothing Then
            ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "VendorPopupScript", js.ToString(), True)
        Else
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "VendorPopupScript", js.ToString(), True)
        End If

        page.Items("VendorPopupScriptRegistered") = True
    End Sub

    Private Function JsEncode(ByVal value As String) As String
        If value Is Nothing Then Return String.Empty

        Return value.Replace(Chr(92), Chr(92) & Chr(92)) _
                    .Replace("'", "\'") _
                    .Replace(vbCrLf, "\n") _
                    .Replace(vbCr, "\n") _
                    .Replace(vbLf, "\n")
    End Function

End Module
