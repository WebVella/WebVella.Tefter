window.Tefter = {
	HtmlEditors: {},
	HtmlEditorsChangeListeners: {},
	HtmlEditorsEnterListeners: {},
	dispose: function () {
		Tefter.HtmlEditors = {};
		Tefter.HtmlEditorsChangeListeners = {};
		Tefter.HtmlEditorsEnterListeners = {};
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
	createQuill: function (quillElement, editorId, dotNetHelper, updateTextMethodName, enterHandlerMethodName, placeholder) {
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
				}
			},
			placeholder: placeholder,
			readOnly: false
		};
		// set quill at the object we can call
		// methods on later
		Tefter.HtmlEditors[editorId] = new Quill(quillElement, options);
		if (updateTextMethodName)
			Tefter.addEditorTextChangeListener(dotNetHelper, editorId, updateTextMethodName);
		if (enterHandlerMethodName)
			Tefter.addEditorEnterKeyListener(dotNetHelper, editorId, enterHandlerMethodName);
		Tefter.HtmlEditors[editorId].keyboard.bindings['Enter'].unshift({
			key: 'Enter',
			shiftKey: false,
			handler: (range, context) => {
				return Tefter.executeEditorEnterKeyListenerCallbacks();
			}
		});
		Tefter.HtmlEditors[editorId].on('text-change', (delta, oldDelta, source) => {
			if (source == 'api') {
				/*console.log('An API call triggered this change.');*/
			} else if (source == 'user') {
				/*console.log('A user action triggered this change.');*/
				Tefter.executeEditorTextChangeListenerCallbacks(delta, oldDelta, source)
			}
		});
	},
	removeQuill: function (editorId) {
		if (Tefter.HtmlEditors[editorId]) {
			delete Tefter.HtmlEditors[editorId];
		}
		if (Tefter.HtmlEditorsChangeListeners[editorId]) {
			delete Tefter.HtmlEditorsChangeListeners[editorId];
		}
		if (Tefter.HtmlEditorsEnterListeners[editorId]) {
			delete Tefter.HtmlEditorsEnterListeners[editorId];
		}
	},
	focusQuill: function (editorId) {
		if (Tefter.HtmlEditors[editorId]) {
			Tefter.HtmlEditors[editorId].focus();
		}
	},
	addEditorTextChangeListener: function (dotNetHelper, editorId, methodName) {
		Tefter.HtmlEditorsChangeListeners[editorId] = { dotNetHelper: dotNetHelper, methodName: methodName };
		return true;
	},
	removeEditorTextChangeListener: function (editorId) {
		if (Tefter.HtmlEditorsChangeListeners[editorId]) {
			delete Tefter.HtmlEditorsChangeListeners[editorId];
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

	addEditorEnterKeyListener: function (dotNetHelper, editorId, methodName) {
		Tefter.HtmlEditorsEnterListeners[editorId] = { dotNetHelper: dotNetHelper, methodName: methodName };
		return true;
	},
	removeEditorEnterKeyListener: function (editorId) {
		if (Tefter.HtmlEditorsEnterListeners[editorId]) {
			delete Tefter.HtmlEditorsEnterListeners[editorId];
		}
		return true;
	},
	executeEditorEnterKeyListenerCallbacks: function () {
		if (Tefter.HtmlEditorsEnterListeners) {
			for (const prop in Tefter.HtmlEditorsEnterListeners) {
				const dotNetHelper = Tefter.HtmlEditorsEnterListeners[prop].dotNetHelper;
				const methodName = Tefter.HtmlEditorsEnterListeners[prop].methodName;
				if (dotNetHelper && methodName) {
					dotNetHelper.invokeMethodAsync(methodName);
				}
			}
			if (Tefter.HtmlEditorsEnterListeners || Object.keys(Tefter.HtmlEditorsEnterListeners).length > 0) {
				return false;
			}
		}
		return true;
	},
	getQuillHtml: function (editorId) {
		if (Tefter.HtmlEditors[editorId]) {
			return Tefter.HtmlEditors[editorId].getSemanticHTML();
		}
		return null;
	},
	setQuillHtml: function (editorId, html) {
		if (Tefter.HtmlEditors[editorId]) {
			Tefter.HtmlEditors[editorId].clipboard.dangerouslyPasteHTML(html, 'silent');
		}
		return true;
	}

}