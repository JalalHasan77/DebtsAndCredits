Imports System.Data
Imports System.Web.Services
Imports System.Web.Script.Services
Partial Class NewOrder
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddJQueryLinks(Page, True)

        If Not Page.IsPostBack Then
            'Dim DT As New Data.DataTable
            'DT = GetDataTable(InfoDB, "Select top 5 MemberName from Members ")
            ''clTemp.lcObject = DT

            'HttpContext.Current.Session("MyTable") = DT
            'LoadFromObject()
            CreateInitialTable()

        End If
        LoadFromObject()


    End Sub

    Sub CreateInitialTable()
        Dim DT As New Data.DataTable
        DT = GetDataTable(InfoDB, " SELECT MemberName,'0.000' as Deposit, '0.000' as Debt,'0.000' as Profit  FROM Members WHERE MemberName IN (" &
    "'Fatima AlHaddad'," &
    "'Fatima Mohammed'," &
    "'Elmeera'," &
    "'Roqaya'," &
    "'Jalal'" &
    ");")

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
            HeaderLevel3 = Enumerable.Repeat("Item", dt.Columns.Count).ToList()
        End If

        If Session("HeaderLevel4") Is Nothing Then
            HeaderLevel4 = Enumerable.Repeat("Price", dt.Columns.Count).ToList()
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
        DT.Columns.Add(DC)

        clTemp.lcObject = dt

        HeaderLevel1.Add("Profit")
        HeaderLevel2.Add("Total")
        HeaderLevel3.Add("Item")
        HeaderLevel4.Add("Price")


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
                    cell.RowSpan = 4
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.VerticalAlign = VerticalAlign.Middle
                    h1.Cells.Add(cell)

                ElseIf i = 1 Then
                    ' Deposit column (span 4 rows)
                    Dim cell As New TableCell()
                    cell.Text = "Deposit"
                    cell.RowSpan = 4
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.VerticalAlign = VerticalAlign.Middle
                    h1.Cells.Add(cell)
                ElseIf i = 2 Then
                    ' Deposit column (span 4 rows)
                    Dim cell As New TableCell()
                    cell.Text = "Debit"
                    cell.RowSpan = 4
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.VerticalAlign = VerticalAlign.Middle
                    h1.Cells.Add(cell)
                ElseIf i = 3 Then
                    ' Deposit column (span 4 rows)
                    Dim cell As New TableCell()
                    cell.Text = "Profit"
                    cell.RowSpan = 4
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


            table.Rows.AddAt(0, h1)
            table.Rows.AddAt(1, h2)
            table.Rows.AddAt(2, h3)
            table.Rows.AddAt(3, h4)

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
            'ElseIf level = 4 Then
            '    Dim list = CType(HttpContext.Current.Session("HeaderLevel3"), List(Of String))
            '    list(colIndex) = value
        Else
            Dim list = CType(HttpContext.Current.Session("HeaderLevel4"), List(Of String))
            list(colIndex) = value
            HttpContext.Current.Session("HeaderLevel4") = list
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

