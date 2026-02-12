Public Module AmountSeeker
    Function GetMaximum(ByVal Amount As Double) As Integer
        Dim ApproxFactor As Integer = getApproxFactor(Amount)
        GetMaximum = Math.Ceiling(Amount / ApproxFactor) * ApproxFactor
    End Function

    Function GetMinimum(ByVal Amount As Double) As Integer
        Dim ApproxFactor As Integer = getApproxFactor(Amount)
        GetMinimum = Math.Floor(Amount / ApproxFactor) * ApproxFactor
    End Function

    Function getApproxFactor(ByVal Amount As Decimal) As Integer
        Dim ApproxFactor As Integer = 1
        'If Amount > 0 And Amount <= 9 Then
        '    ApproxFactor = 1
        'ElseIf Amount > 9 And Amount <= 99.999 Then
        '    ApproxFactor = 10
        'ElseIf Amount > 99.999 And Amount <= 9999.999 Then
        '    ApproxFactor = 100
        'ElseIf Amount > 9999.999 And Amount <= 99999.999 Then
        '    ApproxFactor = 100
        'ElseIf Amount > 99999.999 And Amount <= 999999.999 Then
        '    ApproxFactor = 1000
        'ElseIf Amount > 999999.999 And Amount <= 9999999.999 Then
        '    ApproxFactor = 100000
        'ElseIf Amount > 9999999.999 And Amount <= 99999999.999 Then
        '    ApproxFactor = 1000000
        'ElseIf Amount > 99999999.999 And Amount <= 999999999.999 Then
        '    ApproxFactor = 10000000
        'End If
        Dim Found As Boolean = False
        While Found = False
            If (Amount / (ApproxFactor * 10)) > 1 Then
                ApproxFactor = ApproxFactor * 10
            Else
                Found = True
            End If

        End While


        getApproxFactor = ApproxFactor
    End Function
End Module
