/// <reference path="Pager.js" />/// <reference path="JSON.NET.js" />
/// <reference path="Utilities.js" />

function ScheduleConstants() { }

ScheduleConstants.RowsPerPage = 12;
ScheduleConstants.ShowDelay = 300;
ScheduleConstants.HideDelay = 300;
ScheduleConstants.updateLabel = "Update Schedule";
ScheduleConstants.saveNewScheduleLabel = "Save New Schedule";
ScheduleConstants.getUri = function (method) {
  var uri = "http://" + window.location.host + "/ft-svc/Factoring.svc/ajax/" + method;
  return uri;
};

function ScheduleData() { }

ScheduleData.getData = function () {
  var data = $('#invoice-list').data('invoices-data');
  if (data === undefined) {
    return null;
  }
  return data;
};

ScheduleData.setData = function (data) {
  $('#invoice-list').data('invoices-data', data);
};


ScheduleData.parseInvoice = function (invoice) {
  invoice.Date = Utilities.formatDate(new Date(JSON.parseDateOnly(invoice.Date)));
  invoice.Amount = Utilities.formatCurrency(invoice.Amount);
  return invoice;
};

ScheduleData.getSchedule = function (scheduleId, successCallback) {

  function retrieveSchedule(id) {
    var uri = ScheduleConstants.getUri("GetSchedule");
    var request = $.ajax({
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify({ id: scheduleId }),
      url: uri,
      dataType: "json",
      success: function (result) {
        var schedule = result.GetScheduleResult;
        schedule.total = 0.0;
        $.each(schedule.Invoices, function (idx, invoice) {
          schedule.total += parseFloat(invoice.Amount);
          invoice.index = idx;
          invoice.checked = true;
          invoice = ScheduleData.parseInvoice(invoice);
        });

        var pageData = {
          id: $.trim(id),
          date: Utilities.formatDate(new Date(JSON.parseDateOnly(schedule.Date))),
          startRowIndex: 0,
          maxRows: ScheduleConstants.RowsPerPage,
          total: schedule.total,
          data: { invoices: schedule.Invoices }
        };
        ScheduleData.setData(pageData);

        successCallback();

      }, error: function (jqXHR, error, errorThrown) {
        alert(errorThrown);
      }
    });
  }
  ScheduleData.setData(null);
  var schedule = ScheduleData.getData();

  if (schedule === undefined) {
    retrieveSchedule(scheduleId);
  } else if (schedule === null) {
    retrieveSchedule(scheduleId);
  } else if (schedule.id !== scheduleId) {
    retrieveSchedule(scheduleId);
  } else {
    successCallback();
  }
};

ScheduleData.getInvoices = function (successCallback) {
  var today = new Date();

  var defaultFromInvoice = '0',
      defaultToinvoice = 'ZZZZZZZ',
      defaultFromDate = new Date(),
      defaultToDate = new Date();

  defaultFromDate.setDate(today.getDate() - 3650);
  defaultToDate.setDate(today.getDate() + 3650);

  var useInvoiceDate = $('#use-invoice-date-filter').prop('checked');
  var useInvoiceNumber = $('#use-invoice-number-filter').prop('checked');

  var fromDate = useInvoiceDate ? $('#invoice-from-date-textbox').datepicker('getDate') : defaultFromDate;
  var toDate = useInvoiceDate ? $('#invoice-to-date-textbox').datepicker('getDate') : defaultToDate;
  var fromInvoice = useInvoiceNumber ? $('#from-invoice-number-textbox').val() : defaultFromInvoice;
  var toInvoice = useInvoiceNumber ? $('#to-invoice-number-textbox').val() : defaultToinvoice;
  var onlyNoSchedule = $('#only-no-schedule-invoices-filter').prop('checked');
  var testFilter = {
    FromInvoiceNumber: fromInvoice,
    ToInvoiceNumber: toInvoice,
    FromDate: fromDate,
    ToDate: toDate,
    OnlyWithoutSchedule: onlyNoSchedule,
    RowsPerPage: 200,
    PageNumber: 1
  };

  var wcfFilter = JSON.stringifyWcf({ filter: testFilter });

  var uri = ScheduleConstants.getUri("GetInvoices");

  var request = $.ajax({
    type: "POST",
    contentType: "application/json",
    data: wcfFilter,
    url: uri,
    dataType: "json",
    success: function (result) {
      successCallback(result.GetInvoicesResult);
    }, error: function (jqXHR, error, errorThrown) {
      alert(errorThrown);
    }
  });
};

ScheduleData.update = function (onUpdateSucceeded) {
  var currentData = ScheduleData.getData();

  var invoices = [];
  var invoiceNumbers = [];
  $.each(currentData.data.invoices, function (idx, invoice) {
    if (invoice.checked) {
      invoices.push({
        Number: invoice.Number
      });
      invoiceNumbers.push(invoice.Number);
    }
  });

  var scheduleViewModel = {};
  if (SchedulePopup.isInEditMode()) {
    scheduleViewModel = {
      Id: currentData.id,
      Date: new Date(currentData.date),
      Invoices: invoices
    };
  } else if (SchedulePopup.isInNewMode()) {
    scheduleViewModel = {
      Date: new Date(currentData.date),
      InvoiceNumbers: invoiceNumbers
    };
  } else {
    throw 'Invalid state: not edit, not new';
  }

  var wcfSchedule = JSON.stringifyWcf({ schedule: scheduleViewModel });
  var uri = '';
  if (SchedulePopup.isInEditMode()) {
    uri = ScheduleConstants.getUri('SaveSchedule');
  } else if (SchedulePopup.isInNewMode()) {
    uri = ScheduleConstants.getUri('CreateSchedule');
  }

  var request = $.ajax({
    type: "POST",
    contentType: "application/json",
    data: wcfSchedule,
    url: uri,
    dataType: "json",
    success: function (result) {
      onUpdateSucceeded();
    }, error: function (jqXHR, error, errorThrown) {
      alert(errorThrown);
    }
  });

};

function InvoicePopup() { }
InvoicePopup.overlayId = 'invoice-popup-overlay';

InvoicePopup.show = function () {
  var overlayHeight = $(document).height();
  var overlayWidth = $(window).width();

  var dialogTop = 40;
  var dialogLeft = (overlayWidth / 2) - ($('#invoice-filter-dialog').width() / 2);
  var invoiceOverlay = $('#' + InvoicePopup.overlayId);

  if (invoiceOverlay.length === 0) {
    invoiceOverlay = $('<div>').attr('id', InvoicePopup.overlayId).addClass('dialog-overlay-level-2')
        .css({ height: overlayHeight, width: overlayWidth, top: 0, left: 0 });
    $(document.body).append(invoiceOverlay);
  }
  $(invoiceOverlay).fadeIn(ScheduleConstants.ShowDelay);
  $('#invoice-filter-dialog').css({ top: dialogTop, left: dialogLeft }).fadeIn(ScheduleConstants.ShowDelay);
};

InvoicePopup.close = function () {
  $('#' + InvoicePopup.overlayId).fadeOut(ScheduleConstants.HideDelay);
  $('#invoice-filter-dialog').fadeOut(ScheduleConstants.HideDelay);
};


function SchedulePopup() { }

SchedulePopup.show = function () {
  var overlayHeight = $(document).height();
  var overlayWidth = $(window).width();

  var dialogTop = 40;
  var dialogLeft = (overlayWidth / 2) - ($('#schedule-dialog').width() / 2);

  $('#dialog-overlay').css({ height: overlayHeight, width: overlayWidth, top: 0, left: 0 }).fadeIn(ScheduleConstants.ShowDelay);
  $('#schedule-dialog').css({ top: dialogTop, left: dialogLeft }).fadeIn(ScheduleConstants.ShowDelay);
};

SchedulePopup.isInEditMode = function () {
  if( $('#update-schedule-menu-item').text()=== ScheduleConstants.updateLabel ){
    return true;
  }
  return false;
};

SchedulePopup.isInNewMode = function () {
  if ($('#update-schedule-menu-item').text() === ScheduleConstants.saveNewScheduleLabel) {
    return true;
  }
  return false; 
};

SchedulePopup.setEditMode = function () {
  $('#update-schedule-menu-item').text(ScheduleConstants.updateLabel);
};

SchedulePopup.setNewMode = function () {
  ScheduleData.setData(null);
  $('#schedule-date-textbox').val(Utilities.formatDate(new Date()));
  $('#schedule-number-textbox').val('[New]');
  $('#update-schedule-menu-item').text(ScheduleConstants.saveNewScheduleLabel);
  $('#invoice-list-container').html('');
};


SchedulePopup.clear = function () {
  $('#schedule-date-textbox').val('');
  $('#schedule-number-textbox').val('');
  $('#schedule-total-textbox').val('');
  $('#invoice-list-container').html('');
  $('#view-schedule-pdf-link').attr('href', '#')
    .removeAttr('target');
};

SchedulePopup.close = function () {
  $('#dialog-overlay').fadeOut(ScheduleConstants.HideDelay);
  $('#schedule-dialog').fadeOut(ScheduleConstants.HideDelay);
  SchedulePopup.clear();
};


SchedulePopup.setRowColors = function () {
  $('#invoice-list-container').find('tr:odd').removeClass('alternating-row');
  $('#invoice-list-container').find('tr:even').addClass('alternating-row');
};

SchedulePopup.refreshTotal = function () {
  var data = ScheduleData.getData();
  var total = 0.0;
  $.each(data.data.invoices, function () {
    if (this.checked) {
      total += Utilities.parseCurrency(this.Amount);
    }
  });

  data.total = total;
  $('#schedule-total-textbox').val(Utilities.formatCurrency(total));
  ScheduleData.setData(data);
};


SchedulePopup.renderInvoices = function (startRowIndex, updateChecked) {
  var actualData = ScheduleData.getData();
  var template = $('#invoice-template').html();
  var startRowIndexes = Pager.getPager(startRowIndex, actualData.maxRows, actualData.data.invoices);

  var pagerButtons = [
      {
        text: 'First',
        startRowIndex: 0,
        hide: false //startRowIndexes.buttons.previous == 0
      },
      {
        text: 'Prev',
        startRowIndex: startRowIndexes.buttons.previous,
        hide: false //startRowIndexes.buttons.previous == startRowIndex
      },
      {
        text: 'Next',
        startRowIndex: startRowIndexes.buttons.next,
        hide: false //startRowIndexes.buttons.next == startRowIndex
      },
      {
        text: 'Last',
        startRowIndex: startRowIndexes.buttons.last,
        hide: false //startRowIndexes.next == startRowIndex
      }
    ];

  var toRender = {
    invoices: Pager.getPage(startRowIndex, actualData.maxRows, actualData.data.invoices),
    pagerButtons: pagerButtons,
    totalPages: startRowIndexes.totalPages,
    thisPageNumber: startRowIndexes.thisPageNumber
  };

  var html = Mustache.to_html(template, toRender);
  $('#invoice-list-container').html(html);
  SchedulePopup.setRowColors();

  $('.invoice-row, .invoice-selector').on('click', function (e) {
    var currentData = ScheduleData.getData();
    var ele = this;
    e.stopPropagation();
    if ($(this).hasClass('invoice-selector')) {
      ele = $(this).closest('.invoice-row');
      $(this).prop('checked', !$(this).prop('checked'));
    }
    var idx = $(ele).find('input[type=hidden]').val();
    var checkbox = $(ele).find('.invoice-selector');
    var invoiceNumber = currentData.data.invoices[idx].Number;
    var isChecked = false;
    if ($(checkbox).prop('checked')) {
      isChecked = false;
      $(checkbox).prop('checked', false);
    } else {
      isChecked = true;
      $(checkbox).prop('checked', true);
    }

    currentData.data.invoices[idx].checked = isChecked;
    SchedulePopup.refreshTotal();
  });
};

SchedulePopup.renderSchedule = function (startRowIndex, updateChecked) {
  SchedulePopup.renderInvoices(startRowIndex, updateChecked);
  var actualData = ScheduleData.getData();

  $('#schedule-date-textbox').val(actualData.date);
  $('#schedule-date-textbox').datepicker();
  $('#schedule-number-textbox').val(actualData.id);
  $('#schedule-total-textbox').val(Utilities.formatCurrency(actualData.total));
  $('#view-schedule-pdf-link').attr('href', 'http://192.168.1.200/ft-svc/FactoringPdf.svc/pdf?id=' + actualData.id)
    .attr('target', '_blank')
    .attr('type', 'application/pdf');
};

SchedulePopup.removeUnselected = function () {
  var data = ScheduleData.getData();
  var total = 0.0;
  var newInvoices = [];
  $.each(data.data.invoices, function () {
    if (this.checked) {
      this.index = newInvoices.length;
      newInvoices.push(this);
    }
  });

  data.data.invoices = newInvoices;
  ScheduleData.setData(data);
  SchedulePopup.renderInvoices(data.startRowIndex, false);
};

$(document).ready(function () {

  function setDefaultRangeValues() {
    $('#FromDateInput').datepicker();
    $('#ToDateInput').datepicker();
  }

  function setApplyFiltersVisibility() {
    if (!$('#date-range-container').hasClass('hidden-element')) {
      $('#ApplyFiltersButton').removeClass('hidden-element');
    } else if (!$('#schedule-range-container').hasClass('hidden-element')) {
      $('#ApplyFiltersButton').removeClass('hidden-element');
    } else {
      $('#ApplyFiltersButton').addClass('hidden-element');
    }
  }

  function setDateFilterVisibility(ele) {
    if ($(ele).prop('checked')) {
      $('#date-range-container').removeClass('hidden-element');
    } else {
      $('#date-range-container').addClass('hidden-element');
    }
    setApplyFiltersVisibility();
  }

  function setScheduleFilterVisibility(ele) {
    if ($(ele).prop('checked')) {
      $('#schedule-range-container').removeClass('hidden-element');
    } else {
      $('#schedule-range-container').addClass('hidden-element');
    }
    setApplyFiltersVisibility();
  }

  setDefaultRangeValues();
  setDateFilterVisibility($('#UseDatesCheckbox'));
  setScheduleFilterVisibility($('#UseScheduleNumberCheckbox'));

  $('#UseDatesCheckbox').on('click', function () {
    setDateFilterVisibility(this);
  });
  $('#UseScheduleNumberCheckbox').on('click', function () {
    setScheduleFilterVisibility(this);
  });

  $('.open-schedule-link').on('click', function () {
    function renderInvoices() {
      SchedulePopup.renderSchedule(0, false);
    }
    SchedulePopup.setEditMode();
    var scheduleId = $(this).text();

    ScheduleData.getSchedule(scheduleId, renderInvoices);
    SchedulePopup.show();
    $('#invoice-list-container').html($('<td colspan="5" style="text-align:center;"><br/><br/>Loading...<br/><br/><br/></td>'));
  });
  $('#new-schedule-menu-item').on('click', function () {
    SchedulePopup.setNewMode();
    SchedulePopup.show();
    return false;
  });
  $('#close-schedule-link').on('click', function () {
    SchedulePopup.close();
    return false;
  });
  $('#import-invoices-menu-item').on('click', function () {
    InvoicePopup.show();
    return false;
  });
  $('#close-invoice-filter-dialog-button').on('click', function () {
    InvoicePopup.close();
    return false;
  });

  $('#import-invoices-dialog-button').on('click', function () {

    function onInvoicesRead(result) {

      var currentPageData = ScheduleData.getData();
      var invoiceNums = [];
      var allInvoices = {};
      var total = 0.0;
      var scheduleId = '[New]';
      var scheduleDate = Date();
      var startRowIndex = 0;
      var maxRows = ScheduleConstants.RowsPerPage;

      if (currentPageData !== null) {
        $.each(currentPageData.data.invoices, function () {
          invoiceNums.push(this.Number);
          allInvoices[this.Number] = this;
        });
        scheduleId = currentPageData.id;
        scheduleDate = currentPageData.date;
        maxRows = currentPageData.maxRows;
        total = currentPageData.total;
        startRowIndex = currentPageData.startRowIndex;
      }

      $.each(result.Invoices, function () {
        if (!allInvoices[this.Number]) {
          var invoice = ScheduleData.parseInvoice(this);
          invoiceNums.push(this.Number);
          allInvoices[this.Number] = invoice;
        }
      });

      invoiceNums.sort();

      var newInvoices = [];
      $.each(invoiceNums, function (idx, invoiceNumber) {
        allInvoices[invoiceNumber].index = idx;
        newInvoices.push(allInvoices[invoiceNumber]);
      });
      var pageData = {
        id: scheduleId,
        date: scheduleDate,
        startRowIndex: startRowIndex,
        maxRows: maxRows,
        total: total,
        data: { invoices: newInvoices }
      };

      ScheduleData.setData(pageData);
      InvoicePopup.close();

      SchedulePopup.renderInvoices(pageData.startRowIndex, false);
    }

    ScheduleData.getInvoices(onInvoicesRead);
    return false;
  });
  $('#use-invoice-date-filter, #use-invoice-number-filter').on('click', function () {
    var showFilter = $(this).prop('checked');
    var element = null;
    if ($(this).attr('id') === 'use-invoice-date-filter') {
      element = $('#invoice-date-filter');
    } else if ($(this).attr('id') === 'use-invoice-number-filter') {
      element = $('#invoice-number-filter');
    }
    if (showFilter) {
      $(element).fadeIn(ScheduleConstants.ShowDelay);
    } else {
      $(element).fadeOut(ScheduleConstants.HideDelay);
    }
  });
  $('#remove-unselected-menu-item').on('click', function () {
    SchedulePopup.removeUnselected();
    return false;
  });
  $('#invoice-from-date-textbox').datepicker();
  $('#invoice-to-date-textbox').datepicker();
  $('#update-schedule-menu-item').on('click', function () {
    function updateSucceeded() {
      SchedulePopup.close();
      __doPostBack('update-schedule-menu-item', '');
    }
    try {
      ScheduleData.update(updateSucceeded);
    } catch (e) {
      alert(e);
    }
  });
});                                                                                                                                             //$(document).ready
