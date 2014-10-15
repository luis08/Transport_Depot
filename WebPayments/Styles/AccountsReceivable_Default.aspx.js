var populateCollections = function () {
  function getPostData() {
    var selection = $('input[type=radio][name=overdue-selection]:checked').val();

    var uriPrefix = 'http://netgateway/Settlements/AccountsReceivable.svc/ajax/';
    if (selection === 'overdue-by-days') {
      var threshold = parseInt($('#days-overdue-textbox').val());
      if (isNaN(threshold)) {
        threshold = 30;
      }
      $('#days-overdue-textbox').val(threshold);
      return {
        uri: uriPrefix + "GetAgingSummaryExceedingDays",
        data: { dayCount: threshold }
      };
    } else if (selection === 'overdue-all') {
      return {
        uri: uriPrefix + 'GetOverdueAgingSummary',
        data: {}
      };
    }
  }
  var postData = getPostData();

  $.ajax({
    type: "POST",
    contentType: "application/json",
    dataType: "json",
    data: JSON.stringify(postData.data),
    url: postData.uri,
    success: function (data) {
      var aging = (postData.uri.indexOf('Days') > 0) ?
            data.GetAgingSummaryExceedingDaysResult :
            data.GetOverdueAgingSummaryResult;

      aging.sort(function (lhs, rhs) {
        return (lhs.CustomerName > rhs.CustomerName) ? 1 : -1;
      });

      $.each(aging, function (idx, summary) {
        summary.OverdueBalance = Utilities.formatCurrency(summary.OverdueBalance);
        summary.Balance = Utilities.formatCurrency(summary.Balance);
      });
      var template = $('#collections-template').html();
      var html = Mustache.to_html(template, { customers: aging });
      $('#collection-data-container').html(html);
      $('#collection-customers-container table tbody tr:odd').addClass('alternating-row');
      $('.collections-data-row, .customer-selection-element').on('click', function (e) {
        e.stopPropagation();
        var checkbox = null;
        if ($(this).hasClass('customer-selection-element')) {
          return true;
        } else if ($(this).hasClass('collections-data-row')) {
          checkbox = $(this).find('.customer-selection-element');
        } else {
          return false;
        }

        if ($(checkbox).prop('checked')) {
          $(checkbox).prop('checked', false);
        } else {
          $(checkbox).prop('checked', true);
        }
      });
      $('input[type=radio][name=overdue-selection]').on('change', function () {
        var selection = $(this).val();
        if (selection === 'overdue-by-days') {
          $('.overdue-by-days').css('visibility', 'visible');
        } else if (selection === 'overdue-all') {
          $('.overdue-by-days').css('visibility', 'hidden');
        }
      });
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

$(document).ready(function () {
  populateCollections();

  function getSelectedCustomersUri() {
    var customersRequestUri = '';
    $('#collection-data-container').find('tr').has('input[type=checkbox]:checked')
          .each(function () {
            var customerId = $(this).find('.customer-id-contiainer').val();
            if (customersRequestUri === '') {
              customersRequestUri = customerId;
            } else {
              customersRequestUri = customersRequestUri + '|' + customerId;
            }
          });
    if (customersRequestUri.length === 0) {
      return "";
    }
    return customersRequestUri;
  }

  $('#print-collection-letters-link').on('click', function () {
    var customersRequestUri = getSelectedCustomersUri();
    if (customersRequestUri.length > 0) {
      var uri = 'http://netgateway/Settlements/ARReports.svc/ajax/collections?ids=' + customersRequestUri;
      $(this).attr('href', uri);
    } else {
      alert('Please select at least one customer');
      return false;
    }
  });
  $('#print-final-letters-link').on('click', function () {
    var customersRequestUri = getSelectedCustomersUri();
    if (customersRequestUri.length > 0) {
      var uri = 'http://netgateway/Settlements/ARReports.svc/ajax/final-letters?ids=' + customersRequestUri;
      $(this).attr('href', uri);
    } else {
      alert('Please select at least one customer');
      return false;
    }
  });

  $('#refresh-collections-link').on('click', function () {
    populateCollections();
  });
  $('#print-collections-report').on('click', function () {
    var type = $('#').val();
    var sort = $('#').val();
    var uri = 'http://netgateway/Settlements/ARReports.svc/ajax/collections-report?type='
      + type + '&sort' + sort;
    alert('setting uri : ' + uri);
    $(this).attr('href', uri);
  });
});