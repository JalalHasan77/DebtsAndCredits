<%@ Page Language="VB" AutoEventWireup="true" Codefile="MainPage.aspx.vb" Inherits="MainPage"%>
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
                                    <td style="width: 20%; vertical-align: top; text-align: center;" class="auto-style4">
                                        <asp:Button ID="Button1" runat="server" Text="Button" />
                                    </td>
                                    <td style="width: 60%">
                                        &nbsp;</td>
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

