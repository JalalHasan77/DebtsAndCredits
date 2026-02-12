<%@ Page Language="VB" AutoEventWireup="true" Codefile="NewOrder.aspx.vb" Inherits="NewOrder"%>
<%@ Register assembly="ServerControl1" namespace="ServerControl1" tagprefix="cc1" %>
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

        .cell-wrapper {
    width: 100%;
    height: 100%;
    cursor: pointer;
    padding: 4px;
}

.cell-wrapper:hover {
    background-color: #f5f5f5;
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
            direction: ltr;
        }

        </style>

    <script type="text/javascript">

            function editCell(wrapper) {
        var lbl = wrapper.querySelector("span");
            var txt = wrapper.querySelector("input");
    
            if (txt.style.display === "inline") return;
    
            lbl.style.display = "none";
            txt.style.display = "inline";
            txt.focus();
            txt.select();
        }
    
        function saveCell(textbox) {
            var wrapper = textbox.closest(".cell-wrapper");
        var lbl = textbox.previousElementSibling;
            lbl.innerText = textbox.value;
            textbox.style.display = "none";
            lbl.style.display = "inline";
            //alert('');
            //====================================================
            //====================================================
            // Get cell info
            var rowIndex = wrapper.getAttribute("data-rowindex");
            var columnName = wrapper.getAttribute("data-column");

            PageMethods.SaveCell(
                parseInt(rowIndex),
                columnName,
                textbox.value,
                function () {
                // alert('Saved OK');
                },
                function (error) {
                 //   aler("Error:" + error.get_message());
                }
            );
        }
    
    function handleEnter(e, textbox) {
        if (e.key === "Enter") {
                textbox.blur();
            return false;
        }
        return true;
        }


        function testCall() {
            PageMethods.SaveCell(0, "Col1", "Test",
                function () { alert("Success"); },
                function (err) { alert(err.get_message()); }
            );
        }
</script>


</head>
<body style="margin: 0px;">
    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

        <table cellpadding="0" class="auto-style2">
            <tr>
                <td style="background-color: #3366FF">
                    <table cellpadding="15" cellspacing="15" class="auto-style2">
                        <tr>
                            <td style="width: 50%">
                    <asp:Label ID="Label1" runat="server" Font-Names="Arial" Font-Size="24pt" Text="Order" ForeColor="White"></asp:Label>
                            </td>
                            <td style="vertical-align: top; width: 50%;" align="right">
                            <cc1:anyObject ID="clTemp" runat="server" Visible="False" />
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
                                    <td style="width: 20%; vertical-align: top; text-align: center;" class="auto-style4">
                                        <asp:Button ID="Button1" runat="server" Text="Button" />
                                        <asp:HiddenField ID="hfRowIndex" runat="server" />
                                        <asp:HiddenField ID="hfColumnName" runat="server" />
                                        <asp:HiddenField ID="hfNewValue" runat="server" />
                                        <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
                                        <asp:Button ID="btnTest" runat="server" Text="Test" OnClientClick="testCall(); return false;" />
                                    </td>
                                    <td style="width: 60%;align-items:flex-start" >
                                    <asp:Button ID="Button2" runat="server" Text="Button" />
                                     <br />
                                        <div style="display: flex;flex-direction: row; justify-content:flex-start">

                                        <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333" Font-Names="Arial" AutoGenerateColumns="False" OnRowCreated="GridView1_RowCreated">
                                            <AlternatingRowStyle BackColor="White" />
                                            <Columns>
                                                <asp:BoundField DataField="MemberName" />
                                                <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <div class="cell-wrapper" onclick="editCell(this)">
                                                                <asp:Label ID="lblValue" runat="server"
                                                                    Text='' />

                                                                <asp:TextBox ID="txtValue" runat="server"
                                                                    Text=''
                                                                    Style="display:none; width:85%;"
                                                                    onblur="saveCell(this)"
                                                                    onkeydown="return handleEnter(event, this);" />
                                                            </div>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="150px" />
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
                                       </div>
                                    </td>
                                    <td style="width: 20%; vertical-align: top;">
                                        &nbsp;</td>
                        </tr>
                                </table>
                        </td>
                     </tr>
                        </table>

</form>
</body>
</html>

