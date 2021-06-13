class Communication {
  constructor() {
    this.HTTP_METHODS = {
      POST: "POST",
      GET: "GET",
      DELETE: "DELETE",
      PUT: "PUT"
    };
  }

  async postAsync(url, body, toJson = true) {
    return this.sendRequest(this.HTTP_METHODS.POST, url, body, toJson);
  }

  async getAsync(url, toJson = true) {
    return this.sendRequest(this.HTTP_METHODS.GET, url, null, toJson);
  }

  async putAsync(url, body, toJson = true) {
    return this.sendRequest(this.HTTP_METHODS.PUT, url, body, toJson);
  }

  async deleteAsync(url, toJson = true) {
    return this.sendRequest(this.HTTP_METHODS.DELETE, url, null, toJson);
  }

  sendRequest(method, url, body, toJson = true) {
    return new Promise(function (resolve, reject) {
      var req = new XMLHttpRequest();
      req.open(method, url);
      req.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
      req.onload = function () {
        let response = null;
        if (toJson) {
          response = JSON.parse(this.response);
        } else {
          response = this.response;
        }
        if (req.status === 200) {
          resolve(response);
        } else {
          reject(response);
        }
      };
      req.onerror = function () {
        reject(Error("Network Error"));
      }; 
      let requestBody = null;
      if (body) {
        requestBody = JSON.stringify(body);
      }
      req.send(requestBody);
    });
  };

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
    this._templates = {};
  }

  async getTemplate(path, jsonForMustache) {
    const template = this._templates[path];
    if (template) {
      return template;
    }
    let response = await this._communication.getAsync(path, false);

    if (jsonForMustache) {
      var html = mustache.render(response, jsonForMustache);
      this._templates[path] = html;
      return html;
    } else {
      this._templates[path] = response;
      return response;
    }
  }
}
