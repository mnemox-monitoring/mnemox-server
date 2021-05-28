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

  validateDatabase() {
    const address = document.getElementById("txtDbAddress").value;
    const username = document.getElementById("txtDbUsername").value;
    const password = document.getElementById("txtDbPassword").value;
    if (!address || !username || !password) {
      this._toast.alert('All fields are mandatory!'); 
      return;
    }
    const request = {
      address: address,
      username: username,
      password: password
    }
    const url = this._communication.createUrl(VARIABLES.API_URL, API_ROUTES.SERVER.INIT_STATUS);
    this._communication.post(url, request, (response) => {
      console.log(response);
    });
  }
}

let mnemoxClient = new MnemoxClient();
mnemoxClient.init();