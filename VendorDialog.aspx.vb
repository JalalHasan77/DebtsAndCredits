Imports System.Data

Partial Class VendorDialog
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadVendors()
        End If
    End Sub

    Private Sub LoadVendors()
        Dim SQL As String = ""
        SQL = SQL + vbCrLf + " SELECT "
        SQL = SQL + vbCrLf + " ID AS OptionValue, "
        SQL = SQL + vbCrLf + " VenderName as OptionName "
        SQL = SQL + vbCrLf + " FROM "
        SQL = SQL + vbCrLf + " Venders "
        SQL = SQL + vbCrLf + " ORDER BY "
        SQL = SQL + vbCrLf + " VenderName "
        Dim dt As DataTable
        dt = GetDataTable(InfoDB, SQL)

        rptVendors.DataSource = dt
        rptVendors.DataBind()

        pnlEmpty.Visible = (dt Is Nothing OrElse dt.Rows.Count = 0)
    End Sub
End Class
