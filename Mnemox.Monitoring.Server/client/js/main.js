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

  init() {
    const url = this._communication.createUrl(VARIABLES.API_URL, API_ROUTES.SERVER.INIT_STATUS);
    this._communication.get(url, (response) => {
      if (!response.isInitialized) {
        router.navigateTo(CLIENT_ROUTES.INIT_PAGE);
      } else {
        router.navigateTo(CLIENT_ROUTES.SIGNIN_PAGE);
      }
    });
  }
}

let mnemoxClient = new MnemoxClient();
mnemoxClient.init();