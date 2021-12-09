/**element-extend**/
if ($.fn.elementExtend) {
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
}

/**vue-Extend**/
if ($.fn.vueExtend) {
	$.fn.vueExtend = {
		Ajax: {
			errorText: 'The requested URL could not be retrieved, please try again!'
        },
        openFileView: {
            title: 'Upload File'
        }
	}
}

/**echart**/
if ($.fn.echartExtend) {
    $.fn.echartExtend = {
        AjaxSet: {
            loadingText: 'Loading...'
        }
    }
}