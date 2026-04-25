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

        VendorPopupHelper.RegisterPopupSelectionAndClose(
                            page:=Me,
                            selectedValue:=selectedValue,
                            selectedText:=selectedText,
                            additionalFieldKey:="NewORder",
                            additionalFieldValue:=selectedType,
                            startupScriptKey:="AddAdjustmentAndClose",
                            skipPostBack:=True)

    End Sub

    Private Function GetSelectedTypeValue() As String
        If RadioButton6.Checked Then Return "Reduction"
        If RadioButton7.Checked Then Return "Addition"
        If RadioButton8.Checked Then Return "Neutral"
        Return String.Empty
    End Function


    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim script As String = "(function () {" &
                       "    if (window.parent && typeof window.parent.closeVendorDialog === 'function') {" &
                       "        window.parent.closeVendorDialog();" &
                       "    }" &
                       "})();"

        If ScriptManager.GetCurrent(Me) IsNot Nothing Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "ClosePopupOnly", script, True)
        Else
            Me.ClientScript.RegisterStartupScript(Me.GetType(), "ClosePopupOnly", script, True)
        End If
    End Sub
End Class
