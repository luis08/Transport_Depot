<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false"
  CodeFile="Workarounds.aspx.vb" Inherits="Misc_Workarounds" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
  <div ng-app="Workarounds">
  <div ng-controller="wkAController">
    <fieldset>
      <div ng-show="isReadyToWork">
        <span>You can work normally.</span><br /><br />
        <button type="button" ng-click="prepareToOpen()">
          Prepare to Open</button><br />
      </div>
      <div ng-show="isReadyToOpen">
         <span>You can open the application.</span><br /><br />
         <button type="button" ng-click="prepareToWork()">
           Prepare to Work</button>
      </div>
    </fieldset>
    <div ng-form name="tractorForm" ng-show="isReadyToWork">
      <fieldset>
        <legend>New Tractor</legend>
        <label>
          Id:
          <input type="text" name="tractorId" ng-model="tractorId" required ng-minlength="4" ng-maxlength="8"/>
          <span class="invalid" ng-show="tractorForm.tractorId.$invalid" >*</span></label>
        <button ng-disabled="tractorForm.tractorId.$invalid" type="button" ng-click="appendTractor()">
          Save</button>
      </fieldset>
    </div>
    <div ng-form name="driverForm" ng-show="isReadyToWork">
      <fieldset>
        <legend>New Driver</legend>
        <label>
          Id:
          <input name="driverId" type="text" ng-model="driverId" ng-minlength="4" ng-maxlength="12" required/>
          <span class="invalid" ng-show="driverForm.driverId.$invalid">*</span></label>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <label>
          First Name:
          <input name="firstName" type="text" ng-model="firstName" ng-minlength="2" ng-maxlength="15" required/>
          <span class="invalid" ng-show="driverForm.firstName.$invalid">*</span></label>
        <button ng-disabled="driverForm.$invalid" type="button" ng-click="appendDriver()">
          Save</button>
      </fieldset>
    </div>
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JavaScriptPlaceHolder" runat="Server">
  <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.5.7/angular.min.js"></script>
  <script>
    var app = angular.module("Workarounds", []);
    app.controller("wkAController", ['$scope', '$http', function ($scope, $http) {
      var url = TransportApp.svcPath + "/workarounds.svc/ajax/";
      var setStatus = function () {
        $http.get(url + "isReadyToOpen").then(function (resp) {
          $scope.isReadyToOpen = resp.data.IsReadyToOpenResult;
        });
        $http.get(url + "isReadyToWork").then(function (resp) {
          $scope.isReadyToWork = resp.data.IsReadyToWorkResult;
        });
      };
      setStatus();

      $scope.prepareToOpen = function () {
        var req = {
          method: 'POST',
          url: url + 'prepareToOpen'
        };

        $http(req).then(function () {
          setStatus();
        }, function () {
          alert("There was an error");
        });
      };
      $scope.prepareToWork = function () {
        var req = {
          method: 'POST',
          url: url + 'prepareToWork'
        };
        $http(req).then(function () {
          setStatus();
        }, function () {
          alert("There was an error");
        });
      };

      $scope.appendDriver = function () {
        var req = {
          method: 'POST',
          url: url + 'driver?driverId=' + $scope.driverId + '&firstName=' + $scope.firstName,
          data: {
            Id: $scope.driverId,
            Name: $scope.firstName
          }
        };

        $http(req).then(function () {
          alert("Driver saved");
          $scope.driverId = $scope.firstName = '';
        }, function () {
          alert("There was an error");
        });
      };

      $scope.appendTractor = function () {
        var req = {
          method: 'POST',
          url: url + 'tractor?id=' + $scope.tractorId
        };

        $http(req).then(function () {
          alert("Tractor saved");
          $scope.tractorId = '';
        }, function () {
          alert("There was an error");
        });
      };
    } ]);
  </script>
</asp:Content>
