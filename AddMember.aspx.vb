
Partial Class AddMember
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim DT As New Data.DataTable
            DT = DB.GetDataTable(DB.InfoDB, "Select ID,MemberName from Members")

            CheckBoxList1.DataSource = DT
            CheckBoxList1.DataTextField = "MemberName"
            CheckBoxList1.DataValueField = "ID"
            CheckBoxList1.DataBind()

        End If
    End Sub
    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim L As New List(Of String)
        For Each oneItem As ListItem In CheckBoxList1.Items
            If oneItem.Selected Then
                L.Add(oneItem.Text)
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
