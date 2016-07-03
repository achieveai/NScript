// Type definitions for Chrome Cast application development
// Project: https://developers.google.com/cast/
// Definitions by: Thomas Stig Jacobsen <https://github.com/eXeDK>
// Definitions: https://github.com/DefinitelyTyped/DefinitelyTyped

////////////////////
// Cast
// @see https://code.google.com/p/chromium/codesearch#chromium/src/ui/file_manager/externs/chrome_cast.js
////////////////////
declare namespace chrome.cast {
    interface Error {
        /**
         * @param {!chrome.cast.ErrorCode} code
         * @param {string=} opt_description
         * @param {Object=} opt_details
         * @constructor
         * @see https://developers.google.com/cast/docs/reference/chrome/chrome.cast.Error
         */
        new(
            code: chrome.cast.ErrorCode,
            description?: string,
            details?: Object
        ):chrome.cast.Error;

        code: chrome.cast.ErrorCode;
        description?: string;
        details?: string;

    }

	declare var Error : chrome.cast.Error;
}
