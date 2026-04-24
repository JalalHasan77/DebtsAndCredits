Imports System.Data
Imports System.Text
Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Partial Class NewOrder
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'AddJQueryLinks(Page, True)

        If Not Page.IsPostBack Then
            CreateInitialTable()
        End If

        If Not String.IsNullOrEmpty(hdnSelectedVendorText.Value) Then
            TextBox1.Text = hdnSelectedVendorText.Value
        End If

        LoadFromObject()

        RegisterVendorPopup(LinkButton3,
                            "VendorPopup.aspx",
                            760,
                            450,
                            PopupPlacement.Center,
                            "Select Vendor")

        RegisterVendorPopup(LinkButton6,
                            "VendorPopup.aspx",
                            600,
                            450,
                            PopupPlacement.Center,
                            "Select Vendor")


    End Sub

    Sub CreateInitialTable()
        Dim DT As New Data.DataTable
        DT = GetDataTable(InfoDB, " SELECT MemberName,'0.000' as Deposit, '0.000' as Debt,'0.000' as Profit  FROM Members WHERE MemberName IN (" &
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


        If Session("HeaderLevel1") Is Nothing Then
            HeaderLevel1 = Enumerable.Repeat("Profit", dt.Columns.Count).ToList()
        End If

        If Session("HeaderLevel2") Is Nothing Then
            HeaderLevel2 = Enumerable.Repeat("Total", dt.Columns.Count).ToList()
        End If

        If Session("HeaderLevel3") Is Nothing Then
            HeaderLevel3 = Enumerable.Repeat("NoOfItems", dt.Columns.Count).ToList()
        End If

        If Session("HeaderLevel4") Is Nothing Then
            HeaderLevel4 = Enumerable.Repeat("Item", dt.Columns.Count).ToList()
        End If

        If Session("HeaderLevel5") Is Nothing Then
            HeaderLevel5 = Enumerable.Repeat("Price", dt.Columns.Count).ToList()
        End If

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
        Dim colIndex As Integer = 0
        For Each dc As DataColumn In DT.Columns
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


    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
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
                    ' Member column (span 4 rows)
                    Dim cell As New TableCell()
                    cell.Text = "Member"
                    cell.RowSpan = 5
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.VerticalAlign = VerticalAlign.Middle
                    h1.Cells.Add(cell)

                ElseIf i = 1 Then
                    ' Deposit column (span 4 rows)
                    Dim cell As New TableCell()
                    cell.Text = "Deposit"
                    cell.RowSpan = 5
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.VerticalAlign = VerticalAlign.Middle
                    h1.Cells.Add(cell)
                ElseIf i = 2 Then
                    ' Deposit column (span 4 rows)
                    Dim cell As New TableCell()
                    cell.Text = "Debit"
                    cell.RowSpan = 5
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.VerticalAlign = VerticalAlign.Middle
                    h1.Cells.Add(cell)
                ElseIf i = 3 Then
                    ' Deposit column (span 4 rows)
                    Dim cell As New TableCell()
                    cell.Text = "Profit"
                    cell.RowSpan = 5
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.VerticalAlign = VerticalAlign.Middle
                    h1.Cells.Add(cell)
                Else
                    ' Only LAST column gets grouped headers
                    h1.Cells.Add(CreateEditableHeaderCell(HeaderLevel1(i), i, 1))
                End If

            Next


            '========================
            ' SECOND HEADER ROW
            '========================
            Dim h2 As New GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert)

            ' Only for grouped column(s)
            For i As Integer = 4 To colCount - 1
                h2.Cells.Add(CreateEditableHeaderCell(HeaderLevel2(i), i, 2))
            Next


            '========================
            ' THIRD HEADER ROW
            '========================
            Dim h3 As New GridViewRow(2, 0, DataControlRowType.Header, DataControlRowState.Insert)
            For i As Integer = 4 To colCount - 1
                h3.Cells.Add(CreateEditableHeaderCell(HeaderLevel3(i), i, 3))
            Next


            '========================
            ' FOURTH HEADER ROW
            '========================
            Dim h4 As New GridViewRow(3, 0, DataControlRowType.Header, DataControlRowState.Insert)
            For i As Integer = 4 To colCount - 1
                h4.Cells.Add(CreateEditableHeaderCell(HeaderLevel4(i), i, 4))
            Next


            '========================
            ' Fifthe HEADER ROW
            '========================
            Dim h5 As New GridViewRow(3, 0, DataControlRowType.Header, DataControlRowState.Insert)
            For i As Integer = 4 To colCount - 1
                h5.Cells.Add(CreateEditableHeaderCell(HeaderLevel5(i), i, 5))
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

        Dim dt As DataTable =
        CType(HttpContext.Current.Session("MyTable"), DataTable)


    End Sub

    Protected Sub LinkButton3_Click(sender As Object, e As EventArgs) Handles LinkButton3.Click
        Dim selectedVendorValue As String = hdnSelectedVendorValue.Value
        Dim selectedVendorText As String = hdnSelectedVendorText.Value

        TextBox1.Text = selectedVendorText
        Label6.Text = hdnSelectedVendorText.Value

        ' Add your vendor-related server-side logic here.
        ' Example:
        ' Label2.Text = "Selected vendor: " & selectedVendorText & " (" & selectedVendorValue & ")"
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


    Private Enum PopupPlacement
        Center
        RightSide
    End Enum

    Private Sub RegisterVendorPopup(ByVal triggerControl As WebControl,
                                    ByVal popupPageUrl As String,
                                    ByVal popupWidth As Integer,
                                    ByVal popupHeight As Integer,
                                    ByVal placement As PopupPlacement,
                                    Optional ByVal popupTitle As String = "Select Vendor")

        RegisterVendorPopupStyles()
        RegisterVendorPopupMarkup(popupWidth, popupHeight, placement, popupTitle)
        RegisterVendorPopupScript(popupPageUrl, triggerControl.UniqueID)

        triggerControl.Attributes("onclick") = "openVendorDialog(); return false;"
    End Sub

    Private Sub RegisterVendorPopupStyles()
        If Page.Items("VendorPopupStylesRegistered") IsNot Nothing Then Exit Sub

        Dim css As New StringBuilder()
        css.AppendLine("<style type=""text/css"">")
        css.AppendLine("#vendorModalOverlay {")
        css.AppendLine("    display: none;")
        css.AppendLine("    position: fixed;")
        css.AppendLine("    top: 0;")
        css.AppendLine("    left: 0;")
        css.AppendLine("    width: 100%;")
        css.AppendLine("    height: 100%;")
        css.AppendLine("    background-color: rgba(0, 0, 0, 0.55);")
        css.AppendLine("    z-index: 1000;")
        css.AppendLine("}")
        css.AppendLine("#vendorModalDialog {")
        css.AppendLine("    position: absolute;")
        css.AppendLine("    max-width: 95vw;")
        css.AppendLine("    max-height: 95vh;")
        css.AppendLine("    background-color: #ffffff;")
        css.AppendLine("    border-radius: 8px;")
        css.AppendLine("    box-shadow: 0 8px 30px rgba(0,0,0,0.3);")
        css.AppendLine("    overflow: hidden;")
        css.AppendLine("    display: flex;")
        css.AppendLine("    flex-direction: column;")
        css.AppendLine("}")
        css.AppendLine(".vendor-modal-header {")
        css.AppendLine("    display: flex;")
        css.AppendLine("    align-items: center;")
        css.AppendLine("    justify-content: space-between;")
        css.AppendLine("    padding: 15px 15px 10px 15px;")
        css.AppendLine("    border-bottom: 1px solid #e5e5e5;")
        css.AppendLine("}")
        css.AppendLine(".vendor-modal-title {")
        css.AppendLine("    font-family: Arial, sans-serif;")
        css.AppendLine("    font-size: 20px;")
        css.AppendLine("    font-weight: bold;")
        css.AppendLine("    color: #333;")
        css.AppendLine("}")
        css.AppendLine(".btn-close-x {")
        css.AppendLine("    background: transparent;")
        css.AppendLine("    border: none;")
        css.AppendLine("    color: #666;")
        css.AppendLine("    cursor: pointer;")
        css.AppendLine("    font-size: 24px;")
        css.AppendLine("    line-height: 1;")
        css.AppendLine("    padding: 0 4px;")
        css.AppendLine("}")
        css.AppendLine(".btn-close-x:hover {")
        css.AppendLine("    color: #cc0000;")
        css.AppendLine("}")
        css.AppendLine("#vendorPopupFrame {")
        css.AppendLine("    width: 100%;")
        css.AppendLine("    height: 100%;")
        css.AppendLine("    min-height: 0;")
        css.AppendLine("    border: none;")
        css.AppendLine("    flex: 1 1 auto;")
        css.AppendLine("}")
        css.AppendLine(".vendor-modal-footer {")
        css.AppendLine("    padding: 12px 15px;")
        css.AppendLine("    text-align: right;")
        css.AppendLine("    border-top: 1px solid #e5e5e5;")
        css.AppendLine("}")
        css.AppendLine(".btn-close {")
        css.AppendLine("    padding: 8px 18px;")
        css.AppendLine("    background: #cc0000;")
        css.AppendLine("    color: #fff;")
        css.AppendLine("    border: none;")
        css.AppendLine("    border-radius: 4px;")
        css.AppendLine("    cursor: pointer;")
        css.AppendLine("    font-size: 14px;")
        css.AppendLine("}")
        css.AppendLine(".btn-close:hover {")
        css.AppendLine("    background: #a80000;")
        css.AppendLine("}")
        css.AppendLine("</style>")

        Page.Header.Controls.Add(New LiteralControl(css.ToString()))
        Page.Items("VendorPopupStylesRegistered") = True
    End Sub

    Private Sub RegisterVendorPopupMarkup(ByVal popupWidth As Integer,
                                          ByVal popupHeight As Integer,
                                          ByVal placement As PopupPlacement,
                                          ByVal popupTitle As String)

        If Page.Items("VendorPopupMarkupRegistered") IsNot Nothing Then Exit Sub

        Dim popupTop As String = "50%"
        Dim popupLeft As String = "50%"
        Dim popupRight As String = "auto"
        Dim popupTransform As String = "translate(-50%, -50%)"

        If placement = PopupPlacement.RightSide Then
            popupTop = "20px"
            popupLeft = "auto"
            popupRight = "20px"
            popupTransform = "none"
        End If

        Dim popupMarkup As New StringBuilder()
        popupMarkup.AppendLine("<div id=""vendorModalOverlay"" onclick=""closeVendorDialog();"">")
        popupMarkup.AppendLine("    <div id=""vendorModalDialog"" role=""dialog"" aria-modal=""true"" aria-labelledby=""vendorModalTitle"" onclick=""if (event.stopPropagation) event.stopPropagation(); event.cancelBubble = true;"" style=""width:" & popupWidth & "px;height:" & popupHeight & "px;top:" & popupTop & ";left:" & popupLeft & ";right:" & popupRight & ";transform:" & popupTransform & ";"">")
        popupMarkup.AppendLine("        <div class=""vendor-modal-header"">")
        popupMarkup.AppendLine("            <span id=""vendorModalTitle"" class=""vendor-modal-title"">" & Server.HtmlEncode(popupTitle) & "</span>")
        popupMarkup.AppendLine("            <button type=""button"" class=""btn-close-x"" onclick=""closeVendorDialog(); return false;"" aria-label=""Close popup"">&#10005;</button>")
        popupMarkup.AppendLine("        </div>")
        popupMarkup.AppendLine("        <iframe id=""vendorPopupFrame"" src=""about:blank""></iframe>")
        popupMarkup.AppendLine("        <div class=""vendor-modal-footer"">")
        popupMarkup.AppendLine("            <button type=""button"" class=""btn-close"" onclick=""closeVendorDialog(); return false;"">Cancel</button>")
        popupMarkup.AppendLine("        </div>")
        popupMarkup.AppendLine("    </div>")
        popupMarkup.AppendLine("</div>")

        Page.Form.Controls.Add(New LiteralControl(popupMarkup.ToString()))
        Page.Items("VendorPopupMarkupRegistered") = True
    End Sub

    Private Sub RegisterVendorPopupScript(ByVal popupPageUrl As String,
                                          ByVal postBackUniqueId As String)

        If Page.Items("VendorPopupScriptRegistered") IsNot Nothing Then Exit Sub

        Dim js As New StringBuilder()
        js.AppendLine("function openVendorDialog() {")
        js.AppendLine("    document.getElementById('vendorPopupFrame').src = '" & JsEncode(ResolveUrl(popupPageUrl)) & "';")
        js.AppendLine("    document.getElementById('vendorModalOverlay').style.display = 'block';")
        js.AppendLine("    document.body.style.overflow = 'hidden';")
        js.AppendLine("}")
        js.AppendLine("")
        js.AppendLine("function closeVendorDialog() {")
        js.AppendLine("    var overlay = document.getElementById('vendorModalOverlay');")
        js.AppendLine("    var frame = document.getElementById('vendorPopupFrame');")
        js.AppendLine("    if (overlay) overlay.style.display = 'none';")
        js.AppendLine("    if (frame) frame.src = 'about:blank';")
        js.AppendLine("    document.body.style.overflow = '';")
        js.AppendLine("}")
        js.AppendLine("")
        js.AppendLine("function receiveVendorValue(selectedValue, displayText) {")
        js.AppendLine("    var vendorText = displayText || selectedValue || '';")
        js.AppendLine("    document.getElementById('" & JsEncode(hdnSelectedVendorValue.ClientID) & "').value = selectedValue || '';")
        js.AppendLine("    document.getElementById('" & JsEncode(hdnSelectedVendorText.ClientID) & "').value = vendorText;")
        js.AppendLine("    document.getElementById('" & JsEncode(TextBox1.ClientID) & "').value = vendorText;")
        js.AppendLine("    closeVendorDialog();")
        js.AppendLine("    __doPostBack('" & JsEncode(postBackUniqueId) & "', '');")
        js.AppendLine("}")

        ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "VendorPopupScript", js.ToString(), True)
        Page.Items("VendorPopupScriptRegistered") = True
    End Sub

    Private Function JsEncode(ByVal value As String) As String
        If value Is Nothing Then Return String.Empty

        Return value.Replace(Chr(92), Chr(92) & Chr(92)).Replace("'", "\'").Replace(vbCrLf, "\n").Replace(vbCr, "\n").Replace(vbLf, "\n")
    End Function

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

