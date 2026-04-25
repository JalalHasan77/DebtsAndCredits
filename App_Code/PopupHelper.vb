Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Public Module VendorPopupHelper

    Private Const StylesRegisteredKey As String = "VendorPopupStylesRegistered"
    Private Const MarkupRegisteredKey As String = "VendorPopupMarkupRegistered"
    Private Const ScriptRegisteredKey As String = "VendorPopupScriptRegistered"

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

        If page Is Nothing Then Throw New ArgumentNullException("page")
        If triggerControl Is Nothing Then Throw New ArgumentNullException("triggerControl")
        If selectedVendorValueField Is Nothing Then Throw New ArgumentNullException("selectedVendorValueField")
        If selectedVendorTextField Is Nothing Then Throw New ArgumentNullException("selectedVendorTextField")
        If displayTextBox Is Nothing Then Throw New ArgumentNullException("displayTextBox")

        RegisterVendorPopupStyles(page)
        RegisterVendorPopupMarkup(page)
        RegisterVendorPopupScript(page)

        Dim resolvedUrl As String = ResolvePopupUrl(page, popupPageUrl)

        Dim clientScript As String = BuildOpenDialogScript(
            resolvedUrl,
            popupTitle,
            popupWidth,
            popupHeight,
            placement,
            triggerControl.UniqueID,
            selectedVendorValueField.ClientID,
            selectedVendorTextField.ClientID,
            displayTextBox.ClientID)

        triggerControl.Attributes("onclick") = clientScript
    End Sub

    Private Sub RegisterVendorPopupStyles(ByVal page As Page)
        If page.Items(StylesRegisteredKey) IsNot Nothing Then Exit Sub

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
        css.AppendLine("    z-index: 10000;")
        css.AppendLine("}")
        css.AppendLine("")
        css.AppendLine("#vendorModalDialog {")
        css.AppendLine("    position: fixed;")
        css.AppendLine("    top: 50%;")
        css.AppendLine("    left: 50%;")
        css.AppendLine("    transform: translate(-50%, -50%);")
        css.AppendLine("    width: 600px;")
        css.AppendLine("    height: 400px;")
        css.AppendLine("    max-width: 95vw;")
        css.AppendLine("    max-height: 95vh;")
        css.AppendLine("    background-color: #ffffff;")
        css.AppendLine("    border-radius: 8px;")
        css.AppendLine("    box-shadow: 0 8px 30px rgba(0,0,0,0.30);")
        css.AppendLine("    overflow: hidden;")
        css.AppendLine("    display: flex;")
        css.AppendLine("    flex-direction: column;")
        css.AppendLine("}")
        css.AppendLine("")
        css.AppendLine(".vendor-modal-header {")
        css.AppendLine("    display: flex;")
        css.AppendLine("    align-items: center;")
        css.AppendLine("    justify-content: space-between;")
        css.AppendLine("    padding: 15px 15px 10px 15px;")
        css.AppendLine("    border-bottom: 1px solid #e5e5e5;")
        css.AppendLine("    font-family: Arial, sans-serif;")
        css.AppendLine("}")
        css.AppendLine("")
        css.AppendLine(".vendor-modal-title {")
        css.AppendLine("    font-size: 20px;")
        css.AppendLine("    font-weight: bold;")
        css.AppendLine("    color: #333333;")
        css.AppendLine("}")
        css.AppendLine("")
        css.AppendLine(".btn-close-x {")
        css.AppendLine("    background: transparent;")
        css.AppendLine("    border: none;")
        css.AppendLine("    color: #666666;")
        css.AppendLine("    cursor: pointer;")
        css.AppendLine("    font-size: 24px;")
        css.AppendLine("    line-height: 1;")
        css.AppendLine("    padding: 0 4px;")
        css.AppendLine("}")
        css.AppendLine("")
        css.AppendLine(".btn-close-x:hover {")
        css.AppendLine("    color: #cc0000;")
        css.AppendLine("}")
        css.AppendLine("")
        css.AppendLine("#vendorPopupFrame {")
        css.AppendLine("    width: 100%;")
        css.AppendLine("    height: 100%;")
        css.AppendLine("    border: none;")
        css.AppendLine("    flex: 1 1 auto;")
        css.AppendLine("}")
        css.AppendLine("")
        css.AppendLine(".vendor-modal-footer {")
        css.AppendLine("    padding: 12px 15px;")
        css.AppendLine("    text-align: right;")
        css.AppendLine("    border-top: 1px solid #e5e5e5;")
        css.AppendLine("}")
        css.AppendLine("")
        css.AppendLine(".btn-close {")
        css.AppendLine("    padding: 8px 18px;")
        css.AppendLine("    background: #cc0000;")
        css.AppendLine("    color: #ffffff;")
        css.AppendLine("    border: none;")
        css.AppendLine("    border-radius: 4px;")
        css.AppendLine("    cursor: pointer;")
        css.AppendLine("    font-size: 14px;")
        css.AppendLine("}")
        css.AppendLine("")
        css.AppendLine(".btn-close:hover {")
        css.AppendLine("    background: #a80000;")
        css.AppendLine("}")
        css.AppendLine("</style>")

        If page.Header IsNot Nothing Then
            page.Header.Controls.Add(New LiteralControl(css.ToString()))
        ElseIf page.Form IsNot Nothing Then
            page.Form.Controls.AddAt(0, New LiteralControl(css.ToString()))
        Else
            page.Controls.Add(New LiteralControl(css.ToString()))
        End If

        page.Items(StylesRegisteredKey) = True
    End Sub

    Private Sub RegisterVendorPopupMarkup(ByVal page As Page)
        If page.Items(MarkupRegisteredKey) IsNot Nothing Then Exit Sub

        Dim markup As New StringBuilder()

        markup.AppendLine("<div id=""vendorModalOverlay"" onclick=""return closeVendorDialog();"">")
        markup.AppendLine("    <div id=""vendorModalDialog"" role=""dialog"" aria-modal=""true"" aria-labelledby=""vendorModalTitle"" onclick=""if (event.stopPropagation) event.stopPropagation(); event.cancelBubble = true;"">")
        markup.AppendLine("        <div class=""vendor-modal-header"">")
        markup.AppendLine("            <span id=""vendorModalTitle"" class=""vendor-modal-title"">Select Vendor</span>")
        markup.AppendLine("            <button type=""button"" class=""btn-close-x"" onclick=""return closeVendorDialog();"" aria-label=""Close popup"">&#10005;</button>")
        markup.AppendLine("        </div>")
        markup.AppendLine("        <iframe id=""vendorPopupFrame"" src=""about:blank""></iframe>")
        markup.AppendLine("        <div class=""vendor-modal-footer"">")
        markup.AppendLine("            <button type=""button"" class=""btn-close"" onclick=""return closeVendorDialog();"">Cancel</button>")
        markup.AppendLine("        </div>")
        markup.AppendLine("    </div>")
        markup.AppendLine("</div>")

        If page.Form IsNot Nothing Then
            page.Form.Controls.Add(New LiteralControl(markup.ToString()))
        Else
            page.Controls.Add(New LiteralControl(markup.ToString()))
        End If

        page.Items(MarkupRegisteredKey) = True
    End Sub

    Private Sub RegisterVendorPopupScript(ByVal page As Page)
        If page.Items(ScriptRegisteredKey) IsNot Nothing Then Exit Sub

        Dim js As New StringBuilder()

        js.AppendLine("var vendorPopupContext = {")
        js.AppendLine("    postBackId: '',")
        js.AppendLine("    valueFieldId: '',")
        js.AppendLine("    textFieldId: '',")
        js.AppendLine("    displayTextBoxId: ''")
        js.AppendLine("};")
        js.AppendLine("")
        js.AppendLine("function vendorPopupGet(id) {")
        js.AppendLine("    if (!id) return null;")
        js.AppendLine("    return document.getElementById(id);")
        js.AppendLine("}")
        js.AppendLine("")
        js.AppendLine("function vendorPopupSetPlacement(placement) {")
        js.AppendLine("    var dialog = document.getElementById('vendorModalDialog');")
        js.AppendLine("    if (!dialog) return;")
        js.AppendLine("")
        js.AppendLine("    if (placement === 'RightSide') {")
        js.AppendLine("        dialog.style.top = '20px';")
        js.AppendLine("        dialog.style.left = 'auto';")
        js.AppendLine("        dialog.style.right = '20px';")
        js.AppendLine("        dialog.style.transform = 'none';")
        js.AppendLine("    } else {")
        js.AppendLine("        dialog.style.top = '50%';")
        js.AppendLine("        dialog.style.left = '50%';")
        js.AppendLine("        dialog.style.right = 'auto';")
        js.AppendLine("        dialog.style.transform = 'translate(-50%, -50%)';")
        js.AppendLine("    }")
        js.AppendLine("}")
        js.AppendLine("")
        js.AppendLine("function openVendorDialog(popupUrl, popupTitle, popupWidth, popupHeight, placement, postBackId, valueFieldId, textFieldId, displayTextBoxId) {")
        js.AppendLine("    var overlay = document.getElementById('vendorModalOverlay');")
        js.AppendLine("    var dialog = document.getElementById('vendorModalDialog');")
        js.AppendLine("    var title = document.getElementById('vendorModalTitle');")
        js.AppendLine("    var frame = document.getElementById('vendorPopupFrame');")
        js.AppendLine("")
        js.AppendLine("    if (!overlay || !dialog || !title || !frame) return false;")
        js.AppendLine("")
        js.AppendLine("    vendorPopupContext.postBackId = postBackId || '';")
        js.AppendLine("    vendorPopupContext.valueFieldId = valueFieldId || '';")
        js.AppendLine("    vendorPopupContext.textFieldId = textFieldId || '';")
        js.AppendLine("    vendorPopupContext.displayTextBoxId = displayTextBoxId || '';")
        js.AppendLine("")
        js.AppendLine("    title.innerHTML = popupTitle || 'Select Vendor';")
        js.AppendLine("")
        js.AppendLine("    if (!popupWidth || popupWidth < 1) popupWidth = 600;")
        js.AppendLine("    if (!popupHeight || popupHeight < 1) popupHeight = 400;")
        js.AppendLine("")
        js.AppendLine("    dialog.style.width = popupWidth + 'px';")
        js.AppendLine("    dialog.style.height = popupHeight + 'px';")
        js.AppendLine("")
        js.AppendLine("    vendorPopupSetPlacement(placement || 'Center');")
        js.AppendLine("")
        js.AppendLine("    frame.src = popupUrl || 'about:blank';")
        js.AppendLine("    overlay.style.display = 'block';")
        js.AppendLine("")
        js.AppendLine("    if (document.body) {")
        js.AppendLine("        document.body.style.overflow = 'hidden';")
        js.AppendLine("    }")
        js.AppendLine("")
        js.AppendLine("    return false;")
        js.AppendLine("}")
        js.AppendLine("")
        js.AppendLine("function closeVendorDialog() {")
        js.AppendLine("    var overlay = document.getElementById('vendorModalOverlay');")
        js.AppendLine("    var frame = document.getElementById('vendorPopupFrame');")
        js.AppendLine("")
        js.AppendLine("    if (overlay) overlay.style.display = 'none';")
        js.AppendLine("    if (frame) frame.src = 'about:blank';")
        js.AppendLine("")
        js.AppendLine("    if (document.body) {")
        js.AppendLine("        document.body.style.overflow = '';")
        js.AppendLine("    }")
        js.AppendLine("")
        js.AppendLine("    return false;")
        js.AppendLine("}")
        js.AppendLine("")
        js.AppendLine("function receiveVendorValue(selectedValue, displayText) {")
        js.AppendLine("    var selected = selectedValue || '';")
        js.AppendLine("    var text = displayText || selected;")
        js.AppendLine("")
        js.AppendLine("    var valueField = vendorPopupGet(vendorPopupContext.valueFieldId);")
        js.AppendLine("    var textField = vendorPopupGet(vendorPopupContext.textFieldId);")
        js.AppendLine("    var displayBox = vendorPopupGet(vendorPopupContext.displayTextBoxId);")
        js.AppendLine("")
        js.AppendLine("    if (valueField) valueField.value = selected;")
        js.AppendLine("    if (textField) textField.value = text;")
        js.AppendLine("    if (displayBox) displayBox.value = text;")
        js.AppendLine("")
        js.AppendLine("    closeVendorDialog();")
        js.AppendLine("")
        js.AppendLine("    if (typeof __doPostBack === 'function' && vendorPopupContext.postBackId) {")
        js.AppendLine("        __doPostBack(vendorPopupContext.postBackId, '');")
        js.AppendLine("    }")
        js.AppendLine("}")
        js.AppendLine("")
        js.AppendLine("if (!window.vendorPopupEscapeHandlerRegistered) {")
        js.AppendLine("    window.vendorPopupEscapeHandlerRegistered = true;")
        js.AppendLine("    if (document.addEventListener) {")
        js.AppendLine("        document.addEventListener('keydown', function (e) {")
        js.AppendLine("            e = e || window.event;")
        js.AppendLine("            var key = e.key || e.keyCode;")
        js.AppendLine("            var overlay = document.getElementById('vendorModalOverlay');")
        js.AppendLine("            if (overlay && overlay.style.display === 'block' && (key === 'Escape' || key === 'Esc' || key === 27)) {")
        js.AppendLine("                closeVendorDialog();")
        js.AppendLine("            }")
        js.AppendLine("        });")
        js.AppendLine("    }")
        js.AppendLine("}")

        If ScriptManager.GetCurrent(page) IsNot Nothing Then
            ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "VendorPopupScript", js.ToString(), True)
        Else
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "VendorPopupScript", js.ToString(), True)
        End If

        page.Items(ScriptRegisteredKey) = True
    End Sub

    Private Function BuildOpenDialogScript(ByVal popupUrl As String,
                                           ByVal popupTitle As String,
                                           ByVal popupWidth As Integer,
                                           ByVal popupHeight As Integer,
                                           ByVal placement As PopupPlacement,
                                           ByVal postBackUniqueId As String,
                                           ByVal selectedVendorValueClientId As String,
                                           ByVal selectedVendorTextClientId As String,
                                           ByVal displayTextBoxClientId As String) As String

        If popupWidth <= 0 Then popupWidth = 600
        If popupHeight <= 0 Then popupHeight = 400

        Dim script As New StringBuilder()

        script.Append("return openVendorDialog('")
        script.Append(JsEncode(popupUrl))
        script.Append("','")
        script.Append(JsEncode(popupTitle))
        script.Append("',")
        script.Append(popupWidth.ToString())
        script.Append(",")
        script.Append(popupHeight.ToString())
        script.Append(",'")
        script.Append(placement.ToString())
        script.Append("','")
        script.Append(JsEncode(postBackUniqueId))
        script.Append("','")
        script.Append(JsEncode(selectedVendorValueClientId))
        script.Append("','")
        script.Append(JsEncode(selectedVendorTextClientId))
        script.Append("','")
        script.Append(JsEncode(displayTextBoxClientId))
        script.Append("');")

        Return script.ToString()
    End Function

    Private Function ResolvePopupUrl(ByVal page As Page, ByVal popupPageUrl As String) As String
        If popupPageUrl Is Nothing Then Return "about:blank"

        popupPageUrl = popupPageUrl.Trim()
        If popupPageUrl = String.Empty Then Return "about:blank"

        Return page.ResolveClientUrl(popupPageUrl)
    End Function

    Private Function JsEncode(ByVal value As String) As String
        If value Is Nothing Then Return String.Empty

        Return value.Replace("\", "\\") _
                    .Replace("'", "\'") _
                    .Replace(vbCrLf, "\n") _
                    .Replace(vbCr, "\n") _
                    .Replace(vbLf, "\n")
    End Function

End Module
