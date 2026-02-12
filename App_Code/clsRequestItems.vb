Imports Microsoft.VisualBasic

Public Class clsRequestItems
    Private _Request_Id As String
    Private _Quantity As String
    Private _UnitPrice As Double
    Private _ItemTotal As Double
    Private _ItemDiscount As Double
    Private _ItemNetTotal As Double
    Private _Description As String
    Sub New()

    End Sub

    Public Property Request_ID() As String
        Get
            Return _Request_Id
        End Get
        Set(ByVal value As String)
            _Request_Id = value
        End Set
    End Property

    Public Property Quantity() As Double
        Get
            Return _Quantity
        End Get
        Set(ByVal value As Double)
            _Quantity = value
        End Set
    End Property

    Public Property UnitPrice() As Double
        Get
            Return _UnitPrice
        End Get
        Set(ByVal value As Double)
            _UnitPrice = value
        End Set
    End Property

    Public Property ItemTotal() As Double
        Get
            Return _ItemTotal
        End Get
        Set(ByVal value As Double)
            _ItemTotal = value
        End Set
    End Property

    Public Property ItemDiscount() As Double
        Get
            Return _ItemDiscount
        End Get
        Set(ByVal value As Double)
            _ItemDiscount = value
        End Set
    End Property

    Public Property ItemNetTotal() As Double
        Get
            Return _ItemNetTotal
        End Get
        Set(ByVal value As Double)
            _ItemNetTotal = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return _Description
        End Get
        Set(ByVal value As String)
            _Description = value
        End Set
    End Property

    Sub New(ByVal Request_ID As String,
            ByVal Quantity As String,
            ByVal UnitPrice As Double,
            ByVal ItemTotal As Double,
            ByVal ItemDiscount As Double,
            ByVal ItemNetTotal As Double,
            ByVal Description As String)
        _Request_Id = Request_ID
        _Quantity = Quantity
        _UnitPrice = UnitPrice
        _ItemTotal = ItemTotal
        _ItemDiscount = ItemDiscount
        _ItemNetTotal = ItemNetTotal
        _Description = Description
    End Sub
    '    Public Function GetTheDatabase() As Data.DataTable
    '        Dim udf As New GUDF
    '        Dim dt As Data.DataTable = udf.GetDataTable(udf.EBDB_CS, "Select * from RFE_RequestItems")
    '        Return (dt)
    '    End Function

End Class
