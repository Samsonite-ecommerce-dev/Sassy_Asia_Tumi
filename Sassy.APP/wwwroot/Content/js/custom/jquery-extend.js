//扩展方法
$.extend({
	requestQueryString: function (key) {
		var _data = {};
		var aPairs, aTmp;
		var queryString = new String(window.location.search);
		queryString = queryString.substr(1, queryString.length);
		aPairs = queryString.split("&");
		for (var i = 0; i < aPairs.length; i++) {
			aTmp = aPairs[i].split("=");
			_data[aTmp[0]] = aTmp[1];
		}
		return _data[key];
	},
	virtualSubForm: function (url, params) {
		//获取模拟form
		var formId = 'JQ_virtualSubForm';
		var $subform = $('#' + formId);
		if ($subform.length == 0) {
			//创建form
			$subform = $("<form id='" + formId + "' method='post'></form>");
			$subform.attr({ "action": url });
			//提交参数
			var input;
			$.each(params, function (key, value) {
				input = $("<input type='hidden'>");
				input.attr({ "name": key });
				input.val(value);
				$subform.append(input);
			});
			$(document.body).append($subform);
		}
		else {
			//input重新赋值
			var $input = $subform.find('input[type=hidden]');
			var index = 0;
			for (key in params) {
				$input.eq(index).val(params[key]);
				index++;
			}
		}
		//提交数据
		$subform.submit();
	},
	ajaxPost: function (settings) {
		var _url = (settings.url === undefined) ? "" : settings.url;
		var _dataType = (settings.dataType === undefined) ? 'json' : settings.dataType;
		var _params = (settings.params === undefined) ? {} : settings.params;
		var _beforeSend = (typeof (settings.beforeSend) === 'function') ? settings.beforeSend : null;
		var _error = (typeof (settings.error) === 'function') ? settings.error : null;
		var _success = (typeof (settings.success) === 'function') ? settings.success : null;
		$.ajax(
			{
				url: _url,
				type: 'post',
				data: _params,
				dataType: _dataType,
				timeout: 20000,
				beforeSend: function (XMLHttpRequest) {
					if (_beforeSend) _beforeSend();
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					if (_error) _error();
					alert(errorThrown);
				},
				success: function (data, textStatus) {
					if (_success) _success(data);
				}
			});
	}
});

//扩展对象
$.fn.extend({
	//在字符串中插入字符
	insertAtCaret: function (myValue) {
		var $t = $(this)[0];
		if (document.selection) {
			this.focus();
			sel = document.selection.createRange();
			sel.text = myValue;
			this.focus();
		}
		else
			if ($t.selectionStart || $t.selectionStart == '0') {
				var startPos = $t.selectionStart;
				var endPos = $t.selectionEnd;
				var scrollTop = $t.scrollTop;
				$t.value = $t.value.substring(0, startPos) + myValue + $t.value.substring(endPos, $t.value.length);
				this.focus();
				$t.selectionStart = startPos + myValue.length;
				$t.selectionEnd = startPos + myValue.length;
				$t.scrollTop = scrollTop;
			}
			else {
				this.value += myValue;
				this.focus();
			}
	}
});