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
            var colIndex = wrapper.getAttribute("data-columnindex");
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

                },
                function (error) {

                }
            );


            calculateColumn(colIndex);
           //updateRowProfit(rowIndex);
            //alert('rowIndex :' + rowIndex + 'colIndex: ' + colIndex);
            iterateRowCells(rowIndex);
           // iterateColumnCells(colIndex);

        }



        function iterateRowCells(rowIndex) {
            rowIndex = rowIndex.toString();

            var rowCells = document.querySelectorAll(
                ".cell-wrapper[data-rowindex='" + rowIndex + "']"
            );
            var MasterTotalProfit;
            MasterTotalProfit = 0;

            rowCells.forEach(function (cellWrapper) {
                var TotalProfit;
                TotalProfit = 0;

                var colIndex = parseInt(cellWrapper.getAttribute("data-columnindex"));

    
                // ✅ Skip first 4 columns
                if (colIndex <= 3) return;

                var span = cellWrapper.querySelector("span");
                var input = cellWrapper.querySelector("input");

                var value = 0;

                if (span && span.innerText.trim() !== "") {
                    value = parseFloat(span.innerText) || 0;
                }
                else if (input && input.value.trim() !== "") {
                    value = parseFloat(input.value) || 0;
                  
                }
                
                if (value != 0) {
                   // alert(value);
                    var cellWrapper2 = document.querySelector(
                        ".cell-wrapper[data-headercol='" + colIndex + "'][data-headerlevel='1']"
                    );
                    var value2;
                    var span2 = cellWrapper2.querySelector("span");
                    var input2 = cellWrapper2.querySelector("input");
                    if (span2 && span2.innerText.trim() !== "") {
                        value2 = parseFloat(span2.innerText) || 0;
                    }
                    else if (input2 && input2.value.trim() !== "") {
                        value2 = parseFloat(input2.value) || 0;
                    }

                    if (value2 != 0) {
                      //  alert('Net profit' + value * value2);
                        MasterTotalProfit = MasterTotalProfit + value * value2;
                    }
                }
                // 👉 Your calculation here
            });


            var profitCell = document.querySelector(
                ".cell-wrapper[data-rowindex='" + rowIndex + "'][data-columnindex='3'] span"
            );
            var columnName = "Profit";
            if (profitCell) {
                profitCell.innerText = MasterTotalProfit.toFixed(3);

                PageMethods.SaveCell(
                    parseInt(rowIndex),
                    columnName,
                    profitCell.innerText,
                    function () {
                    },
                    function (error) {
                    }
                );
            }
        }



        function iterateColumnCells(colIndex) {
            colIndex = colIndex.toString();

            var columnCells = document.querySelectorAll(
                ".cell-wrapper[data-columnindex='" + colIndex + "']"
            );


            columnCells.forEach(function (cellWrapper) {

                var rowIndex = cellWrapper.getAttribute("data-rowindex");
               
                var span = cellWrapper.querySelector("span");
                var input = cellWrapper.querySelector("input");

                var value = 0;

                if (span && span.innerText.trim() !== "")
                    value = parseFloat(span.innerText) || 0;
                else if (input && input.value.trim() !== "")
                    value = parseFloat(input.value) || 0;
                // 👉 Do your calculation here
            });
        }


    
    function handleEnter(e, textbox) {
        if (e.key === "Enter") {
                textbox.blur();
            return false;
        }
        return true;
        }

        function saveHeader(textbox) {

            var wrapper = textbox.closest(".cell-wrapper");
            var colIndex = wrapper.getAttribute("data-columnindex");
            var lbl = textbox.previousElementSibling;

            lbl.innerText = textbox.value;
            textbox.style.display = "none";
            lbl.style.display = "inline";

            var colIndex = wrapper.getAttribute("data-headercol");
            var level = wrapper.getAttribute("data-headerlevel");

            PageMethods.SaveHeader(
                parseInt(colIndex),
                parseInt(level),
                textbox.value,
                function () { },
                function (err) { console.log(err.get_message()); }
            );

            calculateColumn(colIndex);

            if (level == 1) {
             iterateThroughAllCells()
            }

        }


        function iterateThroughAllCells() {
            //====================
            var rowCells = document.querySelectorAll(
                ".cell-wrapper[data-columnindex='4']"
            );
            rowCells.forEach(function (cellWrapper) {
                var rowIndex = cellWrapper.getAttribute("data-rowindex");

                if (parseInt(rowIndex) > 0) {
                    // safe to execute
                    iterateRowCells(rowIndex);
                }
            })
            }
        
        function calculateColumn(colIndex) {

            var sum = 0;

            // Sum all data cells in this column
            var cells = document.querySelectorAll(
                ".data-cell[data-columnindex='" + colIndex + "'] input"
            );

            cells.forEach(function (txt) {
                var val = parseFloat(txt.value);
                if (!isNaN(val)) {
                    sum += val;
                }
            });
    
            // Get price header
            var priceInput = document.querySelector(
                ".cell-wrapper[data-columnindex='" + colIndex + "'][data-level='5'] input"
            );

            if (!priceInput) return;

            var price = parseFloat(priceInput.value);
            if (isNaN(price)) price = 0;

            var lcValue = sum * price;

            updateHeader(colIndex, '2', lcValue);
            updateHeader(colIndex, '3', sum);
        }


        function updateHeader(colIndex, level, lcValue) {

            var wrapper = document.querySelector(
                ".cell-wrapper[data-columnindex='" + colIndex + "'][data-level='" + level + "']"
            );

            var lcLabel = wrapper ? wrapper.querySelector("span") : null;

            if (lcLabel) {
                lcLabel.innerText = lcValue.toFixed(3);

                // Get Header info
                var headerColIndex = wrapper.getAttribute("data-headercol");
                var headerLevel = wrapper.getAttribute("data-headerlevel");

                PageMethods.SaveHeader(
                    parseInt(headerColIndex),
                    parseInt(headerLevel),
                    lcLabel.innerText,
                    function () { },
                    function (err) { console.log(err.get_message()); }
                );
            }
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
                                                                             <br />
                                        <div style="display: flex;flex-direction: row; justify-content:flex-start">

                                        <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333" Font-Names="Arial" AutoGenerateColumns="False" OnRowCreated="GridView1_RowCreated" Font-Size="12px">
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

