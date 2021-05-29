class ServerInitialization {
  
  constructor(textHelpers, uiHelpers, communication) {
    this.MANDATORY_FIELDS_TEXT = "Address, username and password fields are mandatory!";
    this._textHelpers = textHelpers;
    this._uiHelpers = uiHelpers;
    this._communication = communication;
  }

  validateDatabase() {
    const DATABASE_DETAILS_ID = "divDatabaseDetails";
    const SERVER_DETAILS_ID = "divServerDetails";
    const DB_INIT_STORY_ID = "divDbInitStory";
    const SERVER_DETAILS_STORY_ID = "divServerDetailsStory";
    const DB_NEXT_BUTTON_ID = "divBtnDbNext";
    const address = document.getElementById("txtDbAddress").value;
    const username = document.getElementById("txtDbUsername").value;
    const password = document.getElementById("txtDbPassword").value;
    const NEXT_TEXT = "Next";
    const port = document.getElementById("txtDbPort").value;
    if (!address || !username || !password) {
      this._uiHelpers.alert(this.MANDATORY_FIELDS_TEXT);
      return;
    }
    const request = {
      address: address,
      username: username,
      password: password,
      port: isNaN(parseInt(port)) ? null : port
    }
    const url = this._communication.createUrl(VARIABLES.API_URL, API_ROUTES.SERVER.VALIDATE_DATABASE);
    this._uiHelpers.dotsLoader(DB_NEXT_BUTTON_ID);
    this._communication.post(url, request,
      () => {
        this._uiHelpers.switchElements(DATABASE_DETAILS_ID, SERVER_DETAILS_ID);
        document.getElementById(DB_INIT_STORY_ID).className = "";
        document.getElementById(SERVER_DETAILS_STORY_ID).className = "current";
        this._uiHelpers.setText(DB_NEXT_BUTTON_ID, NEXT_TEXT);
      }, (failResponse) => {
        const errorText = `${this._textHelpers.errorCodeToText(failResponse.errorCode)}. <br/> Response error: ${failResponse.message}`;
        this._uiHelpers.alert(errorText, { duration: 10000 });
        this._uiHelpers.setText(DB_NEXT_BUTTON_ID, NEXT_TEXT);
        console.log(JSON.stringify(failResponse));
      });
  }
}