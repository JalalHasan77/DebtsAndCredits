Imports System.Data
Partial Class _Default
    Inherits System.Web.UI.Page

    Dim DT As DataTable
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddJQueryLinks(Page, True)
        'AddBootStrapLinks(Page)

        SetOnClientClick(btnAddNew, GetAppPath() + "/AddNew.aspx?MODE=NEW", 700, 1000, , False)
        SetOnClientClick(btnAddNewCategory, GetAppPath() + "/AddNewCategory.aspx?MODE=NEW", 700, 1000, , False)
        If Not Page.IsPostBack Then
            DropDownList5.SelectedIndex = 1
            LoadInfo()
        End If


    End Sub

    Sub LoadInfo()
        Dim DBPAth As String = System.AppDomain.CurrentDomain.BaseDirectory & "\App_Data\BibleDB.mdb"
        Dim SQL1 As String = String.Empty
        Dim DT1 As New DataTable


        If DropDownList6.SelectedValue = "Categories View" Then


            Select Case DropDownList1.SelectedValue
                Case "Category Addition Day"
                    SQL1 = SQL1 + " Select ID,CategoryTitle from Categories "
                    SQL1 = SQL1 + " order by ID " & DropDownList5.SelectedValue
                Case "Info Addition Day"
                    SQL1 = SQL1 + " Select Categories.ID As ID , Categories.CategoryTitle  As CategoryTitle from (Select Category, Max(Mid(addOn,1,4) & Mid(addOn,6,2) & Mid(addOn,9,2)) As AdditionDay "
                    SQL1 = SQL1 + " From Info "
                    SQL1 = SQL1 + " Group By Category "
                    SQL1 = SQL1 + " Order By Category, Max(Mid(addOn, 1, 4) & Mid(addOn, 6, 2) & Mid(addOn, 9, 2))) TBL right join Categories  On TBL.Category = Categories.ID "
                    SQL1 = SQL1 + " order by TBL.AdditionDay " & DropDownList5.SelectedValue

                Case "Category has largest Info No"
                    SQL1 = SQL1 + " Select T.ID as ID, T.CategoryTitle as CategoryTitle "
                    SQL1 = SQL1 + " From Categories T "
                    SQL1 = SQL1 + " left Join (Select Count(Info.Category) As lcCount,  Categories.ID  From Info INNER Join Categories On Info.Category = Categories.ID  Group By Categories.ID) TBL on T.ID = TBL.ID "
                    SQL1 = SQL1 + "  Order By lccount " & DropDownList5.SelectedValue

            End Select



            DT1 = GetDataTable(InfoDB, SQL1)

            If Not Page.IsPostBack Then
                For Each R In DT1.Rows
                    Dim L As New ListItem
                    L.Text = R("CategoryTitle").ToString
                    L.Value = "Asc" & "|" & "Info Addition Day"
                    ListBox1.Items.Add(L)
                Next
            End If

            GridView2.DataSource = DT1
            GridView2.DataBind()
        Else
            SQL1 = SQL1 + "SELECT CategoryTitle, Info.Title as Title, AddOn, NoOfRevisions,Info.ID as ID "
            SQL1 = SQL1 + " From Categories INNER Join Info On Categories.ID = Info.Category order by @SortBy @Order"

            If DropDownList7.SelectedValue = "Info Addition Day" Then
                'Sort by: Info Addition Day
                SQL1 = SQL1.Replace("@SortBy", "AddOn")

            Else
                'sort by: No Of Revisions
                SQL1 = SQL1.Replace("@SortBy", "NoOfRevisions")
            End If

            SQL1 = SQL1.Replace("@Order", DropDownList8.SelectedValue)

            DT1 = GetDataTable(InfoDB, SQL1)
            GridView3.DataSource = DT1
            GridView3.DataBind()
        End If



    End Sub


    Private Sub GridView2_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView2.RowDataBound
        Dim R As GridViewRow
        R = e.Row

        If R.RowType = DataControlRowType.DataRow Then


            Dim gv As GridView
            gv = R.FindControl("GridView1")

            Dim ListItem As New ListItem
            ListItem = ListBox1.Items.FindByText(R.DataItem("CategoryTitle"))
            Dim ListValueComp As String() = Split(ListItem.Value, "|")

            Dim DT As New DataTable
            DT = BringDT(CategoryName:=R.DataItem("CategoryTitle"))
            gv.DataSource = DT.DefaultView
            gv.DataBind()


            Dim ddl1 As DropDownList = R.FindControl("DropDownList3")
            Dim ddl2 As DropDownList = R.FindControl("DropDownList4")


            Dim L As ListItem
            L = ddl1.Items.FindByText(ListValueComp(0))
            L.Selected = True

            L = ddl2.Items.FindByText(ListValueComp(1))
            L.Selected = True

            '===============================================================================
            '===============================================================================
            Dim DT2 As New DataTable
            DT2 = GetDataTable(InfoDB, "SELECT ID, Category, Title FROM Categories_FileTypes where Category='" & R.DataItem("ID") & "'")
            Dim gv2 As GridView
            gv2 = R.FindControl("GridView4")
            gv2.DataSource = DT2.DefaultView
            gv2.DataBind()
        End If

    End Sub

    Private Sub GridView3_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView3.RowDataBound
        Dim R As GridViewRow
        R = e.Row

        If R.RowType = DataControlRowType.DataRow Then
            AddLinkToLinkButtons(R)

            Dim Hex As String = "2011"
            Dim Value As Integer = Convert.ToInt32(Hex, 16)
            Dim nonBreakingDash As String = Char.ConvertFromUtf32(Value)

            Hex = "002D"
            Value = Convert.ToInt32(Hex, 16)
            Dim BreakingDash As String = Char.ConvertFromUtf32(Value)

            Dim L As Label
            L = R.FindControl("Label1")
            L.Text = R.DataItem("AddOn")
            L.Text = L.Text.Replace(BreakingDash, nonBreakingDash)
        End If
    End Sub



    Function BringDT(ByVal CategoryName As String) As DataTable
        Dim SortingOrder As Integer = 0
        Dim SortingBy As Integer = 1


        Dim ListItem As New ListItem
        ListItem = ListBox1.Items.FindByText(CategoryName)
        Dim index As Integer = ListBox1.Items.IndexOf(ListItem)

        Dim ListValueComp As String() = Split(ListItem.Value, "|")


        Dim SQL As String = ""
        SQL = "Select Info.ID As ID, * from Categories inner join Info On Categories.ID = Info.Category "
        SQL = SQL + " where CAtegories.CategoryTitle = '" & CategoryName & "' "
        SQL = SQL + " order by @Sorting "
        SQL = SQL + " @OrderBy "



        Select Case ListValueComp(SortingBy)
            Case "Info Addition Day"
                SQL = SQL.Replace("@Sorting", "Mid(addOn,1,4) & Mid(addOn,6,2) & Mid(addOn,9,2)")
            Case "No Of Revision"
                SQL = SQL.Replace("@Sorting", "Cint(NoOfRevisions)")
            Case "Info Latest Update"
                SQL = SQL.Replace("@Sorting", "Mid(LastUpdate,1,4) & Mid(LastUpdate,6,2) & Mid(LastUpdate,9,2)")
        End Select

        SQL = SQL.Replace("@OrderBy", ListValueComp(SortingOrder))
        DT = GetDataTable(InfoDB, SQL)
        BringDT = DT

    End Function


    Protected Sub GridView1_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        Dim R As GridViewRow
        R = e.Row
        If R.RowType = DataControlRowType.DataRow Then
            AddLinkToLinkButtons(R)
        End If
        ''
    End Sub

    Private Sub AddLinkToLinkButtons(ByRef R As GridViewRow)
        Dim L As LinkButton
        L = R.FindControl("LinkButton1")
        L.Text = R.DataItem("Title")
        SetOnClientClick(L, GetAppPath() + "/AddNew.aspx?MODE=SHOW&ID=" & R.DataItem("ID"), 700, 1000, , False)

        Dim i As ImageButton
        i = R.FindControl("ImageButton1")
        SetOnClientClick(i, GetAppPath() + "/AddNew.aspx?MODE=EDIT&ID=" & R.DataItem("ID"), 700, 1000, , False)
    End Sub

    Protected Sub DropDownList1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList1.SelectedIndexChanged, DropDownList5.SelectedIndexChanged
        LoadInfo()
    End Sub

    Protected Sub DropDownList4_SelectedIndexChanged(sender As Object, e As EventArgs)
        'that means the change will be On desc , Asc
        ChangeSort(sender:=sender, Component:=1)
        LoadInfo()

    End Sub

    Protected Sub DropDownList3_SelectedIndexChanged(sender As Object, e As EventArgs)
        'that means the change will be On desc , Asc
        ChangeSort(sender:=sender, Component:=0)
        LoadInfo()
    End Sub

    Sub ChangeSort(sender As Object, ByVal Component As Integer)
        Dim SortingOrder As Integer = 0
        Dim SortingBy As Integer = 1

        Dim C As System.Web.UI.WebControls.DataControlFieldCell
        C = CType(sender, DropDownList).Parent


        Dim R As GridViewRow
        R = C.Parent

        Dim l As Label
        l = R.FindControl("Label2")


        Dim ListItem As New ListItem
        ListItem = ListBox1.Items.FindByText(l.Text)
        Dim index As Integer = ListBox1.Items.IndexOf(ListItem)

        Dim ListValueComp As String() = Split(ListItem.Value, "|")
        ListValueComp(Component) = CType(sender, DropDownList).SelectedValue

        ListBox1.Items(index).Value = ListValueComp(SortingOrder) & "|" & ListValueComp(SortingBy)


    End Sub

    Protected Sub DropDownList5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList5.SelectedIndexChanged

    End Sub

    Protected Sub btnAddNew0_Click(sender As Object, e As EventArgs) Handles btnAddNew0.Click
        Response.Redirect("DMS_GroupsMembers.aspx")
    End Sub

    Protected Sub btnAddNew1_Click(sender As Object, e As EventArgs) Handles btnAddNew1.Click
        Response.Redirect("ICBS_SearchFields.aspx")
    End Sub

    Protected Sub DropDownList6_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList6.SelectedIndexChanged, DropDownList7.SelectedIndexChanged, DropDownList8.SelectedIndexChanged
        Dim Index As Integer = DropDownList6.SelectedIndex
        MultiView1.ActiveViewIndex = Index
        LoadInfo()

    End Sub

    Protected Sub btnAddNew2_Click(sender As Object, e As EventArgs) Handles btnAddNew2.Click

        Dim Dcom As New Odbc.OdbcCommand
        Dim Dcon As New Odbc.OdbcConnection

        Dcon.ConnectionString = "DRIVER={SQL Server};SERVER=S0365;UID=imagelinks;PWD=Eskan@2022;DATABASE=imagelinks;"
        Dcom.Connection = Dcon
        Dim DT, DT0 As New DataTable

        Dim SQL As String = ""
        SQL = SQL + " Select U.USER_NAME as USER_NAME,u.USER_CUSER as USER_CUSER,G.GROUP_NAME as GROUP_NAME from GROUPS G "
        SQL = SQL + " inner Join GROUPMBR GM on G.GROUP_ID =  GM.GROUP_ID "
        SQL = SQL + " right Join USERS U on GM.USER_ID =  U.USER_ID "
        'SQL = SQL + " @Where "
        SQL = SQL + " Order by U.USER_NAME "
        SQL = "Select * from USERS where USER_NAME like '%2556%' "

        Dcom.CommandText = SQL
        Dim DA As New Odbc.OdbcDataAdapter(Dcom)
        DA.Fill(DT)
    End Sub
    Protected Sub Menu1_MenuItemClick(sender As Object, e As MenuEventArgs)
        Dim Cell As DataControlFieldCell
        Cell = CType(sender, Menu).Parent

        Dim lcMultiView As MultiView
        lcMultiView = Cell.FindControl("MultiView2")

        Dim lcMenu As Menu
        lcMenu = CType(sender, Menu)

        lcMultiView.ActiveViewIndex = lcMenu.Items.IndexOf(lcMenu.SelectedItem)
    End Sub


    Protected Sub GridView4_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        Dim R As GridViewRow
        R = e.Row
        If R.RowType = DataControlRowType.DataRow Then
            Dim LnkBtton As LinkButton
            LnkBtton = R.FindControl("LinkButton2")
            LnkBtton.Text = R.DataItem("Title")
            '==========================================================
            '==========================================================
            Dim grdView As GridView
            grdView = R.FindControl("GridView5")
            Dim DT2 As New DataTable
            DT2 = GetDataTable(InfoDB, "SELECT ID, Category, FileType, Title  FROM CAtegories_FileType_ACtions " &
                               " where FileType ='" & R.DataItem("ID") & "'")

            grdView.DataSource = DT2.DefaultView
            grdView.DataBind()

        End If
    End Sub
    Protected Sub GridView5_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        Dim R As GridViewRow
        R = e.Row
        If R.RowType = DataControlRowType.DataRow Then
            Dim LnkBtton As LinkButton
            LnkBtton = R.FindControl("LinkButton3")
            LnkBtton.Text = R.DataItem("Title")
        End If

    End Sub
    Protected Sub LinkButton3_Click(sender As Object, e As EventArgs)
        MsgBox(CType(sender, LinkButton).Text)
    End Sub
    Protected Sub LinkButton2_Click(sender As Object, e As EventArgs)

    End Sub
End Class

Public Class dN
    Private MainNumber As Decimal

    Default ReadOnly Property immd(ByVal oneDecimal As Decimal) As dN
        Get
            Return New dN(oneDecimal)
        End Get
    End Property

    Private Sub New(ByVal onedecimal As Decimal) 'private new, cannot be accessed from outside
        Me.MainNumber = onedecimal
    End Sub

    Sub New() 'this is a must

    End Sub

    Function getIntegerPart() As Integer
        Return Math.Truncate(MainNumber)
    End Function

    Function getFractionPArt() As Decimal
        Return MainNumber - Me.getIntegerPart
    End Function

    Function roundUpTo005() As Decimal
        Return roundUpTo(0.005)
    End Function
    ReadOnly Property roundDownTo005() As Decimal
        Get
            Return roundDownTo(0.005)
        End Get

    End Property

    Private Function roundUpTo(ByVal Fraction As Decimal)
        Return Math.Ceiling(MainNumber / Fraction) * Fraction
    End Function

    Private Function roundDownTo(ByVal Fraction As Decimal)
        Return Math.Floor(MainNumber / Fraction) * Fraction
    End Function

End Class



'Public Class AnyListItem

'    Dim _SortOrder As String
'    Dim _SortingBy As String
'    Dim _ItemTitle As String


'    Property SortOrder As String
'        Get
'            Return _SortOrder
'        End Get
'        Set(value As String)
'            _SortOrder = value
'        End Set
'    End Property
'    Property SortingBy As String
'        Get
'            Return _SortingBy
'        End Get
'        Set(value As String)
'            _SortingBy = value
'        End Set
'    End Property
'    Property ItemTitle As String
'        Get
'            Return _ItemTitle
'        End Get
'        Set(value As String)
'            _ItemTitle = value
'        End Set
'    End Property

'    Public Overrides Function ToString() As String 'this is the heart of the mission
'        Return _ItemTitle
'    End Function


'End Class
