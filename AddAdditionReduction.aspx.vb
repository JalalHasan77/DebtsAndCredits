Imports System.Data

Partial Class AddAdditionReduction
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim selectedType As String = GetSelectedTypeValue()
        Dim selectedValue As String = String.Empty
        Dim selectedText As String = String.Empty
        Dim amountType As String = GetSelectedAmountTypeValue()
        Dim amount As String = If(TextBox2 Is Nothing, String.Empty, TextBox2.Text.Trim())

        If DropDownList2 IsNot Nothing Then
            selectedValue = DropDownList2.SelectedValue
            If DropDownList2.SelectedItem IsNot Nothing Then
                selectedText = DropDownList2.SelectedItem.Text
            Else
                selectedText = selectedValue
            End If
        End If

        Dim returnTable As New DataTable("AdjustmentReturnValue")
        returnTable.Columns.Add("SelectedValue", GetType(String))
        returnTable.Columns.Add("SelectedText", GetType(String))
        returnTable.Columns.Add("AdjustmentType", GetType(String))
        returnTable.Columns.Add("AmountType", GetType(String))
        returnTable.Columns.Add("Amount", GetType(String))

        Dim returnValue As DataRow = returnTable.NewRow()
        returnValue("SelectedValue") = selectedValue
        returnValue("SelectedText") = selectedText
        returnValue("AdjustmentType") = selectedType
        returnValue("AmountType") = amountType
        returnValue("Amount") = amount
        returnTable.Rows.Add(returnValue)

        VendorPopupHelper.RegisterPopupSelectionAndClose(
            page:=Me,
            returnValue:=returnValue,
            startupScriptKey:="AddAdjustmentAndClose",
            skipPostBack:=False)

    End Sub

    Private Function GetSelectedTypeValue() As String
        If RadioButton6.Checked Then Return "Reduction"
        If RadioButton7.Checked Then Return "Addition"
        If RadioButton8.Checked Then Return "Neutral"
        Return String.Empty
    End Function

    Private Function GetSelectedAmountTypeValue() As String
        If RadioButton9.Checked Then Return "Percentage"
        If RadioButton10.Checked Then Return "Fixed Amount"
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
