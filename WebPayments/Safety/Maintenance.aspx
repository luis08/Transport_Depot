<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false"
  CodeFile="Maintenance.aspx.vb" Inherits="Safety_Maintenance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
  <link rel="stylesheet" type="text/css" href="../Styles/Safety/Maintenance.aspx.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
  <div ng-app="maintenanceApp" ng-controller="maintenanceController" ng-cloak>
    <div id="summary-reports">
      <h3>
        Summary Reports</h3>
      <fieldset>
        <legend>Date Range</legend>
        <label>
          From:</label>
        <input type="date" ng-model="summaryFrom" /><br />
        <label>
          To:</label>
        <input type="date" ng-model="summaryTo" /><br />
      </fieldset>
      <div id="tractors-container" class="vehicle-list">
        &nbsp;<a href="javascript:void(0)" ng-click="tractorsSetChecked(true)">Select All</a>
		&nbsp;<a href="javascript:void(0)" ng-click="tractorsSetChecked(false)">Select None</a>
        &nbsp;&nbsp;&nbsp;
        <button type="button" ng-click="printTractorSummaries()" ng-show="getSelectedTractors().length">
          Print
        </button>
		<div>
			<ul>
			  <li ng-repeat="tractor in tractors">
				<label>
				  <input type="checkbox" ng-click="checkTractor(tractor)" ng-checked="tractor.checked" />
				  {{tractor.Id + ' - ' + tractor.VIN}}
				</label>
			  </li>
			</ul>
		</div>
      </div>
      <div id="trailers-container" class="vehicle-list">
        &nbsp;<a href="javascript:void(0)" ng-click="trailersSetChecked(true)">Select All</a>
		&nbsp;<a href="javascript:void(0)" ng-click="trailersSetChecked(false)">Select None</a>
        &nbsp;&nbsp;&nbsp;
        <button type="button" ng-click="printTrailerSummaries()" ng-show="getSelectedTrailers().length">
          Print
        </button>
		<div>
			<ul>
			  <li ng-repeat="trailer in trailers">
				<label>
				  <input type="checkbox" ng-click="checkTrailer(trailer)" ng-checked="trailer.checked" />
				  {{trailer.Id + ' - ' + trailer.VIN}}
				</label>
			  </li>
			</ul>
		</div>
      </div>
    </div>
    <div id="pending-report">
      <h3>
        Pending Report</h3>
      <fieldset>
        <legend>Date Range</legend><legend>Date Range</legend>
        <label>
          From:</label>
        <input type="date" ng-model="pendingFrom" /><br />
        <label>
          To:</label>
        <input type="date" ng-model="pendingTo" /><br />
        <button type="button" ng-click="printTractorsPending()">
          Print Tractors Pending
        </button>
        <button type="button" ng-click="printTrailersPending()">
          Print Trailers Pending
        </button>
      </fieldset>
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JavaScriptPlaceHolder" runat="Server">
  <script src="//ajax.googleapis.com/ajax/libs/angularjs/1.2.26/angular.min.js"></script>
  <script src="../Scripts/json.js" type="text/javascript"></script>
  <script src="../Scripts/JSON.NET.js" type="text/javascript"></script>
  <script src="//code.jquery.com/jquery-1.11.1.min.js"></script>
  <script src="../Scripts/Safety/Maintenance.js" type="text/javascript"></script>
</asp:Content>
