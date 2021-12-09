/*initialize frame*/

$(function () {
	/*初始化menu*/
	InitializeMenu();

	/*客户控制面板*/
	InitializeUserControlPanel();

	/*初始化菜单和frame高度*/
	InitializeHeight();
});

function InitializeMenu() {
	var $first_level = $('.nav').find('.nav-first-level');
	/*初始化菜单显示*/
	var first_level_0 = $first_level.eq(0);
	first_level_0.find('a').addClass('active');
	first_level_0.find('a span').attr('class', 'fa fa-angle-down nav-first-arrow');
	first_level_0.parent().find('.nav-second-level').show();
	/*一级菜单点击事件*/
	$('.nav').find('.nav-first-level a').bind('click', function () {
		/*显示下拉列表*/
		var second_level = $(this).parent().parent().find('.nav-second-level');
		if (second_level.css('display') == 'none') {
			/*关闭已打开的菜单栏*/
			for (var t = 0; t < $first_level.length; t++) {
				$first_level.eq(t).find('a').removeClass('active');
				$first_level.eq(t).find('a span').attr('class', 'fa fa-angle-left nav-first-arrow');
				$first_level.eq(t).parent().find('.nav-second-level').hide();
			}
			/*更换一级菜单小图标*/
			$(this).find('span').attr('class', 'fa fa-angle-down nav-first-arrow');
			$(this).addClass('active');
			second_level.slideDown(500);
		}
		else {
			/*打开子菜单*/
			second_level.slideUp(500);
		}
	});

	/*二级菜单点击事件*/
	var $second_level = $('.nav').find('.nav-second-level');
	$('.nav').find('.nav-second-level li a').bind('click', function () {
		/*清空其它ID*/
		for (var t = 0; t < $second_level.length; t++) {
			$second_level.eq(t).find('li').removeClass('active');
		}
		$(this).parent().addClass('active');
		appFrame.openUrl($(this).attr('rel'));
	});

	/*菜单缩放按钮*/
	$('.header-minimalize').find('a').bind('click', function () {
		if ($('#nav').attr('class') == 'navbar-collapse') {
			navStyle.miniAnimate();
		}
		else {
			navStyle.normalAnimate();
		}
	});
}

function InitializeUserControlPanel() {
	$('.user-control-panel').bind('mouseover', function () {
		$(this).find('.header-right-droplist').show();
	});

	$('.user-control-panel').bind('mouseleave', function () {
		$(this).find('.header-right-droplist').hide();
	});
}

function InitializeHeight() {
	var navbarHeight = $(window).height() - 10;
	$('.navbar .navbar-collapse').css("min-height", navbarHeight + 'px');
	var frameHeight = navbarHeight - 68;
	$('#mainFrame').css("min-height", frameHeight + 'px');
}

$(window).bind("resize", function () {
	var $nav = $('#nav');
	if ($(this).width() < 769) {
		navStyle.mini();
	} else {
		navStyle.normal();
	}
});

var navStyle = {
	mini: function () {
		$('#nav').attr('class', 'navbar-mini');
		$('.frame-left').css('width', '70px');
		$('.frame-right').css('margin', '0 0 0 70px');
	},
	normal: function () {
		$('#nav').attr('class', 'navbar-collapse');
		$('.frame-left').css('width', '220px');
		$('.frame-right').css('margin', '0 0 0 220px');
	},
	miniAnimate: function () {
		$('.frame-left').animate({ width: '70px' }, 500, function () {
			$('#nav').attr('class', 'navbar-mini');
		});
		$('.frame-right').animate({ margin: '0 0 0 70px' }, 500);
	},
	normalAnimate: function () {
		$('#nav').attr('class', 'navbar-collapse');
		$('.frame-left').animate({ width: '220px' }, 500);
		$('.frame-right').animate({ margin: '0 0 0 220px' }, 500);
	}
}