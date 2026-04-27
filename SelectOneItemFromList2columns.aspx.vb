Imports System
Imports System.Data
Imports System.Text
Imports System.Web.UI
Imports System.Web.UI.WebControls

Partial Class SelectOneItemFromList2columns
    Inherits System.Web.UI.Page

    Private Const SqlText As String = "Select ID as [Key], VenderName as Title, Whatsapp as Phone from Venders"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadOptions()
        End If
    End Sub

    Private Sub LoadOptions()
        Dim dt As DataTable = DB.GetDataTable(DB.InfoDB, SqlText)

        gvOptions.DataSource = dt
        gvOptions.DataBind()
    End Sub

    Protected Sub gvOptions_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles gvOptions.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then
            Return
        End If

        Dim drv As DataRowView = CType(e.Row.DataItem, DataRowView)
        Dim valueText As String = String.Empty
        Dim displayText As String = String.Empty
        Dim searchText As New StringBuilder()

        If drv.DataView.Table.Columns.Count > 0 Then
            valueText = Convert.ToString(drv(0))
            displayText = Convert.ToString(drv(Math.Min(1, drv.DataView.Table.Columns.Count - 1)))
        End If

        e.Row.CssClass = "select-row"
        e.Row.Attributes("data-searchtext") = BuildSearchText(drv)
        e.Row.Attributes("onclick") = String.Format("selectItemRow(this, '{0}', '{1}');", JsEncode(valueText), JsEncode(displayText))

        For Each cell As TableCell In e.Row.Cells
            cell.Attributes("data-original-text") = cell.Text.Replace("&nbsp;", String.Empty)
        Next
    End Sub

    Private Function BuildSearchText(ByVal drv As DataRowView) As String
        Dim sb As New StringBuilder()

        For Each col As DataColumn In drv.DataView.Table.Columns
            If sb.Length > 0 Then
                sb.Append(" ")
            End If
            sb.Append(Convert.ToString(drv(col.ColumnName)))
        Next

        Return sb.ToString()
    End Function

    Private Function JsEncode(ByVal value As String) As String
        If value Is Nothing Then
            Return String.Empty
        End If

        Return value.Replace("\", "\\") _
                    .Replace("'", "\'") _
                    .Replace(vbCrLf, "\n") _
                    .Replace(vbCr, "\n") _
                    .Replace(vbLf, "\n")
    End Function
End Class
