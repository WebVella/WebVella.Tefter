window.Tefter = {
	HtmlEditors: {},
	HtmlEditorsChangeListeners: {},
	dispose: function () {
		Tefter.HtmlEditors = {};
		Tefter.HtmlEditorsChangeListeners = {};
	},
	copyToClipboard: function (text) {
		navigator.clipboard.writeText(text).then(function () { })
			.catch(function (error) {
				log.error(error);
			});
	},
	clickElement: function (elementId) {
		var element = document.getElementById(elementId);
		if (!element) return;
		element.click();
	},
	focusElement: function (elementId) {
		var element = document.getElementById(elementId);
		if (!element) return;
		element.focus();
	},
	blurElement: function (elementId) {
		var element = document.getElementById(elementId);
		if (!element) return;
		element.blur();
	},
	createQuill: function (quillElement, editorId) {
		var options = {
			modules: {
				toolbar: {
					container: '#toolbar-' + editorId,
					handlers: {
						'undo': () => Tefter.HtmlEditors[editorId].history.undo(),
						'redo': () => Tefter.HtmlEditors[editorId].history.redo(),
					},
				},
				history: {
					delay: 2000,
					maxStack: 500,
					userOnly: true
				},

			},
			placeholder: '',
			readOnly: false
		};
		// set quill at the object we can call
		// methods on later
		Tefter.HtmlEditors[editorId] = new Quill(quillElement, options);
		Tefter.HtmlEditors[editorId].on('text-change', (delta, oldDelta, source) => {
			if (source == 'api') {
				console.log('An API call triggered this change.');
				Tefter.executeEditorTextChangeListenerCallbacks(delta, oldDelta, source)
			} else if (source == 'user') {
				console.log('A user action triggered this change.');
			}
		});
	},
	addEditorTextChangeListener: function (dotNetHelper, editorId, methodName) {
		Tefter.HtmlEditorsChangeListeners[listenerId] = { dotNetHelper: dotNetHelper, methodName: methodName };
		return true;
	},
	removeEditorTextChangeListener: function (dotNetHelper, editorId, methodName) {
		if (Tefter.HtmlEditorsChangeListeners[listenerId]) {
			delete Tefter.HtmlEditorsChangeListeners[listenerId];
		}
		return true;
	},
	executeEditorTextChangeListenerCallbacks: function (delta, oldDelta, source) {
		if (Tefter.HtmlEditorsChangeListeners) {
			for (const prop in Tefter.HtmlEditorsChangeListeners) {
				const dotNetHelper = Tefter.HtmlEditorsChangeListeners[prop].dotNetHelper;
				const methodName = Tefter.HtmlEditorsChangeListeners[prop].methodName;
				if (dotNetHelper && methodName) {
					dotNetHelper.invokeMethodAsync(methodName);
				}
			}
		}
		return true;
	},
}