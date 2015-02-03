
function parseWcfDate(wcfDate) {
  var d = wcfDate.match(/(\d+)/)[1];
  return new Date(+d);
}
var app = angular.module('apexUpload', [])
      .controller('uploader', ['$scope', '$http',
        function ($scope, $http) {
          function getSelectedPayments() {

            var selectedPayments = [];

            angular.forEach($scope.payments, function (payment, idx) {
              if (!payment.exclude) {
                var p = jQuery.extend(true, {}, payment);
                p.EffectiveDate = payment.OriginalDate;
                delete p.OriginalDate;
                delete p.exclude;
                selectedPayments.push(p);
              }
            });
            return selectedPayments;
          }
          $scope.payments = [];

          $scope.fileChanged = function (elem) {
            $scope.path = elem.value;
            $scope.file = elem.files[0];
            $scope.$apply();
          };

          $scope.render = function (data) {
            $scope.payments = data;
            $scope.$apply();
          };

          $scope.upload = function (event) {

            //var uri = 'http://localhost:22768/Apex.svc/?paymentsCsv=' + $scope.file.name;
            //var uri = 'http://localhost/td/Apex.svc/?paymentsCsv=' + $scope.file.name;
            var uri = 'http://netgateway/settlements/Apex.svc/?paymentsCsv=' + $scope.file.name;

            event.preventDefault();
            var ajaxRequest = $.ajax({
              type: "POST",
              url: uri,
              contentType: false,
              processData: false,
              data: $scope.file
            });

            ajaxRequest.success(function (data) {
              var json = data.ReadCsvResult;

              angular.forEach(json, function (value, idx) {
                value.exclude = false;
                value.OriginalDate = value.EffectiveDate;
                value.EffectiveDate = parseWcfDate(value.EffectiveDate);
              });

              $scope.render(json);
            });

            ajaxRequest.fail(function () {
              alert('error');
            });
          };
          $scope.save = function (event) {
            event.preventDefault();
            var selectedPayments = getSelectedPayments();
            if (selectedPayments.length === 0) {
              alert('No payments to save');
              return;
            }
            var uri = 'http://netgateway/settlements/Apex.svc/GetExistingPayments';
            //var uri = 'http://localhost/td/Apex.svc/SavePayments';
            var model = { 'payments': selectedPayments };
            var wcfModel = JSON.stringifyWcf(model);
            var ajaxRequest = $.ajax({
              type: "POST",
              url: uri,
              contentType: 'application/json; charset=utf-8',
              dataType: 'json',
              data: wcfModel
            });
            ajaxRequest.success(function (data) {
              var existingInvoices = data.GetExistingPaymentsResult;
              if (existingInvoices.length > 0) {
                existingInvoices.sort();
                var dupes = existingInvoices.join('\n');
                alert('The following ' + existingInvoices.length + ' invoice(s) cannot be saved. Either: \n-There is a payment for that invoice\n-There is no such invoice, or \n-Invoice amounts do not match. \n\nThey were all excluded (checked): \n' + dupes);
                angular.forEach(existingInvoices, function (dupe, idx) {
                  angular.forEach($scope.payments, function (payment, idx) {
                    if (payment.InvoiceNumber === dupe) {
                      payment.exclude = true;
                    }
                  });
                });
                $scope.$apply();
              } else {
                uri = 'http://netgateway/settlements/Apex.svc/SavePayments';
                var saveAjaxRequest = $.ajax({
                  type: "POST",
                  url: uri,
                  contentType: 'application/json; charset=utf-8',
                  dataType: 'json',
                  data: wcfModel
                });
                saveAjaxRequest.success(function () {
                  var removedInvoices = [];
                  angular.forEach(selectedPayments, function (paid, idx) {
                    angular.forEach($scope.payments, function (payment, paymentIdx) {
                      if (payment.InvoiceNumber === paid.InvoiceNumber) {
                        var removedInvoice = $scope.payments.splice(paymentIdx, 1 /*how many */)[0];
                        removedInvoices.push(removedInvoice.InvoiceNumber);
                      }
                    });
                  });
                  var removedInvoiceString = removedInvoices.join('\n- ');
                  $scope.$apply();
                  alert('The following saved payments were removed from the list.\n- ' + removedInvoiceString);

                });

                saveAjaxRequest.fail(function (jqXHR, textStatus, errorThrown) {
                  alert(errorThrown);
                });
              }

            });

            ajaxRequest.fail(function () {
              alert('error');
            });
          };
        }
      ]);
app.directive('noclick', [function () {
  return {
    restrict: 'A',
    link: function (scope, element, attrs) {
      element.bind('click', function (e) {
        e.stopPropagation();
      });
    }
  };
} ]);
app.directive('clickbox', [function () {
  return {
    restrict: 'A',
    link: function (scope, element, attrs) {
      element.bind('click', function (e) {
        e.stopPropagation();
        /*
        var chbox = $(e.target).closest('li').find('d');
        $(chbox).prop("checked", !$(chbox).prop("checked"));
        */
        var invoiceNumber = $(element).closest('li').find('.invoice').text();
        angular.forEach(scope.payments, function (payment, idx) {
          
          if( payment.InvoiceNumber === invoiceNumber ){
            payment.exclude = ( ! payment.exclude );
          }
        });
        scope.$apply();
      });
      
    }
  };
} ]);