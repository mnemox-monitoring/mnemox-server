class ServerInitialization {
  
  constructor(uiHelpers, communication) {
    this.DATABASE = "database";
    this.DATABASE_DETAILS_ID = "divDatabaseDetails";
    this.SERVER_DETAILS_ID = "divServerDetails";
    this.DB_INIT_STORY_ID = "divDbInitStory";
    this.SERVER_DETAILS_STORY_ID = "divServerDetailsStory";
    this.ERROR_DURATION_MS = 10000;
    this.DB_NEXT_BUTTON_CAPTION_ID = "spnBtnDbNextCaption";
    this.DB_NEXT_BUTTON_LOADER_ID = "spnBtnDbNextLoad";
    this.SERVER_NEXT_BUTTON_CAPTION_ID = "spnBtnServerNextCaption";
    this.SERVER_NEXT_BUTTON_LOADER_ID = "spnBtnServerNextLoad";

    this._uiHelpers = uiHelpers;
    this._communication = communication;
  }

  async validateDatabase() {
    const address = document.getElementById("txtDbAddress").value;
    const username = document.getElementById("txtDbUsername").value;
    const password = document.getElementById("txtDbPassword").value;
    const port = document.getElementById("txtDbPort").value;
    if (!address || !username || !password) {
      this._uiHelpers.alert(LANGUAGE.databaseInitMandatoryFields);
      return;
    }
    const request = {
      address: address,
      username: username,
      password: password,
      port: isNaN(parseInt(port)) ? null : port
    };
    const url = this._communication.createUrl(CONSTANTS.API_URL, API_ROUTES.SERVER.VALIDATE_DATABASE);
    this._uiHelpers.switchElements(this.DB_NEXT_BUTTON_CAPTION_ID, this.DB_NEXT_BUTTON_LOADER_ID);

    try {
      const response = await this._communication.postAsync(url, request);
      if (response.servers.length === 0) {
        document.getElementById("divExistingServerSection").className = "display-none";
        this.setServersList(response.servers);
      } else {
        this.setServersList(response.servers);
      }
      this.gotToSection(response.serverInitializationState);
    } catch (e) {
      this._uiHelpers.alert(this._uiHelpers.createAlertText(e), { duration: this.ERROR_DURATION_MS });
      this._uiHelpers.switchElements(this.DB_NEXT_BUTTON_LOADER_ID, this.DB_NEXT_BUTTON_CAPTION_ID);
      console.log(JSON.stringify(e));
    }
  }

  async initServerDetails() {
    const serverId = document.getElementById("selExistingServer").value;
    const serverName = document.getElementById("txtServerName").value;
    const port = document.getElementById("txtServerPort").value;
    if (!serverName) {
      this._uiHelpers.alert(LANGUAGE.serverInitMandatoryFields);
      return;
    }
    const request = {
      serverId: isNaN(parseInt(serverId)) ? null : serverId,
      port: isNaN(parseInt(port)) ? null : port,
      serverName: serverName
    };
    const url = this._communication.createUrl(CONSTANTS.API_URL, API_ROUTES.SERVER.INIT_SERVER_DETAILS);
    this._uiHelpers.switchElements(this.SERVER_NEXT_BUTTON_CAPTION_ID, this.SERVER_NEXT_BUTTON_LOADER_ID);
    try {
      const response = await this._communication.postAsync(url, request);
      if (response.systemOwnerExists) {
        router.navigateTo(CLIENT_ROUTES.SIGN_IN_PAGE);
      } else {
        router.navigateTo(`${CLIENT_ROUTES.CREATE_INITIAL_USER_PAGE}/${response.serverId}`);
      }
    } catch (e) {
      this._uiHelpers.alert(this._uiHelpers.createAlertText(e), { duration: this.ERROR_DURATION_MS });
      this._uiHelpers.switchElements(this.DB_NEXT_BUTTON_LOADER_ID, this.DB_NEXT_BUTTON_CAPTION_ID);
      console.log(JSON.stringify(e));
    }
  }

  async createOwnerUser() {
    const firstName = document.getElementById("txtFirstName").value;
    const lastName = document.getElementById("txtLastName").value;
    const userName = document.getElementById("txtUserName").value;
    const password = document.getElementById("txtPassword").value;
    const confirmPassword = document.getElementById("txtConfirmPassword").value;
    if (!firstName || !lastName || !userName || !password || !confirmPassword) {
      this._uiHelpers.alert(LANGUAGE.allFieldsMandatory);
      return;
    }
    if (password !== confirmPassword) {
      this._uiHelpers.alert(LANGUAGE.passwordConfirmFail);
      return;
    }
    var request = {
      firstName: firstName,
      lastName: lastName,
      userName: userName,
      password: password
    };
    const url = this._communication.createUrl(CONSTANTS.API_URL, API_ROUTES.ACCOUNT.OWNER);
    try {
      const response = await this._communication.postAsync(url, request);
      router.navigateTo(`${CLIENT_ROUTES.SIGN_IN_PAGE}`);
    } catch (e) {
      this._uiHelpers.alert(this._uiHelpers.createAlertText(e), { duration: this.ERROR_DURATION_MS });
      console.log(JSON.stringify(e));
    }
  }

  gotToSection(gotTo) {
    switch (gotTo) {
      case CONSTANTS.SERVER_INITIALIZATION_STATES.DATABASE_INITIALIZED:
        this._uiHelpers.switchElements(this.DATABASE_DETAILS_ID, this.SERVER_DETAILS_ID);
        this._uiHelpers.switchElements(this.DB_NEXT_BUTTON_LOADER_ID, this.DB_NEXT_BUTTON_CAPTION_ID);
        this.selectStoryItem(this.SERVER_DETAILS_STORY_ID);
        break;
      case CONSTANTS.SERVER_INITIALIZATION_STATES.SERVER_INITIALIZAD:
        break;
    }
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

  setServersList(servers) {
    const selServersList = document.getElementById("selExistingServer");
    selServersList.options.length = 1;
    if (servers.length > 0) {
      document.getElementById("divExistingServerSection").className = "";
      for (let i = 0; i < servers.length; i++) {
        let option = document.createElement('option');
        option.value = servers[i].serverId;
        option.innerHTML = servers[i].serverName;
        selServersList.appendChild(option);
      }
    }
  }

  serverSelectionChanged() {
    const selServersList = document.getElementById("selExistingServer");
    if (selServersList.value) {
      const txtServerName = document.getElementById("txtServerName");
      txtServerName.value = selServersList.options[selServersList.selectedIndex].text;
      txtServerName.disabled = true;
    }
    else {
      document.getElementById("txtServerName").value = "";
      txtServerName.disabled = false;
    }
  }
}