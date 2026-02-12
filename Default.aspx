<%@ Page Language="VB" AutoEventWireup="true" Codefile="Default.aspx.vb" Inherits="_Default"%>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style2 {
            width: 100%;
            border-collapse: collapse;
        }
        .auto-style3 {
            width: 90%;
            border-collapse: collapse;
        }

        .MenuText {
            text-align: center;
            vertical-align: middle;
        }
        /*        .auto-style5 {
            width: 862px; 
        }*/
        
        /*.lblSpacing 
        { 
    padding-left: 3px;
    padding-right: 3px; 
    padding-top: 3px; 
    padding-bottom: 3px; 
    -moz-border-radius: 3px;
    -webkit-border-radius: 3px;
    border-radius: 3px;
    border: 1px solid grey;
    }

                .lblSpacing2 
        { 
    padding-left: 3px;
    padding-right: 3px; 
    padding-top: 3px; 
    padding-bottom: 3px; 
    -moz-border-radius: 3px;
    -webkit-border-radius: 3px;
    border-radius: 7px;
    border: 1px solid grey;
    }*/

        /*.sash {
  position: relative;
 overflow: hidden;*/
 /* width: 300px;*/
/*}
.sash:after {
  content: attr(data-ribbon);
  position: absolute;
  width: 90px;
  height: 30px;
  background: red;
  top: 5px;
  text-align: center;
  line-height: 30px;
  right: -27px;
  transform: rotate(49deg);
}*/

/*.container {*/
/*  margin:50px;
  width:400px;
  height:200px;
  background: white;*/
  /*box-shadow:0px 0px 8px rgba(0,0,0,0.25);
  position: relative;
}

.ribbon-container {
  width:120px;
  height:120px;
  overflow: hidden;
  position: absolute;
  top: -3px;
  right: -3px;
}

.ribbon {
  background-color:maroon;
  font:15px;
  color: white;
  text-align: center;
  text-shadow: rgba(0,0,0,0.8) 0px 1px 0px;
  transform: rotate(45deg);
  position: relative;
  padding: 5px;
  left: -10px;
  top: 25px;
  width: 160px;
  background-image: -webkit-gradient(linear, left top, left bottom,  from(rgb(200,50,50)), to(rgb(255,100,100)));
  box-shadow:0px 0px 3px rgba(0,0,0,0.3);
}

.ribbon2{
  background-color:maroon;
  font:15px;
  font-family: "Raleway",sans-serif;
  font-weight:300;
  color: white;
  text-align: center;
  text-shadow: rgba(0,0,0,0.8) 0px 1px 0px;
  position: relative;
  padding: 5px;
  width: 160px;
  -webkit-transition: all 0.4s ease 0s;
  -o-transition: all 0.4s ease 0s;
  transition: all 0.4s ease 0s;
  background-image: -webkit-gradient(linear, left top, left bottom,  from(rgb(200,50,50)), to(rgb(255,100,100)));
  box-shadow:0px 0px 3px rgba(0,0,0,0.3);
}

.contain{
  width:120px;
  height:120px;
  overflow: hidden;
  position: absolute;
  top: -3px;
  right: -3px;
}

.space{
    margin-top:10px;
}*/

        .auto-style4 {
            width: 100%;
        }

        .auto-style5 {
            direction: ltr;
        }

        .auto-style6 {
            width: 20%;
            height: 38px;
        }
        .auto-style7 {
            width: 60%;
            height: 38px;
        }

        .auto-style8 {
            height: 38px;
        }

    </style>

    <script type="text/javascript">
        function getSelectedElement(Select,ToTextBox) {
            //var e = document.getElementById("Select1");
            //var strUser = e.value;
            var e = document.getElementById(Select);
            var strUser = e.options[e.selectedIndex].text;

           // var txt = document.getElementById("TextBox2");
            if (strUser != 'From') {
                document.getElementById(ToTextBox).disabled = true;
            }
            else
            {
                document.getElementById(ToTextBox).disabled = false;
            }
           // alert(strUser);
        }
 
   </script>



</head>
<body style="margin: 0px;">
    <form id="form1" runat="server">

        <table cellpadding="0" class="auto-style2">
            <tr>
                <td style="background-color: #3366FF">
                    <table cellpadding="15" cellspacing="15" class="auto-style2">
                        <tr>
                            <td style="width: 50%">
                    <asp:Label ID="Label1" runat="server" Font-Names="Arial" Font-Size="24pt" Text="Treasury Deals Validation" ForeColor="White"></asp:Label>
                            </td>
                            <td style="vertical-align: top; width: 50%;" align="right">
                                <br />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center" valign="top">
                    <table cellpadding="10" cellspacing="10" class="auto-style3">
                                <tr>
                                    <td style="width: 20%; vertical-align: top; text-align: center;">
                                        <table class="auto-style4">
                                            <tr>
                                                <td>
                                                    <asp:DropDownList ID="DropDownList6" runat="server" AutoPostBack="True" CssClass="MeBox" Font-Names="Arial">
                                                        <asp:ListItem>Categories View</asp:ListItem>
                                                        <asp:ListItem>Info View</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                        <asp:Button ID="btnAddNew" runat="server" CssClass="MeBox" Text="Add New Info" Width="100%" Font-Names="Arial" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btnAddNewCategory" runat="server" CssClass="MeBox" Text="Add New Category" Width="100%" Font-Names="Arial" />
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:ListBox ID="ListBox1" runat="server" Height="195px" Width="173px" Visible="False"></asp:ListBox>
                                        </td>
                                    <td style="width: 60%">
                                        <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                                            <asp:View ID="View1" runat="server">
                                             <table cellpadding="0" class="auto-style2">
                                            <tr>
                                                <td>
                                        <table class="auto-style4">
                                            <tr>
                                                <td style="width: 20%">&nbsp;<asp:Label ID="Label3" runat="server" Text="Category Order" Font-Names="Arial"></asp:Label>
                                                    &nbsp;</td>
                                                <td style="width: 60%">
                                        <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" CssClass="MeBox" Font-Names="Arial">
                                            <asp:ListItem>Category Addition Day</asp:ListItem>
                                            <asp:ListItem>Info Addition Day</asp:ListItem>
                                            <asp:ListItem>Category has largest Info No</asp:ListItem>
                                            <asp:ListItem>Category Number</asp:ListItem>
                                        </asp:DropDownList>
                                                </td>
                                                <td style="width: 20%">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td class="auto-style6">
                                                    <asp:Label ID="Label4" runat="server" Text="Info Order" Font-Names="Arial"></asp:Label>
                                                </td>
                                                <td align="left" class="auto-style7">
                                        <asp:DropDownList ID="DropDownList5" runat="server" AutoPostBack="True" CssClass="MeBox" Width="20%" Font-Names="Arial">
                                            <asp:ListItem>Desc</asp:ListItem>
                                            <asp:ListItem>Asc</asp:ListItem>
                                        </asp:DropDownList>
                                                </td>
                                                <td class="auto-style6">
                                                    &nbsp;</td>
                                            </tr>
                                        </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                        <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" GridLines="None" Width="100%" SelectedIndex="1">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <div class="auto-style5">
                                                            <table cellpadding="5" cellspacing="5" class="auto-style4">
                                                                <tr>
                                                                    <td>
                                                                        <table class="auto-style4">
                                                                            <tr>
                                                                                <td style="background-color: #EAF4FF" align="Left" width="60%">
                                                                                    <asp:Label ID="Label2" runat="server" Font-Names="Arial" Font-Size="30px" Text='<%# Eval("CategoryTitle") %>'></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: #EAF4FF"  width="40%">
                                                                                    <table class="auto-style2">
                                                                                        <tr>
                                                                                            <td width="80%">
                                                                                                <asp:DropDownList ID="DropDownList4" runat="server" AutoPostBack="True" CssClass="MeBox" OnSelectedIndexChanged="DropDownList4_SelectedIndexChanged" Font-Names="Arial">
                                                                                                    <asp:ListItem>Info Addition Day</asp:ListItem>
                                                                                                    <asp:ListItem>No Of Revision</asp:ListItem>
                                                                                                    <asp:ListItem>Info Latest Update</asp:ListItem>
                                                                                                </asp:DropDownList>
                                                                                            </td>
                                                                                            <td width="20%">
                                                                                                <asp:DropDownList ID="DropDownList3" runat="server" AutoPostBack="True" CssClass="MeBox" OnSelectedIndexChanged="DropDownList3_SelectedIndexChanged" Font-Names="Arial">
                                                                                                    <asp:ListItem>Desc</asp:ListItem>
                                                                                                    <asp:ListItem>Asc</asp:ListItem>
                                                                                                </asp:DropDownList>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td colspan="2" width="60%">
                                                                                    <asp:Menu ID="Menu1" runat="server" Font-Names="Arial" Orientation="Horizontal" Width="100%" CssClass="MenuText" Height="50px" BorderStyle="Solid" BorderWidth="1px" OnMenuItemClick="Menu1_MenuItemClick" BackColor="#99CCFF">
                                                                                        <Items>
                                                                                            <asp:MenuItem Text="State-Specific Actions" Value="State-Specific Actions"></asp:MenuItem>
                                                                                            <asp:MenuItem Text="File-Specific Actions" Value="File-Specific Actions"></asp:MenuItem>
                                                                                        </Items>
                                                                                        <StaticHoverStyle BackColor="White" Height="50px" Width="100%" />
                                                                                        <StaticMenuItemStyle Height="50px" Width="100%" />
                                                                                        <StaticMenuStyle Height="50px" Width="100%" />
                                                                                        <StaticSelectedStyle BackColor="#FF66CC" Height="50px" Width="100%" />
                                                                                    </asp:Menu>
                                                                                </td>
                                                                            </tr>
                                                                          <tr>
                                                                                <td colspan="2" align="Left" class="auto-style4" width="60%">
                                                                                    <asp:MultiView ID="MultiView2" runat="server" ActiveViewIndex="0">
                                                                                        <asp:View ID="View3" runat="server">
                                                                                            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" Font-Names="Arial" Font-Size="15px" ForeColor="#333333" OnRowDataBound="GridView1_RowDataBound" Width="100%">
                                                                                                <AlternatingRowStyle BackColor="White" />
                                                                                                <Columns>
                                                                                                    <asp:TemplateField HeaderText="Title" ShowHeader="False">
                                                                                                        <ItemTemplate>
                                                                                                            <table cellpadding="2" class="auto-style4">
                                                                                                                <tr>
                                                                                                                    <td>
                                                                                                                        <asp:ImageButton ID="ImageButton1" runat="server" Height="16px" ImageUrl="~/Images/note_edit.png" Width="16px" />
                                                                                                                        &nbsp;<asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="False" Font-Italic="False" Font-Size="15px"></asp:LinkButton>
                                                                                                                        <br />
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </ItemTemplate>
                                                                                                    </asp:TemplateField>
                                                                                                    <asp:BoundField DataField="NoOfRevisions" HeaderText="No Of Rev." ItemStyle-Width="100">
                                                                                                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                                                                                                    </asp:BoundField>
                                                                                                </Columns>
                                                                                                <EditRowStyle BackColor="#2461BF" Height="150px" />
                                                                                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                                                                <RowStyle BackColor="#EFF3FB" />
                                                                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                                                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                                                                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                                                                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                                                                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                                                                            </asp:GridView>
                                                                                        </asp:View>
                                                                                        <asp:View ID="View4" runat="server">
                                                                                            <asp:GridView ID="GridView4" runat="server" AutoGenerateColumns="False" CellPadding="4" Font-Names="Arial" ForeColor="#333333" GridLines="None" OnRowDataBound="GridView4_RowDataBound" ShowHeader="False" Width="100%">
                                                                                                <AlternatingRowStyle BackColor="White" />
                                                                                                <Columns>
                                                                                                    <asp:TemplateField HeaderText="Link">
                                                                                                        <ItemTemplate>
                                                                                                            <table cellpadding="0" class="auto-style2">
                                                                                                                <tr>
                                                                                                                    <td>
                                                                                                                        <asp:LinkButton ID="LinkButton2" runat="server" Font-Names="Arial" Font-Bold="True" Font-Size="30px" OnClick="LinkButton2_Click">LinkButton</asp:LinkButton>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td>
                                                                                                                        <asp:GridView ID="GridView5" runat="server" AutoGenerateColumns="False" BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px" CellPadding="2" Font-Names="Arial" ForeColor="Black" GridLines="None" OnRowDataBound="GridView5_RowDataBound" ShowHeader="False">
                                                                                                                            <AlternatingRowStyle BackColor="PaleGoldenrod" />
                                                                                                                            <Columns>
                                                                                                                                <asp:TemplateField>
                                                                                                                                    <ItemTemplate>
                                                                                                                                        <asp:LinkButton ID="LinkButton3" runat="server" OnClick="LinkButton3_Click">LinkButton</asp:LinkButton>
                                                                                                                                    </ItemTemplate>
                                                                                                                                </asp:TemplateField>
                                                                                                                            </Columns>
                                                                                                                            <FooterStyle BackColor="Tan" />
                                                                                                                            <HeaderStyle BackColor="Tan" Font-Bold="True" />
                                                                                                                            <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" HorizontalAlign="Center" />
                                                                                                                            <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
                                                                                                                            <SortedAscendingCellStyle BackColor="#FAFAE7" />
                                                                                                                            <SortedAscendingHeaderStyle BackColor="#DAC09E" />
                                                                                                                            <SortedDescendingCellStyle BackColor="#E1DB9C" />
                                                                                                                            <SortedDescendingHeaderStyle BackColor="#C2A47B" />
                                                                                                                        </asp:GridView>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </ItemTemplate>
                                                                                                    </asp:TemplateField>
                                                                                                </Columns>
                                                                                                <EditRowStyle BackColor="#2461BF" />
                                                                                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                                                                <RowStyle BackColor="#EFF3FB" />
                                                                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                                                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                                                                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                                                                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                                                                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                                                                            </asp:GridView>
                                                                                        </asp:View>
                                                                                    </asp:MultiView>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <br />
                                                            <br />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                            </asp:View>
                                            <asp:View ID="View2" runat="server">
                                                <table class="auto-style4">
                                                    <tr>
                                                        <td style="width: 20%">&nbsp;<asp:Label ID="Label5" runat="server" Font-Names="Arial" Text="Sort By"></asp:Label>
                                                            &nbsp;</td>
                                                        <td style="width: 60%">
                                                            <asp:DropDownList ID="DropDownList7" runat="server" AutoPostBack="True" CssClass="MeBox" Font-Names="Arial">
                                                                <asp:ListItem>Info Addition Day</asp:ListItem>
                                                                <asp:ListItem>No Of Revisions</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td style="width: 20%">&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="auto-style6">
                                                            <asp:Label ID="Label6" runat="server" Font-Names="Arial" Text="Info Order"></asp:Label>
                                                        </td>
                                                        <td align="left" class="auto-style7">
                                                            <asp:DropDownList ID="DropDownList8" runat="server" AutoPostBack="True" CssClass="MeBox" Font-Names="Arial" Width="20%">
                                                                <asp:ListItem>Desc</asp:ListItem>
                                                                <asp:ListItem>Asc</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td class="auto-style6">&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="auto-style8" colspan="3">
                                                            <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False" CellPadding="4" Font-Names="Arial" ForeColor="#333333" Width="100%">
                                                                <AlternatingRowStyle BackColor="White" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="CategoryTitle" HeaderText="Category">
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:TemplateField HeaderText="Title">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Title") %>'></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="ImageButton1" runat="server" Height="16px" ImageUrl="~/Images/note_edit.png" Width="16px" />
                                                                            &nbsp;<asp:LinkButton ID="LinkButton1" runat="server">LinkButton</asp:LinkButton>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Add On">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("AddOn") %>'></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("AddOn") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="NoOfRevisions" HeaderText="No Of Revisions">
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:BoundField>
                                                                </Columns>
                                                                <EditRowStyle BackColor="#2461BF" />
                                                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                                <RowStyle BackColor="#EFF3FB" />
                                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <br />
                                            </asp:View>
                                        </asp:MultiView>
                                    </td>
                                    <td style="width: 20%; vertical-align: top;">
                                        <table cellpadding="10" cellspacing="10" class="auto-style2">
                                            <tr>
                                                <td>
                                        <asp:Button ID="btnAddNew1" runat="server" class="btn btn-primary" Text="Open ICBS Tool" Width="100%" Font-Names="Arial" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                        <asp:Button ID="btnAddNew0" runat="server" class="btn btn-primary" Text="Open DMS Tools" Width="100%" Font-Names="Arial" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                        <asp:Button ID="btnAddNew2" runat="server" class="btn btn-primary" Text="Open DMS Tools" Width="100%" Font-Names="Arial" />
                                                </td>
                                            </tr>
                                        </table>
                                        </td>
                        </tr>
                                </table>
                            </td>
                        </tr>
                        </table>

</form>
</body>
</html>

