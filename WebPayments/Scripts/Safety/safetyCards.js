/*


var card = {
  title: '',
  items: [
    {
    label: '',
    value: ''
    },
    {
    label: '',
    value: ''
    },
    {
    label: '',
    value: ''
    },
  ]
};
*/
const TRACTORS_URI  = '/trans-svc/Safety.svc/ajax/GetTractors';

class SafetyCardsController {
  constructor($http) {
    this.cards = [];
    this._$http = $http;
    this._refresh();
  }

  _refresh() {
    this._$http.post(TRACTORS_URI, {activeOnly: true}).then((response) => {
      this.cards = response.data.GetTractorsResult;
    });
    
  }
}

let safetyCardsApp = angular.module('safetyCardsApp', []);
safetyCardsApp.controller('safetyCardsController', SafetyCardsController)
safetyCardsApp.component('safetyCards', {
  templateUrl: '../Scripts/Safety/templates/safetyCards.html',
  controller: SafetyCardsController
});