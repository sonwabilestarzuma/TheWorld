// simpleCopntrols.js

(function () {

    "use strict";

    angular.module("simpleControls", [])
        .directive("waitCursor", waitCursor);

    function waitCursor() {
        // lets rertun an object
        return {
            scope: {
                show: "=displayWhen"
            },
            restrict: "E",
            templateUrl: "/Views/waitCursor.html"
        };
    }

})();