/// <reference path="../jquery-1.10.2.js" />
/// <reference path="../Utilities.js" />


function Report() {
}

Report.modes = {
  allDispatchers: 0,
  singleDispatcher: 1
};

Report.getMode = function () {
  
  var checked = $('input[name=commission-source]:checked').val();
  if (checked === 'all') {
    return Report.modes.allDispatchers;
  } else if (checked === 'specific') {
    return Report.modes.singleDispatcher;
  } else {
    throw 'Invalid dispatcher selection';
  }
};

Report.getDate = function () {
  var date = $('#paydate-selector').val();
  return date;
};

Report.getDispatcher = function () {
  var mode = Report.getMode();

  if (mode === Report.modes.allDispatchers) {
    return '';
  }
  var dispatcher = $('#dispatcher-selector').val();
  return dispatcher;
};

Report.showDispatchers = function () {
  function populateDispatchers(dispatchers) {
    var mode = Report.getMode();
    if (mode === Report.modes.allDispatchers) {
      $('#dispatcher-menu').hide();
      return;
    }  
    var dropdown = $('#dispatcher-selector').empty();

    $.each(dispatchers, function (idx, d) {
      var option = $('<option/>').val(d.VendorId).text(d.Name);
      if( idx === 0 ){
        $(option).attr('selected', 'selected');
      }
      $(option).appendTo(dropdown);
    });
  }
  
  

  var dispatcherData = $('#dispatcher-selector').data('dispatchers');
  $('#dispatcher-menu').show();
  if( ! dispatcherData ){
    var uri = 'http://netgateway/settlements/Dispatch.svc/GetDispatchers';
    $.ajax({
      type: "POST",
      contentType: "application/json",
      url: uri,
      success: function (data) {
        dispatcherData = data.GetDispatchersResult;
        $('#dispatcher-selector').data('dispatchers', dispatcherData);
        populateDispatchers(dispatcherData);
      },
      error: function (xhr) {
        try {
          var json = $.parseJSON(xhr.responseText);
          alert(json.errorMessage);
        } catch (e) {
          alert('something bad happened');
        }
      }
    }); 
  } else {
    populateDispatchers(dispatcherData);
  }
};

Report.showDates = function () {

  function populateDates(dates) {
    var d1= Utilities.formatDate(dates[0]);
    
    dates = $.map( dates, function(d, i){
      return {
        dateValue: JSON.parseDateOnly(d),
        date: Utilities.formatDate(JSON.parseDateOnly(d)),
        wcfDate : d
      };
    });
    dates.sort( function( lhs, rhs ){
      return lhs.dateValue > rhs.dateValue ? -1: 1;
    });
    var dropdown = $('#paydate-selector').empty();

    $.each(dates, function (idx, d) {
      var option = $('<option/>').val(d.wcfDate).text(d.date);
      if( idx === 0 ){
        $(option).attr('selected', 'selected');
      }
      $(option).appendTo(dropdown);
    });
    Report.render();
  }

  var mode = Report.getMode();

  if (mode === Report.modes.allDispatchers) {
    $.ajax({
      type: "POST",
      contentType: "application/json",
      url: "http://netgateway/settlements/Dispatch.svc/GetAllCommissionDates",
      success: function (data) {
        populateDates(data.GetAllCommissionDatesResult);
      },
      error: function (xhr) {
        try {
          var json = $.parseJSON(xhr.responseText);
          alert(json.errorMessage);
        } catch (e) {
          alert('something bad happened');
        }
      }
    });
  } else if (mode === Report.modes.singleDispatcher) {
    var dispatcher = Report.getDispatcher();
    var model = JSON.stringify({ 'dispatcherId': dispatcher });
    $.ajax({
      type: "POST",
      contentType: "application/json",
      dataType: "json",
      data: model,
      url: "http://netgateway/settlements/Dispatch.svc/GetCommissionDates",
      success: function (data) {
        var dispatcherDates = data.GetCommissionDatesResult;
        populateDates(dispatcherDates);
      },
      error: function (xhr) {
        try {
          var json = $.parseJSON(xhr.responseText);
          alert(json.errorMessage);
        } catch (e) {
          alert('something bad happened');
        }
      }
    });
  }
};

Report.showDispatches = function () {
  var mode = Report.getMode();
  if (mode === Report.modes.allDispatchers) {

  } else if (mode === Report.modes.singleDispatcher) {
    $('#dispatcher-menu').show();
  }
};

Report.render = function () {
  function populate(commissions) {

    var commissionTotal = 0.0;
    var loadCount = commissions.length;

    $.each(commissions, function (idx, c) {
      commissionTotal += c.CommissionTotal;
      var invDate = JSON.parseDateOnly(c.BillDate);
      c.InvoiceDate = Utilities.formatDate(invDate);
      c.CommissionTotal = Utilities.formatCurrency(c.CommissionTotal);
      c.InvoiceAmount = Utilities.formatCurrency(c.InvoiceAmount);
    });

    var commissionContainer = $('#commission-details')
    $(commissionContainer).find('.commission-detail').remove();
    var template = $('#commission-report-template').html();
    var html = Mustache.to_html(template, { commissions: commissions });
    $(commissionContainer).html(html);
    $( commissionContainer ).find('li:odd div').addClass('alternating-item');
    commissionTotal = Utilities.formatCurrency(commissionTotal);
    $('#load-count').text(loadCount);
    $('#commission-total').text(commissionTotal);
  }
  var model = {
    request: {
      DispatcherId: Report.getDispatcher(),
      CommissionPaymentDate: Report.getDate()
    }
  };

  $.ajax({
    type: "POST",
    contentType: "application/json",
    dataType: "json",
    data: JSON.stringifyWcf(model),
    url: "http://netgateway/settlements/Dispatch.svc/GetCommissions",
    success: function (data) {
      var commissions = data.GetCommissionsResult;
      populate(commissions);
    },
    error: function (xhr) {
      try {
        var json = $.parseJSON(xhr.responseText);
        alert(json.errorMessage);
      } catch (e) {
        alert('something bad happened');
      }
    }
  });
};



$(document).ready(function () {
  var b = $("body");
  $(document).on({
    
    ajaxStart: function() { $(b).addClass("loading-in-progress");    },
    ajaxStop:  function() { $(b).removeClass("loading-in-progress"); }    
  });  
  Report.showDispatchers();
  Report.showDates();

  $('input[name=commission-source]').on('change', function () {
    Report.showDispatchers();
    Report.showDates();
  });
  $('#show-report').on('click', function (e) {
    e.preventDefault();
    Report.render();
  });
  $('#paydate-selector').on('change', function () {
    Report.render();
  });
  $('#dispatcher-selector').on('change', function () {
    Report.showDates();
  });
});