function ShowInDialog(lcTitle, myURL, lcHeight, lcWidth, CallBackCode, lcGUID) {

    var dynamicDialog = window.parent.$(
        '<div id="' + lcGUID + '" class="' + lcGUID + '" >\
                    <input id="xDialogInput" type="hidden" /> \
                    <iframe class="iframe ui-corner-all" style="border: 1px solid #c5c5c5;width: 100%; height: 100%;" src="' + myURL + '"></iframe> \
            </div>');

    dynamicDialog.dialog({
        border: 0,
        Padding: 0,
        height: lcHeight,
        width: lcWidth,
        title: lcTitle,
        draggable: true,
        resizable: true,
        modal: true,
        show: "slideDown",
        position: { my: "center" },
        create: function (event) {
            $("body").css({ overflow: 'hidden' });
        },
        beforeClose: function (event, ui) {
            $("body").css({ overflow: 'inherit' })
        },
        close: function () {
            eval(CallBackCode);
        }
        , overlay: {
            opacity: 1,
            background: "black"
        },
        dialogClass: 'ui-widget-shadow'
    });
    // register an even ro capture the dialog close
    //$('div#xMyDialog').on('dialogclose', function (event) {
    //    alert('closed');
    //});
    return false;
}

function ShowInWindow(lcTitle, myURL, lcHeight, lcWidth, CallBackCode, lcGUID) {
    var dynamicDialog = window.parent.$(
        '<div id="' + lcGUID + '" class="' + lcGUID + '" >\
                    <input id="xDialogInput" type="hidden" /> \
                    <iframe class="iframe ui-corner-all" style="border: 1px solid #c5c5c5;width: 100%; height: 100%;" src="' + myURL + '"></iframe> \
            </div>');
    dynamicDialog.dialog({
        border: 0,
        Padding: 0,
        height: lcHeight,
        width: lcWidth,
        title: lcTitle,
        draggable: true,
        resizable: true,
        modal: false,
        show: "slideDown",
        position: { my: "center" },
        create: function (event) {
            $("body").css({ overflow: 'hidden' });
        },
        beforeClose: function (event, ui) {
            $("body").css({ overflow: 'inherit' })
        },
        close: function () {
            eval(CallBackCode);
        }
        , overlay: {
            opacity: 1,
            background: "black"
        },
        dialogClass: 'ui-widget-shadow'
    });
    return false;
}

function CloseDialog(lcDialogId) {
    window.parent.$('.' + lcDialogId).dialog('close')
  
}

function GoBack() {
    alert(document.referrer);
    history.go(-1);
    //var backLocation = document.referrer;
    //if (backLocation) {
    //    if (backLocation.indexOf("?") > -1) {
    //        backLocation += "&randomParam=" + new Date().getTime();
    //    } else {
    //        backLocation += "?randomParam=" + new Date().getTime();
    //    }
    //    window.location.assign(backLocation);
    //}
}


//function loaded() {
//    console.log('loaded');
//}

//$('button').on('click', function () {
//    var iframe = $('<iframe src="http://www.w3schools.com"></iframe>');
//    iframe.insertAfter('button');
//    iframe.load(function () {
//        loaded();
//    });
//});

//$(window).on('load', function () {
//    loaded();
//});



function loaded() {
    //    console.log('loaded');
    //alert('Loaded');
    //iframe.style.visibility = "visible";
    var iframe = window.parent.document.getElementById("MainIFrame")
    iframe.style.visibility = "visible";
}

//function iFrameLoaded(id, src) {
//    var deferred = $.Deferred(),
//        iframe = $("<iframe class='hiddenFrame'></iframe>").attr({
//            "id": id,
//            "src": src
//        });

//    iframe.load(deferred.resolve);
//    iframe.appendTo("body");

//    deferred.done(function () {
//        console.log("iframe loaded: " + id);
//    });

//    return deferred.promise();
//}

function SetInIframe_CMM(myURL) {
    var iframe = window.parent.document.getElementById("MainIFrame")
    iframe.src = myURL;
    return false;
}

function SetInIframe(myURL) {
    var iframe = window.parent.document.getElementById("MainIFrame")
    //$(iframe).block({ message: null });
    //({ css: { backgroundColor: '#ffffff', color: '#fff' } }); ''
    window.parent.$.blockUI({
        css: {
            backgroundColor: '#ffffff',
            color: '#000000',
            border: "0px",
            padding: "10px",
            opacity: 0.8
        }
    });
    //$window.parent.document.blockUI({ message: '<h1><img src="busy.gif" /> Just a moment...</h1>' });
    iframe.src = myURL;
    return false;
}


//function CreateSidePannel(myURL) {
//    var dynamicDialog = window.parent.$(
//        '<div id="TEST">\
//                    <input id="xDialogInput" type="hidden" /> \
//                    <iframe class="iframe ui-widget-content ui-corner-all" style="border:1px;width: 100%; height: 100%;" src="'+ myURL + '"></iframe> \
//            </div>');
//    dynamicDialog.dialog({
//        position:{
//            my: 'left',
//            at: 'left'},
//        height: 600,
//        width: 500,
//        title: "Main Menu",
//        draggable: true,
//        resizable: true,
//        modal: false,
//             overlay: {
//                opacity: 1,
//                background: "black"
//            }
//    });
//    return false;
//}

function CreateSidePannel(lcTitle, myURL, lcWidth, CallBackCode, lcGUID) {
    var dynamicDialog = window.parent.$(
        '<div id="' + lcGUID + '" class="' + lcGUID + '" >\
                    <input id="xDialogInput" type="hidden" /> \
                    <iframe class="iframe ui-widget-content ui-corner-all" style="border: 1px solid #c5c5c5;width: 100%; height: 100%;" src="' + myURL + '"></iframe> \
         </div>');
    dynamicDialog.dialog({
        border: 0,
        Padding: 0,
        height: $(window.parent).height(),
        //height: "auto !important",
        width: lcWidth,
        //title: lcTitle,
        draggable: true,
        resizable: true,
        modal: true,
        show: "slideDown",
        position: { my: "right top", of: $(window.parent), at: "right top" },

        create: function (event) {
            $("body").css({ overflow: 'hidden' });
        },
        beforeClose: function (event, ui) {
            $("body").css({ overflow: 'inherit' })
        },
        //create: function (event) { $(event.target).parent().css('position', 'fixed'); },
        close: function () {
            eval(CallBackCode);
        }
        , overlay: {
            opacity: 10,
            background: "red"
        },
        dialogClass: 'ui-widget-shadow',

    });
    return false;
}

function CreateMainPannel() {
    var dynamicDialog = window.parent.$(
        '<div id="xMyMainPannel" class="ui-panel ui-panel-position-left ui-panel-display-push ui-body-a ui-panel-animate ui-panel-closed"> \
                <input id="xDialogInput" type="hidden" /> \
            </div>');
    dynamicDialog.draggable({
        height: 500,
        width: 600,
        draggable: true,
        resizable: true,
        modal: false,
        show: "slideDown",
        position: { my: "Left" }
    });

    $(window).load(function () {
        $(function () {
            $(".leftcolumn").resizable(
            {
                autoHide: true,
                handles: 'e',
                resize: function (e, ui) {
                    var parent = ui.element.parent();
                    var greenboxspace = parent.width() - ui.element.outerWidth(),
                        redbox = ui.element.next(),
                        redboxwidth = (greenboxspace - (redbox.outerWidth() - redbox.width())) / parent.width() * 100 + "%";
                    redbox.width(redboxwidth);
                },
                stop: function (e, ui) {
                    var parent = ui.element.parent();
                    ui.element.css(
                    {
                        width: ui.element.width() / parent.width() * 100 + "%",
                    });
                }
            });
        });
    });
    return false;
}

function ShowMessage(myMessage) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    toastr["success"](myMessage)
    //toastr.info(myMessage);
    return false;
}

function ShowError(myError) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    toastr["error"](myError)
}


function Block_Input() {
    //$.blockUI({ message: '<img src="busy.gif" /><h1> Please wait...</h1>' });
    $.blockUI({
        css: {
            backgroundColor: '#ffffff',
            color: '#000000',
            border: "0px",
            padding: "10px",
            opacity: 0.8
        }
    });
}


$(function () {
    $(".date").datepicker({
        changeMonth: true,
        changeYear: true,
        buttonImage: "images/calendar.gif",
        dateFormat: "dd/mm/yy"
    });
});


$(function () {
    $(".MeBoxDate").datepicker({
        changeMonth: true,
        changeYear: true,
        buttonImage: "images/calendar.gif",
        dateFormat: "dd/mm/yy"
    });

    $(".CMM-Date").datepicker({
        changeMonth: true,
        changeYear: true,
        buttonImage: "images/calendar.gif",
        dateFormat: "dd M yy"
    });

});

