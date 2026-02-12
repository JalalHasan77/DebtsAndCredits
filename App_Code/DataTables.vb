
Namespace DT
    Public Module DataTables
        Public Function GetDataTable(ByVal lcConnection As String, lcSql As String) As Data.DataTable
            Dim dt As Data.DataTable = New Data.DataTable
            Dim oda As Data.OleDb.OleDbDataAdapter = New Data.OleDb.OleDbDataAdapter(lcSql, lcConnection)
            oda.Fill(dt)
            GetDataTable = dt
        End Function

        Public Function DTColumnsToArray(ByVal FieldName As String, ByVal DT As Data.DataTable) As String()
            Dim DL As New List(Of String)

            If IsNumeric(DT.Rows.Count) AndAlso DT.Rows.Count > 0 Then
                Dim DC As New Data.DataColumn
                For Each DR In DT.Rows
                    DL.Add(DR(FieldName).ToString.Trim)
                Next
            End If

            Return DL.ToArray

        End Function

        Public Function DTColumnsToArray(ByVal DT As Data.DataTable) As String()
            Dim DL As New List(Of String)
            If IsNumeric(DT.Rows.Count) AndAlso DT.Rows.Count > 0 Then
                Dim DC As New Data.DataColumn
                For Each DR As Data.DataRow In DT.Rows
                    DL.Add(DR(0).ToString.Trim)
                Next
            End If
            Return DL.ToArray
        End Function

        Public Function DTColumnsToString(ByVal DT As Data.DataTable) As String
            Dim arr As String()
            arr = DTColumnsToArray(DT)

            Dim strArr As String
            strArr = Join(arr, ",")
            DTColumnsToString = strArr

        End Function

        Public Function DTColumnsToString(ByVal FieldName As String, ByVal DT As Data.DataTable) As String
            Dim arr As String()
            arr = DTColumnsToArray(FieldName, DT)

            Dim strArr As String
            strArr = Join(arr, ",")

            DTColumnsToString = strArr
        End Function


    End Module
End Namespace
