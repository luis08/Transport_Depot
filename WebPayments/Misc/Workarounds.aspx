<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false"
  CodeFile="Workarounds.aspx.vb" Inherits="Misc_Workarounds" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
  <style type="text/css">
    label 
    {
      display: inline-block;
      width: 6em;
      text-align:right;
    }
    input[type='text']
    {
      width: 10em;
    }
    div[name='driverForm'] button 
    {
      margin-left: 6em;
      margin-top: 1em;
    }
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
  <div ng-app="Workarounds">
  <div ng-controller="wkAController">
    <fieldset>
      <div ng-show="isReadyToWork">
        <span>You can work normally.</span><br /><br />
        <button type="button" ng-disabled="inAjaxCall" ng-click="prepareToOpen()">
          Prepare to Open</button><br />
      </div>
      <div ng-show="isReadyToOpen">
         <span>You can open the application.</span><br /><br />
         <button type="button" ng-disabled="inAjaxCall" ng-click="prepareToWork()">
           Prepare to Work</button>
      </div>
    </fieldset>
    <div ng-form name="tractorForm" ng-show="isReadyToWork">
      <fieldset>
        <legend>New Tractor</legend>
        <label>Id:</label>
          <input type="text" name="tractorId" ng-model="tractorId" required ng-minlength="4" ng-maxlength="8" ng-pattern="/^[A-Za-z0-9\.]+$/"/> 
          <button ng-disabled="tractorForm.tractorId.$invalid" type="button" ng-click="appendTractor()">
          Save</button>
          &nbsp;&nbsp;
          <span class="invalid" ng-show="tractorForm.tractorId.$invalid" >* Only letters and '.'  Min 4 characters, Max 8.</span>
      </fieldset>
    </div>
    <div ng-form name="driverForm" ng-show="isReadyToWork">
      <fieldset>
        <legend>New Driver</legend>
        <label for="driverId">Id:</label>
        <input id="driverId" name="driverId" type="text" ng-model="driverId" ng-minlength="4" ng-maxlength="12" required ng-pattern="/^[A-Za-z0-9\.]+$/"/>
          &nbsp;&nbsp;
          <span class="invalid" ng-show="driverForm.driverId.$invalid">* Required and only letters and '.'  Min 4 characters, Max 12.</span>
          <br /><br />
        <label for="firstName">First Name:</label>
        <input id="firstName" name="firstName" type="text" ng-model="firstName" ng-minlength="2" ng-maxlength="15" required/>
        &nbsp;&nbsp;
        <span class="invalid" ng-show="driverForm.firstName.$invalid">* Required.  Min 2 characters.</span><br />
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
      var url = 'http://' + TransportApp.svcPath + "/workarounds.svc/ajax/";
      var setStatus = function () {
        $http.get(url + "isReadyToOpen").then(function (resp) {
          $scope.isReadyToOpen = resp.data.IsReadyToOpenResult;
        });
        $http.get(url + "isReadyToWork").then(function (resp) {
          $scope.isReadyToWork = resp.data.IsReadyToWorkResult;
        });
      };
      setStatus();
      $scope.inAjaxCall = false;

      $scope.prepareToOpen = function () {
        var req = {
          method: 'POST',
          url: url + 'prepareToOpen'
        };
        $scope.inAjaxCall = true;
        $http(req).then(function () {
          setStatus();
          $scope.inAjaxCall = false;
        }, function () {
          alert("There was an error");
          $scope.inAjaxCall = false;
        });
      };
      $scope.prepareToWork = function () {
        var req = {
          method: 'POST',
          url: url + 'prepareToWork'
        };
        $scope.inAjaxCall = true;
        $http(req).then(function () {
          setStatus();
          $scope.inAjaxCall = false;
        }, function () {
          alert("There was an error");
          $scope.inAjaxCall = false;
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
