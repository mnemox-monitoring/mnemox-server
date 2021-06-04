let defaultPageRoute = 'client';

const API_ROUTES = {
  SERVER: {
    INIT_STATUS: "server/init-status",
    VALIDATE_DATABASE: "server/validate/database"
  }
};

const CLIENT_ROUTES = {
  INIT_PAGE: `/${defaultPageRoute}/init`,
  SIGNIN_PAGE: `/${defaultPageRoute}/sign-in`
};

const TEMPLATES_ROUTES = {
  INIT_TEMPLATE: `/${defaultPageRoute}/pages/init.html`
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
    TEMPLATES_ROUTES.INIT_TEMPLATE,
    (response) => {
      document.getElementById("main").innerHTML = response;
    }, { language: LANGUAGE });
});

router.addUriListener();

router.navigateTo(defaultPageRoute);