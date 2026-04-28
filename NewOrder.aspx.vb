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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'AddJQueryLinks(Page, True)

        If Not Page.IsPostBack Then
            CreateInitialTable()
        End If

        If Not String.IsNullOrEmpty(hdnSelectedVendorText.Value) Then
            TextBox1.Text = hdnSelectedVendorText.Value
        End If

        LoadFromObject()



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
        Dim arrSelectMembersParameters() As String = {"Select ID, MemberName as [Name] from Members order by CInt(NoOfMovement) desc", "Select Members", "NN"}
        Dim SelectMembersParameters As String = encryNdecry.Encrypt(arrSelectMembersParameters)
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


            table.Rows.AddAt(0, h1)
            table.Rows.AddAt(1, h2)
            table.Rows.AddAt(2, h3)
            table.Rows.AddAt(3, h4)
            table.Rows.AddAt(4, h5)

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


    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        'Dim A() As String {"A","B","C"}

        'Dim B As String = encryNdecry.Encrypt(A()))

    End Sub

    Protected Sub LinkButton3_Click(sender As Object, e As EventArgs) Handles LinkButton3.Click
        'Dim selectedVendorValue As String = hdnSelectedVendorValue.Value
        'Dim selectedVendorText As String = hdnSelectedVendorText.Value

        'TextBox1.Text = selectedVendorText
        'Label6.Text = hdnSelectedVendorText.Value

        ' Add your vendor-related server-side logic here.
        ' Example:
        ' Label2.Text = "Selected vendor: " & selectedVendorText & " (" & selectedVendorValue & ")"

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
    Protected Sub btnAddExpRdc_Click(sender As Object, e As EventArgs) Handles btnAddExpRdc.Click
        Dim returnValue As Object = VendorPopupHelper.GetPopupReturnValue(Me, "AddAdjustmentAndClose")

        If returnValue Is Nothing Then Exit Sub

        Dim selectedRow As DataRow = TryCast(returnValue, DataRow)
        If selectedRow Is Nothing Then Exit Sub
        Dim DT As New DataTable
        If ViewState("AddRdcTable") Is Nothing Then
            ViewState("AddRdcTable") = selectedRow.Table.DefaultView
            DT = TryCast(selectedRow.Table, DataTable)
        Else
            DT = TryCast(ViewState("AddRdcTable"), DataTable)
            DT.Merge(TryCast(selectedRow.Table, DataTable))

        End If
        DT.AcceptChanges()
        ViewState("AddRdcTable") = DT


        GridView2.DataSource = DT.DefaultView
        GridView2.DataBind()

        'TextBox5.Text = String.Format("{0} - {1} ({2}: {3})", selectedRow.Item(0).ToString, selectedRow.Item(1).ToString, selectedRow.Item(2).ToString, selectedRow.Item(3).ToString)
    End Sub

    Protected Sub GridView2_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView2.RowCommand
        If e.CommandName <> "DeleteRow" Then Exit Sub

        Dim dt As DataTable = TryCast(ViewState("AddRdcTable"), DataTable)
        If dt Is Nothing Then Exit Sub

        Dim rowIndex As Integer
        If Not Integer.TryParse(Convert.ToString(e.CommandArgument), rowIndex) Then Exit Sub

        If rowIndex >= 0 AndAlso rowIndex < dt.Rows.Count Then
            dt.Rows.RemoveAt(rowIndex)
            dt.AcceptChanges()
        End If

        If dt.Rows.Count = 0 Then
            ViewState("AddRdcTable") = Nothing
            GridView2.DataSource = Nothing
        Else
            ViewState("AddRdcTable") = dt
            GridView2.DataSource = dt.DefaultView
        End If

        GridView2.DataBind()
    End Sub


    Protected Sub lnkBtnAddMembers_Click(sender As Object, e As EventArgs) Handles lnkBtnAddMembers.Click
        Dim returnValue As Object = VendorPopupHelper.GetPopupReturnValue(Me, "SelectedMembers")
        Dim L As List(Of ListItem) = TryCast(returnValue, List(Of ListItem))

        If L Is Nothing OrElse L.Count = 0 Then Exit Sub

        Dim dt As DataTable = TryCast(HttpContext.Current.Session("MyTable"), DataTable)
        If dt Is Nothing Then Exit Sub

        For Each li As ListItem In L
            If li Is Nothing Then Continue For

            ' Skip if member already exists
            Dim exists As Boolean = dt.AsEnumerable().
            Any(Function(r) r("ID").ToString() = li.Value)

            If exists Then Continue For

            Dim dr As DataRow = dt.NewRow()

            For Each dc As DataColumn In dt.Columns
                Select Case dc.ColumnName
                    Case "ID"
                        dr("ID") = li.Value              ' ListItem.Value
                    Case "MemberName"
                        dr("MemberName") = li.Text       ' ListItem.Text
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


        LoadFromObject()
    End Sub
    Protected Sub lnkBttnAddItems_Click(sender As Object, e As EventArgs) Handles lnkBttnAddItems.Click
        Dim selectedItems = TryCast(
    VendorPopupHelper.GetPopupReturnValue(Me, "SelectedItems"),
    List(Of Dictionary(Of String, Object))
)

        Dim dtSelected As DataTable = PF.ConvertSelectedItemsToDataTable(selectedItems)

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




