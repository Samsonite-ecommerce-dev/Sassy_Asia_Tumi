//vueExtend
var vueExtend = {};
//语言包
$.fn.vueExtend = {
	Ajax: {
		errorText: 'The requested URL could not be retrieved, please try again!'
	},
	openFileView: {
		title: 'Upload File'
	}
}

/*
 * Ajax的相关方法
 * >>Get
 * >>Post
 * >>SubmitForm
*/
vueExtend.Ajax = {
	Get: function (settings) {
		var _url = (settings.url === undefined) ? "" : settings.url;
		var _dataType = (settings.dataType === undefined) ? 'json' : settings.dataType;
		var _headers = (settings.headers === undefined) ? {} : settings.headers;
		var _params = (settings.params === undefined) ? {} : settings.params;
		var _loading = (settings.loading === undefined) ? false : settings.loading;
		var _beforeSend = (typeof (settings.beforeSend) === 'function') ? settings.beforeSend : null;
		var _afterSend = (typeof (settings.afterSend) === 'function') ? settings.afterSend : null;
		var _success = (typeof (settings.success) === 'function') ? settings.success : null;
		//设置拦截器显示loading
		if (_loading) {
			axios.interceptors.request.use(function (config) {
				if (_beforeSend)
					_beforeSend(config);
				return config;
			}, function (error) {
				if (_afterSend)
					_afterSend(config);
				return Promise.reject(error);
			});

			axios.interceptors.response.use(function (data) {
				if (_afterSend)
					_afterSend(data);
				return data;
			}, function (error) {
				if (_afterSend)
					_afterSend(data);
				return Promise.reject(error);
			});
		}
		axios({
			url: _url,
			method: 'get',
			responseType: _dataType,
			headers: _headers,
			params: _params,
			timeout: 20000,
		}).then(function (response) {
			if (_success) _success(response.data);
		}).catch(function (error) {
			alert(error);
		});
	},
	Post: function (settings) {
		var _url = (settings.url === undefined) ? "" : settings.url;
		var _dataType = (settings.dataType === undefined) ? 'json' : settings.dataType;
		var _headers = (settings.headers === undefined) ? {} : settings.headers;
		var _params = (settings.params === undefined) ? {} : settings.params;
		var _loading = (settings.loading === undefined) ? false : settings.loading;
		var _beforeSend = (typeof (settings.beforeSend) === 'function') ? settings.beforeSend : null;
		var _afterSend = (typeof (settings.afterSend) === 'function') ? settings.afterSend : null;
		var _success = (typeof (settings.success) === 'function') ? settings.success : null;
		//设置拦截器显示loading
		if (_loading) {
			axios.interceptors.request.use(function (config) {
				if (_beforeSend)
					_beforeSend(config);
				return config;
			}, function (error) {
				if (_afterSend)
					_afterSend(config);
				return Promise.reject(error);
			});

			axios.interceptors.response.use(function (data) {
				if (_afterSend)
					_afterSend(data);
				return data;
			}, function (error) {
				if (_afterSend)
					_afterSend(data);
				return Promise.reject(error);
			});
		}
		//转换成参数
		var _axiosParams;
		var _browserType = browserType();
		//注:URLSearchParams不支持IE和Opera
		if (_browserType == "Opera" || _browserType == "Edge") {
			_axiosParams = '';
			for (key in _params) {
				if (_axiosParams == '') {
					_axiosParams = key + '=' + encodeURIComponent(_params[key]);
				}
				else {
					_axiosParams += '&' + key + '=' + encodeURIComponent(_params[key]);
				}
			}
		}
		else {
			//By default, axios serializes JavaScript objects to JSON.To send data in the application/x-www-form-urlencoded format instead, you can use one of the following options
			_axiosParams = new URLSearchParams();
			for (key in _params) {
				_axiosParams.append(key, _params[key]);
			}
		}
		//CSRF验证
		if (verificationTokenJs.enabled) {
			if (!_headers.hasOwnProperty(verificationTokenJs.key)) {
				_headers.__RequestVerificationToken = $('input[name=' + verificationTokenJs.key + ']').val();
			}
		}
		axios({
			url: _url,
			method: 'post',
			responseType: _dataType,
			headers: _headers,
			data: _axiosParams,
			timeout: 20000,
		}).then(function (response) {
			if (_success) _success(response.data);
		}).catch(function (error) {
			alert(error);
		});
	}
};

//格式化货币
vueExtend.formatCurrency = function (num) {
	var _result = '';
	num = num.toString().replace(/\$|\,/g, '');
	if (isNaN(num))
		num = '0';
	var sign = (num == (num = Math.abs(num)));
	num = Math.floor(num * 100 + 0.50000000001);
	var cents = num % 100;
	num = Math.floor(num / 100).toString();
	if (cents > 0 && cents < 10)
		cents = '0' + cents;
	for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++) {
		num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));
	}
	if (cents == 0) {
		_result = (((sign) ? '' : '-') + num);
	}
	else {
		_result = (((sign) ? '' : '-') + num + '.' + cents);
	}
	return _result;
}

//生成随机密钥
vueExtend.createKey = function (num) {
	var _result = '';
	var _keyArrays = new Array('a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
	for (var t = 0; t < num; t++) {
		var _l = Math.floor(Math.random() * 2);
		var _k = (_l == 1) ? _keyArrays[Math.floor(Math.random() * 36)].toUpperCase() : _keyArrays[Math.floor(Math.random() * 36)];
		_result += _k;
	}
	return _result;
}

//上传文件
/*
 * vue
 * title
 * model
 * catalog
 * width
 * height
 */
vueExtend.openFileView = function (settings) {
	var $vue = (settings.vue === undefined) ? null : settings.vue;
	if (settings.title === undefined) settings.title = $.fn.vueExtend.openFileView.title;
	if (settings.catalog === undefined) settings.catalog = '';
	if (settings.width === undefined) settings.width = '100%';
	if (settings.height === undefined) settings.height = '100%';
	//对当前input设置一个标识
	//settings.object.parent().find('input[type="hidden"]').attr("title", "value");
	//settings.object.parent().find('input[type="text"]').attr("title", "url");
	if ($vue) {
		elementExtend.Grid.OpenDialog({
			vue: $vue,
			url: '/Upload/Index?model=' + settings.model + '&catalog=' + settings.catalog,
			title: settings.title,
			width: settings.width
		});
	}
}

//判断浏览器
function browserType() {
	var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串
	var isOpera = userAgent.indexOf("Opera") > -1; //判断是否Opera浏览器
	var isIE = userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1 && !isOpera || userAgent.indexOf("rv:11") > -1; //判断是否IE浏览器
	var isEdge = userAgent.indexOf("Chrome") > -1 && userAgent.indexOf("Edge") > -1 && !isIE; //判断是否IE的Edge浏览器
	var isFF = userAgent.indexOf("Firefox") > -1; //判断是否Firefox浏览器
	var isSafari = userAgent.indexOf("Safari") > -1 && userAgent.indexOf("Chrome") == -1; //判断是否Safari浏览器
	var isChrome = userAgent.indexOf("Chrome") > -1 && userAgent.indexOf("Safari") > -1; //判断Chrome浏览器
	if (isIE) {
		if (userAgent.indexOf("rv:11") > -1) {
			return "IE11";
		}
		if (userAgent.indexOf("rv:12") > -1) { //这一段还没验证
			return "IE12";
		}
		var reIE = new RegExp("MSIE (\\d+\\.\\d+);");
		reIE.test(userAgent);
		var fIEVersion = parseFloat(RegExp["$1"]);
		if (fIEVersion == 7) { return "IE7"; }
		else if (fIEVersion == 8) { return "IE8"; }
		else if (fIEVersion == 9) { return "IE9"; }
		else if (fIEVersion == 10) { return "IE10"; }
		else if (fIEVersion == 11) { return "IE11"; }
		else if (fIEVersion == 12) { return "IE12"; }
		else { return "0" }//IE版本过低  
	}

	if (isOpera) { return "Opera"; }
	if (isEdge) { return "Edge"; }
	if (isFF) { return "FF"; }
	if (isSafari) { return "Safari"; }
	if (isChrome) { return "Chrome"; }
}