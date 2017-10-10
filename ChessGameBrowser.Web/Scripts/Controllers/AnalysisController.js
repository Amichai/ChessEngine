var app = angular.module('chessApp', []);

app.controller('analysisCtrl', ['$scope', '$http',
    function ($scope, $http) {
        $scope.analyze = function() {
            $http.post(baseUrl + 'api/home/analyze', $scope.gamePGN).then(function(result) {
                $scope.mistakes = result;
            });
        }
}]);
