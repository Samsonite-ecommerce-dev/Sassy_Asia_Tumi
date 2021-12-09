//elementExtend
var elementExtend = {};
//语言包
$.fn.elementExtend = {
	Grid: {
		missObject: 'Object reference not set to an instance of an object.',
		onlyOneText: 'Please select only one record!',
		atLeastOneText: 'Please select at least one record',
		confirmText: 'Do you wish to proceed?',
		defaultButtonText: 'Save'
	},
	MessageBox: {
		title: 'System alert',
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel'
	}
}

/*
 * Grid的相关方法
 * >>MixinVue
 * >>Toolbar
 * >>List
 * >>Search
 * >>OpenDialog
 * >>CloseDialog
 * >>SaveDialog
 * >>EditDialog
 * >>SingleOper
 * >>ComplexOper
 * >>CommonOper
 * >>SubForm
*/
elementExtend.Grid = {
	/*
	 * 初始化Vue对象参数
	 */
	MixinVue: function () {
		return {
			data: {
				permission: {
					permissionTools: [],
					hideTools: []
				},
				navMenu: {
					html: ''
				},
				searchBar: {
					searchBtn: {
						loading: false
					}
				},
				toolbar: {
					operate: {
					}
				},
				grid: {
					url: '',
					data: [],
					height: $(window).height() - 256,
					loading: false
				},
				sort: {
					prop: '',
					//ascending|descending
					order: ''
				},
				pagination: {
					pageList: [10, 15, 20, 25, 30, 50],
					pageSize: 10,
					totalCount: 1,
					currentPage: 1,
					hideOnSinglePage: false
				},
				dialog: {
					dialogVisible: false,
					title: '',
					top: "3px",
					width: '99%',
					url: '',
					saveUrl: '',
					dialogLoading: false,
					buttonLoading: false,
					//保存时是否确认框提示
					isConfirm: false
				}
			},
			methods: {
				getGridList: function () {
					elementExtend.Grid.List({
						vue: this,
						url: this.grid.url
					});
				},
				indexFormat: function (index) {
					index = index + 1;
					if (this.pagination.currentPage > 1) {
						return (this.pagination.currentPage - 1) * this.pagination.pageSize + index;
					}
					else {
						return index;
					}
				},
				clickRow: function (row) {
					this.$refs.mainGrid.clearSelection();
					this.$refs.mainGrid.toggleRowSelection(row);
				},
				pageSizeChange: function (val) {
					this.pagination.pageSize = val;
					this.pagination.currentPage = 1;
					this.getGridList();

				},
				pageCurrentChange: function (val) {
					this.pagination.currentPage = val;
					this.getGridList();
				},
				sortChange: function (val) {
					this.sort.prop = val.prop;
					this.sort.order = val.order;
					this.getGridList();
				},
				//Dialog打开的回调
				dialogOpenCallback: function () {
					const loading = this.$loading({
						lock: true
					});
					setTimeout(() => {
						loading.close();
					}, 500);
				},
				//Dialog保存
				dialogSave: function () {
					//读取数据
					var $iframe = $("iframe")[0].contentWindow;
					var $dialogVue = $iframe.appVue;
					var _subFormParams = $dialogVue.subForm;
					//CSRF验证
					var _headers = {};
					if (verificationTokenJs.enabled) {
						_headers.__RequestVerificationToken = $iframe.$('input[name=' + verificationTokenJs.key + ']').val();
					}
					//创建参数对象
					var _params = {};
					for (key in _subFormParams) {
						if (typeof (_subFormParams[key]) === 'object') {
							_params[key] = JSON.stringify(_subFormParams[key]);
						}
						else {
							_params[key] = _subFormParams[key];
						}
					}
					//是否有提示框,默认无
					if (!this.dialog.isConfirm) {
						//post请求
						elementExtend.Grid.SubmitForm({
							vue: this,
							dialogVue: $dialogVue,
							url: this.dialog.saveUrl,
							headers: _headers,
							params: _params,
							iswin: true
						});
					}
					else {
						let $vue = this;
						elementExtend.Confirm(this, $.fn.elementExtend.Grid.confirmText, function () {
							//post请求
							elementExtend.Grid.SubmitForm({
								vue: $vue,
								dialogVue: $dialogVue,
								url: $vue.dialog.saveUrl,
								headers: _headers,
								params: _params,
								iswin: true
							});
						}, function () { return true });
					}
				},
				//Dialog关闭的回调
				dialogClosedCallback: function () {
					elementExtend.Grid.Search({
						vue: this
					});
				}
			}
		}
	},
	/* 
	 * vue
	 */
	Toolbar: function (settings) {
		settings = settings || {};
		var $vue = (settings.vue === undefined) ? null : settings.vue;
		var _permissionTools = ($vue.permission.permissionTools === undefined) ? [] : $vue.permission.permissionTools;
		var _hideTools = ($vue.permission.hideTools === undefined) ? [] : $vue.permission.hideTools;
		if ($vue) {
			if (_permissionTools != undefined) {
				for (key in $vue.toolbar.operate) {
					if ($.inArray(key, _permissionTools) > -1) {
						if (_hideTools == null) {
							$vue.toolbar.operate[key] = true;
						}
						else {
							if ($.inArray(key, _hideTools) > -1) {
								$vue.toolbar.operate[key] = false;
							}
							else {
								$vue.toolbar.operate[key] = true;
							}
						}
					}
					else {
						$vue.toolbar.operate[key] = false;
					}
				}
			}
		}
		else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	},
	/* 
	 * vue,
	 * url,
	 * addtionParams,
	 * success
	 */
	List: function (settings) {
		settings = settings || {};
		var $vue = (settings.vue === undefined) ? null : settings.vue;
		var _url = (settings.url === undefined) ? "" : settings.url;
		var _addtionParams = settings.addtionParams || {};
		var _success = (typeof (settings.success) === 'function') ? settings.success : null;
		if ($vue) {
			//查询条件
			var _params = $vue.searchParams;
			//翻页参数
			_params.rows = $vue.pagination.pageSize;
			_params.page = $vue.pagination.currentPage;
			//排序参数
			if ($vue.sort.prop != '') {
				_params.prop = $vue.sort.prop;
				_params.order = $vue.sort.order;
			}
			//额外参数
			for (key in _addtionParams) {
				_params[key] = _addtionParams[key];
			}
			//post请求
			vueExtend.Ajax.Post({
				url: _url,
				dataType: 'json',
				params: _params,
				loading: true,
				beforeSend: function () {
					if ($vue.grid.loading !== undefined)
						$vue.grid.loading = true;
				},
				afterSend: function () {
					if ($vue.searchBar.searchBtn.loading !== undefined)
						$vue.searchBar.searchBtn.loading = false;
					if ($vue.grid.loading !== undefined)
						$vue.grid.loading = false;
				},
				success: function (data) {
					$vue.grid.data = data.rows;
					$vue.pagination.totalCount = data.total;
					//额外操作
					if (_success) _success(data);
				}
			});
		}
		else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	},
	/*
	 * vue
	 */
	Search: function (settings) {
		settings = settings || {};
		var $vue = (settings.vue === undefined) ? null : settings.vue;
		if ($vue) {
			$vue.searchBar.searchBtn.loading = true
			//如果有需要隐藏的工具栏,则重新加载权限
			if ($vue.permission.hideTools != []) {
				elementExtend.Grid.Toolbar({
					vue: $vue
				});
			}
			//重置成第一页
			$vue.pagination.currentPage = 1;
			//重新获取数据
			$vue.getGridList();
		}
		else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	},
	/*
	 * vue
	 * url
	 * title
	 * width
	 */
	OpenDialog: function (settings) {
		settings = settings || {};
		var $vue = (settings.vue === undefined) ? null : settings.vue;
		if ($vue) {
			$vue.dialog.url = settings.url;
			if (settings.title != undefined)
				$vue.dialog.title = settings.title;
			if (settings.width != undefined)
				$vue.dialog.width = settings.width;
			$vue.dialog.dialogVisible = true;
		}
		else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	},
	/*
	 * vue
	 */
	CloseDialog: function (vue) {
		var $vue = (vue === undefined) ? null : vue;
		if ($vue) {
			$vue.dialog.dialogVisible = false;
		}
		else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	},
	/*
	 * vue
	 * title
	 * url
	 * width
	 * height
	 * saveUrl
	 * isConfirm
	 */
	SaveDialog: function (settings) {
		settings = settings || {};
		var $vue = (settings.vue === undefined) ? null : settings.vue;
		if (settings.isConfirm === undefined) {
			$vue.dialog.isConfirm = false;
		}
		else {
			$vue.dialog.isConfirm = settings.isConfirm;
		}

		if ($vue) {
			elementExtend.Grid.OpenDialog({
				vue: $vue,
				url: settings.url,
				title: settings.title,
				width: settings.width,
				height: settings.height,
			});
			$vue.dialog.saveUrl = (settings.saveUrl === undefined) ? '' : settings.saveUrl;
		}
		else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	},
	/*
	 * vue
	 * url
	 * title
	 * width
	 * height
	 * saveUrl
	 * isConfirm
	 */
	EditDialog: function (settings) {
		settings = settings || {};
		var $vue = (settings.vue === undefined) ? null : settings.vue;
		if ($vue) {
			var _rows = $vue.$refs.mainGrid.selection;
			if (_rows) {
				if (_rows.length == 0) {
					elementExtend.Tips.Alert($vue, $.fn.elementExtend.Grid.atLeastOneText, 5);
				}
				else {
					if (_rows.length == 1) {
						settings.url = settings.url.replace('$', _rows[0].ck);
						//保存信息
						elementExtend.Grid.SaveDialog({
							vue: $vue,
							url: settings.url,
							title: settings.title,
							width: settings.width,
							height: settings.height,
							saveUrl: settings.saveUrl,
							isConfirm: settings.isConfirm
						});
					}
					else {
						elementExtend.Tips.Alert($vue, $.fn.elementExtend.Grid.onlyOneText, 5);
					}
				}
			} else {
				elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
			}
		}
		else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	},
	/*
	 * vue
	 */
	SingleOper: function (settings) {
		settings = settings || {};
		var $vue = (settings.vue === undefined) ? null : settings.vue;
		if ($vue) {
			var _rows = $vue.$refs.mainGrid.selection;
			if (_rows.length == 0) {
				elementExtend.Tips.Alert($vue, $.fn.elementExtend.Grid.atLeastOneText, 5);
			} else {
				if (_rows.length == 1) {
					settings.url = settings.url.replace('$', _rows[0].ck);
					elementExtend.Grid.OpenDialog({
						vue: settings.vue,
						url: settings.url,
						title: settings.title,
						width: settings.width,
						height: settings.height
					});
				}
				else {
					elementExtend.Tips.Alert($vue, $.fn.elementExtend.Grid.onlyOneText, 5);
				}
			}
		}
		else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	},
	/*
	 * vue
	 * url
	 */
	ComplexOper: function (vue, url) {
		var $vue = vue;
		if ($vue) {
			var _rows = $vue.$refs.mainGrid.selection;
			if (_rows) {
				if (_rows.length == 0) {
					elementExtend.Tips.Alert($vue, $.fn.elementExtend.Grid.atLeastOneText, 5);
				} else {
					elementExtend.Confirm($vue, $.fn.elementExtend.Grid.confirmText, function () {
						//选中的id集合
						var _ids = new Array();
						for (var i = 0; i < _rows.length; i++) {
							_ids.push(_rows[i].ck);
						}
						//post请求
						vueExtend.Ajax.Post({
							url: url,
							dataType: 'json',
							params: {
								ids: _ids
							},
							loading: true,
							beforeSend: function () {
								if ($vue.grid.loading !== undefined)
									$vue.grid.loading = true;
							},
							afterSend: function () {
								if ($vue.grid.loading !== undefined)
									$vue.grid.loading = false;
							},
							success: function (data) {
								if (data.result) {
									elementExtend.Tips.Success($vue, data.msg, 2, function () {
										$vue.getGridList();
									});
								} else {
									elementExtend.Tips.Alert($vue, data.msg, 5, function () {
										$vue.getGridList();
									});
								}
							}
						});
					}, function () {
						return true;
					});
				}
			}
			else {
				elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
			}
		} else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	},
	/*
	 * vue
	 * url
	 * params
	 */
	CommonOper: function (vue, url, params) {
		params = params || {};
		var $vue = (vue === undefined) ? null : vue;
		if ($vue) {
			//post请求
			vueExtend.Ajax.Post({
				url: url,
				dataType: 'json',
				params: params,
				loading: true,
				beforeSend: function () {
					if ($vue.grid.loading !== undefined)
						$vue.grid.loading = true;
				},
				afterSend: function () {
					if ($vue.grid.loading !== undefined)
						$vue.grid.loading = false;
				},
				success: function (data) {
					if (data.result) {
						elementExtend.Tips.Success($vue, data.msg, 2, function () {
							$vue.getGridList();
						});
					} else {
						elementExtend.Tips.Alert($vue, data.msg, 5, function () {
							$vue.getGridList();
						});
					}
				}
			});
		}
		else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	},
	/*
	 * vue
	 * dialogVue
	 * url
	 * headers
	 * params
	 * iswin
	 */
	SubmitForm: function (settings) {
		settings = settings || {};
		var $vue = (settings.vue === undefined) ? "" : settings.vue;
		var $dialogVue = (settings.dialogVue === undefined) ? "" : settings.dialogVue;
		var _url = (settings.url === undefined) ? "" : settings.url;
		var _headers = (settings.headers === undefined) ? {} : settings.headers;
		var _params = (settings.params === undefined) ? {} : settings.params;
		//post请求
		vueExtend.Ajax.Post({
			url: _url,
			dataType: 'json',
			headers: _headers,
			params: _params,
			loading: true,
			beforeSend: function () {
				$dialogVue.fullScreenLoading = true;
				$vue.dialog.buttonLoading = true;
			},
			afterSend: function () {
				$dialogVue.fullScreenLoading = false;
				$vue.dialog.buttonLoading = false;
			},
			success: function (data) {
				if (data.result) {
					if (settings.iswin) {
						elementExtend.Tips.Success($vue, data.msg, 2, function () {
							elementExtend.Grid.CloseDialog($vue);
						});
					}
					else {
						elementExtend.Tips.Success($vue, data.msg, 2);
					}
				} else {
					elementExtend.Tips.Alert($vue, data.msg, 5);
				}
			}
		});
	}
};

/*
 * Base的相关方法
 * >>MixinVue
 * >>ParseData
 * >>OpenDialog
 * >>CloseDialog
*/
elementExtend.Base = {
	/*
	 * 初始化Vue对象参数
	 */
	MixinVue: function () {
		return {
			data: {
				searchButton: {
					loading: false
				},
				fullScreenLoading: false,
				dialog: {
					dialogVisible: false,
					title: '',
					top: "3px",
					width: '99%',
					url: '',
					textarea: {
						text: '',
						readonly: false
					},
					dialogLoading: false
				},
			},
			methods: {
				//Dialog打开的回调
				dialogOpenCallback: function () {
					const loading = this.$loading({
						lock: true
					});
					setTimeout(() => {
						loading.close();
					}, 500);
				},
				//Dialog关闭的回调
				dialogClosedCallback: function () {

				}
			}
		}
	},
	/*
	 * url
	 * params
	 */
	ParseData: function (settings) {
		settings = settings || {};
		var _url = (settings.url === undefined) ? "" : settings.url;
		var _params = (settings.params === undefined) ? {} : settings.params;
		//post请求
		vueExtend.Ajax.Get({
			url: _url,
			dataType: 'json',
			params: _params,
			success: function (data) {
				if (typeof (settings.success) === 'function') settings.success(data);
			}
		});
	},
	/*
	 * vue
	 * url
	 * params
	 * success
	 */
	PostForm: function (settings) {
		settings = settings || {};
		var $vue = (settings.vue === undefined) ? "" : settings.vue;
		var _url = (settings.url === undefined) ? "" : settings.url;
		var _params = (settings.params === undefined) ? {} : settings.params;
		var _success = (typeof (settings.success) === 'function') ? settings.success : null;
		//创建参数对象
		var _subParams = {};
		for (key in _params) {
			if (typeof (_params[key]) === 'object') {
				_subParams[key] = JSON.stringify(_params[key]);
			}
			else {
				_subParams[key] = _params[key];
			}
		}
		//CSRF验证
		var _headers = {};
		if (verificationTokenJs.enabled) {
			_headers.__RequestVerificationToken = $('input[name=' + verificationTokenJs.key + ']').val();
		}
		//post请求
		vueExtend.Ajax.Post({
			url: _url,
			dataType: 'json',
			headers: _headers,
			params: _subParams,
			loading: true,
			beforeSend: function () {
				$vue.searchButton.loading = true;
			},
			afterSend: function () {
				$vue.searchButton.loading = false;
			},
			success: function (data) {
				if (_success != null) _success(data);
			}
		});
	},
	/*
	 * vue
	 * url
	 * title
	 * width
	 */
	OpenDialog: function (settings) {
		settings = settings || {};
		var $vue = (settings.vue === undefined) ? null : settings.vue;
		if ($vue) {
			$vue.dialog.url = settings.url;
			if (settings.title != undefined)
				$vue.dialog.title = settings.title;
			if (settings.width != undefined)
				$vue.dialog.width = settings.width;
			$vue.dialog.dialogVisible = true;
		}
		else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	},
	OpenTextAreaDialog: function (settings) {
		settings = settings || {};
		var $vue = (settings.vue === undefined) ? null : settings.vue;
		var _text = (settings.text === undefined) ? '' : settings.text;
		var _readonly = (settings.readonly === undefined) ? false : settings.readonly;
		var _callBackFunc = (typeof (settings.callBackFunc) === 'function') ? settings.callBackFunc : null;
		if ($vue) {
			$vue.dialog.url = settings.url;
			if (settings.title != undefined)
				$vue.dialog.title = settings.title;
			$vue.dialog.width = '80%';
			$vue.dialog.top = '50px';
			$vue.dialog.textarea.text = _text;
			$vue.dialog.textarea.readonly = _readonly;
			$vue.dialog.dialogVisible = true;
			$vue.dialogClosedCallback = function () {
				_callBackFunc($vue.dialog.textarea.text);
			}
		}
		else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	},
	/*
	 * vue
	 */
	CloseDialog: function (vue) {
		var $vue = (vue === undefined) ? null : vue;
		if ($vue) {
			$vue.dialog.dialogVisible = false;
		}
		else {
			elementExtend.Tips.Error($vue, $.fn.elementExtend.Grid.missObject, 2);
		}
	}
}

/*
 * Tips
 * >>Success
 * >>Alert
 */
elementExtend.Tips = {
	Success: function (vue, message, duration, callback) {
		var $vue = vue;
		$vue.$message({
			message: message,
			type: 'success',
			duration: (duration === undefined) ? 2000 : duration * 1000,
			offset: 100,
			onClose: (typeof (callback) === 'function') ? callback : null
		});
	},
	Alert: function (vue, message, duration, callback) {
		var $vue = vue;
		$vue.$message({
			message: message,
			type: 'warning',
			duration: (duration === undefined) ? 2000 : duration * 1000,
			offset: 100,
			onClose: (typeof (callback) === 'function') ? callback : null
		});
	},
	Info: function (vue, message, duration, callback) {
		var $vue = vue;
		$vue.$message({
			message: message,
			type: 'info',
			duration: (duration === undefined) ? 2000 : duration * 1000,
			offset: 100,
			onClose: (typeof (callback) === 'function') ? callback : null
		});
	},
	Error: function (vue, message, time, callback) {
		var $vue = vue;
		$vue.$message({
			message: message,
			type: 'error',
			duration: (duration === undefined) ? 2000 : duration * 1000,
			offset: 100,
			onClose: (typeof (callback) === 'function') ? callback : null
		});
	}
}

/*
 * Confirm
 */
elementExtend.Confirm = function (vue, message, comfirmCallback, cancelCallback) {
	var $vue = vue;
	$vue.$confirm(message, $.fn.elementExtend.MessageBox.title, {
		showClose: false,
		confirmButtonText: $.fn.elementExtend.MessageBox.confirmButtonText,
		cancelButtonText: $.fn.elementExtend.MessageBox.cancelButtonText,
		type: 'warning'
	}).then(() => {
		if (typeof (comfirmCallback) === 'function') {
			comfirmCallback();
		}
	}).catch(() => {
		if (typeof (cancelCallback) === 'function') {
			cancelCallback();
		}
	});
}

/*
 * MessageBox
 */
elementExtend.MessageBox = function (vue, message, comfirmCallback, cancelCallback) {
	var $vue = vue;
	$vue.$msgbox({
		title: $.fn.elementExtend.MessageBox.title,
		message: message,
		showCancelButton: true,
		showClose: false,
		dangerouslyUseHTMLString: true,
		confirmButtonText: $.fn.elementExtend.MessageBox.confirmButtonText,
		cancelButtonText: $.fn.elementExtend.MessageBox.cancelButtonText
	}).then(() => {
		if (typeof (comfirmCallback) === 'function') {
			comfirmCallback();
		}
	}).catch(() => {
		if (typeof (cancelCallback) === 'function') {
			cancelCallback();
		}
	});
}

/*
 * InputMessageBox
 */
elementExtend.InputMessageBox = function (vue, message, comfirmCallback, cancelCallback) {
	var $vue = vue;
	$vue.$prompt(message, '', {
		confirmButtonText: $.fn.elementExtend.MessageBox.confirmButtonText,
		cancelButtonText: $.fn.elementExtend.MessageBox.cancelButtonText
	}).then(({ value }) => {
		if (typeof (comfirmCallback) === 'function') {
			comfirmCallback(value);
		}
	}).catch(() => {
		if (typeof (cancelCallback) === 'function') {
			cancelCallback();
		}
	});
}

/*
 *Preview Picture
 */
elementExtend.previewImage = function (vue, imageSrc, title) {
	var $vue = vue;
	var _title = (title === undefined) ? '' : title;
	$vue.$alert('<img src="' + imageSrc + '" style="width:100%;" alt="' + _title + '" />', _title, {
		dangerouslyUseHTMLString: true,
		showConfirmButton: false,
		closeOnClickModal: true,
		closeOnPressEscape: true
	}).catch(action => {

	});
}