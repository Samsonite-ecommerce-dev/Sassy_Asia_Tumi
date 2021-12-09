/*
 * echartExtend.Charts相关操作
 * >>Init :初始化对象
 * >>Set: 创建图表
 * >>AjaxSet: Ajax方式创建图表
*/
var echartExtend = {};
var defaultStyle = 'shine';
// 语言
$.fn.echartExtend = {
	AjaxSet: {
		loadingText: 'Loading...'
	}
}


echartExtend.Charts = {
	Init: function (id) {
		return echarts.init(document.getElementById(id), defaultStyle)
	},
	Set: function (object, option) {
		object.setOption(option);
	},
	AjaxSet: function (object, settings) {
		var $vue = (settings.vue === undefined) ? null : settings.vue;
		var _url = (settings.url === undefined) ? "" : settings.url;
		var _para = (settings.para === undefined) ? {} : settings.para;
		var _success = (typeof (settings.success) === 'function') ? settings.success : null;
		$.ajax(
			{
				url: _url,
				type: 'post',
				data: _para,
				dataType: 'json',
				timeout: 20000,
				beforeSend: function (XMLHttpRequest) {
					object.showLoading({ text: $.fn.echartExtend.AjaxSet.loadingText });
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					object.hideLoading();
					elementExtend.Tips.Error($vue, $.fn.vueExtend.ajax.errorText, 2);
				},
				success: function (data, textStatus) {
					object.hideLoading();
					if (_success) _success(data);
				}
			});
	}
};