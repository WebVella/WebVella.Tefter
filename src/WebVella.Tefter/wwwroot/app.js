window.Tefter = {
	copyToClipboard: function(text) {
        navigator.clipboard.writeText(text).then(function () {})
        .catch(function (error) {
            log.error(error);
        });
    }
}