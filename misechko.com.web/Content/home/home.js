mp.HomeLogic = (function($) {
    "use strict";

    var onPause = false;

    function InitImageRotator() {
        var item = 0,
            nextItem = item + 1,
            prefix = '',
            intv,
            main_function,
            slide_next_func,
            slide_previous_func;

        $('.pause-button').on('click', function() {
            if (onPause) {
                onPause = false;
            } else onPause = true;
        });

        var return_prefix = function() {
            var ww = $(window).width();

            if (ww <= 480) {
                return '-s';
            } else if (ww <= 768) {
                return '-m';
            }

            return '';
        };

        $(window).resize(function() {
            prefix = return_prefix();


            //$('[class*="item_"]:not(.rotator_bg)').css({ left: $(this).width() + 'px' })
            var $window = $(this);

            $('[class*="item_"]:not(.rotator_bg)').each(function() {
                var $this = $(this);
                if ($this.data('right')) {
                    $this.css({ right: $this.data('right') });
                } else {
                    mobile_menu_link
                    $this.css({ left: $window.width() + 'px' });
                }
            });

            $('.item_' + item + ':not(.rotator_bg)').each(function() {
                var $this = $(this),
                    top = ($this.data('top' + prefix)) ? $this.data('top' + prefix) : $this.data('top');

                if ($this.data('right')) {
                    var right = ($this.data('right' + prefix)) ? $this.data('right' + prefix) : $this.data('right');
                    $this.css({ top: top, right: right });
                } else {
                    var left = ($this.data('left' + prefix)) ? $this.data('left' + prefix) : $this.data('left');
                    $this.css({ top: top, left: left });
                }


            });
        }).resize();

        main_function = function() {
            prefix = return_prefix();
            var ww = $(window).width();


            // setup next frame
            $('.item_' + nextItem + ':not(.rotator_bg)').each(function() {
                var $this = $(this),
                    top = '',
                    left = '',
                    right = '';

                if ($this.data('start-top')) {
                    top = $this.data('start-top');
                } else if ($this.data('top' + prefix)) {
                    top = $this.data('top' + prefix);
                } else {
                    top = $this.data('top');
                }

                if ($this.data('right')) {
                    if ($this.data('start-right')) {
                        right = $this.data('start-right');
                    } else {
                        right = $this.data('right');
                    }
                    $this.css({ top: top, right: right });
                } else {
                    if ($this.data('start-left')) {
                        left = $this.data('start-left');
                    } else {
                        //left = '200%';
                        left = ww + 'px';
                    }
                    $this.css({ top: top, left: left });
                }
            });

            

            // animate current frame out

            $('.item_' + item + ':not(.rotator_bg)').each(function() {
                var $this = $(this);
                if ($this.data('right')) {
                    $this.animate({ right: '-' + ww + 'px' }, 250);
                } else {
                    $this.animate({ left: '-' + ww + 'px' }, 250);
                }
            });

            //$('.rotator_bg.spot_' + spot).fadeOut(250);

            var delay = 0;
            var delay2 = (nextItem === 4) ? 0 : 100;

            // animate next spot
            $('.item_' + nextItem + ':not(.rotator_bg)').each(function() {
                var $this = $(this),
                    top = ($this.data('top' + prefix)) ? $this.data('top' + prefix) : $this.data('top');

                if ($this.data('right')) {
                    var right = ($this.data('right' + prefix)) ? $this.data('right' + prefix) : $this.data('right');

                    setTimeout(function() {
                        $this.animate({ top: top, right: right });
                    }, delay += delay2);
                } else {
                    var left = ($this.data('left' + prefix)) ? $this.data('left' + prefix) : $this.data('left');

                    setTimeout(function() {
                        $this.animate({ top: top, left: left });
                    }, delay += delay2);
                }
            });
            //$('.rotator_bg.spot_' + nextItem).fadeIn(250);
        };

        slide_next_func = function(man) {
            if (onPause && !man) return;

            //if (item === 1) return;

            nextItem = (item === 4) ? 0 : item + 1; //(item === n) - here, n is a zero-based count of current banner items

            main_function();

            item = nextItem;
        };
        slide_previous_func = function(man) {
            nextItem = (item === 0) ? 4 : item - 1; //Here ? n : is a zero-based count of current banner items 

            main_function();

            item = nextItem;
        };

        intv = setInterval(slide_next_func, 8000);

        $('.rotator_control').fadeTo(0, 0).on('click', function() {
            var $this = $(this);

            window.clearInterval(intv);

            if ($this.hasClass('left')) {
                slide_previous_func(true);
            } else if ($this.hasClass('right')) {
                slide_next_func(true);
            }

            intv = setInterval(slide_next_func, 8000);

            return false;
        });

        $('#banner-block').hover(function() {
            $('.rotator_control').fadeTo(0, 1);
        }, function() {
            $('.rotator_control').fadeTo(0, 0);
        });

        $('.rotator_control').hover(function() {
            $(this).find('> .bg').fadeIn();
        }, function() {
            $(this).find('> .bg').fadeOut();
        });
    };

    var init = function() {
        $('#tri-block').equalHeights();
        InitImageRotator();
    };

    return { init: init };
}($));

mp.Home = (function ($) {
    "use strict";

    var ready = $(function() {
        mp.HomeLogic.init();
    });
}($));
        
        


       



