let defaultPageRoute = 'client';

let router = new Router({
  mode: 'history',
  page404: function (path) {
    console.log('"/' + path + '" Page not found');
  }
});

router.add(defaultPageRoute, function () {
  console.log('Home page');
});

router.addUriListener();

router.navigateTo(defaultPageRoute);