
let app = angular.module('maintenanceApp', []);
app.controller('maintenanceController', ['$scope', '$http', '$filter', '$window', ($scope, $http, $filter, $window) => {

  function getDate(date) {
    return $filter("date")(date, 'yyyy-MM-dd');
  }
  
  function getSelected(collectionName) {
    return $scope[collectionName].filter(t => t.checked).map(t => t.Id);
  }
    
  function getSafetyUri (method) {
    let hostLocation = window.location.host
    var uri = 'http://' + hostLocation + '/trans-svc/Safety.svc/ajax/' + method;
    return uri;
  };

  function getReportUri(method) {
    let hostLocation = window.location.host;
    var uri = 'http://' + hostLocation + '/trans-svc/SafetyReports.svc/ajax/' + method;
    return uri;
  }
  
  $scope.tractors = [];
  $scope.trailers = [];
  let today = new Date();
  let aYearAgo = new Date();
  let aMonthAgo = new Date();
  aMonthAgo.setMonth(today.getMonth() -1);
  aYearAgo.setFullYear(aYearAgo.getFullYear() - 1);
  $scope.summaryFrom = getDate(aMonthAgo);
  $scope.summaryTo = getDate(today);
  $scope.pendingFrom = getDate(aYearAgo);
  $scope.pendingTo = getDate(today);
  
  $scope.printTractorSummaries = function () {
    let selectedTractors = getSelected('tractors');
    console.log('tractors:');
    console.log(selectedTractors);
    
    let uri = getReportUri('TractorMaintenance') 
      + '?ids='
      + selectedTractors.join() 
      + '&from=' + $scope.summaryFrom
      + '&to=' + $scope.summaryTo;
    $window.open(uri, '_blank');
  };
  
  $scope.printTrailerSummaries = function () {
    let selectedTrailers = getSelected('trailers');
    console.log('trailers:');
    console.log(selectedTrailers);
    let uri = getReportUri('TrailerMaintenance') 
      + '?ids='
      + selectedTrailers.join() 
      + '&from=' + $scope.summaryFrom 
      + '&to=' + $scope.summaryTo;
    $window.open(uri, '_blank');
  };
  
  $scope.printTractorsPending = function () {
    let uri = getReportUri('TractorMaintenance') 
      + '/pending?'
      + 'from=' + $scope.pendingFrom + '' 
      + '&to=' + $scope.pendingTo;
    $window.open(uri, '_blank');
  };
  
  $scope.printTrailersPending = function () {
    let uri = getReportUri('TrailerMaintenance') 
      + '/pending?'
      + 'from=' + $scope.pendingFrom + '' 
      + '&to=' + $scope.pendingTo;
    $window.open(uri, '_blank');
  };
  
  $scope.getSelectedTractors = function () {
    return getSelected('tractors');
  };
  
  $scope.getSelectedTrailers = function () {
    return getSelected('trailers');
  };
  
  $scope.checkTractor = function (tractor) {
    tractor.checked = !tractor.checked;
    console.log(tractor.Id + ' checked: ' + tractor.checked);
  };
  
  $scope.checkTrailer = function (trailer) {
    trailer.checked = !trailer.checked;
    console.log(trailer.Id + ' checked: ' + trailer.checked);
  };

  $scope.tractorsSetChecked =  function (checked) {
    $scope.tractors.forEach(t => t.checked = checked);
  };
  
  $scope.trailersSetChecked = function (checked) {
    $scope.trailers.forEach(t => t.checked = checked);
  };
  
  let req = {
    method: 'POST',
    url: getSafetyUri('GetTractors'),
    headers: {
      'Content-Type': undefined
    },
    data: { activeOnly: true },
    headers: {'Content-Type': 'application/json'}
  };
  
  $http(req).then(function(response) {
    $scope.tractors = response.data.GetTractorsResult;
	$scope.tractors.sort((lhs, rhs)=> {
		let a = lhs.Id.toUpperCase();
		let b = rhs.Id.toUpperCase();
		if(a < b) return -1;
		if(a > b) return 1;
		return 0;
	});
    $scope.tractors.forEach(t => t.checked = false);
  });
  
  req.url = getSafetyUri('GetTrailers');
  $http(req).then(function(response) {
    $scope.trailers = response.data.GetTrailersResult;
	$scope.trailers.sort((lhs, rhs) => {
		let a = lhs.Id.toUpperCase();
		let b = rhs.Id.toUpperCase();
		if(a < b) return -1;
		if(a > b) return 1;
		return 0;		
	});
    $scope.trailers.forEach(t => t.checked = false);
  });
}]);