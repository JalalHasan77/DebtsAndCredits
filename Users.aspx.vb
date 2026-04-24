Imports Microsoft.AspNet.SignalR
Imports System.Data

Partial Class Users
    Inherits System.Web.UI.Page

    Private Sub form1_Load(sender As Object, e As EventArgs) Handles form1.Load
        If Not Page.IsPostBack Then
            populateData()
        End If
    End Sub

    Sub populateData()
        Dim SQL As String = "SELECT ID, NAME, USERCOLOR FROM TRSRY_TEMPUSERS"

        Dim DT As New DataTable
        DT = GetDataTable(EBDB, SQL)

        GridView1.DataSource = DT.DefaultView
        GridView1.DataBind()
    End Sub

    Protected Sub GridView1_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As DataRowView = CType(e.Row.DataItem, DataRowView)

            Dim id As String = drv("ID").ToString
            Dim isChecked As Boolean = ToBooleanStatus(drv("USERCOLOR"))

            ' put the row id in html so javascript can find the same row later
            e.Row.Attributes("data-id") = id.ToString()

            Dim chk As CheckBox = CType(e.Row.FindControl("CheckBox1"), CheckBox)
            If chk IsNot Nothing Then
                chk.Checked = isChecked
            End If

            SetRowColor(e.Row, isChecked)
        End If
    End Sub

    Protected Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs)
        Dim chk As CheckBox = CType(sender, CheckBox)
        Dim row As GridViewRow = CType(chk.NamingContainer, GridViewRow)

        Dim id As String = GridView1.DataKeys(row.RowIndex).Value
        Dim isChecked As Boolean = chk.Checked

        ' 1) save to database so future page loads show the same state
        UpdateUserStatus(id, isChecked)

        ' 2) update current user immediately
        SetRowColor(row, isChecked)

        ' 3) push change to all currently connected users
        Dim hubContext = GlobalHost.ConnectionManager.GetHubContext(Of UsersHub)()
        hubContext.Clients.All.userStatusChanged(id, isChecked)
    End Sub

    Private Sub SetRowColor(row As GridViewRow, isChecked As Boolean)
        If isChecked Then
            row.BackColor = Drawing.Color.Green
            row.ForeColor = Drawing.Color.White
        Else
            row.BackColor = Drawing.Color.White
            row.ForeColor = Drawing.Color.Black
        End If
    End Sub

    Private Function ToBooleanStatus(value As Object) As Boolean
        If value Is Nothing OrElse IsDBNull(value) Then Return False

        Dim s As String = value.ToString().Trim().ToUpper()

        Return (s = "G" OrElse s = "TRUE" OrElse s = "YES")
    End Function

    Private Sub UpdateUserStatus(id As String, isChecked As Boolean)
        ' Change this value if your USERSTATUS column stores Y/N instead of 1/0
        Dim statusValue As String = If(isChecked, "G", "W")

        Dim SQL As String = "UPDATE TRSRY_TEMPUSERS " &
                            "SET USERCOLOR = '" & statusValue & "' " &
                            "WHERE ID = '" & id.ToString() & "'"

        ' Replace this with your own execute/update helper
        ExecuteNonQuery(EBDB, SQL)
    End Sub
End Class
