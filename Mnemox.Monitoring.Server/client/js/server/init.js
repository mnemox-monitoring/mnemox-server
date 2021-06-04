class ServerInitialization {
  
  constructor(textHelpers, uiHelpers, communication) {
    this.MANDATORY_FIELDS_TEXT = "Address, username and password fields are mandatory!";
    this.DATABASE = "database";
    this.DATABASE_DETAILS_ID = "divDatabaseDetails";
    this.SERVER_DETAILS_ID = "divServerDetails";
    this.DB_INIT_STORY_ID = "divDbInitStory";
    this.SERVER_DETAILS_STORY_ID = "divServerDetailsStory";
    this.ERROR_DURATION_MS = 10000;

    this._textHelpers = textHelpers;
    this._uiHelpers = uiHelpers;
    this._communication = communication;
  }

  validateDatabase() {
    const DB_NEXT_BUTTON_CAPTION_ID = "spnBtnDbNextCaption";
    const DB_NEXT_BUTTON_LOADER_ID = "spnBtnDbNextLoad";
    const address = document.getElementById("txtDbAddress").value;
    const username = document.getElementById("txtDbUsername").value;
    const password = document.getElementById("txtDbPassword").value;
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
    this._uiHelpers.switchElements(DB_NEXT_BUTTON_CAPTION_ID, DB_NEXT_BUTTON_LOADER_ID);
    this._communication.post(url, request,
      () => {
        this._uiHelpers.switchElements(this.DATABASE_DETAILS_ID, this.SERVER_DETAILS_ID);
        this._uiHelpers.switchElements(DB_NEXT_BUTTON_LOADER_ID, DB_NEXT_BUTTON_CAPTION_ID);
        this.selectStoryItem(this.SERVER_DETAILS_STORY_ID);
      }, (failResponse) => {
        const errorText = `${this._textHelpers.errorCodeToText(failResponse.errorCode)}. <br/> ${LANGUAGE.responseError}: ${failResponse.message}`;
        this._uiHelpers.alert(errorText, { duration: this.ERROR_DURATION_MS });
        this._uiHelpers.switchElements(DB_NEXT_BUTTON_LOADER_ID, DB_NEXT_BUTTON_CAPTION_ID);
        console.log(JSON.stringify(failResponse));
      });
  }

  backTo(destination) {
    switch (destination) {
      case this.DATABASE:
        this.selectStoryItem(this.DB_INIT_STORY_ID);
        this._uiHelpers.switchElements(this.SERVER_DETAILS_ID, this.DATABASE_DETAILS_ID);
        break;
    }
  }

  selectStoryItem(selectedItemId) {
    document.getElementById(this.DB_INIT_STORY_ID).className = "";
    document.getElementById(this.SERVER_DETAILS_STORY_ID).className = "";

    document.getElementById(selectedItemId).className = "current";
  }
}