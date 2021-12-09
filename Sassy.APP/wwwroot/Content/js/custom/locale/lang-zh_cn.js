/**element-extend**/
if ($.fn.elementExtend) {
	$.fn.elementExtend = {
		Grid: {
			missObject: '对象不存在!',
			onlyOneText: '请至少选择一条要操作的信息!',
			atLeastOneText: '请最多选择一条要操作的信息!',
			confirmText: '确实要进行该操作吗?',
			defaultButtonText: '保存'
		},
		MessageBox: {
			title: '系统提示',
			confirmButtonText: '确定',
			cancelButtonText: '取消'
		}
	}
}

/**vue-Extend**/
if ($.fn.vueExtend) {
	$.fn.vueExtend = {
		Ajax: {
			errorText: '远程地址未响应,请重新尝试!'
		},
		openFileView: {
			title: '上传文件'
		}
	}
}

/**echart**/
if ($.fn.echartExtend) {
	$.fn.echartExtend = {
		AjaxSet: {
			loadingText: '数据加载中...'
		}
	}
}