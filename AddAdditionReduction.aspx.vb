Partial Class AddAdditionReduction
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim selectedType As String = GetSelectedTypeValue()
        Dim selectedValue As String = String.Empty
        Dim selectedText As String = String.Empty

        If DropDownList2 IsNot Nothing Then
            selectedValue = DropDownList2.SelectedValue
            If DropDownList2.SelectedItem IsNot Nothing Then
                selectedText = DropDownList2.SelectedItem.Text
            Else
                selectedText = selectedValue
            End If
        End If

        Dim script As String = BuildParentReturnScript(selectedType, selectedValue, selectedText)
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "AddAdjustmentAndClose", script, True)
    End Sub

    Private Function GetSelectedTypeValue() As String
        If RadioButton6.Checked Then Return "Reduction"
        If RadioButton7.Checked Then Return "Addition"
        If RadioButton8.Checked Then Return "Neutral"
        Return String.Empty
    End Function

    Private Function BuildParentReturnScript(ByVal selectedType As String,
                                             ByVal selectedValue As String,
                                             ByVal selectedText As String) As String
        Dim sb As New System.Text.StringBuilder()

        sb.AppendLine("(function () {")
        sb.AppendLine("    if (!window.parent || !window.parent.document) return;")
        sb.AppendLine("")
        sb.AppendLine("    var parentDoc = window.parent.document;")
        sb.AppendLine("    var target = parentDoc.getElementById('NewORder');")
        sb.AppendLine("    var i;")
        sb.AppendLine("")
        sb.AppendLine("    if (!target) {")
        sb.AppendLine("        var inputs = parentDoc.getElementsByTagName('input');")
        sb.AppendLine("        for (i = 0; i < inputs.length; i++) {")
        sb.AppendLine("            if ((inputs[i].id && /NewORder$/i.test(inputs[i].id)) ||")
        sb.AppendLine("                (inputs[i].name && /NewORder$/i.test(inputs[i].name))) {")
        sb.AppendLine("                target = inputs[i];")
        sb.AppendLine("                break;")
        sb.AppendLine("            }")
        sb.AppendLine("        }")
        sb.AppendLine("    }")
        sb.AppendLine("")
        sb.AppendLine("    if (!target) {")
        sb.AppendLine("        var selects = parentDoc.getElementsByTagName('select');")
        sb.AppendLine("        for (i = 0; i < selects.length; i++) {")
        sb.AppendLine("            if ((selects[i].id && /NewORder$/i.test(selects[i].id)) ||")
        sb.AppendLine("                (selects[i].name && /NewORder$/i.test(selects[i].name))) {")
        sb.AppendLine("                target = selects[i];")
        sb.AppendLine("                break;")
        sb.AppendLine("            }")
        sb.AppendLine("        }")
        sb.AppendLine("    }")
        sb.AppendLine("")
        sb.AppendLine("    if (typeof window.parent.vendorPopupPersistFieldValue === 'function') {")
        sb.AppendLine("        window.parent.vendorPopupPersistFieldValue('NewORder', '" & JsEncode(selectedType) & "');")
        sb.AppendLine("    }")
        sb.AppendLine("")
        sb.AppendLine("    if (target) {")
        sb.AppendLine("        if (typeof target.value !== 'undefined') {")
        sb.AppendLine("            target.value = '" & JsEncode(selectedType) & "';")
        sb.AppendLine("            if (target.setAttribute) target.setAttribute('value', '" & JsEncode(selectedType) & "');")
        sb.AppendLine("        } else {")
        sb.AppendLine("            target.innerHTML = '" & JsEncode(selectedType) & "';")
        sb.AppendLine("        }")
        sb.AppendLine("")
        sb.AppendLine("        if (parentDoc.createEvent) {")
        sb.AppendLine("            var inputEvent = parentDoc.createEvent('HTMLEvents');")
        sb.AppendLine("            inputEvent.initEvent('input', true, true);")
        sb.AppendLine("            target.dispatchEvent(inputEvent);")
        sb.AppendLine("            var changeEvent = parentDoc.createEvent('HTMLEvents');")
        sb.AppendLine("            changeEvent.initEvent('change', true, true);")
        sb.AppendLine("            target.dispatchEvent(changeEvent);")
        sb.AppendLine("        } else if (target.fireEvent) {")
        sb.AppendLine("            target.fireEvent('onchange');")
        sb.AppendLine("        }")
        sb.AppendLine("    }")
        sb.AppendLine("")
        sb.AppendLine("    if (typeof window.parent.receiveVendorValue === 'function') {")
        sb.AppendLine("        window.parent.receiveVendorValue('" & JsEncode(selectedValue) & "', '" & JsEncode(selectedText) & "', true);")
        sb.AppendLine("        return;")
        sb.AppendLine("    }")
        sb.AppendLine("")
        sb.AppendLine("    if (typeof window.parent.closeVendorDialog === 'function') {")
        sb.AppendLine("        window.parent.closeVendorDialog();")
        sb.AppendLine("    }")
        sb.AppendLine("})();")

        Return sb.ToString()
    End Function

    Private Function JsEncode(ByVal value As String) As String
        If value Is Nothing Then Return String.Empty

        Return value.Replace("\", "\\") _
                    .Replace("'", "\'") _
                    .Replace(vbCrLf, "\n") _
                    .Replace(vbCr, "\n") _
                    .Replace(vbLf, "\n")
    End Function

End Class
