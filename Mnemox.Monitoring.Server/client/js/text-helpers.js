class TextHelpers {
  errorCodeToText(errorCode) {
    let errorText = null;
    switch (errorCode) {
      case "CANNOT_CONNECT_TO_THE_DATABASE":
        errorText = LANGUAGE.cannotEstablishConnectionError;
        break;
      case "CANNOT_CREATE_EXTENSION":
        errorText = LANGUAGE.cannotCreateDbExtension;
        break;
      case "CANNOT_CREATE_SCHEMA":
        errorText = LANGUAGE.cannotCreateSchema;
        break;
      case "CANNOT_DROP_SCHEMA":
        errorText = LANGUAGE.cannotDropSchema;
        break;
      case "CANNOT_RUN_QUERY":
        errorText = LANGUAGE.cannotRunQuery;
        break;
      case "CANNOT_SET_DATABASE_STATE_PARAMETER":
        errorText = LANGUAGE.cannotSetDatabaseStateParameter
        break;
      default:
        errorText = LANGUAGE.systemError;
        break;
    }
    return errorText;
  }
}