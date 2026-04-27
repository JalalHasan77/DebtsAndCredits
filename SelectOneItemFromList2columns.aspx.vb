Imports System
Imports System.Collections.Generic

Partial Class SelectOneItemFromList2columns
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadOptions()
        End If
    End Sub

    Private Sub LoadOptions()
        Dim options As New List(Of OptionItem)()
        Dim DT As New Data.DataTable
        DT = DB.GetDataTable(DB.InfoDB, "Select ID, VenderName, Whatsapp from Venders")

        For Each DR As Data.DataRow In DT.Rows
            options.Add(New OptionItem With {
                .OptionName = DR("VenderName").ToString(),
                .OptionValue = DR("ID").ToString(),
                .Whatsapp = If(IsDBNull(DR("Whatsapp")), String.Empty, DR("Whatsapp").ToString())
            })
        Next

        rptOptions.DataSource = options
        rptOptions.DataBind()
    End Sub

    Public Class OptionItem
        Public Property OptionName As String
        Public Property OptionValue As String
        Public Property Whatsapp As String
    End Class
End Class
