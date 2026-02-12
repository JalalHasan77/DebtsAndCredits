<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Charts_Test.aspx.vb" Inherits="JQ_charts_Charts_Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="/Jq/Charts/Chart.js"></script>
    <style type="text/css">
        .auto-style1 {
            width: 50%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table cellpadding="10" cellspacing="0" class="auto-style1" style="width: 200px">
                <tr>
                    <td><canvas id="myChart1" runat="server" style="width: 400px; height: 200px; "></canvas></td>
                </tr>
                <tr>
                    <td><canvas id="myChart2" runat="server" style="width: 400px; height: 100px; "></canvas></td>
                </tr>
            </table>
            
        </div>
    </form>
    <script>
        var ctx1 = document.getElementById("myChart1");
        var myChart1 = new Chart(ctx1, {
            type: 'bar',
            data: {
                labels: ["Red", "Blue", "Yellow", "Green", "Purple", "Orange"],
                datasets: [ {label: '# of Votes',data: [12, 19, 3, 5, 2, 3]} ]
            }
        });
    </script>

</body>

</html>
