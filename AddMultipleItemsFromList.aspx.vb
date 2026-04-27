Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports System.Web
Imports System.Web.UI.WebControls

Partial Class AddMultipleItemsFromList
    Inherits System.Web.UI.Page
    Dim EncryNDecry As New EncryDecry


    Private SqlText As String = ""
    Private Const SessionDataKey As String = "AddMultipleItemsFromList_Data"
    Private HideID As Boolean = True

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim arrParametres As String()
            arrParametres = EncryNDecry.DecryptToArray(Request("Parameters"))
            SqlText = arrParametres(0)
            Label1.Text = arrParametres(1)
            HideID = IIf(arrParametres(2).ToUpper = "Y", True, False)
            LoadOptions(GetSelectedIdsFromRequest())
        End If

    End Sub

    Private Sub LoadOptions(ByVal selectedIds As HashSet(Of String))
        Dim dt As DataTable = DB.GetDataTable(DB.InfoDB, SqlText)
        Session(SessionDataKey) = dt
        litMembersTable.Text = BuildMembersTableHtml(dt, selectedIds)
    End Sub

    Private Function BuildMembersTableHtml(ByVal dt As DataTable, ByVal selectedIds As HashSet(Of String)) As String
        Dim html As New StringBuilder()

        html.Append("<table id=""membersTable"" class=""members-table"">")
        html.Append("<thead><tr>")
        html.Append("<th class=""members-selector""></th>")

        If dt IsNot Nothing Then
            For colIndex As Integer = 0 To dt.Columns.Count - 1
                If HideID AndAlso colIndex = 0 Then
                    Continue For
                End If

                Dim col As DataColumn = dt.Columns(colIndex)
                html.Append("<th>")
                html.Append(HttpUtility.HtmlEncode(col.ColumnName))
                html.Append("</th>")
            Next
        End If

        html.Append("</tr></thead>")
        html.Append("<tbody>")

        If dt IsNot Nothing Then
            For Each dr As DataRow In dt.Rows
                Dim idValue As String = String.Empty
                If dt.Columns.Count > 0 Then
                    idValue = Convert.ToString(dr(0))
                End If

                html.Append("<tr data-searchtext=""")
                html.Append(HttpUtility.HtmlAttributeEncode(BuildSearchText(dr)))
                html.Append(""">")

                html.Append("<td class=""members-selector-cell""><input type=""checkbox"" name=""selectedItem"" value=""")
                html.Append(HttpUtility.HtmlAttributeEncode(idValue))
                html.Append("""")

                If selectedIds IsNot Nothing AndAlso selectedIds.Contains(idValue) Then
                    html.Append(" checked=""checked""")
                End If

                html.Append(" /></td>")

                For colIndex As Integer = 0 To dt.Columns.Count - 1
                    If HideID AndAlso colIndex = 0 Then
                        Continue For
                    End If

                    Dim col As DataColumn = dt.Columns(colIndex)
                    Dim cellText As String = Convert.ToString(dr(col.ColumnName))
                    html.Append("<td data-original-text=""")
                    html.Append(HttpUtility.HtmlAttributeEncode(cellText))
                    html.Append(""">")
                    html.Append(HttpUtility.HtmlEncode(cellText))
                    html.Append("</td>")
                Next

                html.Append("</tr>")
            Next
        End If

        html.Append("</tbody></table>")
        Return html.ToString()
    End Function

    Private Function BuildSearchText(ByVal dr As DataRow) As String
        Dim sb As New StringBuilder()

        For Each col As DataColumn In dr.Table.Columns
            If sb.Length > 0 Then
                sb.Append(" ")
            End If
            sb.Append(Convert.ToString(dr(col.ColumnName)))
        Next

        Return sb.ToString()
    End Function

    Private Function GetSelectedIdsFromRequest() As HashSet(Of String)
        Dim selected As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim values As String() = Request.Form.GetValues("selectedItem")

        If values Is Nothing Then
            Return selected
        End If

        For Each value As String In values
            If Not String.IsNullOrEmpty(value) Then
                selected.Add(value)
            End If
        Next

        Return selected
    End Function

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim selectedIds As HashSet(Of String) = GetSelectedIdsFromRequest()
        Dim l As New List(Of ListItem)()
        Dim dt As DataTable = TryCast(Session(SessionDataKey), DataTable)

        If dt Is Nothing Then
            dt = DB.GetDataTable(DB.InfoDB, SqlText)
        End If

        If dt IsNot Nothing AndAlso dt.Columns.Count > 0 Then
            For Each dr As DataRow In dt.Rows
                Dim valueText As String = Convert.ToString(dr(0))
                If selectedIds.Contains(valueText) Then
                    Dim item As New ListItem()
                    item.Value = valueText
                    item.Text = If(dt.Columns.Count > 1, Convert.ToString(dr(1)), valueText)
                    l.Add(item)
                End If
            Next
        End If

        VendorPopupHelper.RegisterPopupSelectionAndClose(
            page:=Me,
            returnValue:=l,
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
