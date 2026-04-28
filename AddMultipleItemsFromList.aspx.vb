Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports System.Web
Imports System.Web.UI.WebControls

Partial Class AddMultipleItemsFromList
    Inherits System.Web.UI.Page

    Private EncryNDecry As New EncryDecry()
    Private Const SessionDataKey As String = "AddMultipleItemsFromList_Data"
    Private Const ViewStateSqlTextKey As String = "AddMultipleItemsFromList_SqlText"
    Private Const ViewStateHideMaskKey As String = "AddMultipleItemsFromList_HideColumnsMask"
    Private Const ViewStateEditableMaskKey As String = "AddMultipleItemsFromList_EditableColumnsMask"

    Private Property SqlText As String
        Get
            Return Convert.ToString(ViewState(ViewStateSqlTextKey))
        End Get
        Set(value As String)
            ViewState(ViewStateSqlTextKey) = value
        End Set
    End Property

    Private Property HideColumnsMask As String
        Get
            Return Convert.ToString(ViewState(ViewStateHideMaskKey))
        End Get
        Set(value As String)
            ViewState(ViewStateHideMaskKey) = NormalizeMask(value)
        End Set
    End Property

    Private Property EditableColumnsMask As String
        Get
            Return Convert.ToString(ViewState(ViewStateEditableMaskKey))
        End Get
        Set(value As String)
            ViewState(ViewStateEditableMaskKey) = NormalizeMask(value)
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            InitializeFromParameters()
        End If

        LoadOptions(GetSelectedIdsFromRequest())
    End Sub

    Private Sub InitializeFromParameters()
        Dim encryptedParameters As String = Request("Parameters")
        Dim ListParameters As clsListProperties = EncryNDecry.DecryptObject(Of clsListProperties)(encryptedParameters)

        SqlText = ListParameters.SQL
        Label1.Text = ListParameters.FormTitle
        HideColumnsMask = ListParameters.ColumnHideAndShow
        EditableColumnsMask = ListParameters.EditableColumns
        Dim ColumnsWidths() As Double = ListParameters.ColumnsWidth

    End Sub

    Private Sub LoadOptions(ByVal selectedIds As HashSet(Of String))
        Dim dt As DataTable

        If String.IsNullOrWhiteSpace(SqlText) Then
            dt = New DataTable()
        Else
            dt = DB.GetDataTable(DB.InfoDB, SqlText)
        End If

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
                If IsColumnHidden(colIndex) Then
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
            For rowIndex As Integer = 0 To dt.Rows.Count - 1
                Dim dr As DataRow = dt.Rows(rowIndex)
                Dim originalIdValue As String = String.Empty

                If dt.Columns.Count > 0 Then
                    originalIdValue = Convert.ToString(dr(0))
                End If

                html.Append("<tr data-searchtext=""")
                html.Append(HttpUtility.HtmlAttributeEncode(BuildSearchText(dr, rowIndex)))
                html.Append(""">")

                html.Append("<td class=""members-selector-cell""><input type=""checkbox"" name=""selectedItem"" value=""")
                html.Append(HttpUtility.HtmlAttributeEncode(originalIdValue))
                html.Append("""")

                If selectedIds IsNot Nothing AndAlso selectedIds.Contains(originalIdValue) Then
                    html.Append(" checked=""checked""")
                End If

                html.Append(" /></td>")

                For colIndex As Integer = 0 To dt.Columns.Count - 1
                    If IsColumnHidden(colIndex) Then
                        Continue For
                    End If

                    Dim originalCellText As String = Convert.ToString(dr(colIndex))
                    Dim renderedCellText As String = GetRenderedCellValue(rowIndex, colIndex, originalCellText)

                    If IsColumnEditable(colIndex) Then
                        html.Append("<td class=""editable-cell""><input type=""text"" name=""")
                        html.Append(HttpUtility.HtmlAttributeEncode(GetCellInputName(rowIndex, colIndex)))
                        html.Append(""" value=""")
                        html.Append(HttpUtility.HtmlAttributeEncode(renderedCellText))
                        html.Append(""" data-search-input=""1"" /></td>")
                    Else
                        html.Append("<td data-original-text=""")
                        html.Append(HttpUtility.HtmlAttributeEncode(renderedCellText))
                        html.Append(""">")
                        html.Append(HttpUtility.HtmlEncode(renderedCellText))
                        html.Append("</td>")
                    End If
                Next

                html.Append("</tr>")
            Next
        End If

        html.Append("</tbody></table>")
        Return html.ToString()
    End Function

    Private Function BuildSearchText(ByVal dr As DataRow, ByVal rowIndex As Integer) As String
        Dim sb As New StringBuilder()

        For colIndex As Integer = 0 To dr.Table.Columns.Count - 1
            If IsColumnHidden(colIndex) Then
                Continue For
            End If

            If sb.Length > 0 Then
                sb.Append(" ")
            End If

            Dim cellText As String = Convert.ToString(dr(colIndex))
            sb.Append(GetRenderedCellValue(rowIndex, colIndex, cellText))
        Next

        Return sb.ToString()
    End Function

    Private Function NormalizeMask(ByVal value As String) As String
        If String.IsNullOrEmpty(value) Then
            Return String.Empty
        End If

        Dim sb As New StringBuilder()

        For Each ch As Char In value.Trim().ToUpperInvariant()
            If ch = "Y"c OrElse ch = "N"c Then
                sb.Append(ch)
            End If
        Next

        Return sb.ToString()
    End Function

    Private Function IsColumnHidden(ByVal columnIndex As Integer) As Boolean
        If String.IsNullOrEmpty(HideColumnsMask) Then
            Return False
        End If

        If columnIndex < 0 OrElse columnIndex >= HideColumnsMask.Length Then
            Return False
        End If

        Return HideColumnsMask(columnIndex) = "Y"c
    End Function

    Private Function IsColumnEditable(ByVal columnIndex As Integer) As Boolean
        If IsColumnHidden(columnIndex) Then
            Return False
        End If

        If String.IsNullOrEmpty(EditableColumnsMask) Then
            Return False
        End If

        If columnIndex < 0 OrElse columnIndex >= EditableColumnsMask.Length Then
            Return False
        End If

        Return EditableColumnsMask(columnIndex) = "Y"c
    End Function

    Private Function GetCellInputName(ByVal rowIndex As Integer, ByVal colIndex As Integer) As String
        Return String.Format("cell_{0}_{1}", rowIndex, colIndex)
    End Function

    Private Function GetRenderedCellValue(ByVal rowIndex As Integer, ByVal colIndex As Integer, ByVal defaultValue As String) As String
        If IsColumnHidden(colIndex) OrElse Not Page.IsPostBack OrElse Not IsColumnEditable(colIndex) Then
            Return defaultValue
        End If

        Dim postedValue As String = Request.Form(GetCellInputName(rowIndex, colIndex))
        If postedValue Is Nothing Then
            Return defaultValue
        End If

        Return postedValue
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
            If String.IsNullOrWhiteSpace(SqlText) Then
                dt = New DataTable()
            Else
                dt = DB.GetDataTable(DB.InfoDB, SqlText)
            End If
        End If

        If dt IsNot Nothing AndAlso dt.Columns.Count > 0 Then
            For rowIndex As Integer = 0 To dt.Rows.Count - 1
                Dim dr As DataRow = dt.Rows(rowIndex)
                Dim originalIdValue As String = Convert.ToString(dr(0))

                If selectedIds.Contains(originalIdValue) Then
                    Dim item As New ListItem()
                    item.Value = GetRenderedCellValue(rowIndex, 0, originalIdValue)

                    If dt.Columns.Count > 1 Then
                        item.Text = GetRenderedCellValue(rowIndex, 1, Convert.ToString(dr(1)))
                    Else
                        item.Text = item.Value
                    End If

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
