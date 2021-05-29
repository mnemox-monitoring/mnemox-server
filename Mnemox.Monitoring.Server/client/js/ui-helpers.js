class UiHelpers {
  constructor(toast) {
    this._toast = toast;
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
    this._toast.alert(message, options);
  }
}