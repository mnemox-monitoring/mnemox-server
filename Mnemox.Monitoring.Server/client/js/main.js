class MnemoxClient {
  constructor() {
    this.CONSTANTS = {
      LOGIN_COOKIE_NAME: "MLC"
    };
    this._communication = new Communication();
    this._cookieManager = new CookieManager();
    this._templatesManager = new TemplatesManager(this._communication);
    this._toast = SiiimpleToast.setOptions({
      position: 'top|right'
    });
    this._textHelpers = new TextHelpers();
    this._uiHelpers = new UiHelpers(this._toast);
    this._serverInitializer = new ServerInitialization(this._textHelpers, this._uiHelpers, this._communication);
  }

  async init() {
    const url = this._communication.createUrl(CONSTANTS.API_URL, API_ROUTES.SERVER.INIT_STATUS);
    let response = await this._communication.getAsync(url);
    if (!response.isInitialized) {
      router.navigateTo(CLIENT_ROUTES.INIT_PAGE);
    } else {
      router.navigateTo(CLIENT_ROUTES.SIGNIN_PAGE);
    }
  }
}

let mnemoxClient = new MnemoxClient();
mnemoxClient.init();


///////////////////////////////////////////////////////
//Implement string.format 
//First, checks if it isn't implemented yet.
if (!String.prototype.format) {
  String.prototype.format = function () {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function (match, number) {
      return typeof args[number] !== 'undefined'
        ? args[number]
        : match
        ;
    });
  };
}