<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Dispatch_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
  
  <style type="text/css">

    body { font-size:small; }
    table { border-collapse: collapse; }
    table tr td 
    { 
      border: solid 1px #ccc; 
      padding: .4em;
    }
    .right {
      text-align: right;
    }
    .even { background-color:#ddd; }
    table.center {
      margin-left:auto; 
      margin-right:auto;
    }
  </style>
  <script src="//code.jquery.com/jquery-1.11.1.min.js"></script>
  <script src="//ajax.googleapis.com/ajax/libs/angularjs/1.2.26/angular.min.js"></script>  
  <script type="text/javascript"> 
    $(document).ready(function(){
      setInterval("my_function();",10000); 
  
      function my_function(){
          window.location = location.href;
      }
    });
    function parseWcfDate(wcfDate){
        var d=wcfDate.match(/(\d+)/)[1];
        return new Date(+d);
    }
    var app = angular.module('freightApp', []);
    
    app.controller('freightController', function($scope, $http){
       $http.get("http://netgateway/settlements/Dispatch.svc/GetMovingFreight")
        .success(function(response) {
          $scope.trips = response.GetMovingFreightResult;
          angular.forEach($scope.trips, function(trip, key) {
            trip.From.Date = parseWcfDate(trip.From.DateTime);
            trip.To.Date = parseWcfDate(trip.To.DateTime);
          });
          
        }).error(function (http, status, fnc, httpObj ) {
          console.log('Cannot retrieve.',http,status,httpObj);
        });
    });
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <div ng-app='freightApp'>
   
    <table class='center'>
      <thead>
        <tr>
          <th>
            Truck<br/>Trip#
          </th>
          <th>
            Trailer<br/>Driver Phone
          </th>
          <th>
            Driver<br/>Card
          </th>
          <th>
            Line Haul<br/>Pickup
          </th>
          <th>
            Arrival
          </th>
          <th>
            Truck<br/>Trip#
          </th>                
      </tr>
        </thead>
      <tbody ng-controller="freightController" > 
        <tr ng-repeat="trip in trips">
          <td ng-class-odd="'alternating-item'">{{trip.Tractor}}<br/>{{trip.TripNumber}}</td>
          <td ng-class-odd="'alternating-item'">Trl:&nbsp;{{trip.Trailer}}<br/>{{trip.Drivers[0].CellPhone}}</td>
          <td ng-class-odd="'alternating-item'">{{trip.Drivers[0].Name}}<br/>{{trip.Drivers[0].Card}}</td>
          <td ng-class-odd="'alternating-item'">{{trip.From.State}} - {{trip.To.State}}<br/>{{trip.From.Date | date: "MM/dd/yyyy"}}</td>
          <td ng-class-odd="'alternating-item'">{{trip.To.Date | date: "EEEE"}}<br/>{{trip.To.City}}</td>
          <td ng-class-odd="'alternating-item'">{{trip.Customer.Name}}<br/>{{trip.Customer.Phone}}</td>
        </tr>
      </tbody>
    </table>

  </div>
</asp:Content>

