class TextHelpers {
  errorCodeToText(errorCode) {
    let errorText = null;
    switch (errorCode) {
      case "CANNOT_CONNECT_TO_THE_DATABASE":
        errorText = "Cannot establish connection to the database, please validate the database credentials";
        break;
    }

    return errorText;
  }
}