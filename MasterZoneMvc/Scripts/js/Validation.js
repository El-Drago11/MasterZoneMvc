function isValidHttpLink(link) {
    // Regular expression to match HTTP/HTTPS links
    var linkPattern = /^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/i;
    return linkPattern.test(link);
}