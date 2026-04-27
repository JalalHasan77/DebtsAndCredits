
Partial Class AddMultipleItemsFromList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim DT As New Data.DataTable
            DT = DB.GetDataTable(DB.InfoDB, "Select ID,MemberName, NoOfMovement from Members order by cint(NoOfMovement) desc")
            CheckBoxList1.Items.Clear()
            For Each DR As Data.DataRow In DT.Rows

                Dim item As New ListItem()
                item.Text = DR("MemberName").ToString()   'display text
                item.Value = DR("ID").ToString()          'main value
                'item.Attributes("NoOfMovement") = DR("NoOfMovement").ToString() 'third value

                CheckBoxList1.Items.Add(item)
            Next
        End If
    End Sub
    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim L As New List(Of ListItem)
        For Each oneItem As ListItem In CheckBoxList1.Items
            If oneItem.Selected Then
                L.Add(oneItem)
            End If
        Next

        VendorPopupHelper.RegisterPopupSelectionAndClose(
    page:=Me,
    returnValue:=L,
    startupScriptKey:="SelectedMembers",
    skipPostBack:=False)
    End Sub
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

