window.dragDrop = {
    setupDragAndDrop: function (elementId, callback) {
        const element = document.getElementById(elementId);

        if (!element) return;

        // Prevent default drag behaviors
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            element.addEventListener(eventName, preventDefaults, false);
        });

        // Highlight drop area when item is dragged over it
        ['dragenter', 'dragover'].forEach(eventName => {
            element.addEventListener(eventName, highlight, false);
        });

        ['dragleave', 'drop'].forEach(eventName => {
            element.addEventListener(eventName, unhighlight, false);
        });

        // Handle dropped files
        element.addEventListener('drop', handleDrop, false);

        function preventDefaults(e) {
            e.preventDefault();
            e.stopPropagation();
        }

        function highlight() {
            element.classList.add('drag-over');
        }

        function unhighlight() {
            element.classList.remove('drag-over');
        }

        function handleDrop(e) {
            const dt = e.dataTransfer;
            const files = dt.files;

            if (files.length > 0 && callback) {
                // Convert FileList to array
                const fileArray = Array.from(files);
                callback(fileArray);
            }
        }
    },

    clickElement: function(elementRef) {
        elementRef.click();
    }
};
