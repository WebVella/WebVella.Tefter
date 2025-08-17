window.Tefter = {
	HtmlEditors: {},
	HtmlEditorsChangeListeners: {},
	HtmlEditorsEnterListeners: {},
	ColumnResizeListeners: {},
	ColumnSortListeners: {},
	dispose: function () {
		Tefter.HtmlEditors = {};
		Tefter.HtmlEditorsChangeListeners = {};
		Tefter.HtmlEditorsEnterListeners = {};
		ColumnResizeListeners = {};
		ColumnSortListeners = {};
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
	createQuill: function (quillElement, editorId, dotNetHelper, updateTextMethodName, enterHandlerMethodName, placeholder, readonly) {
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
			readOnly: readonly
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
	},
	getIsDarkTheme: function () {
		if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
			return true;
		}
		return false;
	},
	makeTableResizable: function (tableId) {
		const table = document.getElementById(tableId);
		if (table == null) return;
		const colgroup = table.querySelector('colgroup');
		if (colgroup == null) return;
		const headers = table.querySelectorAll('th');
		if (headers == null) return;
		if (!colgroup) {
			console.warn('No colgroup found in table');
			return;
		}

		// Get all col elements
		const cols = colgroup.querySelectorAll('col');

		// Add resize handles to each header (except first)
		headers.forEach((header, index) => {
			if (index === 0) return; // First column is unresizable

			let handle = header.querySelector('.resize-handle');
			if (handle == null) return;
			// Add mouse event listeners
			handle.addEventListener('mousedown', (e) => {
				e.preventDefault();
				startResize(e, index, cols, headers);
			});
		});

		function startResize(e, columnIndex, cols, headers) {
			const startX = e.clientX;
			const colElement = cols[columnIndex];
			const thElement = headers[columnIndex];
			let newWidth = 0;
			if (!colElement) return;

			//remove minWith
			colElement.style.minWidth = "unset";

			//add resizing class to th
			thElement.classList.add("tf-resizing");

			// Get current width and min-width
			const startWidth = parseFloat(getComputedStyle(colElement).width);
			const startMinWidth = parseFloat(getComputedStyle(colElement).minWidth) || 30;

			document.body.style.cursor = 'col-resize';
			document.body.style.userSelect = 'none';

			// Create a temporary element to track resize
			const resizeTracker = document.createElement('div');
			resizeTracker.style.position = 'fixed';
			resizeTracker.style.top = '0';
			resizeTracker.style.left = '0';
			resizeTracker.style.width = '100%';
			resizeTracker.style.height = '100%';
			resizeTracker.style.zIndex = '9999';
			document.body.appendChild(resizeTracker);

			function doResize(e) {
				const diff = e.clientX - startX;
				newWidth = Math.max(startMinWidth, startWidth + diff); // Respect min-width
				newWidth = parseInt(newWidth);
				// Update both width and min-width styles
				colElement.style.width = newWidth + 'px';
				colElement.style.minWidth = newWidth + 'px';

				// Also update the header width for visual consistency
				//headers[columnIndex].style.width = newWidth + 'px';

			}

			function stopResize() {
				document.body.style.cursor = '';
				document.body.style.userSelect = '';
				document.removeEventListener('mousemove', doResize);
				document.removeEventListener('mouseup', stopResize);
				document.body.removeChild(resizeTracker);
				thElement.classList.remove("tf-resizing");

				const event = new CustomEvent('tf-column-resize', {
					detail: {
						position: columnIndex,
						width: newWidth
					}
				});
				document.dispatchEvent(event);
			}

			document.addEventListener('mousemove', doResize);
			document.addEventListener('mouseup', stopResize);
		}
	},
	addColumnResizeListener: function (dotNetHelper, listenerId, methodName) {
		Tefter.ColumnResizeListeners[listenerId] = { dotNetHelper: dotNetHelper, methodName: methodName };
		return true;
	},
	removeColumnResizeListener: function (listenerId) {
		if (Tefter.ColumnResizeListeners[listenerId]) {
			delete Tefter.ColumnResizeListeners[listenerId];
		}
		return true;
	},
	executeColumnResizeListenerCallbacks: function (evtObj) {
		if (Tefter.ColumnResizeListeners) {
			for (const listenerId in Tefter.ColumnResizeListeners) {
				const dotNetHelper = Tefter.ColumnResizeListeners[listenerId].dotNetHelper;
				const methodName = Tefter.ColumnResizeListeners[listenerId].methodName;
				if (dotNetHelper && methodName) {
					dotNetHelper.invokeMethodAsync(methodName, evtObj.detail.position, evtObj.detail.width);
				}

				return true;
			}
		}
		return false;
	},
	addColumnSortListener: function (dotNetHelper, listenerId, methodName) {
		Tefter.ColumnSortListeners[listenerId] = { dotNetHelper: dotNetHelper, methodName: methodName };
		return true;
	},
	removeColumnSortListener: function (listenerId) {
		if (Tefter.ColumnSortListeners[listenerId]) {
			delete Tefter.ColumnSortListeners[listenerId];
		}
		return true;
	},
	executeColumnSortListenerCallbacks: function (column, shiftKey) {
		if (Tefter.ColumnSortListeners) {
			for (const listenerId in Tefter.ColumnSortListeners) {
				const dotNetHelper = Tefter.ColumnSortListeners[listenerId].dotNetHelper;
				const methodName = Tefter.ColumnSortListeners[listenerId].methodName;
				if (dotNetHelper && methodName) {
					dotNetHelper.invokeMethodAsync(methodName, column, shiftKey);
				}

				return true;
			}
		}
		return false;
	},
}

//Listeners
document.addEventListener("tf-column-resize", function (evtObj) {
	Tefter.executeColumnResizeListenerCallbacks(evtObj)
});
document.addEventListener("click", function (evtObj) {
	let target = evtObj.target.closest(".tf-column-sort");
	if (target) {
		evtObj.preventDefault();
		evtObj.stopPropagation();
		var shiftKey = evtObj.shiftKey;
		document.getSelection().removeAllRanges();
		Tefter.executeColumnSortListenerCallbacks(parseInt(target.dataset.column), shiftKey);
	}
});