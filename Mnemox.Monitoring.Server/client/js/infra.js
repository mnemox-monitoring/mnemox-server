class Communication {
  constructor() {
    this.HTTP_METHODS = {
      POST: "POST",
      GET: "GET",
      DELETE: "DELETE",
      PUT: "PUT"
    };
  }

  post(url, body, onSuccess, onFail = null, toJson = true) {
    this.sendRequest(this.HTTP_METHODS.POST, url, body, onSuccess, onFail, toJson);
  }

  get(url, onSuccess, onFail = null, toJson = true) {
    this.sendRequest(this.HTTP_METHODS.GET, url, null, onSuccess, onFail, toJson);
  }

  put(url, body, onSuccess, onFail = null, toJson = true) {
    this.sendRequest(this.HTTP_METHODS.PUT, url, body, onSuccess, onFail, toJson);
  }

  delete(url, onSuccess, onFail = null, toJson = true) {
    this.sendRequest(this.HTTP_METHODS.DELETE, url, null, onSuccess, onFail, toJson);
  }

  sendRequest(method, url, body, onSuccess, onFail = null, toJson = true) {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
      if (this.readyState === 4 && this.status === 200) {
        let response = null;
        if (toJson) {
          response = JSON.parse(this.responseText);
        } else {
          response = this.responseText;
        }
        onSuccess(response);
      }
    };
    xhttp.open(method, url, true);
    xhttp.send(body);
  }

  createUrl(baseUrl, route) {
    let url = `${baseUrl}/${route}`;
    return url;
  }
}

class CookieManager {
  get(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
  }

  set(name, value, expireInMinutes, domainName) {
    var expires = "";
    var domain = "";
    if (days) {
      var date = new Date();
      date.setTime(date.getTime() + (expireInMinutes * 60 * 1000));
      expires = "; expires=" + date.toUTCString();
    }
    if (domainName) {
      domain = "; domain=" + domainName;
    }
    document.cookie = name + "=" + (value || "") + expires + domain + "; path=/";
  }
}

class TemplatesManager{
  constructor(communicationManager) {
    this._communication = communicationManager;
  }
  getTemplate(path, onSuccess) {
    this._communication.get(path, onSuccess, null, false);
  }
}
