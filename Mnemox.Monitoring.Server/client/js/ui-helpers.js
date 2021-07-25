class UiHelpers {
  constructor(toast, textHelpers) {
    this._toast = toast;
    this._textHelpers = textHelpers;
    this.ERROR_DURATION_MS = 3000;
  }

  switchElements(idHide, idShow) {
    document.getElementById(idHide).style.display = "none";
    document.getElementById(idShow).style.display = "block";
  }

  dotsLoader(elementId) {
    document.getElementById(elementId).innerHTML = "<img src='/client/images/dots-loader.gif' class='dots-loader'/>";
  }

  setText(elementId, text) {
    document.getElementById(elementId).innerHTML = text;
  }

  alert(message, options) {
    if (!options) {
      options = {
        duration: this.ERROR_DURATION_MS
      };
    }
    this._toast.alert(message, options);
  }

  createAlertText(e) {
    const errorText = `${this._textHelpers.errorCodeToText(e.errorCode)}. <br/> ${LANGUAGE.responseError}: ${e.message}`;
    return errorText;
  }
}