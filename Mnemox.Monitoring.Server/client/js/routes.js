let defaultPageRoute = 'client';

const API_ROUTES = {
  SERVER: {
    INIT_STATUS: "server/init-status",
    VALIDATE_DATABASE: "server/validate/database",
    INIT_SERVER_DETAILS: "server/init"
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

router.add('client/init', async function () {
  let responseTemplate = await mnemoxClient._templatesManager.getTemplate(TEMPLATES_ROUTES.INIT_TEMPLATE, { language: LANGUAGE });
  document.getElementById("main").innerHTML = responseTemplate;
});

router.addUriListener();

router.navigateTo(defaultPageRoute);