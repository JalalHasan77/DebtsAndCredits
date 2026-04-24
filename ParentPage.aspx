<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ParentPage.aspx.vb" Inherits="ParentPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Parent Page</title>
    <style>
        /* ── Overlay: covers entire screen, blocks clicks ── */
        #modalOverlay {
            display:          none;
            position:         fixed;
            top:              0;
            left:             0;
            width:            100%;
            height:           100%;
            background-color: rgba(0, 0, 0, 0.55);  /* dark translucent backdrop */
            z-index:          1000;                   /* sits above everything     */
        }

        /* ── Dialog box: centered on screen ── */
        #modalDialog {
            position:         absolute;
            top:              50%;
            left:             50%;
            transform:        translate(-50%, -50%);
            width:            420px;
            background-color: #ffffff;
            border-radius:    8px;
            box-shadow:       0 8px 30px rgba(0,0,0,0.3);
            padding:          30px;
            z-index:          1001;
        }

        #modalDialog h3 {
            margin-top: 0;
            font-family: Arial, sans-serif;
        }

        .option-link {
            display:         block;
            padding:         10px 14px;
            margin:          6px 0;
            background:      #f0f4ff;
            border-radius:   5px;
            text-decoration: none;
            color:           #333;
            font-family:     Arial, sans-serif;
            font-size:       15px;
            cursor:          pointer;
        }

        .option-link:hover {
            background: #d0dcff;
        }

        .btn-close {
            margin-top:    16px;
            padding:       8px 18px;
            background:    #cc0000;
            color:         #fff;
            border:        none;
            border-radius: 4px;
            cursor:        pointer;
            font-size:     14px;
        }
    </style>

    <script type="text/javascript">
        // ── Open the modal ──────────────────────────────────────────
        function openDialog() {
            document.getElementById('modalOverlay').style.display = 'block';

            // Prevent background scrolling while modal is open
            document.body.style.overflow = 'hidden';

            // Optional: trap Tab key inside modal so focus never leaves it
            trapFocus(document.getElementById('modalDialog'));
        }

        // ── Close the modal ─────────────────────────────────────────
        function closeDialog() {
            document.getElementById('modalOverlay').style.display = 'none';
            document.body.style.overflow = '';  // restore scrolling
        }

        // ── Receive selected value and close ────────────────────────
        function receiveValue(selectedValue, displayText) {
            // Store in hidden field (accessible server-side)
            document.getElementById('<%= hdnSelectedValue.ClientID %>').value = selectedValue;

            // Show human-readable label in the textbox
            document.getElementById('<%= txtSelectedValue.ClientID %>').value = displayText || selectedValue;

            closeDialog();

            // Optional: auto-postback to process server-side immediately
            // __doPostBack('<%= btnProcess.UniqueID %>', '');
        }

        // ── Prevent clicking the backdrop from closing ───────────────
        // (remove this function and add onclick="closeDialog()" to
        //  #modalOverlay if you WANT backdrop-click to close)
        function overlayClicked(e) {
            // Only close if user clicked the overlay itself, not the dialog box
            if (e.target === document.getElementById('modalOverlay')) {
                // closeDialog();   ← uncomment to allow backdrop-click-to-close
            }
        }

        // ── Focus trap: keeps Tab key inside the modal ───────────────
        function trapFocus(element) {
            var focusableSelectors = 'a, button, input, select, textarea, [tabindex]:not([tabindex="-1"])';
            var focusableElements = element.querySelectorAll(focusableSelectors);
            var firstEl = focusableElements[0];
            var lastEl = focusableElements[focusableElements.length - 1];

            element.addEventListener('keydown', function (e) {
                if (e.key !== 'Tab') {
                    // Block Escape key closing (keep modal strict)
                    if (e.key === 'Escape') { e.preventDefault(); }
                    return;
                }
                if (e.shiftKey) {          // Shift+Tab
                    if (document.activeElement === firstEl) {
                        e.preventDefault();
                        lastEl.focus();
                    }
                } else {                   // Tab
                    if (document.activeElement === lastEl) {
                        e.preventDefault();
                        firstEl.focus();
                    }
                }
            });

            if (firstEl) firstEl.focus();  // auto-focus first element
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">

        <h2>Parent Page</h2>
        <p>Some content on the parent page that will be blocked...</p>
        <input type="text" value="This field is unreachable while modal is open" size="45" />

        <!-- ── Link that opens the modal ── -->
        <br /><br />
        <a href="javascript:void(0);" onclick="openDialog();"
           style="font-size:16px; color:#0055cc; text-decoration:underline;">
            &#128269; Click here to select an option
        </a>

        <!-- ── Hidden field: bridges JS value → VB.NET server-side ── -->
        <asp:HiddenField ID="hdnSelectedValue" runat="server" />

        <br /><br />
        <b>Selected Value:</b>
        <asp:TextBox ID="txtSelectedValue" runat="server" ReadOnly="true" Width="200px" />

        <asp:Button ID="btnProcess" runat="server" Text="Process" style="margin-left:10px;" />
        <asp:Label  ID="lblResult"  runat="server" ForeColor="Green" style="margin-left:10px;" />


        <!-- ══════════════════════════════════════════════════════════
             MODAL OVERLAY — stays on same page, blocks parent clicks
             ══════════════════════════════════════════════════════════ -->
        <div id="modalOverlay" onclick="overlayClicked(event);">
            <div id="modalDialog">

                <h3>&#10022; Select an Option</h3>

                <!-- Static options -->
                <a class="option-link" href="javascript:void(0);"
                   onclick="receiveValue('ALPHA', 'Product Alpha');">Product Alpha</a>

                <a class="option-link" href="javascript:void(0);"
                   onclick="receiveValue('BETA', 'Product Beta');">Product Beta</a>

                <a class="option-link" href="javascript:void(0);"
                   onclick="receiveValue('GAMMA', 'Product Gamma');">Product Gamma</a>

                <hr />

                <!-- Dynamic options from server (Repeater) -->
                <asp:Repeater ID="rptOptions" runat="server">
                    <ItemTemplate>
                        <a class="option-link" href="javascript:void(0);"
                           onclick="receiveValue('<%# Eval("OptionValue") %>', '<%# Eval("OptionName") %>');">
                            <%# Eval("OptionName") %>
                        </a>
                    </ItemTemplate>
                </asp:Repeater>

                <br />
                <button type="button" class="btn-close" onclick="closeDialog();">&#10005; Cancel</button>
            </div>
        </div>
        <!-- ── end modal ── -->

    </form>
</body>
</html>
