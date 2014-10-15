/// <reference path='Utilities.js' />

function Pager() {
  if (!Utilities) {
    alert('Utilities must be referenced to use Pager');
  }
}

Pager.validate = function (maxRows, startRowIndex, array) {
  if (!$.isArray(array)) {
    throw 'Pager: Not an array';
  } else if (array.length === 0) {
    return;
  } else if (startRowIndex >= array.length) {
    throw 'Pager: Array out of bounds';
  } 
  totalCount = array.length;
  if (!Utilities.isInt(maxRows)) {
    throw 'Pager: maxRows is not an int';
  } else if (!Utilities.isInt(startRowIndex)) {
    throw 'Pager: startRowIndex is not an int';
  } else if (maxRows < 1) {
    throw 'Pager: maxRows must be greater than zero';
  } else if (startRowIndex < 0) {
    throw 'Pager: startRowIndex must be at least zero';
  }
};

Pager.getPage = function (startRowIndex, maxRows, array) {
  Pager.validate(maxRows, startRowIndex, array);

  var i = startRowIndex;
  var arr = [];
  var lastRowIndexThisPage = Math.min(startRowIndex + maxRows, array.length - 1);
  while (i <= lastRowIndexThisPage) {
    arr.push(array[i++]);
  }
  return arr;
};

Pager.getPager = function (startRowIndex, maxRows, array) {
  Pager.validate(maxRows, startRowIndex, array);
  var lastFullPageNumber = Math.floor(array.length / maxRows);
  var itemsInLastPage = array.length % maxRows;
  var lastPageNumber = itemsInLastPage > 0 ? lastFullPageNumber + 1 : lastFullPageNumber;
  var lastStartRowIndex = 0;
  var hasFullPagesOnly = (array.length % maxRows) === 0;

  if (hasFullPagesOnly) {
    lastStartRowIndex = (array.length - maxRows);
  } else {
    lastStartRowIndex = lastFullPageNumber * maxRows;
  }

  var thisPageNumber = Math.floor(startRowIndex / maxRows) + 1;
  var nextStartRowIndex = Math.min(startRowIndex + maxRows, lastStartRowIndex);

  var startRowIndexes = {
    totalPages: lastPageNumber,
    thisPageNumber: thisPageNumber,
    buttons: {
      previous: Math.max(0, startRowIndex - maxRows),
      next: nextStartRowIndex,
      last: lastStartRowIndex
    }
  };

  return startRowIndexes;
};

