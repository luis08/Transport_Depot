$(document).ready(function () {
  Payee.clear();
  
  $('#payees-table tbody tr td a.payee-delete-button').click(function (event) {
    var deleteConfirmation = confirm("Are you sure you want to delete this payee?");
    if (deleteConfirmation == true) {
      //ValidatorEnable($('#DFIRequiredFieldValidator'), false);
      //ValidatorEnable($('#DFIIdentificationValidator'), false);
      //ValidatorEnable($('#AccountFieldValidator'), false);
      //ValidatorEnable($('#EditAccountNumberValidator'), false);
      //ValidationSummaryOnSubmit();
      return true;
    }
    return false;
  });
  $('#payees-table tbody tr td.payee-data-column').click(function () {
    Payee.editPayeeRow(this);
  });
  $('#payees-table tbody tr').hover(function () {
    $(this).css('background-color', '#ccc');
  }, function () {
    $(this).css('background-color', '#fff');
  });
  $('#clear-link').click(function () {
    Payee.clear();
  });
});



function Payee() {

  this.loadValues = function (td) {
    var row = $(td).closest('tr');
    var payee = {
      PayeeID: $(row).find('td:nth-child(1) input').val(),
      TruckwinId: $(row).find('td:nth-child(1) span').text(),
      DFIIdentification: $.trim($(row).find('td:nth-child(2)').text()),
      AccountNumber: $.trim( $(row).find('td:nth-child(3)').text() )
    };
    $('#SaveEditedPayeeLinkButton').text('Save');
    $('#PayeeEditContainer').text('Edit Lessor');
    setPayeeContainerLegend(false);
    populatePayeeControls(payee);
  }

  this.clear = function () {
    var payee = {
      PayeeID: '',
      TruckwinId: '',
      DFIIdentification: '',
      AccountNumber: ''
    };
    populatePayeeControls(payee);
    $('#SaveEditedPayeeLinkButton').text('Add');
    setPayeeContainerLegend(true);
  }

  function populatePayeeControls(payee) {
    $('#EditPayeeIdHiddenField').val(payee.PayeeID);
    $('#EditTruckWinId').val(payee.TruckwinId);
    $('#EditDFIIdentification').val(payee.DFIIdentification);
    $('#EditAccountNumber').val(payee.AccountNumber);
  }
  function setPayeeContainerLegend(isNew) {
    var newOrEdit = 'New ';
    if (!isNew) {
      newOrEdit = 'Edit ';
      setupEdit();
    } else {
      setupNew();
    }


    if ($('#LessorsRadioButton').attr('checked')) {
      $('#PayeeEditContainer').text(newOrEdit + 'Lessor');
    } else if ($('#EmployeesRadioButton').attr('checked')) {
      $('#PayeeEditContainer').text(newOrEdit + 'Employee');
    }
  }


  function setupNew() {
    $('#NewPayeesDropDown').css('display', 'inline');
    $('#EditTruckWinId').css('display', 'none');
    $('#AddInput').prop("checked", true);
    $('#SaveInput').prop("checked", false); 
  }

  function setupEdit() {
    $('#NewPayeesDropDown').css('display', 'none');
    $('#EditTruckWinId').css('display', 'inline');
    $('#AddInput').prop("checked", false);
    $('#SaveInput').prop("checked", true); 
  }
}

Payee.prototype = {
  payeeIdControl:  $('#PayeeIDHiddenField'),
  truckWinIdControl: $('#EditTruckWinId'),
  dfiControl: $('#EditDFIIdentification'),
  accountControl: $('#EditAccountNumber')
}

Payee.editPayeeRow = function (td) {
  var payee = new Payee();
  payee.loadValues(td);
}

Payee.clear = function () {
  var payee = new Payee();
  payee.clear();
}








/*

function PayeeValidator(){
}

PayeeValidator.testState = function(){
  var truckWinId = PayeeValidator.truckwinId();
  if( truckWinId === '' ){
    PayeeValidator.clear();
  } else {
    PayeeValidator.setDirty();
  }
}

PayeeValidator.truckwinId = function(){
  return $.trim($('#EditTruckWinId').text());
}

PayeeValidator.clear = function(){
  $('#SaveEditedPayeeLinkButton').css('visibility', 'hidden');
  $('#validate-truckwinId-link').css('visibility', 'hidden');
  $('#clear-link').css('visibility', 'hidden');
}

PayeeValidator.setDirty = function(){
  $('#SaveEditedPayeeLinkButton').css('visibility', 'hidden');
  $('#validate-truckwinId-link').css('visibility', 'visible');
  $('#clear-link').css('visibility', 'visible');
}

PayeeValidator.setValid = function () {
  $('#SaveEditedPayeeLinkButton').css('visibility', 'visible');
  $('#validate-truckwinId-link').css('visibility', 'hidden');
  $('#clear-link').css('visibility', 'hidden');
}

PayeeValidator.validate = function () {

  var uri = '/WebPayments/Services/ClientValidator.ashx?PayeeType=' + getType()
            + '&Id=' + PayeeValidator.truckwinId();

  $.ajax({
    url: uri,
    success: function (response) {
      setValidStatus(response);
    },
    error: function (a, b, c) { alert(b); }
  });

  function setValidStatus(response) {
    if (response === '<<NOT_FOUND>>') {
      alert("Invalid Payee");
    } else {
      $('#EditTruckWinId').val(response);
      PayeeValidator.setValid();
    }
  }

  function getType() {
    if ($('#LessorsRadioButton').prop('checked')) {
      return 'LessorId';
    } else if ($('#EmployeesRadioButton').prop('checked')) {
      return 'EmployeeId';
    }
    return '';
  }
}

*/