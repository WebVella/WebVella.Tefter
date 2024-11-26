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
		if (window.isSecureContext) {
			navigator.clipboard.writeText(text).then(function () { })
				.catch(function (error) {
					log.error(error);
				});
		}
		else {
			const textArea = document.createElement("textarea");
			textArea.value = text;
			document.body.appendChild(textArea);
			textArea.focus();
			textArea.select();
			try {
				document.execCommand('copy');
			} catch (err) {
				console.error('Unable to copy to clipboard', err);
			}
			document.body.removeChild(textArea);
		}
	},
	clickElement: function (elementRef) {
		if (!elementRef) return;
		elementRef.click();
	},
	clickElementById: function (elementId) {
		var element = document.getElementById(elementId);
		if (!element) return;
		element.click();
	},
	focusElementById: function (elementId) {
		var element = document.getElementById(elementId);
		if (!element) return;
		element.focus();
	},
	blurElementById: function (elementId) {
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
		if (enterHandlerMethodName) {
			Tefter.addEditorEnterKeyListener(dotNetHelper, editorId, enterHandlerMethodName);
			Tefter.HtmlEditors[editorId].keyboard.bindings['Enter'].unshift({
				key: 'Enter',
				shiftKey: false,
				handler: (range, context) => {
					return Tefter.executeEditorEnterKeyListenerCallbacks(editorId);
				}
			});
		}
		Tefter.HtmlEditors[editorId].on('text-change', (delta, oldDelta, source) => {
			if (source == 'api') {
				/*console.log('An API call triggered this change.');*/
			} else if (source == 'user') {
				/*console.log('A user action triggered this change.');*/
				Tefter.executeEditorTextChangeListenerCallbacks(editorId, delta, oldDelta, source)
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
	executeEditorTextChangeListenerCallbacks: function (editorId, delta, oldDelta, source) {
		if (Tefter.HtmlEditorsChangeListeners && Tefter.HtmlEditorsChangeListeners[editorId]) {
			const dotNetHelper = Tefter.HtmlEditorsChangeListeners[editorId].dotNetHelper;
			const methodName = Tefter.HtmlEditorsChangeListeners[editorId].methodName;
			if (dotNetHelper && methodName) {
				dotNetHelper.invokeMethodAsync(methodName);
			}
			return true;
		}
		return false;
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
	executeEditorEnterKeyListenerCallbacks: function (editorId) {
		if (Tefter.HtmlEditorsEnterListeners && Tefter.HtmlEditorsEnterListeners[editorId]) {
			const dotNetHelper = Tefter.HtmlEditorsEnterListeners[editorId].dotNetHelper;
			const methodName = Tefter.HtmlEditorsEnterListeners[editorId].methodName;
			if (dotNetHelper && methodName) {
				dotNetHelper.invokeMethodAsync(methodName);
			}

			return true;
		}
		return false;
	},
	getQuillHtml: function (editorId) {
		if (Tefter.HtmlEditors[editorId]) {
			return Tefter.HtmlEditors[editorId].getSemanticHTML();
		}
		return null;
	},
	setQuillHtml: function (editorId, html) {
		if (Tefter.HtmlEditors[editorId]) {
			Tefter.HtmlEditors[editorId].setContents(null, 'silent');
			if (html && html !== '') {
				Tefter.HtmlEditors[editorId].clipboard.dangerouslyPasteHTML(html, 'silent');
			}
		}
		return true;
	}

}