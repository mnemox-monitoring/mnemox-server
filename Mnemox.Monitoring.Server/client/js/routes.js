﻿let DEFAULT_PAGE_ROUTE = 'client';

const API_ROUTES = {
  SERVER: {
    INIT_STATUS: "server/init-status",
    VALIDATE_DATABASE: "server/validate/database",
    INIT_SERVER_DETAILS: "server/init",
    ACCOUNT_RESOURCE: "account"
  },
  ACCOUNT: {
    OWNER: "account/owner"
  }
};

const CLIENT_ROUTES = {
  INIT_PAGE: `/${DEFAULT_PAGE_ROUTE}/init`,
  SIGN_IN_PAGE: `/${DEFAULT_PAGE_ROUTE}/sign-in`,
  CREATE_INITIAL_USER_PAGE: `/${DEFAULT_PAGE_ROUTE}/account/owner/sign-up`
};

const TEMPLATES_ROUTES = {
  INIT_TEMPLATE: `/${DEFAULT_PAGE_ROUTE}/pages/init.html`,
  USER_BASE_DETAILS: `/${DEFAULT_PAGE_ROUTE}/pages/user-base-details.html`,
  CREATE_OWNER_USER: `/${DEFAULT_PAGE_ROUTE}/pages/account/create-owner-user.html`,
  SIGN_IN_TEMPLATE: `/${DEFAULT_PAGE_ROUTE}/pages/account/sign-in.html`
};

let router = new Router({
  mode: 'history',
  page404: function (path) {
    console.log('"/' + path + '" Page not found');
  }
});

router.add(DEFAULT_PAGE_ROUTE, function () {
  console.log('Home page');
});

router.add(CLIENT_ROUTES.INIT_PAGE, async function () {
  let responseTemplate = await mnemoxClient._templatesManager.getTemplate(TEMPLATES_ROUTES.INIT_TEMPLATE, { language: LANGUAGE });
  document.getElementById("main").innerHTML = responseTemplate;
});

router.add(`${CLIENT_ROUTES.CREATE_INITIAL_USER_PAGE}/{serverId}`, async function (serverId) {
  let baseUserTemplate = await mnemoxClient._templatesManager.getTemplate(TEMPLATES_ROUTES.USER_BASE_DETAILS, { language: LANGUAGE });
  let createOwnerTemplate = await mnemoxClient._templatesManager.getTemplate(TEMPLATES_ROUTES.CREATE_OWNER_USER, { language: LANGUAGE });
  document.getElementById("main").innerHTML = createOwnerTemplate;
  document.getElementById("createOwnerUserContent").innerHTML = baseUserTemplate;
  document.getElementById("hidServerId").value = serverId;
});

router.add(`${CLIENT_ROUTES.SIGN_IN_PAGE}`, async function () {
  let template = await mnemoxClient._templatesManager.getTemplate(TEMPLATES_ROUTES.SIGN_IN_TEMPLATE, { language: LANGUAGE });
  document.getElementById("main").innerHTML = template;
});

router.addUriListener();
router.navigateTo(DEFAULT_PAGE_ROUTE);