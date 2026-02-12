Imports System.Data
Imports System.Web.Services
Imports System.Web.Script.Services
Partial Class NewOrder
    Inherits System.Web.UI.Page

    Dim DT As DataTable
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddJQueryLinks(Page, True)

        If Not Page.IsPostBack Then
            Dim DT As New Data.DataTable
            DT = GetDataTable(InfoDB, "Select top 5 MemberName from Members ")
            clTemp.lcObject = DT

            HttpContext.Current.Session("MyTable") = DT
            LoadFromObject()
        End If



    End Sub


    Sub LoadFromObject()
        'Dim DT As New Data.DataTable
        'DT = CType(clTemp.lcObject, DataTable)
        Dim dt As DataTable =
        CType(HttpContext.Current.Session("MyTable"), DataTable)

        BuildGrid(dt)
    End Sub

    Private Sub BuildGrid(ByVal DT As DataTable)
        GridView1.Columns.Clear()

        For Each col As DataColumn In DT.Columns
            GridView1.Columns.Add(CreateEditableTemplate(col.ColumnName))
        Next

        GridView1.DataSource = DT
        GridView1.DataBind()
    End Sub
    Private Function CreateEditableTemplate(columnName As String) As TemplateField
        Dim tf As New TemplateField
        tf.HeaderText = columnName
        tf.ItemStyle.Width = Unit.Pixel(150)

        tf.ItemTemplate = New EditableTemplate(columnName)

        Return tf
    End Function


    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Dim DT As New Data.DataTable
        'DT = CType(clTemp.lcObject, DataTable)

        'Dim DC As New DataColumn
        'DT.Columns.Add(DC)

        'clTemp.lcObject = DT
        Dim dt As DataTable =
        CType(HttpContext.Current.Session("MyTable"), DataTable)
        Dim DC As New DataColumn
        dt.Columns.Add(DC)

        LoadFromObject()

    End Sub

    Protected Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.Header Then

            ' Remove default header
            e.Row.Cells.Clear()

            Dim table As Table = CType(GridView1.Controls(0), Table)

            ' ========= HEADER LEVEL 1 =========
            Dim h1 As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)

            For i As Integer = 0 To GridView1.Columns.Count - 1
                h1.Cells.Add(CreateEditableHeaderCell("Item"))
            Next

            ' ========= HEADER LEVEL 2 =========
            Dim h2 As New GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert)

            For i As Integer = 0 To GridView1.Columns.Count - 1
                h2.Cells.Add(CreateEditableHeaderCell("Price"))
            Next

            ' Add header rows
            table.Rows.AddAt(0, h1)
            table.Rows.AddAt(1, h2)

        End If
    End Sub

    Private Function CreateEditableHeaderCell(text As String) As TableCell
        Dim cell As New TableCell
        cell.HorizontalAlign = HorizontalAlign.Center

        Dim wrapper As New HtmlGenericControl("div")
        wrapper.Attributes("class") = "cell-wrapper"
        wrapper.Attributes("onclick") = "editCell(this)"

        Dim lbl As New Label()
        lbl.Text = text

        Dim txt As New TextBox()
        txt.Text = text
        txt.Style("display") = "none"
        txt.Style("width") = "85%"
        txt.Attributes("onblur") = "saveCell(this)"
        txt.Attributes("onkeydown") = "return handleEnter(event, this);"

        wrapper.Controls.Add(lbl)
        wrapper.Controls.Add(txt)
        cell.Controls.Add(wrapper)

        Return cell
    End Function

    '<WebMethod()>
    '<ScriptMethod()>
    'Public Shared Sub SaveCell(rowIndex As Integer, columnName As String, value As String)

    '    Dim dt As DataTable =
    '    CType(HttpContext.Current.Session("MyTable"), DataTable)

    '    If dt IsNot Nothing Then
    '        dt.Rows(rowIndex)(columnName) = value
    '        HttpContext.Current.Session("MyTable") = dt
    '    End If

    'End Sub



End Class


Public Class EditableTemplate
    Implements ITemplate

    Private _columnName As String

    Public Sub New(columnName As String)
        _columnName = columnName
    End Sub

    Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
        Dim wrapper As New HtmlGenericControl("div")
        wrapper.Attributes("class") = "cell-wrapper"
        wrapper.Attributes("onclick") = "editCell(this)"


        ' LABEL
        Dim lbl As New Label()
        lbl.ID = "lblValue"

        AddHandler lbl.DataBinding, Sub(sender As Object, e As EventArgs)
                                        Dim l = CType(sender, Label)
                                        Dim row = CType(l.NamingContainer, GridViewRow)
                                        l.Text = Convert.ToString(DataBinder.Eval(row.DataItem, _columnName))
                                    End Sub

        ' TEXTBOX
        Dim txt As New TextBox()
        txt.ID = "txtValue"
        txt.Style("display") = "none"
        txt.Style("width") = "85%"
        txt.Attributes("onblur") = "saveCell(this)"
        txt.Attributes("onkeydown") = "return handleEnter(event, this);"

        AddHandler txt.DataBinding, Sub(sender As Object, e As EventArgs)
                                        Dim t = CType(sender, TextBox)
                                        Dim row = CType(t.NamingContainer, GridViewRow)
                                        t.Text = Convert.ToString(DataBinder.Eval(row.DataItem, _columnName))
                                    End Sub

        wrapper.Controls.Add(lbl)
        wrapper.Controls.Add(txt)
        container.Controls.Add(wrapper)
    End Sub
End Class

