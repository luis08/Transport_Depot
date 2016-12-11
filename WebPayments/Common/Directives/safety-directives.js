///<reference path="https://ajax.googleapis.com/ajax/libs/angularjs/1.5.7/angular.min.js"/>
angular.module('safetyCommon', []).component('tractorMaintenance', {
    templateUrl: '../Views/tractor.maintenance.html',
    controller: function ($scope) {
        $scope.tractorId = '';
        $scope.date = '';
        $scope.description = '';
        $scope.type = '';
    }
});