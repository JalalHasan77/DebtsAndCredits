Imports System
Imports System.Collections.Generic

Partial Class SelectOneItemFromList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadVendorOptions()
        End If
    End Sub

    Private Sub LoadVendorOptions()
        'Dim options As New List(Of OptionItem) From {
        '    New OptionItem With {.OptionName = "Product Alpha", .OptionValue = "ALPHA"},
        '    New OptionItem With {.OptionName = "Product Beta", .OptionValue = "BETA"},
        '    New OptionItem With {.OptionName = "Product Gamma", .OptionValue = "GAMMA"}
        '}
        'rptVendorOptions.DataSource = options
        'rptVendorOptions.DataBind()




        Dim options As New List(Of OptionItem)
        Dim DT As New Data.DataTable
        DT = DB.GetDataTable(DB.InfoDB, "Select ID , VenderName from Venders")

        For Each DR As Data.DataRow In DT.Rows

            options.Add(New OptionItem With {.OptionName = DR("VenderName").ToString, .OptionValue = DR("ID").ToString})
        Next

        rptVendorOptions.DataSource = options
        rptVendorOptions.DataBind()
    End Sub

    Public Class OptionItem
        Public Property OptionName As String
        Public Property OptionValue As String
    End Class
End Class
