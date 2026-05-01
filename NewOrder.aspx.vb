Imports System.Data
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Partial Class NewOrder
    Inherits System.Web.UI.Page
    Dim encryNdecry As New EncryDecry
    Public GrandTotal As Decimal = 0
    Public Subtotal As Decimal

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If HttpContext.Current.Session("MyTable") Is Nothing Then
            CreateInitialTable()
        End If

        LoadFromObject()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsPostBack Then
            ' Sync AddRdcTable ViewState from session so JS-edited NetValues survive postback.
            Dim wmTable As DataTable = TryCast(HttpContext.Current.Session("AddRdcTable_WM"), DataTable)
            If wmTable IsNot Nothing Then
                ViewState("AddRdcTable") = wmTable
            End If
            PersistPostedGridValues()
            ' Keep litSubtotal, rptSummary and litGrandTotal current for all postback paths.
            RefreshSubtotal()
        Else
            ' First load only
            RefreshSubtotal()
        End If

        If Not String.IsNullOrEmpty(hdnSelectedVendorText.Value) Then
            TextBox1.Text = hdnSelectedVendorText.Value
        End If

        Dim ListParameters As New clsListProperties
        ListParameters.SQL = "Select VenderID as [Key], VenderName as Title, Whatsapp as Phone from Venders"
        ListParameters.FormTitle = "Select Vendor"
        ListParameters.ColumnHideAndShow = "YNN"
        ListParameters.EditableColumns = "NNN"
        ListParameters.ColumnsWidth = New Double() {1, 3, 1}
        ListParameters.HoverableList = "Y"
        VendorPopupHelper.RegisterVendorPopup(Me,
                                      LinkButton3,
                                      "SelectOneItemFromListMultiColumns.aspx?Parameters=" & encryNdecry.EncryptObject(Of clsListProperties)(ListParameters),
                                      450,
                                      760,
                                      VendorPopupHelper.PopupPlacement.Center,
                                      hdnSelectedVendorValue,
                                      hdnSelectedVendorText,
                                      lblVendorValue,
                                      lblVendorText,
                                      "Select Vendor",
                                      VendorPopupHelper.PopupDisplayMode.FrameOnly)


        VendorPopupHelper.RegisterVendorPopup(Me,
                                      btnAddExpRdc,
                                      "AddAdditionReduction.aspx",
                                      600, 400,
                                      PopupPlacement.Center,
                                      "Select Adj",
                                      VendorPopupHelper.PopupDisplayMode.FrameOnly)

        Dim MemberListParameters As New clsListProperties
        With MemberListParameters
            .SQL = "Select MemberID as ID, MemberName as [Name] from Members order by CInt(NoOfMovement) desc"
            .FormTitle = "Select Members"
            .ColumnHideAndShow = "YN"
            .EditableColumns = "NN"
            .ColumnsWidth = New Double() {1, 3}
            .HoverableList = "Y"
        End With
        Dim SelectMembersParameters As String = encryNdecry.EncryptObject(Of clsListProperties)(MemberListParameters)

        VendorPopupHelper.RegisterVendorPopup(Me,
                                      lnkBtnAddMembers,
                                      "AddMultipleItemsFromList.aspx?Parameters=" & SelectMembersParameters,
                                      400,
                                      600,
                                      PopupPlacement.Center,
                                      "Select Adj",
                                      VendorPopupHelper.PopupDisplayMode.FrameOnly)

    End Sub

    Sub CreateInitialTable()
        Dim DT As New Data.DataTable
        DT = GetDataTable(InfoDB, " SELECT MemberID, MemberName,'0.000' as Deposit, '0.000' as Debt,'0.000' as Profit  FROM Members WHERE MemberName IN (" &
    "'Fatima AlHaddad'," &
    "'Fatima Mohammed'," &
    "'Elmeera'," &
    "'Roqaya'," &
    "'Jalal'," &
    "'Safa Shamsan','Areej') order by MemberID;")

        HttpContext.Current.Session("MyTable") = DT
    End Sub

    Sub LoadFromObject()
        Dim dt As DataTable =
        CType(HttpContext.Current.Session("MyTable"), DataTable)

        Dim visibleColumnCount As Integer = Math.Max(dt.Columns.Count - 1, 0)

        HeaderLevel1 = EnsureHeaderList("HeaderLevel1", "Profit", visibleColumnCount)
        HeaderLevel2 = EnsureHeaderList("HeaderLevel2", "Total", visibleColumnCount)
        HeaderLevel3 = EnsureHeaderList("HeaderLevel3", "NoOfItems", visibleColumnCount)
        HeaderLevel4 = EnsureHeaderList("HeaderLevel4", "Item", visibleColumnCount)
        HeaderLevel5 = EnsureHeaderList("HeaderLevel5", "Price", visibleColumnCount)
        HeaderItemIds = EnsureHeaderList("HeaderItemIds", "", visibleColumnCount)

        BuildGrid(dt)
    End Sub

    ' -----------------------------------------------------------------------
    ' WebMethod: SaveCell
    ' Saves a single cell value into the in-session DataTable.
    ' -----------------------------------------------------------------------
    <WebMethod(EnableSession:=True)>
    <ScriptMethod()>
    Public Shared Sub SaveCell(rowIndex As Integer, columnName As String, value As String)

        Dim dt As DataTable =
        CType(HttpContext.Current.Session("MyTable"), DataTable)

        dt.Rows(rowIndex)(columnName) = value
        dt.AcceptChanges()
        HttpContext.Current.Session("MyTable") = dt
    End Sub

    ' -----------------------------------------------------------------------
    ' WebMethod: SaveNetValue
    ' Updates the NetValue for a single GridView2 row (identified by __RowGuid)
    ' in the ViewState-backed AddRdcTable.
    ' -----------------------------------------------------------------------
    <WebMethod(EnableSession:=True)>
    <ScriptMethod()>
    Public Shared Sub SaveNetValue(rowGuid As String, value As String)
        Dim dt As DataTable = TryCast(HttpContext.Current.Session("AddRdcTable_WM"), DataTable)
        If dt Is Nothing Then Exit Sub

        Dim dr As DataRow = dt.AsEnumerable().
            FirstOrDefault(Function(r) String.Equals(
                Convert.ToString(r("__RowGuid")), rowGuid, StringComparison.Ordinal))
        If dr Is Nothing Then Exit Sub

        dr("NetValue") = value
        dt.AcceptChanges()
        HttpContext.Current.Session("AddRdcTable_WM") = dt
    End Sub


    ' Called by the JS recalculateSubtotal() function after every cell or
    ' header edit.  Formula: SUM over all item columns of (Price + Profit) * NoOfItems
    ' where NoOfItems = sum of all member row quantities for that column.
    ' Returns the formatted string directly so the JS can update litSubtotal.
    ' -----------------------------------------------------------------------
    <WebMethod(EnableSession:=True)>
    <ScriptMethod()>
    Public Shared Function RecalculateSubtotal() As String
        Dim dt As DataTable = TryCast(HttpContext.Current.Session("MyTable"), DataTable)
        If dt Is Nothing Then Return "0.000"

        Dim level1 As List(Of String) = TryCast(HttpContext.Current.Session("HeaderLevel1"), List(Of String))  ' Profit per column
        Dim level5 As List(Of String) = TryCast(HttpContext.Current.Session("HeaderLevel5"), List(Of String))  ' Price per column

        If level1 Is Nothing OrElse level5 Is Nothing Then Return "0.000"

        Dim subtotal As Decimal = 0D

        ' Dynamic item columns start at data index 5 (0=ID, 1=MemberName, 2=Deposit, 3=Debt, 4=Profit)
        For dataColumnIndex As Integer = 5 To dt.Columns.Count - 1
            Dim headerIndex As Integer = dataColumnIndex - 1   ' header lists are 0-based from column 1

            If headerIndex >= level1.Count OrElse headerIndex >= level5.Count Then Continue For

            Dim price As Decimal = 0D
            Decimal.TryParse(level5(headerIndex),
                             System.Globalization.NumberStyles.Any,
                             System.Globalization.CultureInfo.InvariantCulture, price)

            Dim profit As Decimal = 0D
            Decimal.TryParse(level1(headerIndex),
                             System.Globalization.NumberStyles.Any,
                             System.Globalization.CultureInfo.InvariantCulture, profit)

            ' Sum all member quantities in this item column
            For Each row As DataRow In dt.Rows
                Dim qty As Decimal = 0D
                Decimal.TryParse(Convert.ToString(row(dataColumnIndex)),
                                 System.Globalization.NumberStyles.Any,
                                 System.Globalization.CultureInfo.InvariantCulture, qty)

                subtotal += (price + profit) * qty
            Next
        Next

        Return subtotal.ToString("0.000")
    End Function

    ' -----------------------------------------------------------------------
    ' CalculateSubtotal (instance method)
    ' Same logic as the WebMethod but used server-side during Page_Load so
    ' litSubtotal is correctly populated on initial load and postbacks.
    ' -----------------------------------------------------------------------
    Private Function CalculateSubtotal() As Decimal
        Dim dt As DataTable = TryCast(HttpContext.Current.Session("MyTable"), DataTable)
        If dt Is Nothing Then Return 0D

        Dim level1 As List(Of String) = TryCast(Session("HeaderLevel1"), List(Of String))  ' Profit per column
        Dim level5 As List(Of String) = TryCast(Session("HeaderLevel5"), List(Of String))  ' Price per column

        If level1 Is Nothing OrElse level5 Is Nothing Then Return 0D

        Dim subtotal As Decimal = 0D

        For dataColumnIndex As Integer = 5 To dt.Columns.Count - 1
            Dim headerIndex As Integer = dataColumnIndex - 1

            If headerIndex >= level1.Count OrElse headerIndex >= level5.Count Then Continue For

            Dim price As Decimal = 0D
            Decimal.TryParse(level5(headerIndex),
                             System.Globalization.NumberStyles.Any,
                             System.Globalization.CultureInfo.InvariantCulture, price)

            Dim profit As Decimal = 0D
            Decimal.TryParse(level1(headerIndex),
                             System.Globalization.NumberStyles.Any,
                             System.Globalization.CultureInfo.InvariantCulture, profit)

            For Each row As DataRow In dt.Rows
                Dim qty As Decimal = 0D
                Decimal.TryParse(Convert.ToString(row(dataColumnIndex)),
                                 System.Globalization.NumberStyles.Any,
                                 System.Globalization.CultureInfo.InvariantCulture, qty)

                subtotal += (price + profit) * qty
            Next
        Next

        Return subtotal
    End Function

    Private Sub BuildGrid(ByVal DT As DataTable)

        GridView1.Columns.Clear()

        Dim actionField As New TemplateField()
        actionField.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        actionField.ItemStyle.Width = Unit.Pixel(35)
        actionField.ItemTemplate = New ImageButtonTemplate("MemberID")
        GridView1.Columns.Add(actionField)

        Dim colIndex As Integer = 0
        For Each dc As DataColumn In DT.Columns
            If dc.ColumnName = "MemberID" Then Continue For

            Dim tf As New TemplateField()
            tf.ItemTemplate = New EditableTemplate(dc.ColumnName, colIndex)
            GridView1.Columns.Add(tf)
            colIndex += 1
        Next

        GridView1.DataSource = DT
        GridView1.DataBind()
    End Sub

    Private Function CreateEditableTemplate(columnName As String, colIndex As String) As TemplateField
        Dim tf As New TemplateField
        tf.HeaderText = columnName
        tf.ItemStyle.Width = Unit.Pixel(150)
        tf.ItemTemplate = New EditableTemplate(columnName, colIndex)
        Return tf
    End Function

    Private Sub PersistPostedGridValues()
        ' The SaveCell WebMethod already writes every cell edit directly into
        ' Session("MyTable") as the user types, so Session is always the
        ' authoritative source of truth.  The only edge-case we need to handle
        ' here is a cell that was still open (hfVis = "1") at the exact moment
        ' the user clicked Save — in that case the blur/saveCell JS sequence may
        ' not have fired, so we read the posted TextBox value directly.
        ' We must NOT read TextBox values for closed cells (hfVis = "0") because
        ' the TextBox contains the original page-render value, not the JS-edited
        ' value — overwriting Session with that stale value is precisely the bug
        ' that caused Deposit / Debt / NetValue to revert after Save.

        Dim dt As DataTable = TryCast(HttpContext.Current.Session("MyTable"), DataTable)
        If dt Is Nothing Then Exit Sub
        If GridView1.Rows.Count = 0 Then Exit Sub

        Dim colIndex As Integer = 0
        Dim anyChanged As Boolean = False

        For Each dc As DataColumn In dt.Columns
            If dc.ColumnName = "MemberID" Then Continue For

            For Each row As GridViewRow In GridView1.Rows
                If row.RowType <> DataControlRowType.DataRow Then Continue For
                If row.RowIndex < 0 OrElse row.RowIndex >= dt.Rows.Count Then Continue For

                ' Only act on cells that were open during the postback.
                Dim hfVis As HiddenField = TryCast(row.FindControl("hfVis_" & row.RowIndex & "_" & colIndex), HiddenField)
                If hfVis Is Nothing OrElse hfVis.Value <> "1" Then Continue For

                Dim txt As TextBox = TryCast(row.FindControl("txtValue_" & row.RowIndex & "_" & colIndex), TextBox)
                If txt Is Nothing Then Continue For

                ' Cell was open — the TextBox value IS the user's current edit.
                dt.Rows(row.RowIndex)(dc.ColumnName) = txt.Text
                anyChanged = True
            Next

            colIndex += 1
        Next

        If anyChanged Then
            RecalculateDynamicColumnSummaries(dt)
            RecalculateRowProfits(dt)
            dt.AcceptChanges()
            HttpContext.Current.Session("MyTable") = dt
            clTemp.lcObject = dt
        End If
    End Sub

    Protected Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs)

        If e.Row.RowType = DataControlRowType.Header Then

            e.Row.Cells.Clear()
            Dim table As Table = CType(GridView1.Controls(0), Table)

            Dim colCount As Integer = GridView1.Columns.Count

            Dim hasDynamicColumns As Boolean = (colCount > 5)
            Dim h0 As GridViewRow = Nothing

            If hasDynamicColumns Then
                h0 = New GridViewRow(-1, 0, DataControlRowType.Header, DataControlRowState.Insert)

                For i As Integer = 0 To colCount - 1
                    If i <= 4 Then
                        Dim spacerCell As New TableCell()
                        spacerCell.Text = "&nbsp;"
                        spacerCell.HorizontalAlign = HorizontalAlign.Center
                        spacerCell.VerticalAlign = VerticalAlign.Middle
                        spacerCell.Height = Unit.Pixel(24)
                        spacerCell.BackColor = Drawing.Color.WhiteSmoke
                        h0.Cells.Add(spacerCell)
                    Else
                        h0.Cells.Add(CreateDeleteHeaderCell(i - 1, HeaderItemIds(i - 1)))
                    End If
                Next
            End If

            '========================
            ' FIRST HEADER ROW
            '========================
            Dim h1 As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)

            For i As Integer = 0 To colCount - 1

                If i = 0 Then
                    Dim cell As New TableCell()
                    cell.Text = ""
                    cell.RowSpan = 5
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.VerticalAlign = VerticalAlign.Middle
                    h1.Cells.Add(cell)

                ElseIf i = 1 Then
                    Dim cell As New TableCell()
                    cell.Text = "Member"
                    cell.RowSpan = 5
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.VerticalAlign = VerticalAlign.Middle
                    h1.Cells.Add(cell)

                ElseIf i = 2 Then
                    Dim cell As New TableCell()
                    cell.Text = "Deposit"
                    cell.RowSpan = 5
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.VerticalAlign = VerticalAlign.Middle
                    h1.Cells.Add(cell)
                ElseIf i = 3 Then
                    Dim cell As New TableCell()
                    cell.Text = "Debit"
                    cell.RowSpan = 5
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.VerticalAlign = VerticalAlign.Middle
                    h1.Cells.Add(cell)
                ElseIf i = 4 Then
                    Dim cell As New TableCell()
                    cell.Text = "Profit"
                    cell.RowSpan = 5
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.VerticalAlign = VerticalAlign.Middle
                    h1.Cells.Add(cell)
                Else
                    h1.Cells.Add(CreateEditableHeaderCell(HeaderLevel1(i - 1), i - 1, 1))
                End If

            Next

            '========================
            ' SECOND HEADER ROW
            '========================
            Dim h2 As New GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert)
            For i As Integer = 5 To colCount - 1
                h2.Cells.Add(CreateEditableHeaderCell(HeaderLevel2(i - 1), i - 1, 2))
            Next

            '========================
            ' THIRD HEADER ROW
            '========================
            Dim h3 As New GridViewRow(2, 0, DataControlRowType.Header, DataControlRowState.Insert)
            For i As Integer = 5 To colCount - 1
                h3.Cells.Add(CreateEditableHeaderCell(HeaderLevel3(i - 1), i - 1, 3))
            Next

            '========================
            ' FOURTH HEADER ROW
            '========================
            Dim h4 As New GridViewRow(3, 0, DataControlRowType.Header, DataControlRowState.Insert)
            For i As Integer = 5 To colCount - 1
                h4.Cells.Add(CreateEditableHeaderCell(HeaderLevel4(i - 1), i - 1, 4))
            Next

            '========================
            ' FIFTH HEADER ROW
            '========================
            Dim h5 As New GridViewRow(3, 0, DataControlRowType.Header, DataControlRowState.Insert)
            For i As Integer = 5 To colCount - 1
                h5.Cells.Add(CreateEditableHeaderCell(HeaderLevel5(i - 1), i - 1, 5))
            Next

            Dim insertRowIndex As Integer = 0

            If hasDynamicColumns AndAlso h0 IsNot Nothing Then
                table.Rows.AddAt(insertRowIndex, h0)
                insertRowIndex += 1
            End If

            table.Rows.AddAt(insertRowIndex, h1)
            table.Rows.AddAt(insertRowIndex + 1, h2)
            table.Rows.AddAt(insertRowIndex + 2, h3)
            table.Rows.AddAt(insertRowIndex + 3, h4)
            table.Rows.AddAt(insertRowIndex + 4, h5)

        End If

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Sub SaveHeader(colIndex As Integer, level As Integer, value As String)

        If level = 1 Then
            Dim list = CType(HttpContext.Current.Session("HeaderLevel1"), List(Of String))
            list(colIndex) = value
            HttpContext.Current.Session("HeaderLevel1") = list
        ElseIf level = 2 Then
            Dim list = CType(HttpContext.Current.Session("HeaderLevel2"), List(Of String))
            list(colIndex) = value
            HttpContext.Current.Session("HeaderLevel2") = list
        ElseIf level = 3 Then
            Dim list = CType(HttpContext.Current.Session("HeaderLevel3"), List(Of String))
            list(colIndex) = value
            HttpContext.Current.Session("HeaderLevel3") = list
        ElseIf level = 4 Then
            Dim list = CType(HttpContext.Current.Session("HeaderLevel4"), List(Of String))
            list(colIndex) = value
            HttpContext.Current.Session("HeaderLevel4") = list
        Else
            Dim list = CType(HttpContext.Current.Session("HeaderLevel5"), List(Of String))
            list(colIndex) = value
            HttpContext.Current.Session("HeaderLevel5") = list
        End If

    End Sub

    Private Function EnsureHeaderList(sessionKey As String, defaultValue As String, requiredCount As Integer) As List(Of String)
        Dim list As List(Of String) = TryCast(Session(sessionKey), List(Of String))

        If list Is Nothing Then
            list = Enumerable.Repeat(defaultValue, requiredCount).ToList()
        Else
            While list.Count < requiredCount
                list.Add(defaultValue)
            End While

            If list.Count > requiredCount Then
                list = list.Take(requiredCount).ToList()
            End If
        End If

        Session(sessionKey) = list
        Return list
    End Function

    Private Function CreateEditableHeaderCell(text As String, colIndex As Integer, level As Integer) As TableCell

        Dim cell As New TableCell()
        cell.Width = Unit.Pixel(100)
        cell.HorizontalAlign = HorizontalAlign.Center
        cell.VerticalAlign = VerticalAlign.Middle

        Select Case level
            Case 1 ' Profit
                cell.BackColor = Drawing.Color.Orange
                cell.ForeColor = Drawing.Color.Black
            Case 2 ' Total
                cell.BackColor = Drawing.Color.Yellow
                cell.ForeColor = Drawing.Color.Black
            Case 4 ' Item name
                cell.BackColor = Drawing.Color.Black
                cell.ForeColor = Drawing.Color.White
        End Select

        Dim wrapper As New HtmlGenericControl("div")
        wrapper.Attributes("class") = "cell-wrapper"
        wrapper.Attributes("onclick") = "editCell(this)"
        wrapper.Attributes("data-columnindex") = colIndex.ToString()
        wrapper.Attributes("data-level") = level.ToString()
        wrapper.Attributes("data-headercol") = colIndex.ToString()
        wrapper.Attributes("data-headerlevel") = level.ToString()
        wrapper.Style("width") = "100%"
        wrapper.Style("text-align") = "center"

        Dim lbl As New Label()
        lbl.ID = "lblHeader_" & level & "_" & colIndex
        lbl.Text = text
        lbl.ForeColor = cell.ForeColor
        lbl.Style("display") = "inline-block"
        lbl.Style("width") = "100%"
        lbl.Style("text-align") = "center"

        Dim txt As New TextBox()
        txt.ID = "txtHeader_" & level & "_" & colIndex
        txt.Text = text
        txt.Style("display") = "none"
        txt.Width = Unit.Pixel(90)
        txt.Style("text-align") = "center"
        txt.BackColor = cell.BackColor
        txt.ForeColor = cell.ForeColor
        txt.Attributes("onblur") = "saveHeader(this)"
        txt.Attributes("onkeydown") = "return handleEnter(event, this);"

        wrapper.Controls.Add(lbl)
        wrapper.Controls.Add(txt)
        cell.Controls.Add(wrapper)

        Return cell

    End Function

    Private Function CreateDeleteHeaderCell(colIndex As Integer, itemId As String) As TableCell
        Dim cell As New TableCell()
        cell.Width = Unit.Pixel(100)
        cell.HorizontalAlign = HorizontalAlign.Center
        cell.VerticalAlign = VerticalAlign.Middle
        cell.BackColor = Drawing.Color.WhiteSmoke
        cell.Height = Unit.Pixel(24)
        cell.Style("padding") = "2px 0px"

        Dim imgBtn As New ImageButton()
        imgBtn.ID = "imgDeleteColumn_" & colIndex.ToString()
        imgBtn.ImageUrl = GetTrashImageUrl()
        imgBtn.CausesValidation = False
        imgBtn.ToolTip = "Delete this column"
        imgBtn.AlternateText = "Delete column"
        imgBtn.CommandName = "DeleteDynamicColumn"
        imgBtn.CommandArgument = colIndex.ToString() & "|" & If(itemId, "")
        imgBtn.Attributes("data-itemid") = If(itemId, "")
        imgBtn.OnClientClick = "return confirm('Delete this column?');"

        cell.Controls.Add(imgBtn)
        Return cell
    End Function

    Private Function GetTrashImageUrl() As String
        Dim preferredVirtualPath As String = "~/Image/Trash16x16.png"
        Dim fallbackVirtualPath As String = "~/Images/Trash16x16.png"

        Try
            Dim preferredPhysicalPath As String = Server.MapPath(preferredVirtualPath)
            If System.IO.File.Exists(preferredPhysicalPath) Then
                Return preferredVirtualPath
            End If
        Catch
        End Try

        Return fallbackVirtualPath
    End Function

    Private Sub RemoveDynamicColumn(headerColumnIndex As Integer)
        If headerColumnIndex < 4 Then Exit Sub

        Dim dt As DataTable = TryCast(HttpContext.Current.Session("MyTable"), DataTable)
        If dt Is Nothing Then Exit Sub

        Dim dataColumnIndex As Integer = headerColumnIndex + 1
        If dataColumnIndex < 0 OrElse dataColumnIndex >= dt.Columns.Count Then Exit Sub
        If dataColumnIndex <= 4 Then Exit Sub

        dt.Columns.RemoveAt(dataColumnIndex)

        Dim level1 = HeaderLevel1
        Dim level2 = HeaderLevel2
        Dim level3 = HeaderLevel3
        Dim level4 = HeaderLevel4
        Dim level5 = HeaderLevel5
        Dim itemIds = HeaderItemIds

        RemoveHeaderValue(level1, headerColumnIndex)
        RemoveHeaderValue(level2, headerColumnIndex)
        RemoveHeaderValue(level3, headerColumnIndex)
        RemoveHeaderValue(level4, headerColumnIndex)
        RemoveHeaderValue(level5, headerColumnIndex)
        RemoveHeaderValue(itemIds, headerColumnIndex)

        HeaderLevel1 = level1
        HeaderLevel2 = level2
        HeaderLevel3 = level3
        HeaderLevel4 = level4
        HeaderLevel5 = level5
        HeaderItemIds = itemIds

        RecalculateRowProfits(dt)
        dt.AcceptChanges()
        HttpContext.Current.Session("MyTable") = dt
        clTemp.lcObject = dt

        LoadFromObject()
        RefreshSubtotal()
    End Sub

    ' -----------------------------------------------------------------------
    ' RefreshSubtotal — call after any server-side data change so litSubtotal,
    ' rptSummary, and litGrandTotal all reflect the latest data.
    ' -----------------------------------------------------------------------
    Private Sub RefreshSubtotal()
        Dim sub_total As Decimal = CalculateSubtotal()
        litSubtotal.Text = sub_total.ToString("0.000")
        BindSummaryRepeater(sub_total)
    End Sub

    ' -----------------------------------------------------------------------
    ' BindSummaryRepeater
    ' Reads every row from AddRdcTable, builds a {RowGuid, Label, Value} list
    ' for rptSummary, and writes Grand Total (sum of NetValues + Subtotal)
    ' to litGrandTotal and Label8.
    ' -----------------------------------------------------------------------
    Private Sub BindSummaryRepeater(sub_total As Decimal)
        Dim summaryRows As New List(Of Object)
        Dim netTotal As Decimal = 0D

        ' Prefer the WebMethod session copy (AddRdcTable_WM) because it receives
        ' SaveNetValue edits directly and is always up-to-date.  Fall back to
        ' ViewState only when the WM copy does not exist (e.g. first page load).
        Dim wmDt As DataTable = TryCast(HttpContext.Current.Session("AddRdcTable_WM"), DataTable)
        Dim dt As DataTable = If(wmDt IsNot Nothing, wmDt, TryCast(ViewState("AddRdcTable"), DataTable))

        If dt IsNot Nothing Then
            ' Keep both stores in sync so subsequent reads see the same data.
            ViewState("AddRdcTable") = dt
            HttpContext.Current.Session("AddRdcTable_WM") = dt

            For Each dr As DataRow In dt.Rows
                Dim rowGuid As String = Convert.ToString(dr("__RowGuid"))
                Dim label As String = Convert.ToString(dr("adjusmentName"))
                Dim netVal As Decimal = ParseDecimalValue(dr("NetValue"))
                netTotal += netVal
                summaryRows.Add(New With {.RowGuid = rowGuid, .Label = label, .Value = netVal})
            Next
        End If

        rptSummary.DataSource = summaryRows
        rptSummary.DataBind()

        Dim grandTotal As String = (sub_total + netTotal).ToString("0.000")
        litGrandTotal.Text = grandTotal
        Label8.Text = grandTotal

        Dim totalIn As Decimal = CalculateTotalIn()
        litTotalIn.Text = totalIn.ToString("0.000")
        Label10.Text = totalIn.ToString("0.000")

        If Math.Abs(sub_total + netTotal - totalIn) < 0.0005D Then
            balanceBadge.Text = "Balanced"
            balanceBadge.Style("background-color") = "#add8e6"
        Else
            balanceBadge.Text = "Unbalanced"
            balanceBadge.Style("background-color") = "#ffb3b3"
        End If
    End Sub

    ' -----------------------------------------------------------------------
    ' CalculateTotalIn — SUM(Deposit) - SUM(Debt) across all member rows.
    ' -----------------------------------------------------------------------
    Private Function CalculateTotalIn() As Decimal
        Dim dt As DataTable = TryCast(HttpContext.Current.Session("MyTable"), DataTable)
        If dt Is Nothing Then Return 0D
        If Not dt.Columns.Contains("Deposit") OrElse Not dt.Columns.Contains("Debt") Then Return 0D

        Dim totalDeposit As Decimal = 0D
        Dim totalDebt As Decimal = 0D

        For Each row As DataRow In dt.Rows
            totalDeposit += ParseDecimalValue(row("Deposit"))
            totalDebt += ParseDecimalValue(row("Debt"))
        Next

        Return totalDeposit - totalDebt
    End Function

    Private Sub RemoveHeaderValue(values As List(Of String), index As Integer)
        If values Is Nothing Then Exit Sub
        If index < 0 OrElse index >= values.Count Then Exit Sub
        values.RemoveAt(index)
    End Sub

    Private Function GetRowValue(row As DataRow, columnName As String) As String
        If row Is Nothing Then Return String.Empty
        If row.Table Is Nothing OrElse Not row.Table.Columns.Contains(columnName) Then Return String.Empty
        If row.IsNull(columnName) Then Return String.Empty
        Return Convert.ToString(row(columnName)).Trim()
    End Function

    Private Function BuildUniqueItemColumnName(dt As DataTable, itemId As String, itemTitle As String) As String
        Dim baseName As String = If(String.IsNullOrWhiteSpace(itemId), itemTitle, itemId)
        If String.IsNullOrWhiteSpace(baseName) Then
            baseName = "ItemColumn"
        End If

        Dim safeName As New StringBuilder()
        For Each ch As Char In baseName
            If Char.IsLetterOrDigit(ch) OrElse ch = "_"c Then
                safeName.Append(ch)
            Else
                safeName.Append("_")
            End If
        Next

        Dim columnName As String = safeName.ToString().Trim("_"c)
        If String.IsNullOrWhiteSpace(columnName) Then
            columnName = "ItemColumn"
        End If

        Dim uniqueName As String = columnName
        Dim suffix As Integer = 1
        While dt.Columns.Contains(uniqueName)
            uniqueName = columnName & "_" & suffix.ToString()
            suffix += 1
        End While

        Return uniqueName
    End Function

    Private Function ParseDecimalValue(value As Object) As Decimal
        If value Is Nothing OrElse value Is DBNull.Value Then Return 0D

        Dim text As String = Convert.ToString(value).Trim()
        If String.IsNullOrWhiteSpace(text) Then Return 0D

        Dim result As Decimal
        If Decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, result) Then
            Return result
        End If

        If Decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, result) Then
            Return result
        End If

        Return 0D
    End Function

    Private Sub RecalculateDynamicColumnSummaries(dt As DataTable)
        If dt Is Nothing Then Exit Sub

        Dim level2 = HeaderLevel2
        Dim level3 = HeaderLevel3
        Dim level5 = HeaderLevel5

        For dataColumnIndex As Integer = 5 To dt.Columns.Count - 1
            Dim headerIndex As Integer = dataColumnIndex - 1
            If headerIndex < 0 OrElse headerIndex >= level2.Count OrElse headerIndex >= level3.Count Then
                Continue For
            End If

            Dim quantityTotal As Decimal = 0D
            For Each row As DataRow In dt.Rows
                quantityTotal += ParseDecimalValue(row(dataColumnIndex))
            Next

            level3(headerIndex) = quantityTotal.ToString("0.###")

            Dim priceValue As Decimal = 0D
            If headerIndex >= 0 AndAlso headerIndex < level5.Count Then
                priceValue = ParseDecimalValue(level5(headerIndex))
            End If

            level2(headerIndex) = (quantityTotal * priceValue).ToString("0.###")
        Next

        HeaderLevel2 = level2
        HeaderLevel3 = level3
    End Sub

    Private Sub RecalculateRowProfits(dt As DataTable)
        If dt Is Nothing Then Exit Sub
        If Not dt.Columns.Contains("Profit") Then Exit Sub

        Dim level1 = HeaderLevel1

        For Each row As DataRow In dt.Rows
            Dim totalProfit As Decimal = 0D

            For dataColumnIndex As Integer = 5 To dt.Columns.Count - 1
                Dim headerIndex As Integer = dataColumnIndex - 1
                If headerIndex < 0 OrElse headerIndex >= level1.Count Then
                    Continue For
                End If

                Dim quantityValue As Decimal = ParseDecimalValue(row(dataColumnIndex))
                Dim profitValue As Decimal = ParseDecimalValue(level1(headerIndex))
                totalProfit += quantityValue * profitValue
            Next

            row("Profit") = totalProfit.ToString("0.000")
        Next
    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
    End Sub

    Protected Sub LinkButton3_Click(sender As Object, e As EventArgs) Handles LinkButton3.Click

        Dim selectedItems As List(Of Dictionary(Of String, Object)) =
                   TryCast(VendorPopupHelper.GetPopupReturnValue(Me, "SelectedItems"),
                    List(Of Dictionary(Of String, Object)))
        Dim DT As New DataTable
        DT = PF.ConvertSelectedItemsToDataTable(selectedItems)
        Dim DR As DataRow
        DR = DT.Rows(0)

        Label2.Text = DR.Item("Key")
        TextBox1.Text = DR.Item("Title")

        Dim SQL As String =
            "SELECT Items.ItemID AS ID, Items.Description AS Title, VendorItems.Price AS Price, '0.0' AS Profit " &
            "FROM Items INNER JOIN VendorItems ON VendorItems.ItemID = Items.ItemID " &
            "WHERE VenderID = " & Label2.Text


        Dim ListProperties As New clsListProperties
        With ListProperties
            .SQL = SQL
            .FormTitle = "Select Items"
            .ColumnHideAndShow = "YNNN"
            .EditableColumns = "NNYY"
            .ColumnsWidth = New Double() {1.0, 2.5, 1.0, 1.0}
        End With

        Dim SelectItemsParameters As String = encryNdecry.EncryptObject(Of clsListProperties)(ListProperties)
        VendorPopupHelper.RegisterVendorPopup(Me,
                                      lnkBttnAddItems,
                                      "AddMultipleItemsFromList.aspx?Parameters=" & SelectItemsParameters,
                                      600,
                                      400,
                                      PopupPlacement.Center,
                                      "Select Adj",
                                      VendorPopupHelper.PopupDisplayMode.FrameOnly)

    End Sub

    Protected Sub hfRowIndex_ValueChanged(sender As Object, e As EventArgs) Handles hfRowIndex.ValueChanged
    End Sub

    Private Property HeaderLevel1 As List(Of String)
        Get
            If Session("HeaderLevel1") Is Nothing Then
                Session("HeaderLevel1") = New List(Of String)
            End If
            Return CType(Session("HeaderLevel1"), List(Of String))
        End Get
        Set(value As List(Of String))
            Session("HeaderLevel1") = value
        End Set
    End Property

    Private Property HeaderLevel2 As List(Of String)
        Get
            If Session("HeaderLevel2") Is Nothing Then
                Session("HeaderLevel2") = New List(Of String)
            End If
            Return CType(Session("HeaderLevel2"), List(Of String))
        End Get
        Set(value As List(Of String))
            Session("HeaderLevel2") = value
        End Set
    End Property

    Private Property HeaderLevel3 As List(Of String)
        Get
            If Session("HeaderLevel3") Is Nothing Then
                Session("HeaderLevel3") = New List(Of String)
            End If
            Return CType(Session("HeaderLevel3"), List(Of String))
        End Get
        Set(value As List(Of String))
            Session("HeaderLevel3") = value
        End Set
    End Property

    Private Property HeaderLevel4 As List(Of String)
        Get
            If Session("HeaderLevel4") Is Nothing Then
                Session("HeaderLevel4") = New List(Of String)
            End If
            Return CType(Session("HeaderLevel4"), List(Of String))
        End Get
        Set(value As List(Of String))
            Session("HeaderLevel4") = value
        End Set
    End Property

    Private Property HeaderLevel5 As List(Of String)
        Get
            If Session("HeaderLevel5") Is Nothing Then
                Session("HeaderLevel5") = New List(Of String)
            End If
            Return CType(Session("HeaderLevel5"), List(Of String))
        End Get
        Set(value As List(Of String))
            Session("HeaderLevel5") = value
        End Set
    End Property

    Private Property HeaderItemIds As List(Of String)
        Get
            If Session("HeaderItemIds") Is Nothing Then
                Session("HeaderItemIds") = New List(Of String)
            End If
            Return CType(Session("HeaderItemIds"), List(Of String))
        End Get
        Set(value As List(Of String))
            Session("HeaderItemIds") = value
        End Set
    End Property

    Private Const AddRdcRowKeyColumn As String = "__RowGuid"

    Private Function EnsureAddRdcTable(table As DataTable) As DataTable
        Dim dt As DataTable = table
        If dt Is Nothing Then
            dt = New DataTable()
        End If
        If Not dt.Columns.Contains(AddRdcRowKeyColumn) Then
            dt.Columns.Add(AddRdcRowKeyColumn, GetType(String))
        End If
        If Not dt.Columns.Contains("NetValue") Then
            dt.Columns.Add("NetValue", GetType(String))
        End If
        Return dt
    End Function

    Private Function CreateAddRdcTableFromSource(source As DataTable) As DataTable
        Dim dt As DataTable = If(source IsNot Nothing, source.Clone(), New DataTable())
        Return EnsureAddRdcTable(dt)
    End Function

    Private Sub BindAddRdcGrid(dt As DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            ViewState("AddRdcTable") = Nothing
            GridView2.DataSource = Nothing
        Else
            dt.AcceptChanges()
            ViewState("AddRdcTable") = dt
            GridView2.DataSource = dt.DefaultView
        End If
        GridView2.DataBind()
    End Sub

    Protected Sub btnAddExpRdc_Click(sender As Object, e As EventArgs) Handles btnAddExpRdc.Click
        Dim returnValue As Object = VendorPopupHelper.GetPopupReturnValue(Me, "AddAdjustmentAndClose")
        If returnValue Is Nothing Then Exit Sub

        Dim selectedRow As DataRow = TryCast(returnValue, DataRow)
        If selectedRow Is Nothing Then Exit Sub

        ' Prefer the WM session copy so JS-edited NetValues survive postback.
        Dim dt As DataTable = TryCast(HttpContext.Current.Session("AddRdcTable_WM"), DataTable)
        If dt Is Nothing Then dt = TryCast(ViewState("AddRdcTable"), DataTable)
        If dt Is Nothing Then
            dt = CreateAddRdcTableFromSource(selectedRow.Table)
        Else
            dt = EnsureAddRdcTable(dt)
        End If

        Dim newRow As DataRow = dt.NewRow()
        For Each col As DataColumn In selectedRow.Table.Columns
            If dt.Columns.Contains(col.ColumnName) Then
                newRow(col.ColumnName) = selectedRow(col.ColumnName)
            End If
        Next
        newRow(AddRdcRowKeyColumn) = Guid.NewGuid().ToString("N")

        ' ── Calculate NetValue ──────────────────────────────────────────────
        ' adjusmentCalculation : "Fixed Amount" | "Percentage"
        ' adjusmentType        : "Addition"     | "Reduction"
        ' CalculationAmount    : numeric amount entered by the user
        If dt.Columns.Contains("NetValue") Then
            Dim calcType As String = Convert.ToString(newRow("adjusmentCalculation")).Trim()
            Dim incDec As String = Convert.ToString(newRow("adjusmentType")).Trim()
            Dim amount As Decimal = ParseDecimalValue(newRow("CalculationAmount"))
            Dim netVal As Decimal

            If calcType.Equals("Percentage", StringComparison.OrdinalIgnoreCase) Then
                Dim sub_total As Decimal = CalculateSubtotal()
                netVal = (amount / 100D) * sub_total
            Else   ' "Fixed Amount" or anything else
                netVal = amount
            End If

            If incDec.Equals("Reduction", StringComparison.OrdinalIgnoreCase) Then
                netVal = netVal * -1D
            End If

            newRow("NetValue") = netVal.ToString("0.000")
        End If
        ' ────────────────────────────────────────────────────────────────────

        dt.Rows.Add(newRow)

        BindAddRdcGrid(dt)
        LoadFromObject()
        RefreshSubtotal()
    End Sub

    Protected Sub GridView2_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView2.RowCommand
        If e.CommandName <> "DeleteRow" Then Exit Sub

        ' Prefer the WM session copy so JS-edited NetValues survive postback.
        Dim dt As DataTable = TryCast(HttpContext.Current.Session("AddRdcTable_WM"), DataTable)
        If dt Is Nothing Then dt = TryCast(ViewState("AddRdcTable"), DataTable)
        If dt Is Nothing Then Exit Sub
        dt = EnsureAddRdcTable(dt)

        Dim rowKey As String = Convert.ToString(e.CommandArgument)
        If String.IsNullOrWhiteSpace(rowKey) Then Exit Sub

        Dim dr As DataRow = dt.AsEnumerable().FirstOrDefault(Function(r) String.Equals(Convert.ToString(r(AddRdcRowKeyColumn)), rowKey, StringComparison.Ordinal))
        If dr Is Nothing Then Exit Sub

        dt.Rows.Remove(dr)
        BindAddRdcGrid(dt)
        RefreshSubtotal()
    End Sub

    Protected Sub lnkBtnAddMembers_Click(sender As Object, e As EventArgs) Handles lnkBtnAddMembers.Click
        Dim selectedItems As List(Of Dictionary(Of String, Object)) =
        TryCast(VendorPopupHelper.GetPopupReturnValue(Me, "SelectedItems"),
                List(Of Dictionary(Of String, Object)))

        If selectedItems Is Nothing OrElse selectedItems.Count = 0 Then Exit Sub

        Dim dt As DataTable = TryCast(HttpContext.Current.Session("MyTable"), DataTable)
        If dt Is Nothing Then Exit Sub

        For Each item As Dictionary(Of String, Object) In selectedItems
            If item Is Nothing Then Continue For

            Dim memberId As String = ""
            Dim memberName As String = ""

            If item.ContainsKey("ID") Then memberId = Convert.ToString(item("ID"))
            If item.ContainsKey("Name") Then
                memberName = Convert.ToString(item("Name"))
            ElseIf item.ContainsKey("MemberName") Then
                memberName = Convert.ToString(item("MemberName"))
            End If

            If String.IsNullOrWhiteSpace(memberId) Then Continue For

            Dim exists As Boolean = dt.AsEnumerable().
            Any(Function(r) Convert.ToString(r("MemberID")) = memberId)

            If exists Then Continue For

            Dim dr As DataRow = dt.NewRow()

            For Each dc As DataColumn In dt.Columns
                Select Case dc.ColumnName
                    Case "MemberID"
                        dr("MemberID") = memberId
                    Case "MemberName"
                        dr("MemberName") = memberName
                    Case "Deposit", "Debt", "Profit"
                        dr(dc.ColumnName) = "0.000"
                    Case Else
                        dr(dc.ColumnName) = ""
                End Select
            Next

            dt.Rows.Add(dr)
        Next

        dt.AcceptChanges()
        HttpContext.Current.Session("MyTable") = dt
        LoadFromObject()
    End Sub

    Protected Sub ImageButton2_Click(sender As Object, e As ImageClickEventArgs)
    End Sub

    Protected Sub GridView1_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand
        If e.CommandName = "DeleteDynamicColumn" Then
            Dim commandValue As String = Convert.ToString(e.CommandArgument)
            Dim commandParts() As String = commandValue.Split("|"c)
            Dim headerColumnIndex As Integer

            If commandParts.Length > 0 AndAlso Integer.TryParse(commandParts(0), headerColumnIndex) Then
                RemoveDynamicColumn(headerColumnIndex)
            End If

            Exit Sub
        End If

        If e.CommandName <> "DeleteRow" Then Exit Sub

        Dim dt As DataTable = TryCast(HttpContext.Current.Session("MyTable"), DataTable)
        If dt Is Nothing Then Exit Sub

        Dim rowId As String = Convert.ToString(e.CommandArgument)

        Dim dr As DataRow =
            dt.AsEnumerable().
               FirstOrDefault(Function(r) Convert.ToString(r("MemberID")) = rowId)

        If dr IsNot Nothing Then
            dt.Rows.Remove(dr)
            dt.AcceptChanges()
            RecalculateDynamicColumnSummaries(dt)
            HttpContext.Current.Session("MyTable") = dt
        End If

        LoadFromObject()
        RefreshSubtotal()
    End Sub

    Protected Sub LinkButton4_Click(sender As Object, e As EventArgs) Handles LinkButton4.Click
        Dim dt As DataTable =
        CType(HttpContext.Current.Session("MyTable"), DataTable)

        Dim DC As New DataColumn
        dt.Columns.Add(DC)

        clTemp.lcObject = dt

        HeaderLevel1.Add("Profit")
        HeaderLevel2.Add("Total")
        HeaderLevel3.Add("NoOfItems")
        HeaderLevel4.Add("Item")
        HeaderLevel5.Add("Price")
        HeaderItemIds.Add("")

        LoadFromObject()
    End Sub

    Protected Sub lnkBttnAddItems_Click(sender As Object, e As EventArgs) Handles lnkBttnAddItems.Click
        Dim selectedItems = TryCast(
    VendorPopupHelper.GetPopupReturnValue(Me, "SelectedItems"),
    List(Of Dictionary(Of String, Object))
)
        Dim dtSelected As DataTable = PF.ConvertSelectedItemsToDataTable(selectedItems)
        If dtSelected Is Nothing OrElse dtSelected.Rows.Count = 0 Then Exit Sub

        Dim dt As DataTable = TryCast(HttpContext.Current.Session("MyTable"), DataTable)
        If dt Is Nothing Then Exit Sub

        Dim level1 = HeaderLevel1
        Dim level2 = HeaderLevel2
        Dim level3 = HeaderLevel3
        Dim level4 = HeaderLevel4
        Dim level5 = HeaderLevel5
        Dim itemIds = HeaderItemIds
        Dim existingItemIds As New HashSet(Of String)(itemIds.Where(Function(x) Not String.IsNullOrWhiteSpace(x)), StringComparer.OrdinalIgnoreCase)

        For Each selectedRow As DataRow In dtSelected.Rows
            Dim itemId As String = GetRowValue(selectedRow, "ID")
            Dim itemTitle As String = GetRowValue(selectedRow, "Title")
            Dim itemPrice As String = GetRowValue(selectedRow, "Price")
            Dim itemProfit As String = GetRowValue(selectedRow, "Profit")

            If Not String.IsNullOrWhiteSpace(itemId) AndAlso existingItemIds.Contains(itemId) Then
                Continue For
            End If

            Dim columnName As String = BuildUniqueItemColumnName(dt, itemId, itemTitle)
            Dim newColumn As New DataColumn(columnName, GetType(String))
            newColumn.DefaultValue = String.Empty
            dt.Columns.Add(newColumn)

            For Each orderRow As DataRow In dt.Rows
                orderRow(newColumn.ColumnName) = String.Empty
            Next

            level1.Add(If(String.IsNullOrWhiteSpace(itemProfit), "0.0", itemProfit))
            level2.Add("Total")
            level3.Add("NoOfItems")
            level4.Add(If(String.IsNullOrWhiteSpace(itemTitle), columnName, itemTitle))
            level5.Add(itemPrice)
            itemIds.Add(itemId)

            If Not String.IsNullOrWhiteSpace(itemId) Then
                existingItemIds.Add(itemId)
            End If
        Next

        dt.AcceptChanges()
        HttpContext.Current.Session("MyTable") = dt
        clTemp.lcObject = dt

        HeaderLevel1 = level1
        HeaderLevel2 = level2
        HeaderLevel3 = level3
        HeaderLevel4 = level4
        HeaderLevel5 = level5
        HeaderItemIds = itemIds

        LoadFromObject()
    End Sub

    ' =======================================================================
    '  SAVE ORDER
    ' =======================================================================

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            SaveOrder()
        Catch ex As Exception
            ' Save failed silently — log or handle as needed
        End Try

        ' Re-render GridView1 (keeps Deposit/Debt values from Session)
        ' and refresh totals so nothing resets after Save.
        LoadFromObject()
        RefreshSubtotal()

        ' FIX: Re-bind GridView2 after Save so the user-edited NetValue is
        ' displayed correctly.  Without this, GridView2 keeps rendering the
        ' originally-calculated value (e.g. 0.411) even though Session and
        ' ViewState already hold the edited value (e.g. 0.410).
        ' Prefer the WebMethod session copy because SaveNetValue writes there.
        Dim adjTable As DataTable = TryCast(HttpContext.Current.Session("AddRdcTable_WM"), DataTable)
        If adjTable Is Nothing Then adjTable = TryCast(ViewState("AddRdcTable"), DataTable)
        BindAddRdcGrid(adjTable)
    End Sub

    ' -----------------------------------------------------------------------
    ' SaveOrder
    ' Writes all tables inside one OleDb transaction.
    ' Returns new OrderID on success, throws on failure.
    ' -----------------------------------------------------------------------
    Private Function SaveOrder() As Integer

        Dim conn As New OleDb.OleDbConnection(InfoDB)
        conn.Open()
        Dim trans As OleDb.OleDbTransaction = conn.BeginTransaction()

        Try
            ' -- Collect header field values ----------------------------------
            Dim venderID As Integer = 0
            Integer.TryParse(hdnSelectedVendorValue.Value, venderID)

            Dim orderDate As String = If(String.IsNullOrWhiteSpace(TextBox2.Text),
                                         Date.Today.ToString("yyyy-MM-dd"),
                                         TextBox2.Text.Trim())
            Dim orderTime As String = If(String.IsNullOrWhiteSpace(TextBox3.Text),
                                         Date.Now.ToString("HH:mm"),
                                         TextBox3.Text.Trim())

            Dim orderNumber As Object = DBNull.Value
            Dim num As Integer
            If Integer.TryParse(TextBox4.Text, num) Then orderNumber = num

            Dim grandTotal As Decimal = ParseDecimalValue(Label8.Text)
            Dim totalIn As Decimal = ParseDecimalValue(Label10.Text)
            Dim subtotal As Decimal = ParseDecimalValue(litSubtotal.Text)

            ' -- 1. INSERT Orders ---------------------------------------------
            Dim sqlOrder As String =
                "INSERT INTO Orders (VenderID,OrderDate,OrderTime,OrderNumber," &
                "Subtotal,GrandTotal,TotalIn,Status,CreatedAt,UpdatedAt) " &
                "VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,Now(),Now())"

            Dim cmdOrder As New OleDb.OleDbCommand(sqlOrder, conn, trans)
            cmdOrder.Parameters.AddWithValue("@p1", If(venderID = 0, DBNull.Value, CObj(venderID)))
            cmdOrder.Parameters.AddWithValue("@p2", orderDate)
            cmdOrder.Parameters.AddWithValue("@p3", orderTime)
            cmdOrder.Parameters.AddWithValue("@p4", orderNumber)
            cmdOrder.Parameters.AddWithValue("@p5", subtotal)
            cmdOrder.Parameters.AddWithValue("@p6", grandTotal)
            cmdOrder.Parameters.AddWithValue("@p7", totalIn)
            cmdOrder.Parameters.AddWithValue("@p8", "Saved")
            cmdOrder.ExecuteNonQuery()

            Dim orderID As Integer = Convert.ToInt32(
                New OleDb.OleDbCommand("SELECT @@IDENTITY", conn, trans).ExecuteScalar())

            ' -- 2. INSERT OrderItems (one per dynamic column) ----------------
            Dim myTable As DataTable = TryCast(HttpContext.Current.Session("MyTable"), DataTable)
            Dim orderItemIDs As New Dictionary(Of String, Integer)

            If myTable IsNot Nothing Then
                Dim itemColNames As New List(Of String)
                For ci As Integer = 5 To myTable.Columns.Count - 1
                    itemColNames.Add(myTable.Columns(ci).ColumnName)
                Next

                For ci As Integer = 0 To itemColNames.Count - 1
                    Dim colName As String = itemColNames(ci)
                    Dim displayName As String = If(ci < HeaderLevel4.Count, HeaderLevel4(ci), colName)
                    Dim price As Decimal = ParseDecimalValue(If(ci < HeaderLevel5.Count, HeaderLevel5(ci), "0"))
                    Dim profit As Decimal = ParseDecimalValue(If(ci < HeaderLevel1.Count, HeaderLevel1(ci), "0"))
                    Dim noOfItems As Decimal = ParseDecimalValue(If(ci < HeaderLevel3.Count, HeaderLevel3(ci), "0"))
                    Dim total As Decimal = ParseDecimalValue(If(ci < HeaderLevel2.Count, HeaderLevel2(ci), "0"))
                    Dim itemIdStr As String = If(ci < HeaderItemIds.Count, HeaderItemIds(ci), "")
                    Dim itemID As Object = DBNull.Value
                    Dim parsedItemID As Integer
                    If Integer.TryParse(itemIdStr, parsedItemID) Then itemID = parsedItemID

                    Dim sqlOI As String =
                        "INSERT INTO OrderItems " &
                        "(OrderID,ItemID,DisplayName,Price,Profit,NoOfItems,[Total],SortOrder) " &
                        "VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8)"
                    Dim cmdOI As New OleDb.OleDbCommand(sqlOI, conn, trans)
                    cmdOI.Parameters.AddWithValue("@p1", orderID)
                    cmdOI.Parameters.AddWithValue("@p2", itemID)
                    cmdOI.Parameters.AddWithValue("@p3", displayName)
                    cmdOI.Parameters.AddWithValue("@p4", price)
                    cmdOI.Parameters.AddWithValue("@p5", profit)
                    cmdOI.Parameters.AddWithValue("@p6", noOfItems)
                    cmdOI.Parameters.AddWithValue("@p7", total)
                    cmdOI.Parameters.AddWithValue("@p8", ci)
                    cmdOI.ExecuteNonQuery()

                    orderItemIDs(colName) = Convert.ToInt32(
                        New OleDb.OleDbCommand("SELECT @@IDENTITY", conn, trans).ExecuteScalar())
                Next

                ' -- 3. INSERT OrderMembers + OrderMemberItems ----------------
                Dim sortMember As Integer = 0
                For Each dr As DataRow In myTable.Rows
                    Dim memberID As Integer = 0
                    Integer.TryParse(Convert.ToString(dr("MemberID")), memberID)

                    Dim sqlOM As String =
                        "INSERT INTO OrderMembers " &
                        "(OrderID,MemberID,Deposit,Debt,Profit,SortOrder) " &
                        "VALUES (@p1,@p2,@p3,@p4,@p5,@p6)"
                    Dim cmdOM As New OleDb.OleDbCommand(sqlOM, conn, trans)
                    cmdOM.Parameters.AddWithValue("@p1", orderID)
                    cmdOM.Parameters.AddWithValue("@p2", If(memberID = 0, DBNull.Value, CObj(memberID)))
                    cmdOM.Parameters.AddWithValue("@p3", ParseDecimalValue(dr("Deposit")))
                    cmdOM.Parameters.AddWithValue("@p4", ParseDecimalValue(dr("Debt")))
                    cmdOM.Parameters.AddWithValue("@p5", ParseDecimalValue(dr("Profit")))
                    cmdOM.Parameters.AddWithValue("@p6", sortMember)
                    cmdOM.ExecuteNonQuery()
                    sortMember += 1

                    Dim orderMemberID As Integer = Convert.ToInt32(
                        New OleDb.OleDbCommand("SELECT @@IDENTITY", conn, trans).ExecuteScalar())

                    For Each colName As String In itemColNames
                        If Not myTable.Columns.Contains(colName) Then Continue For
                        Dim qty As Decimal = ParseDecimalValue(dr(colName))
                        If qty = 0D Then Continue For
                        Dim oiID As Integer = 0
                        If Not orderItemIDs.TryGetValue(colName, oiID) Then Continue For

                        Dim sqlOMI As String =
                            "INSERT INTO OrderMemberItems " &
                            "(OrderMemberID,OrderItemID,Quantity) " &
                            "VALUES (@p1,@p2,@p3)"
                        Dim cmdOMI As New OleDb.OleDbCommand(sqlOMI, conn, trans)
                        cmdOMI.Parameters.AddWithValue("@p1", orderMemberID)
                        cmdOMI.Parameters.AddWithValue("@p2", oiID)
                        cmdOMI.Parameters.AddWithValue("@p3", qty)
                        cmdOMI.ExecuteNonQuery()
                    Next
                Next
            End If

            ' -- 4. INSERT OrderAdjustments -----------------------------------
            ' Prefer Session("AddRdcTable_WM") because SaveNetValue (WebMethod) writes
            ' JS-edited NetValues directly there.  ViewState may not reflect those edits
            ' if the postback ViewState sync in Page_Load was bypassed or stale.
            Dim addRdcTable As DataTable = TryCast(HttpContext.Current.Session("AddRdcTable_WM"), DataTable)
            If addRdcTable Is Nothing Then
                addRdcTable = TryCast(ViewState("AddRdcTable"), DataTable)
            End If
            If addRdcTable IsNot Nothing Then
                Dim sortAdj As Integer = 0
                For Each dr As DataRow In addRdcTable.Rows
                    Dim distrib As Object = DBNull.Value
                    If dr.Table.Columns.Contains("Distribution") Then
                        distrib = Convert.ToString(dr("Distribution"))
                    End If

                    Dim sqlOA As String =
                        "INSERT INTO OrderAdjustments " &
                        "(OrderID,AdjustmentName,AdjustmentType,AdjustmentCalc," &
                        "CalculationAmount,Distribution,NetValue,SortOrder) " &
                        "VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8)"
                    Dim cmdOA As New OleDb.OleDbCommand(sqlOA, conn, trans)
                    cmdOA.Parameters.AddWithValue("@p1", orderID)
                    cmdOA.Parameters.AddWithValue("@p2", Convert.ToString(dr("adjusmentName")))
                    cmdOA.Parameters.AddWithValue("@p3", Convert.ToString(dr("adjusmentType")))
                    cmdOA.Parameters.AddWithValue("@p4", Convert.ToString(dr("adjusmentCalculation")))
                    cmdOA.Parameters.AddWithValue("@p5", ParseDecimalValue(dr("CalculationAmount")))
                    cmdOA.Parameters.AddWithValue("@p6", distrib)
                    cmdOA.Parameters.AddWithValue("@p7", ParseDecimalValue(dr("NetValue")))
                    cmdOA.Parameters.AddWithValue("@p8", sortAdj)
                    cmdOA.ExecuteNonQuery()
                    sortAdj += 1
                Next
            End If

            ' -- Commit -------------------------------------------------------
            trans.Commit()
            conn.Close()
            Return orderID

        Catch ex As Exception
            trans.Rollback()
            conn.Close()
            Throw
        End Try

    End Function

    ' =======================================================================
    '  LOAD ORDER
    ' =======================================================================

    Protected Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        Try
            LoadOrder(2)
        Catch ex As Exception
            ClientScript.RegisterStartupScript(Me.GetType(), "loadEx",
                "alert('Error loading order: " & ex.Message.Replace("'", "") & "');", True)
        End Try
    End Sub

    ' -----------------------------------------------------------------------
    ' LoadOrder
    ' Reads Orders, OrderItems, OrderMembers and OrderMemberItems for the
    ' given OrderID and rebuilds the in-session state exactly as if the user
    ' had built the order from scratch in NewOrder.
    ' -----------------------------------------------------------------------
    Private Sub LoadOrder(orderID As Integer)

        Dim conn As New OleDb.OleDbConnection(InfoDB)
        conn.Open()

        Try
            ' -- 1. Load the Orders header row --------------------------------
            Dim sqlHdr As String =
                "SELECT VenderID, OrderDate, OrderTime, OrderNumber, " &
                "Subtotal, GrandTotal, TotalIn FROM Orders WHERE OrderID = " & orderID
            Dim dtHdr As DataTable = GetDataTable(InfoDB, sqlHdr)
            If dtHdr Is Nothing OrElse dtHdr.Rows.Count = 0 Then
                ClientScript.RegisterStartupScript(Me.GetType(), "loadNotFound",
                    "alert('Order " & orderID & " not found.');", True)
                conn.Close()
                Exit Sub
            End If
            Dim hdrRow As DataRow = dtHdr.Rows(0)

            ' Populate header controls
            hdnSelectedVendorValue.Value = Convert.ToString(hdrRow("VenderID"))
            TextBox2.Text = Convert.ToString(hdrRow("OrderDate"))
            TextBox3.Text = Convert.ToString(hdrRow("OrderTime"))
            TextBox4.Text = Convert.ToString(hdrRow("OrderNumber"))

            ' -- 2. Load OrderItems (columns) ---------------------------------
            Dim sqlItems As String =
                "SELECT OrderItemID, ItemID, DisplayName, Price, Profit, " &
                "NoOfItems, [Total], SortOrder " &
                "FROM OrderItems WHERE OrderID = " & orderID & " ORDER BY SortOrder"
            Dim dtItems As DataTable = GetDataTable(InfoDB, sqlItems)

            ' -- 3. Load OrderMembers -----------------------------------------
            Dim sqlMembers As String =
                "SELECT om.OrderMemberID, om.MemberID, m.MemberName, " &
                "om.Deposit, om.Debt, om.Profit, om.SortOrder " &
                "FROM OrderMembers om " &
                "INNER JOIN Members m ON m.ID = om.MemberID " &
                "WHERE om.OrderID = " & orderID & " ORDER BY om.SortOrder"
            Dim dtMembers As DataTable = GetDataTable(InfoDB, sqlMembers)

            ' -- 4. Load OrderMemberItems (quantities) ------------------------
            Dim sqlQty As String =
                "SELECT omi.OrderMemberID, omi.OrderItemID, omi.Quantity " &
                "FROM OrderMemberItems omi " &
                "INNER JOIN OrderMembers om ON om.OrderMemberID = omi.OrderMemberID " &
                "WHERE om.OrderID = " & orderID
            Dim dtQty As DataTable = GetDataTable(InfoDB, sqlQty)

            ' -- 5. Load OrderAdjustments (GridView2) -------------------------
            Dim sqlAdj As String =
                "SELECT AdjustmentName AS adjusmentName, AdjustmentType AS adjusmentType, " &
                "AdjustmentCalc AS adjusmentCalculation, CalculationAmount, " &
                "Distribution, NetValue, SortOrder " &
                "FROM OrderAdjustments WHERE OrderID = " & orderID & " ORDER BY SortOrder"
            Dim dtAdj As DataTable = GetDataTable(InfoDB, sqlAdj)

            conn.Close()

            ' ----------------------------------------------------------------
            ' Rebuild MyTable in Session
            ' ----------------------------------------------------------------
            Dim myTable As New DataTable()
            myTable.Columns.Add("MemberID", GetType(String))
            myTable.Columns.Add("MemberName", GetType(String))
            myTable.Columns.Add("Deposit", GetType(String))
            myTable.Columns.Add("Debt", GetType(String))
            myTable.Columns.Add("Profit", GetType(String))

            ' Add one column per OrderItem
            Dim itemColNames As New List(Of String)
            Dim level1 As New List(Of String)
            Dim level2 As New List(Of String)
            Dim level3 As New List(Of String)
            Dim level4 As New List(Of String)
            Dim level5 As New List(Of String)
            Dim itemIds As New List(Of String)

            If dtItems IsNot Nothing Then
                For Each itemRow As DataRow In dtItems.Rows
                    Dim colName As String = BuildUniqueItemColumnName(myTable,
                        Convert.ToString(itemRow("ItemID")),
                        Convert.ToString(itemRow("DisplayName")))
                    myTable.Columns.Add(colName, GetType(String))
                    itemColNames.Add(colName)

                    level1.Add(Convert.ToString(itemRow("Profit")))
                    level2.Add(Convert.ToString(itemRow("Total")))
                    level3.Add(Convert.ToString(itemRow("NoOfItems")))
                    level4.Add(Convert.ToString(itemRow("DisplayName")))
                    level5.Add(Convert.ToString(itemRow("Price")))
                    itemIds.Add(Convert.ToString(itemRow("ItemID")))
                Next
            End If

            ' Add one row per OrderMember
            If dtMembers IsNot Nothing Then
                For Each memRow As DataRow In dtMembers.Rows
                    Dim dr As DataRow = myTable.NewRow()
                    dr("MemberID") = Convert.ToString(memRow("MemberID"))
                    dr("MemberName") = Convert.ToString(memRow("MemberName"))
                    dr("Deposit") = Convert.ToString(memRow("Deposit"))
                    dr("Debt") = Convert.ToString(memRow("Debt"))
                    dr("Profit") = Convert.ToString(memRow("Profit"))

                    Dim omID As Integer = Convert.ToInt32(memRow("OrderMemberID"))

                    If dtItems IsNot Nothing AndAlso dtQty IsNot Nothing Then
                        For ci As Integer = 0 To itemColNames.Count - 1
                            Dim oiID As Integer = Convert.ToInt32(dtItems.Rows(ci)("OrderItemID"))

                            Dim qtyRow As DataRow = dtQty.AsEnumerable().FirstOrDefault(
                                Function(r) Convert.ToInt32(r("OrderMemberID")) = omID AndAlso
                                            Convert.ToInt32(r("OrderItemID")) = oiID)

                            dr(itemColNames(ci)) = If(qtyRow IsNot Nothing,
                                Convert.ToString(qtyRow("Quantity")), "")
                        Next
                    End If

                    myTable.Rows.Add(dr)
                Next
            End If

            myTable.AcceptChanges()
            HttpContext.Current.Session("MyTable") = myTable
            clTemp.lcObject = myTable

            ' Persist header lists to Session
            Session("HeaderLevel1") = level1
            Session("HeaderLevel2") = level2
            Session("HeaderLevel3") = level3
            Session("HeaderLevel4") = level4
            Session("HeaderLevel5") = level5
            Session("HeaderItemIds") = itemIds

            ' Rebuild Adjustments table (GridView2)
            If dtAdj IsNot Nothing AndAlso dtAdj.Rows.Count > 0 Then
                Dim adjTable As New DataTable()
                For Each col As DataColumn In dtAdj.Columns
                    adjTable.Columns.Add(col.ColumnName, col.DataType)
                Next
                adjTable = EnsureAddRdcTable(adjTable)
                For Each adjRow As DataRow In dtAdj.Rows
                    Dim nr As DataRow = adjTable.NewRow()
                    For Each col As DataColumn In dtAdj.Columns
                        If adjTable.Columns.Contains(col.ColumnName) Then
                            nr(col.ColumnName) = adjRow(col.ColumnName)
                        End If
                    Next
                    nr("__RowGuid") = Guid.NewGuid().ToString("N")
                    adjTable.Rows.Add(nr)
                Next
                BindAddRdcGrid(adjTable)
            Else
                BindAddRdcGrid(Nothing)
            End If

            ' Rebuild GridView1 and refresh totals
            LoadFromObject()
            RefreshSubtotal()

            ClientScript.RegisterStartupScript(Me.GetType(), "loadOK",
                "alert('Order " & orderID & " loaded successfully.');", True)

        Catch ex As Exception
            conn.Close()
            Throw
        End Try

    End Sub

End Class


Public Class ImageButtonTemplate
    Implements ITemplate

    Private ReadOnly _idColumn As String

    Public Sub New(idColumn As String)
        _idColumn = idColumn
    End Sub

    Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
        Dim imgBtn As New ImageButton()
        imgBtn.ID = "ImageButton2"
        imgBtn.ImageUrl = "~/Images/Trash16x16.png"
        imgBtn.CausesValidation = False
        imgBtn.ToolTip = "Delete"
        imgBtn.CommandName = "DeleteRow"

        AddHandler imgBtn.DataBinding,
            Sub(sender As Object, e As EventArgs)
                Dim btn = CType(sender, ImageButton)
                Dim row = CType(btn.NamingContainer, GridViewRow)
                Dim idObj = DataBinder.Eval(row.DataItem, _idColumn)
                Dim rowId As String = If(idObj Is DBNull.Value, "", idObj.ToString())

                btn.CommandArgument = rowId
                btn.Attributes("data-id") = rowId
            End Sub

        container.Controls.Add(imgBtn)
    End Sub
End Class


Public Class EditableTemplate
    Implements ITemplate

    Private _columnName As String
    Private _columnIndex As String

    Public Sub New(columnName As String, colIndex As String)
        _columnName = columnName
        _columnIndex = colIndex
    End Sub

    Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn

        Dim wrapper As New HtmlGenericControl("div")
        wrapper.Attributes("class") = "cell-wrapper data-cell"
        wrapper.Attributes("onclick") = "editCell(this)"
        wrapper.Attributes("data-column") = _columnName
        wrapper.Attributes("data-columnindex") = _columnIndex.ToString()

        Dim lbl As New Label()
        Dim txt As New TextBox()
        Dim hfVis As New HiddenField()

        lbl.ID = "lblValue"
        txt.ID = "txtValue"
        hfVis.ID = "hfVis"
        hfVis.Value = "0"

        txt.Style("display") = "none"
        txt.Style("width") = "85%"
        txt.Attributes("onblur") = "saveCell(this)"
        txt.Attributes("onkeydown") = "return handleEnter(event, this);"

        AddHandler wrapper.DataBinding, Sub(sender As Object, e As EventArgs)

                                            Dim w = CType(sender, HtmlGenericControl)
                                            Dim row = CType(w.NamingContainer, GridViewRow)
                                            Dim rowIndex As Integer = row.RowIndex

                                            lbl.ID = "lblValue_" & rowIndex & "_" & _columnIndex
                                            txt.ID = "txtValue_" & rowIndex & "_" & _columnIndex
                                            hfVis.ID = "hfVis_" & rowIndex & "_" & _columnIndex

                                            Dim valueObj = DataBinder.Eval(row.DataItem, _columnName)
                                            Dim value As String = If(valueObj Is DBNull.Value, "", valueObj.ToString())

                                            lbl.Text = value
                                            txt.Text = value

                                            w.Attributes("data-rowindex") = rowIndex.ToString()
                                            w.Attributes("data-column") = _columnName

                                        End Sub

        wrapper.Controls.Add(lbl)
        wrapper.Controls.Add(txt)
        wrapper.Controls.Add(hfVis)
        container.Controls.Add(wrapper)

    End Sub
End Class