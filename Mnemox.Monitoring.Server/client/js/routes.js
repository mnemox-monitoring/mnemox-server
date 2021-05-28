let defaultPageRoute = 'client';

const API_ROUTES = {
  SERVER: {
    INIT_STATUS: "server/init-status",
    VALIDATE_DATABASE: "server/validate/database"
  }
};

const CLIENT_ROUTES = {
  INIT_PAGE: `/${defaultPageRoute}/init`,
  SIGNIN_PAGE: `/${defaultPageRoute}/sign-in`,
  INIT_1_PAGE: `/${defaultPageRoute}/pages/init-1.html`
};

let router = new Router({
  mode: 'history',
  page404: function (path) {
    console.log('"/' + path + '" Page not found');
  }
});

router.add(defaultPageRoute, function () {
  console.log('Home page');
});

router.add('client/test', function () {
  console.log('Test page');
});

router.add('client/init', function () {
  mnemoxClient._templatesManager.getTemplate(
    CLIENT_ROUTES.INIT_1_PAGE,
    (response) => {
      document.getElementById("main").innerHTML = response;
    });
});

router.addUriListener();

router.navigateTo(defaultPageRoute);