Imports System.Data

Partial Class AddAdditionReduction
    Inherits System.Web.UI.Page
    Dim encryNdecry As New EncryDecry


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
        End If
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim adjusmentName As String = String.Empty
        adjusmentName = DropDownList2.SelectedItem.Text
        Dim adjusmentType As String = GetAdjusmentType()
        Dim adjusmentCalculation As String = GetadjusmentCalculation()
        Dim CalculationAmount As String = If(TextBox2 Is Nothing, String.Empty, TextBox2.Text.Trim())
        Dim Distrbution As String = GetadDistributionMethod()


        Dim returnTable As New DataTable("AdjustmentReturnValue")
        returnTable.Columns.Add("adjusmentName", GetType(String))
        returnTable.Columns.Add("adjusmentType", GetType(String))
        returnTable.Columns.Add("adjusmentCalculation", GetType(String))
        returnTable.Columns.Add("CalculationAmount", GetType(String))
        returnTable.Columns.Add("Distribution", GetType(String))

        Dim returnValue As DataRow = returnTable.NewRow()
        returnValue("adjusmentName") = adjusmentName
        returnValue("adjusmentType") = adjusmentType
        returnValue("adjusmentCalculation") = adjusmentCalculation
        returnValue("CalculationAmount") = CalculationAmount
        returnValue("Distribution") = Distrbution
        returnTable.Rows.Add(returnValue)

        VendorPopupHelper.RegisterPopupSelectionAndClose(
            page:=Me,
            returnValue:=returnValue,
            startupScriptKey:="AddAdjustmentAndClose",
            skipPostBack:=False)
    End Sub

    Private Function GetAdjusmentType() As String
        If RadioButton6.Checked Then Return "Reduction"
        If RadioButton7.Checked Then Return "Addition"
        If RadioButton8.Checked Then Return "Neutral"
        Return String.Empty
    End Function

    Private Function GetadjusmentCalculation() As String
        If RadioButton9.Checked Then Return "Percentage"
        If RadioButton10.Checked Then Return "Fixed Amount"
        Return String.Empty
    End Function

    Private Function GetadDistributionMethod() As String
        If RadioButton1.Checked Then Return "Equally"
        If RadioButton2.Checked Then Return "By Percentage"
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
