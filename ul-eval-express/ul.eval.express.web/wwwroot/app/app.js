var app = angular.module("EvalExpress", []);
var mainController = function ($scope, $http) {

    var _localUrl = "https://localhost:44395/api/evaluate";
    var _azureUrl = "https://evalexpressapi.azurewebsites.net/api/evaluate";


    var _apiUrl = _localUrl;

    $scope.sendForEvaluation = function () {

        $scope.error = null;

        $http.post(_apiUrl, JSON.stringify($scope.expression))
            .then(function (result) {

                $scope.evaluatedResult = result.data.number;
                $scope.evaluatedExpression = result.data.computedExpressionAfterCleanup;

            }, function (reason) {

                $scope.error = "API Error";

            });
    }

    $scope.clear = function () {

        $scope.expression = null;
        $scope.evaluatedResult = null;
        $scope.evaluatedExpression = null;
        $scope.error = null;
    }
}

app.controller("mainController", mainController);