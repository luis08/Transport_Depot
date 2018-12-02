/// <reference path="jquery-1.10.2.js" />
/// <reference path="mustache.js" />
/// <reference path="json.js" />
/// <reference path="Utilities.js" />
/// <reference path="JSON.NET.js" />

function SafetyConstants() { }

SafetyConstants.getUri = function (method) {
  var uri = 'http://' + window.location.host + '/trans-svc/Safety.svc/ajax/' + method;
  return uri;
};

SafetyConstants.getSettlementsUri = function (method) {
  var uri = 'http://' + window.location.host + '/trans-svc/Settlements.svc/ajax/' + method;
  return uri;
};

function SafetyData() { }
SafetyData.getUriDate = function (date) {
  return date.getFullYear() + '-' + 
    (date.getMonth() + 1) + '-' +
    date.getDate();
};
SafetyData.updateDriver = function (driver, successCallback) {
  var model = {
    drivers: [driver]
  };
  var wcfModel = JSON.stringifyWcf(model);
  var uri = SafetyConstants.getUri('UpdateDrivers');
  var request = $.ajax({
    type: 'POST',
    contentType: 'application/json',
    data: wcfModel,
    url: uri,
    dataType: 'json'
  });
  request.done(function () {
    successCallback();
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Request failed: ' + textStatus);
  });
};

SafetyData.getDrivers = function (driverIds, successCallback) {
  var model = {
    ids: driverIds
  };
  var wcfModel = JSON.stringify(model);
  var uri = SafetyConstants.getUri('GetDrivers');
  var request = $.ajax({
    type: 'POST',
    contentType: 'application/json',
    data: wcfModel,
    url: uri,
    dataType: 'json'
  });
  request.done(function (result) {
    successCallback(result.GetDriversResult);
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Request failed: ' + textStatus);
  });
};

SafetyData.getAllDrivers = function (successCallback) {
  var uri = SafetyConstants.getUri('GetAllDrivers');
  var request = $.ajax({
    type: 'POST',
    contentType: 'application/json',
    data: JSON.stringify({ activeOnly: true }),
    url: uri,
    dataType: 'json'
  });
  request.done(function (data) {
    var result = { drivers: data.GetAllDriversResult };
    successCallback(result);
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Request failed: ' + textStatus);
  });
};

SafetyData.getUntrackedDrivers = function (successCallback) {
  var uri = SafetyConstants.getUri('GetUntrackedDriversOptions');
  var request = $.ajax({
    type: 'POST',
    url: uri,
    dataType: 'json'
  });
  request.done(function (data) {

    successCallback(data.GetUntrackedDriversOptionsResult);
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Request failed: ' + textStatus);
  });
};

SafetyData.trackDriver = function(id, successCallback){
  var model = JSON.stringify({ 'ids': [id] });
  var uri = SafetyConstants.getUri('TrackDrivers');
  var request = $.ajax({
    type: 'POST',
    contentType: 'application/json',
    data: model,
    url: uri,
    dataType: 'json'
  });
  request.done(function () {
    successCallback();
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Update Tractor Failed: ' + textStatus);
  });
};
SafetyData.unTrackDriver = function(id, successCallback){
  var model = JSON.stringify({ 'ids': [id] });
  var uri = SafetyConstants.getUri('UnTrackDrivers');
  var request = $.ajax({
    type: 'POST',
    contentType: 'application/json',
    data: model,
    url: uri,
    dataType: 'json'
  });
  request.done(function () {
    successCallback();
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Update Tractor Failed: ' + textStatus);
  });
};

SafetyData.updateTractor = function (tractor, successCallback) {

  var wcfModel = JSON.stringifyWcf({ 'tractor': tractor });
  var uri = SafetyConstants.getUri('UdpateTractor');
  var request = $.ajax({
    type: 'POST',
    contentType: 'application/json',
    data: wcfModel,
    url: uri,
    dataType: 'json'
  });
  request.done(function () {
    successCallback();
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Update Tractor Failed: ' + textStatus);
  });
};

SafetyData.getTractors = function (ids, successCallback) {
  var uri = SafetyConstants.getUri('GetTractorsById');
  var model = JSON.stringify({ 'ids': ids });
  var request = $.ajax({
    type: 'POST',
    contentType: 'application/json',
    data: model,
    url: uri,
    dataType: 'json'
  });
  request.done(function (data) {
    successCallback(data.GetTractorsByIdResult);
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Request failed: ' + textStatus);
  });
};

SafetyData.getAllTractors = function (successCallback) {
  var uri = SafetyConstants.getUri('GetTractors');
  var request = $.ajax({
    type: 'POST',
    contentType: 'application/json',
    data: JSON.stringify({ activeOnly: true }),
    url: uri,
    dataType: 'json'
  });
  request.done(function (data) {
    successCallback(data.GetTractorsResult);
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Request failed: ' + textStatus);
  });
};

SafetyData.getLessors = function (successCallback) {
  var uri = SafetyConstants.getSettlementsUri('GetAllLessors');
  var request = $.ajax({
    type: 'POST',
    url: uri,
    dataType: 'json'
  });
  request.done(function (data) {

    successCallback(data.GetAllLessorsResult);
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Request failed: ' + textStatus);
  });
};

SafetyData.getAllTrailers = function (successCallback) {
  var uri = SafetyConstants.getUri('GetTrailers');
  var request = $.ajax({
    type: 'POST',
    contentType: 'application/json',
    data: JSON.stringify({ activeOnly: true }),
    url: uri,
    dataType: 'json'
  });
  request.done(function (data) {
    successCallback(data.GetTrailersResult);
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Request failed: ' + textStatus);
  });
};

SafetyData.getTrailers = function (ids, successCallback) {
  var uri = SafetyConstants.getUri('GetTrailersById');
  var model = JSON.stringify({ 'ids': ids });
  var request = $.ajax({
    type: 'POST',
    contentType: 'application/json',
    data: model,
    url: uri,
    dataType: 'json'
  });
  request.done(function (data) {
    successCallback(data.GetTrailersByIdResult);
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Request failed: ' + textStatus);
  });
};

SafetyData.updateTrailer = function (trailer, successCallback) {

  var wcfModel = JSON.stringifyWcf({ 'trailer': trailer });
  var uri = SafetyConstants.getUri('UdpateTrailer');
  var request = $.ajax({
    type: 'POST',
    contentType: 'application/json',
    data: wcfModel,
    url: uri,
    dataType: 'json'
  });
  request.done(function () {
    successCallback();
  });

  request.fail(function (jqXHR, textStatus) {
    alert('Update Trailer Failed: ' + textStatus);
  });
};

function SafetyMain() { }


SafetyMain.getDriver = function (tr) {
  var driver = {
    Id: $(tr).find('.driver-id').text(),
    Active: $(tr).find('.driver-active').val(),
    TrackDriver: true,
    Application: $(tr).find('.driver-application').prop('checked'),
    OnDutyHours: $(tr).find('.driver-on-duty-hours').prop('checked'),
    DrugTest: $(tr).find('.driver-drug-test').prop('checked'),
    PoliceReport: $(tr).find('.driver-police-report').prop('checked'),
    PreviousEmployerForm: $(tr).find('.driver-previous-employer').prop('checked'),
    SocialSecurity: $(tr).find('.driver-social-security').prop('checked'),
    Agreement: $(tr).find('.driver-agreement').prop('checked'),
    W9: $(tr).find('.driver-w9').prop('checked'),
    AnnualCertificationOfViolations: $(tr).find('.driver-annual-certificate-violations').datepicker('getDate'),
    PhysicalExamExpiration: $(tr).find('.driver-physical-expires').datepicker('getDate'),
    LastValidLogDate: $(tr).find('.driver-last-valid-log').datepicker('getDate'),
    DriversLicenseExpiration: $(tr).find('.driver-licence-expiration').datepicker('getDate'),
    MVRExpiration: $(tr).find('.driver-mvr-expiration').datepicker('getDate'),
    Comments: $(tr).find('.driver-comments').val()
  };
  return driver;
};

SafetyMain.getTractor = function (tr) {
  var tractor = {
    Id: $(tr).find('.tractor-id').text(),
    HasW9: $(tr).find('.tractor-w9').prop('checked'),
    InspectionDue: $(tr).find('.inspection-due').datepicker('getDate'),
    LeaseAgreementDue: $(tr).find('.lease-agreement').datepicker('getDate'),
    LastMaintenance: new Date($(tr).find('.maintenance-due').text()),
    RegistrationExpiration: $(tr).find('.registration-expiration').datepicker('getDate'),
    InsuranceExpiration: $(tr).find('.insurance-expiration').datepicker('getDate'),
    InsuranceName: $(tr).find('.insurance-name').text(),
    LessorOwnerName: $(tr).find('.lessor-owner').val(),
    Unit: $(tr).find('.tractor-unit').text(),
    Comments: $(tr).find('.tractor-comments').text()
  };
  return tractor;
};

SafetyMain.getTrailer = function (tr) {
  var trailer = {
    Id: $(tr).find('.trailer-id').text(),
    InspectionDue: $(tr).find('.inspection-due').datepicker('getDate'),
    LastMaintenance: new Date($(tr).find('.maintenance-due').text()),
    RegistrationExpiration: $(tr).find('.registration-expiration').datepicker('getDate'),
    LessorOwnerName: $(tr).find('.lessor-owner').val(),
    Unit: $(tr).find('.trailer-unit').text(),
    Comments: $(tr).find('.trailer-comments').text()
  };
  return trailer;
};

SafetyMain.populateDriverTracking = function () {
  function populate(untrackedDrivers) {
    var trackedDrivers = $('#driver-list-container').data('all-drivers');
    untrackedDrivers.sort(function (lhs, rhs) {
      return (lhs.Name > rhs.Name) ? 1 : -1;
    });
    trackedDrivers.sort(function (lhs, rhs) {
      return (lhs.Name > rhs.Name) ? 1 : -1;
    });
    var template = $('#driver-tracking-template').html();


    var html = Mustache.to_html(template, {
      'untrackedDrivers': untrackedDrivers,
      'trackedDrivers': trackedDrivers
    });
    $('#driver-track-menu').empty().show().html(html);
    $('#track-driver-button').off().on('click', function () {
      
      var driverToTrack = $.trim($('#untracked-drivers-select').val());
      if (driverToTrack.length > 0) {
        SafetyData.trackDriver(driverToTrack, SafetyMain.populateDrivers);
      }
    });
    $('#untrack-driver-button').off().on('click', function () {
      var driverToUnTrack = $.trim($('#tracked-drivers-select').val());
      if (driverToUnTrack.length > 0) {
        SafetyData.unTrackDriver(driverToUnTrack, SafetyMain.populateDrivers);
      }
    });
  }

  SafetyData.getUntrackedDrivers(populate);
};
SafetyMain.populateDrivers = function () {
  function cleanupDriver(driver) {
    driver.PhysicalExamExpiration = Utilities.formatDate(JSON.parseDateOnly(driver.PhysicalExamExpiration));
    driver.LastValidLogDate = Utilities.formatDate(JSON.parseDateOnly(driver.LastValidLogDate));
    driver.DriversLicenseExpiration = Utilities.formatDate(JSON.parseDateOnly(driver.DriversLicenseExpiration));
    driver.AnnualCertificationOfViolations = Utilities.formatDate(JSON.parseDateOnly(driver.AnnualCertificationOfViolations));
    driver.MVRExpiration = Utilities.formatDate(JSON.parseDateOnly(driver.MVRExpiration));
    return driver;
  }

  function populateSingleDriver(driver, tr) {
    driver = cleanupDriver(driver);
    $(tr).find('.driver-id').text(driver.Id);
    $(tr).find('.driver-active').val(driver.Active);
    $(tr).find('.driver-application').prop('checked', driver.Application);
    $(tr).find('.driver-on-duty-hours').prop('checked', driver.OnDutyHours);
    $(tr).find('.driver-drug-test').prop('checked', driver.DrugTest);
    $(tr).find('.driver-police-report').prop('checked', driver.PoliceReport);
    $(tr).find('.driver-previous-employer').prop('checked', driver.PreviousEmployerForm);
    $(tr).find('.driver-social-security').prop('checked', driver.SocialSecurity);
    $(tr).find('.driver-agreement').prop('checked', driver.Agreement);
    $(tr).find('.driver-w9').prop('checked', driver.W9);
    $(tr).find('.driver-annual-certificate-violations').val(driver.AnnualCertificationOfViolations);
    $(tr).find('.driver-physical-expires').val(driver.PhysicalExamExpiration);
    $(tr).find('.driver-last-valid-log').val(driver.LastValidLogDate);
    $(tr).find('.driver-licence-expiration').val(driver.DriversLicenseExpiration);
    $(tr).find('.driver-mvr-expiration').val(driver.MVRExpiration);
    $(tr).find('.driver-comments').text(driver.Comments);
  }

  function populate(result) {

    var allDrivers = [];
    $.each(result.drivers, function (idx, driver) {
      driver = cleanupDriver(driver);
      allDrivers.push(driver);
    });
    var driverPicker = $('#driver-picker').empty();
    allDrivers.sort(function (lhs, rhs) {
      return (lhs.Name > rhs.Name) ? 1 : -1;
    });
    $.each(allDrivers, function (idx, driver) {
      $(driverPicker).append($('<li>').append(
        $('<a>').attr('href', '#').text(driver.Name)
          .data('driver-id', driver.Id)
          .addClass('picked-driver')));
    });
    var template = $('#driver-safety-template').html();
    var html = Mustache.to_html(template, result);
    $('#driver-list-container').html(html)
      .data('all-drivers', result.drivers);

    $('.picked-driver').on('click', function (e) {
      e.preventDefault();
      var driverId = $(this).data('driver-id');
      var container = $('#scrollable-drivers-container');
      $('#' + driverId).closest('tr')[0].scrollIntoView();
    });
    $('.driver-physical-expires').datepicker();
    $('.driver-last-valid-log').datepicker();
    $('.driver-licence-expiration').datepicker();
    $('.driver-annual-certificate-violations').datepicker();
    $('.driver-mvr-expiration').datepicker();

    $('.safety-entity-menu').on('click', function () {
      var tr = $(this).closest('tr');
      var linkLabel = $(this).text();
      var driver = SafetyMain.getDriver(tr);
      if (linkLabel === 'Save') {
        SafetyData.updateDriver(driver, function () {
          $(tr).find('.last-saved-message').text('Saved: ' + Utilities.getTimeNow());
        });
      } else if (linkLabel === 'Reset') {
        var driversModel = [driver.Id];
        SafetyData.getDrivers(driversModel, function (drivers) {
          driver = drivers[0];
          populateSingleDriver(driver, tr);
        });
      }
      
    });
    SafetyMain.populateDriverTracking();  
  }
  $('#open-safety-report').text('Driver Safety Report')
    .attr('href', 'http://transportserver/trans-svc/SafetyReports.svc/ajax/DriverSafetyReport');
  $('#tractor-safety-container').hide();
  $('#maintenance-reports-menu').hide();
  $('#trailer-safety-container').hide();
  $('#driver-safety-container').show();
  $('#pending-maintenance-report').hide();
  
  SafetyData.getAllDrivers(populate);
};

function populateDialog(dialogTitle, idLabel) {
  $('#dialog-title').text(dialogTitle);
  $('#dialog-id-label').text(idLabel);
}

SafetyMain.populateTractors = function () {
  function cleanupTractor(tractor) {
    tractor.InspectionDue = Utilities.formatDate(JSON.parseDateOnly(tractor.InspectionDue));
    tractor.LeaseAgreementDue = Utilities.formatDate(JSON.parseDateOnly(tractor.LeaseAgreementDue));
    tractor.LastMaintenance = Utilities.formatDate(JSON.parseDateOnly(tractor.LastMaintenance));
    tractor.RegistrationExpiration = Utilities.formatDate(JSON.parseDateOnly(tractor.RegistrationExpiration));
    tractor.InsuranceExpiration = Utilities.formatDate(JSON.parseDateOnly(tractor.InsuranceExpiration));
    return tractor;
  }

  function populateSingleTractor(tractor, tr) {
    tractor = cleanupTractor(tractor);
    $(tr).find('.tractor-unit').text(tractor.Unit);
    $(tr).find('.lease-agreement').val(tractor.LeaseAgreementDue);
    $(tr).find('.maintenance-due').text(tractor.LastMaintenance);
    $(tr).find('.registration-expiration').val(tractor.RegistrationExpiration);
    $(tr).find('.inspection-due').val(tractor.InspectionDue);
    $(tr).find('.insurance-expiration').val(tractor.InsuranceExpiration);
    $(tr).find('.tractor-w9').prop('checked', tractor.HasW9);
    $(tr).find('.vin-number').text(tractor.VIN);
    $(tr).find('.tractor-comments').text(tractor.Comments);
    var lessorOwnerName = tractor.LessorOwnerName;
    var lessorSelect = $(tr).find('.lessor-owner');

    if (lessorOwnerName !== '') {
      $(lessorSelect).val(lessorOwnerName);
    } else {
      $(lessorSelect).val('');
    }
  }

  function populate(wcfTractors) {
    var allTractors = [];
    $.each(wcfTractors, function (idx, tractor) {
      tractor = cleanupTractor(tractor);
      allTractors.push(tractor);
    });


    allTractors.sort(function (lhs, rhs) {
      return (lhs.Id > rhs.Id) ? 1 : -1;
    });
    wcfTractors = allTractors;
    var template = $('#tractor-safety-template').html();
    var html = Mustache.to_html(template, { Tractors: wcfTractors });
    $('#tractor-list-container').html(html);
    $('.lease-agreement').datepicker();
    //$('.maintenance-due').datepicker();
    $('.registration-expiration').datepicker();
    $('.insurance-expiration').datepicker();
    $('.inspection-due').datepicker();
    var tractorPicker = $('#tractor-picker').empty();

    $.each(allTractors, function (idx, tractor) {
      $(tractorPicker).append($('<li>').append(
        $('<a>').attr('href', '#').text(tractor.Id)
          .addClass('picked-tractor')));
    });
    $('.picked-tractor').on('click', function (e) {
      e.preventDefault();
      var tractorId = 'tractor-' + $.trim($(this).text());
      var container = $('#scrollable-tractors-container');
      $('#' + tractorId).closest('tr')[0].scrollIntoView();
    });
    $('.save-tractor-button').on('click', function () {
      var tr = $(this).closest('tr');
      var tractor = SafetyMain.getTractor(tr);
      SafetyData.updateTractor(tractor, function () {
        $(tr).find('.last-saved-message').text('Saved: ' + Utilities.getTimeNow());
      });
    });
    $('.reset-tractor-button').on('click', function () {
      var tr = $(this).closest('tr');
      var tractor = SafetyMain.getTractor(tr);
      SafetyData.getTractors([tractor.Id], function (tractors) {
        populateSingleTractor(tractors[0], tr);
      });
    });
    $('a.open-maintenance-dialog').on('click', function (e) {
      e.preventDefault();
      var tractorId = $(e.target).closest('.tractor-identification')
        .find('.tractorId').val();
      SafetyMain.openMaintenance(this, tractorId);
    });
    $('#tractor-safety-container').show();
  }

  $('#open-safety-report').text('Tractor Safety Report')
    .attr('href', 'http://transportserver/trans-svc/SafetyReports.svc/ajax/TractorSafetyReport');
  

  SafetyData.getAllTractors(function (tractors) {
    SafetyData.getLessors(function (lessors) {
      lessors.sort(function (lhs, rhs) {
        return (lhs.Name > rhs.Name) ? 1 : -1;
      });

      $.each(tractors, function (idx, tractor) {
        tractor.Lessors = [];
        $.each(lessors, function (idx, lessor) {
          var tractorLessor = $.extend({}, lessor);
          tractorLessor.Selected = (tractor.LessorOwnerName === lessor.Name ? "selected='selected'" : '');
          tractor.Lessors.push(tractorLessor);
        });
      });
      populate(tractors);
    });
  });
  populateDialog("Tractor Maintenance", "Tractor Id");

  var to = new Date();
  var from = new Date(to.getFullYear() - 1, to.getMonth(), to.getDate());
  var datesUri = 'from=' + SafetyData.getUriDate(from) + '&to=' + SafetyData.getUriDate(to);
  var fullUri = 'http://transportserver/trans-svc/SafetyReports.svc/ajax/TractorMaintenance/pending?' + datesUri;

  $('#pending-maintenance-report').show();
  $('#pending-maintenance-report').prop('href', fullUri);
  $('#driver-safety-container').hide();
  $('#trailer-safety-container').hide();
  $('#driver-track-menu').hide();
};

SafetyMain.populateTrailers = function () {
  function cleanupTrailer(trailer) {
    trailer.InspectionDue = Utilities.formatDate(JSON.parseDateOnly(trailer.InspectionDue));
    trailer.LeaseAgreementDue = Utilities.formatDate(JSON.parseDateOnly(trailer.LeaseAgreementDue));
    trailer.LastMaintenance = Utilities.formatDate(JSON.parseDateOnly(trailer.LastMaintenance));
    trailer.RegistrationExpiration = Utilities.formatDate(JSON.parseDateOnly(trailer.RegistrationExpiration));
    trailer.InsuranceExpiration = Utilities.formatDate(JSON.parseDateOnly(trailer.InsuranceExpiration));
    return trailer;
  }

  function populateSingleTrailer(trailer, tr) {
    trailer = cleanupTrailer(trailer);
    $(tr).find('.trailer-unit').text(trailer.Unit);
    $(tr).find('.maintenance-due').text(trailer.LastMaintenance);
    $(tr).find('.registration-expiration').val(trailer.RegistrationExpiration);
    $(tr).find('.inspection-due').val(trailer.InspectionDue);
    $(tr).find('.vin-number').text(trailer.VIN);
    $(tr).find('.trailer-comments').text(trailer.Comments);
    var lessorOwnerName = trailer.LessorOwnerName;
    var lessorSelect = $(tr).find('.lessor-owner');

    if (lessorOwnerName !== '') {
      $(lessorSelect).val(lessorOwnerName);
    } else {
      $(lessorSelect).val('');
    }
  }

  function populate(wcfTrailers) {
    var allTrailers = [];
    $.each(wcfTrailers, function (idx, trailer) {
      trailer = cleanupTrailer(trailer);
      allTrailers.push(trailer);
    });

    allTrailers.sort(function (lhs, rhs) {
      return (lhs.Id > rhs.Id) ? 1 : -1;
    });
    wcfTrailers = allTrailers;
    var template = $('#trailer-safety-template').html();
    var html = Mustache.to_html(template, { Trailers: wcfTrailers });
    $('#trailer-list-container').html(html);
    $('.lease-agreement').datepicker();
    $('.registration-expiration').datepicker();
    $('.inspection-due').datepicker();
    var trailerPicker = $('#trailer-picker').empty();

    $.each(allTrailers, function (idx, trailer) {
      $(trailerPicker).append($('<li>').append(
        $('<a>').attr('href', '#').text(trailer.Id + ' - ' + trailer.LessorOwnerName)
          .addClass('picked-trailer')
          .data('trailer-id', trailer.Id)));
    });
    $('.picked-trailer').on('click', function (e) {
      e.preventDefault();
      var trailerId = $(this).data('trailer-id');
      var container = $('#scrollable-trailers-container');
      $('#trailer-' + trailerId).closest('tr')[0].scrollIntoView();
      return false;
    });
    $('.save-trailer-button').on('click', function () {
      var tr = $(this).closest('tr');
      var trailer = SafetyMain.getTrailer(tr);
      SafetyData.updateTrailer(trailer, function () {
        $(tr).find('.last-saved-message').text('Saved: ' + Utilities.getTimeNow());
      });
    });
    $('.reset-trailer-button').on('click', function () {
      var tr = $(this).closest('tr');
      var trailer = SafetyMain.getTrailer(tr);
      SafetyData.getTrailers([trailer.Id], function (trailers) {
        populateSingleTrailer(trailers[0], tr);
      });
    });
    $('#trailer-safety-container').show();
  }
  $('#open-safety-report').text('Trailer Safety Report')
    .attr('href', 'http://transportserver/trans-svc/SafetyReports.svc/ajax/TrailerSafetyReport');

SafetyData.getAllTrailers(function (trailers) {
    SafetyData.getLessors(function (lessors) {
      lessors.sort(function (lhs, rhs) {
        return (lhs.Name > rhs.Name) ? 1 : -1;
      });

      $.each(trailers, function (idx, trailer) {
        trailer.Lessors = [];
        $.each(lessors, function (idx, lessor) {
          var trailerLessor = $.extend({}, lessor);
          trailerLessor.Selected = (trailer.LessorOwnerName === lessor.Name ? "selected='selected'" : '');
          trailer.Lessors.push(trailerLessor);
        });
      });
      populate(trailers);
      $('a.open-maintenance-dialog').on('click', function (e) {
        e.preventDefault();
        let trailerId = $(e.target).closest('.trailer-identification')
          .find('.trailer-id').text();
        SafetyMain.openMaintenance(this, trailerId);
      });
    });
  });
  
  populateDialog("Trailer Maintenance", "TrailerId");
  
  
  var to = new Date();
  var from = new Date(to.getFullYear() - 1, to.getMonth(), to.getDate());
  var datesUri = 'from=' + SafetyData.getUriDate(from) + '&to=' + SafetyData.getUriDate(to);
  var fullUri = 'http://transportserver/trans-svc/SafetyReports.svc/ajax/TrailerMaintenance/pending?' + datesUri;
  $('#pending-maintenance-report').prop('href', fullUri);
  
  $('#driver-safety-container').hide();
  $('#tractor-safety-container').hide();
  $('#maintenance-reports-menu').hide();
  $('#driver-track-menu').hide();
  $('#trailer-safety-container').show();
  $('#pending-maintenance-report').show();
};

SafetyData.getTractorMaintenanceTypes = function(callback) {
  var uri = SafetyConstants.getUri('tractor-maintenance/types');
  $.get(uri, function(data) {
	  callback(data.GetMaintenanceTypesResult);
  });	
};

SafetyData.appendMaintenance = function (successCallback) {
    var getFieldValue = function(fieldName){
        var ele = $('#maintenance-dialog').find('[name="' + fieldName + '"]');
        return (ele.length === 0) ? '' : $(ele).val();
    };
    let idField = '';
    let uri = '';
    
    var save = function(model) {
		var wcfModel = JSON.stringifyWcf({ maintenance: model });
  
  		var request = $.ajax({
			type: 'POST',
			contentType: 'application/json',
			data: wcfModel,
			url: uri,
			dataType: 'json'
		});
		request.done(function () {
			successCallback();
		});

		request.fail(function (jqXHR, textStatus) {
			alert('Request failed: ' + textStatus);
		});
	};
	
	var performedDateEle = $('#maintenance-dialog').find('[name="performedDate"]');
    var model = {
        "Date": performedDateEle.datepicker('getDate'),
        "Type": 'MAINT',
        "Mileage": getFieldValue('mileage') || 0,
        "Description": getFieldValue('maintenanceDescription'),
        "Amount": 0.00
    };
    
    var labelText = $('#dialog-id-label').text();
  
    if(labelText.includes('Tractor')) {
      uri = SafetyConstants.getUri('tractor-maintenance') + '/add';  
      idField = 'TractorId';
    } else if (labelText.includes('TrailerId')) {
      uri = SafetyConstants.getUri('trailer-maintenance') + '/add';  
      idField = 'TrailerId';
    }
  
    model[idField] = getFieldValue('vehicleId');
	if(!model.Type || !model.Date){
		alert("Please complete the form and try again");
		$(performedDateEle).focus();
	} else {
		save(model);
	}
};

SafetyMain.clearMaintenanceDialog = function () {
    $('#maintenance-dialog').find('input').val('');
	$('#maintenance-dialog').find('textarea').val('');
    var now = new Date();
    
    var day = ("0" + now.getDate()).slice(-2);
    var month = ("0" + (now.getMonth() + 1)).slice(-2);

    var today = now.getFullYear() + "-" + (month) + "-" + (day);
    $("input[name='performedDate']").datepicker('setDate', 'today');
};


SafetyMain.openMaintenance = function (ele, vehicleId) {
    SafetyMain.clearMaintenanceDialog();
    $('#vehicleId').val(vehicleId);
    $('.dialog-overlay').show();
    $('#maintenance-dialog').show();
    
    $('input[name="vehicleId"]')
        .val(vehicleId)
        .prop('readonly', true);
};

SafetyMain.closeMaintenance = function () {
    $('.dialog-overlay').hide();
    $('#maintenance-dialog').hide();
    SafetyMain.clearMaintenanceDialog();
};





SafetyMain.initialize = function () {
  $(function () {
    $("#safety-menu-container").buttonset()
      .change(function (e) {
        var id = $("#safety-menu-container").find('input:checked').attr('id');
        if (id === 'driver-safety-menu-option') {
          SafetyMain.populateDrivers();
        } else if (id === 'tractor-safety-menu-option') {
          SafetyMain.populateTractors();
        } else if (id === 'trailer-safety-menu-option') {
          SafetyMain.populateTrailers();
        }
      });
    SafetyMain.populateDrivers();
    

    $('a[name="cancel-maintenance"]').on('click', function (e) {
        e.preventDefault();
        SafetyMain.closeMaintenance();
    });

    $('a[name=save-maintenance]').on('click', function (e) {
        e.preventDefault();
        
        let vehicleId = $('#maintenance-dialog').find('[name="vehicleId"]').val();
        let labelText = $('#dialog-id-label').text();
        
        let tr = null;
        if(labelText.includes('Tractor')) {
          tr = $('#tractor-list-container').find('span[class="tractor-id"]:contains("' + vehicleId + '")')
            .closest('tr');
        } else if(labelText.includes('Trailer')) {
          tr = $('#trailer-list-container').find('span[class="trailer-id"]:contains("' + vehicleId + '")')
            .closest('tr');
        }
        
        SafetyData.appendMaintenance(function () {
            if(labelText.includes('Tractor')) {
              SafetyData.getTractors([vehicleId], function (tractors) {
                var lastMaintenance = Utilities.formatDate(JSON.parseDateOnly(tractors[0].LastMaintenance));
                $(tr).find('.maintenance-due').text(lastMaintenance);
                SafetyMain.closeMaintenance();
              });
            } else if(labelText.includes('Trailer')) {
              SafetyData.getTrailers([vehicleId], function (trailers) {
                var lastMaintenance = Utilities.formatDate(JSON.parseDateOnly(trailers[0].LastMaintenance));
                $(tr).find('.maintenance-due').text(lastMaintenance);
                SafetyMain.closeMaintenance();
              });
            }
        });
    });

    $('input[name="performedDate"').datepicker();
    SafetyData.getTractorMaintenanceTypes(function (data) {
        var byName = function (a, b) {
            var aName = a.Name.toLowerCase();
            var bName = b.Name.toLowerCase();
            return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
        }

        data.sort(byName);
        SafetyData.types = $.map(data, function(i){
            return {
                label: i.Name,
                'id': i.Id
            };
        });
        var labels = $.map(data, function (i) {
            return i.Name;
        });
    });
  });
};

