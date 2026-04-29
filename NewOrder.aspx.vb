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

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If HttpContext.Current.Session("MyTable") Is Nothing Then
            CreateInitialTable()
        End If

        LoadFromObject()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'AddJQueryLinks(Page, True)

        If IsPostBack Then
            PersistPostedGridValues()
        End If

        If Not String.IsNullOrEmpty(hdnSelectedVendorText.Value) Then
            TextBox1.Text = hdnSelectedVendorText.Value
        End If



        'Dim arrSelectVendersParameters() As String = {", "Select A Vender"}
        'Dim SelectVendersParameters As String = encryNdecry.Encrypt(arrSelectVendersParameters)

        Dim ListParameters As New clsListProperties
        ListParameters.SQL = "Select ID as [Key], VenderName as Title, Whatsapp as Phone from Venders"
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

        'Array: SQL to select Members, Title of the Page, HideID Y/N
        'Dim arrSelectMembersParameters() As String = {"", "", "NN"}


        Dim MemberListParameters As New clsListProperties
        With MemberListParameters
            .SQL = "Select ID, MemberName as [Name] from Members order by CInt(NoOfMovement) desc"
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
        DT = GetDataTable(InfoDB, " SELECT ID, MemberName,'0.000' as Deposit, '0.000' as Debt,'0.000' as Profit  FROM Members WHERE MemberName IN (" &
    "'Fatima AlHaddad'," &
    "'Fatima Mohammed'," &
    "'Elmeera'," &
    "'Roqaya'," &
    "'Jalal'," &
    "'Safa Shamsan','Areej') order by ID;")

        'DT.Rows.Add(DR)
        HttpContext.Current.Session("MyTable") = DT
    End Sub




    Sub LoadFromObject()
        'Dim DT As New Data.DataTable
        'DT = CType(clTemp.lcObject, DataTable)

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
    <WebMethod()>
    <ScriptMethod()>
    Public Shared Sub SaveCell(rowIndex As Integer, columnName As String, value As String)

        Dim dt As DataTable =
        CType(HttpContext.Current.Session("MyTable"), DataTable)

        dt.Rows(rowIndex)(columnName) = value

        dt.AcceptChanges()

        HttpContext.Current.Session("MyTable") = dt
    End Sub


    Private Sub BuildGrid(ByVal DT As DataTable)

        GridView1.Columns.Clear()

        Dim actionField As New TemplateField()
        actionField.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        actionField.ItemStyle.Width = Unit.Pixel(35)
        actionField.ItemTemplate = New ImageButtonTemplate("ID")
        GridView1.Columns.Add(actionField)

        Dim colIndex As Integer = 0
        For Each dc As DataColumn In DT.Columns
            If dc.ColumnName = "ID" Then Continue For

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
        Dim dt As DataTable = TryCast(HttpContext.Current.Session("MyTable"), DataTable)
        If dt Is Nothing Then Exit Sub
        If GridView1.Rows.Count = 0 Then Exit Sub

        Dim colIndex As Integer = 0

        For Each dc As DataColumn In dt.Columns
            If dc.ColumnName = "ID" Then Continue For

            For Each row As GridViewRow In GridView1.Rows
                If row.RowType <> DataControlRowType.DataRow Then Continue For
                If row.RowIndex < 0 OrElse row.RowIndex >= dt.Rows.Count Then Continue For

                Dim txt As TextBox = TryCast(row.FindControl("txtValue_" & row.RowIndex & "_" & colIndex), TextBox)
                If txt IsNot Nothing Then
                    dt.Rows(row.RowIndex)(dc.ColumnName) = txt.Text
                End If
            Next

            colIndex += 1
        Next

        RecalculateDynamicColumnSummaries(dt)
        RecalculateRowProfits(dt)
        dt.AcceptChanges()

        HttpContext.Current.Session("MyTable") = dt
        clTemp.lcObject = dt
    End Sub


    'Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
    '    'Dim DT As New Data.DataTable
    '    'DT = CType(clTemp.lcObject, DataTable)
    '    Dim dt As DataTable =
    '    CType(HttpContext.Current.Session("MyTable"), DataTable)

    '    Dim DC As New DataColumn
    '    dt.Columns.Add(DC)

    '    clTemp.lcObject = dt

    '    HeaderLevel1.Add("Profit")
    '    HeaderLevel2.Add("Total")
    '    HeaderLevel3.Add("NoOfItems")
    '    HeaderLevel4.Add("Item")
    '    HeaderLevel5.Add("Price")


    '    LoadFromObject()

    '+End Sub

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

            ' Only for grouped column(s)
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
            ' Fifthe HEADER ROW
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


    <WebMethod()>
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

        ' ===== FIXED CELL WIDTH =====
        cell.Width = Unit.Pixel(100)
        cell.HorizontalAlign = HorizontalAlign.Center
        cell.VerticalAlign = VerticalAlign.Middle

        ' ===== STYLE BASED ON LEVEL =====
        Select Case level

            Case 1 ' Profit
                cell.BackColor = Drawing.Color.Orange
                cell.ForeColor = Drawing.Color.Black

            Case 2 ' Total
                cell.BackColor = Drawing.Color.Yellow
                cell.ForeColor = Drawing.Color.Black

            Case 4 ' Price
                cell.BackColor = Drawing.Color.Black
                cell.ForeColor = Drawing.Color.White

        End Select


        ' Wrapper
        Dim wrapper As New HtmlGenericControl("div")
        wrapper.Attributes("class") = "cell-wrapper"
        wrapper.Attributes("onclick") = "editCell(this)"
        wrapper.Attributes("data-columnindex") = colIndex.ToString()
        wrapper.Attributes("data-level") = level.ToString()
        wrapper.Attributes("data-headercol") = colIndex.ToString()
        wrapper.Attributes("data-headerlevel") = level.ToString()
        wrapper.Style("width") = "100%"
        wrapper.Style("text-align") = "center"

        ' Label
        Dim lbl As New Label()
        lbl.ID = "lblHeader_" & level & "_" & colIndex
        lbl.Text = text
        lbl.ForeColor = cell.ForeColor
        lbl.Style("display") = "inline-block"
        lbl.Style("width") = "100%"
        lbl.Style("text-align") = "center"

        ' TextBox
        Dim txt As New TextBox()
        txt.ID = "txtHeader_" & level & "_" & colIndex
        txt.Text = text
        txt.Style("display") = "none"
        txt.Width = Unit.Pixel(90)   ' ===== TEXTBOX WIDTH 90px =====
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
    End Sub

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

        'Dim A() As String {"A","B","C"}

        'Dim B As String = encryNdecry.Encrypt(A()))

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

        Dim SQL As String = " Select  "
        SQL = SQL + vbCrLf + " Items.ID as ID , "
        SQL = SQL + vbCrLf + " Items.Description As Title, "
        SQL = SQL + vbCrLf + " VenderItems.Price AS Price, "
        SQL = SQL + vbCrLf + " '0.0' AS Profit "
        SQL = SQL + vbCrLf + " FROM "
        SQL = SQL + vbCrLf + " Items "
        SQL = SQL + vbCrLf + " INNER Join VenderItems ON Items.ID = VenderItems.ItemID "
        SQL = SQL + vbCrLf + " WHERE "
        SQL = SQL + vbCrLf + " VenderItems.VenderID = '" & Label2.Text & "'"

        Dim ListProperties As New clsListProperties
        With ListProperties
            .SQL = SQL
            .FormTitle = "Select Items"
            .ColumnHideAndShow = "YNNN"
            .EditableColumns = "NNYY"
            .ColumnsWidth = New Double() {1.0, 2.5, 1.0, 1.0}
        End With


        'Array: SQL to select items, Title of the Page, HideID Y/N
        'Dim arrSelectItemsParameters() As String = {SQL, "Select Items", "YNNN", "NNYY"}
        'Dim SelectItemsParameters As String = encryNdecry.Encrypt(arrSelectItemsParameters)
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
        'MsgBox(hfRowIndex.Value)
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

        Dim dt As DataTable = TryCast(ViewState("AddRdcTable"), DataTable)
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
        dt.Rows.Add(newRow)

        BindAddRdcGrid(dt)
        LoadFromObject()

        'TextBox5.Text = String.Format("{0} - {1} ({2}: {3})", selectedRow.Item(0).ToString, selectedRow.Item(1).ToString, selectedRow.Item(2).ToString, selectedRow.Item(3).ToString)
    End Sub

    Protected Sub GridView2_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView2.RowCommand
        If e.CommandName <> "DeleteRow" Then Exit Sub

        Dim dt As DataTable = TryCast(ViewState("AddRdcTable"), DataTable)
        If dt Is Nothing Then Exit Sub
        dt = EnsureAddRdcTable(dt)

        Dim rowKey As String = Convert.ToString(e.CommandArgument)
        If String.IsNullOrWhiteSpace(rowKey) Then Exit Sub

        Dim dr As DataRow = dt.AsEnumerable().FirstOrDefault(Function(r) String.Equals(Convert.ToString(r(AddRdcRowKeyColumn)), rowKey, StringComparison.Ordinal))
        If dr Is Nothing Then Exit Sub

        dt.Rows.Remove(dr)
        BindAddRdcGrid(dt)
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
            Any(Function(r) Convert.ToString(r("ID")) = memberId)

            If exists Then Continue For

            Dim dr As DataRow = dt.NewRow()

            For Each dc As DataColumn In dt.Columns
                Select Case dc.ColumnName
                    Case "ID"
                        dr("ID") = memberId
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
               FirstOrDefault(Function(r) Convert.ToString(r("ID")) = rowId)

        If dr IsNot Nothing Then
            dt.Rows.Remove(dr)
            dt.AcceptChanges()
            RecalculateDynamicColumnSummaries(dt)
            HttpContext.Current.Session("MyTable") = dt
        End If

        LoadFromObject()
    End Sub

    Protected Sub LinkButton4_Click(sender As Object, e As EventArgs) Handles LinkButton4.Click
        'Dim DT As New Data.DataTable
        'DT = CType(clTemp.lcObject, DataTable)
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

        ' Create wrapper div
        Dim wrapper As New HtmlGenericControl("div")
        wrapper.Attributes("class") = "cell-wrapper data-cell"
        wrapper.Attributes("onclick") = "editCell(this)"
        wrapper.Attributes("data-column") = _columnName
        wrapper.Attributes("data-columnindex") = _columnIndex.ToString()

        ' Create label and textbox
        Dim lbl As New Label()
        Dim txt As New TextBox()

        ' Temporarily assign generic IDs; will update in DataBinding
        lbl.ID = "lblValue"
        txt.ID = "txtValue"

        ' TextBox style & events
        txt.Style("display") = "none"
        txt.Style("width") = "85%"
        txt.Attributes("onblur") = "saveCell(this)"
        txt.Attributes("onkeydown") = "return handleEnter(event, this);"

        ' Bind EVERYTHING in ONE place
        AddHandler wrapper.DataBinding, Sub(sender As Object, e As EventArgs)

                                            Dim w = CType(sender, HtmlGenericControl)
                                            Dim row = CType(w.NamingContainer, GridViewRow)
                                            Dim rowIndex As Integer = row.RowIndex

                                            ' --- Generate unique IDs ---
                                            lbl.ID = "lblValue_" & rowIndex & "_" & _columnIndex
                                            txt.ID = "txtValue_" & rowIndex & "_" & _columnIndex

                                            ' Get the value from the DataItem
                                            Dim valueObj = DataBinder.Eval(row.DataItem, _columnName)
                                            Dim value As String = If(valueObj Is DBNull.Value, "", valueObj.ToString())

                                            lbl.Text = value
                                            txt.Text = value

                                            ' Store row/column info for JavaScript
                                            w.Attributes("data-rowindex") = rowIndex.ToString()
                                            w.Attributes("data-column") = _columnName

                                        End Sub

        ' Add controls to wrapper
        wrapper.Controls.Add(lbl)
        wrapper.Controls.Add(txt)
        container.Controls.Add(wrapper)

    End Sub
End Class




