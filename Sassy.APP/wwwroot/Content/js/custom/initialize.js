/*全局配置*/

/*CSRF验证*/
var verificationTokenJs = {};

/*是否开启防御CSRF验证*/
verificationTokenJs.enabled = true;

/*存储Token的input名称*/
verificationTokenJs.key = '__RequestVerificationToken';

/*初始化*/
$(function () {
	/*初始化高级查询*/
	initSearch();
});

/*高级查询*/
function initSearch() {
	var $sf = $('.search-condition');
	var _curHeight = $sf.height();
	if ($('#seniorSearch').length > 0) {
		$('#seniorSearch').bind('click', function () {
			var $this = $(this);
			if ($sf.css('height') == "60px") {
				var _autoHeight = $sf.css("height", "auto").height();
				$sf.height(_curHeight).animate({ height: _autoHeight }, 500, function () {
					$this.find('i').attr("class", "el-icon-arrow-left el-icon--right");
				});
			}
			else {
				var _autoHeight = $sf.css("height", "auto").height();
				$sf.animate({ height: _curHeight }, 500, function () {
					$this.find('i').attr("class", "el-icon-arrow-up el-icon--right");
				});
			}
		});
	}
}