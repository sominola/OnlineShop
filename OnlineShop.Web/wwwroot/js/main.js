(function ($) {
    "use strict";

    /*------------------
   Non sticky footer
   --------------------*/
    function setFooterStyle() {
        var docHeight = $(window).height();
        var footerHeight = $('#footer').outerHeight();
        var footerTop = $('#footer').position().top + footerHeight;
        if (footerTop < docHeight) {
            $('#footer').css('margin-top', (docHeight - footerTop) + 'px');
        } else {
            $('#footer').css('margin-top', '');
        }
        $('#footer').removeClass('invisible');
    }

    $(document).ready(function () {
        setFooterStyle();
        window.onresize = setFooterStyle;
    })

    $(function () {
        $(document).ready(function () {
            if (window.location.href.indexOf("#_=_") > -1) {
                //remove facebook oAuth response bogus hash
                if (window.history && window.history.pushState) {
                    history.pushState('', document.title, window.location.pathname);
                } else {
                    window.location.href = window.location.href.replace(location.hash, "");
                }
            }
        });
    });

})(jQuery);