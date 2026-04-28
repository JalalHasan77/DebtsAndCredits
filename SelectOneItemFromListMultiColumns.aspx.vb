Imports System
Imports System.Data
Imports System.Text
Imports System.Web.UI
Imports System.Web.UI.WebControls

Partial Class SelectOneItemFromListMultiColumns
    ' = Select One Item From a List (MultiColumns)
    Inherits System.Web.UI.Page
    Dim EncryNDecry As New EncryDecry

    Private SqlText As String '= "Select ID as [Key], VenderName as Title, Whatsapp as Phone from Venders"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'The coming String is encrted and in form: SQL | Title
            Dim Parameters() As String = EncryNDecry.DecryptToArray(Request("Parameters"))
            SqlText = Parameters(0)
            Label1.Text = Parameters(1)
            LoadOptions()
        End If
    End Sub

    Private Sub LoadOptions()
        Dim dt As DataTable = DB.GetDataTable(DB.InfoDB, SqlText)

        BuildGridColumns(dt)
        gvOptions.DataSource = dt
        gvOptions.DataBind()

        If gvOptions.HeaderRow IsNot Nothing Then
            gvOptions.UseAccessibleHeader = True
            gvOptions.HeaderRow.TableSection = TableRowSection.TableHeader
        End If
    End Sub

    Private Sub BuildGridColumns(ByVal dt As DataTable)
        gvOptions.Columns.Clear()

        If dt Is Nothing Then
            Return
        End If

        For Each col As DataColumn In dt.Columns
            Dim field As New BoundField()
            field.DataField = col.ColumnName
            field.HeaderText = col.ColumnName
            field.HtmlEncode = False
            gvOptions.Columns.Add(field)
        Next
    End Sub

    Protected Sub gvOptions_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles gvOptions.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then
            Return
        End If

        Dim drv As DataRowView = CType(e.Row.DataItem, DataRowView)
        Dim valueText As String = String.Empty
        Dim displayText As String = String.Empty

        If drv.DataView.Table.Columns.Count > 0 Then
            valueText = Convert.ToString(drv(0))
            displayText = BuildDisplayText(drv)
        End If

        e.Row.CssClass = "select-row"
        e.Row.Attributes("data-searchtext") = BuildSearchText(drv)
        e.Row.Attributes("onclick") = String.Format("selectItemRow(this, '{0}', '{1}');", JsEncode(valueText), JsEncode(displayText))

        For Each cell As TableCell In e.Row.Cells
            cell.Attributes("data-original-text") = cell.Text.Replace("&nbsp;", String.Empty)
        Next
    End Sub

    Private Function BuildDisplayText(ByVal drv As DataRowView) As String
        If drv.DataView.Table.Columns.Count > 1 Then
            Return Convert.ToString(drv(1))
        End If

        If drv.DataView.Table.Columns.Count > 0 Then
            Return Convert.ToString(drv(0))
        End If

        Return String.Empty
    End Function

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



