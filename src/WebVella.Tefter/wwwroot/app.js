window.Tefter = {
	copyToClipboard: function(text) {
        navigator.clipboard.writeText(text).then(function () {})
        .catch(function (error) {
            log.error(error);
        });
    },
	clickElement: function(elementId) {
        var element = document.getElementById(elementId);
        if(!element) return;
        element.click();
    }
}