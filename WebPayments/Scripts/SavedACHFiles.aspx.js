/// <reference path="jquery-1.10.2.js" />

function PostingAjax() { }

PostingAjax.test = false;



PostingAjax.Uri = {
  POST: 'Post',
  IGNORE: 'IgnoreCheck',
  KEEP: 'DontIgnoreCheck',
  DEFAULT_PAYMENT_ACCOUNTS: 'GetDefaultPaymentAccounts',
  GET_CHECK_EXISTENCE_MODEL: 'GetCheckExistenceModels',
  getHost: function () {
    if (!PostingAjax.test) {
      return location.protocol + '/Settlements';
    } else {
      return "http://localhost:22768";
    }
  }
};
PostingAjax.getServiceUri = function (type) {
  function getUri(methodName) {
    var uri = PostingAjax.Uri.getHost() + "/Posting.svc/" + methodName;
    return uri;
  }

  if (type === PostingAjax.Uri.POST) {
    return getUri(PostingAjax.Uri.POST);
  } else if (type === PostingAjax.Uri.IGNORE) {
    return getUri(PostingAjax.Uri.IGNORE);
  } else if (type === PostingAjax.Uri.KEEP) {
    return getUri(PostingAjax.Uri.KEEP);
  } else if (type === PostingAjax.Uri.DEFAULT_PAYMENT_ACCOUNTS) {
    return getUri(PostingAjax.Uri.DEFAULT_PAYMENT_ACCOUNTS);
  } else if (type === PostingAjax.Uri.GET_CHECK_EXISTENCE_MODEL) {
    return getUri(PostingAjax.Uri.GET_CHECK_EXISTENCE_MODEL);
  } else {
    throw 'Invalid Service Type: ' + type;
  }
};
PostingAjax.get = function (type, successCallback, errorHandler) {
  jQuery.support.cors = true;
  var uri = null;
  try {
    uri = PostingAjax.getServiceUri(type);
  } catch (e) {
    alert(e);
    return;
  }
  $.ajax({
    url: uri,
    type: "POST",
    contentType: "application/json; charset=utf-8", /* To Server */
    success: function (data) {
      successCallback(data);
    },
    dataType: 'json', /* Received from server */
    error: function (XMLHttpRequest, textStatus, errorThrown) {
      alert('textStatus = ' + textStatus + '/nerrorThrown = ' + errorThrown);
    }
  });
};

PostingAjax.query = function (type, model, successCallback, errorHandler) {
  var uri = null;
  var modelClone = null;
  var dataToSend = null;
  try {
    uri = PostingAjax.getServiceUri(type);
    modelClone = $.extend({}, model);
    dataToSend = JSON.stringifyWcf(modelClone);
  }
  catch (e) {
    alert(e);
    return;
  }
  jQuery.support.cors = true;
  $.ajax({
    url: uri,
    type: "POST",
    contentType: "application/json; charset=utf-8", /* To Server */
    data: dataToSend, /* Data sending to server */
    dataType: 'json', /* Received from server */
    success: function (data) {
      /* return data from server */
      successCallback(data);
    },
    error: function (XMLHttpRequest, textStatus, errorThrown) {
      alert('textStatus = ' + textStatus + '/nerrorThrown = ' + errorThrown);
      errorHandler();
    }
  });
};

PostingAjax.command = function (type, model, successCallback, errorHandler) {
  var uri = null;
  var modelClone = null;
  var dataToSend = null;
  try {
    uri = PostingAjax.getServiceUri(type);
    modelClone = $.extend({}, model);
    dataToSend = JSON.stringifyWcf(modelClone);
  }
  catch (e) {
    alert(e);
    return;
  }

  $.ajax({
    url: uri,
    type: "POST",
    contentType: "application/json; charset=utf-8", /* To Server */
    data: dataToSend,
    success: function () {
      successCallback();
    },
    error: function (XMLHttpRequest, textStatus, errorThrown) {
      alert('textStatus = ' + textStatus + '/nerrorThrown = ' + errorThrown);
      errorHandler();
    }
  });
};


PostingAjax.ignore = function (model, successCallback, errorHandler) {
  function getCheckExistenceModel(modelFromServer) {
    successCallback(modelFromServer);
  }
  function callQuery() {
    var models = [];
    var correctModel = {
      FileDateTime: new Date( JSON.parseDateOnly(model.FileDateTime) ),
      FileIDModifier: model.FileIDModifier
    };
    models.push(correctModel);
    PostingAjax.query(PostingAjax.Uri.GET_CHECK_EXISTENCE_MODEL,
      { 'models': models },
        getCheckExistenceModel, errorHandler);
  }

  PostingAjax.command(PostingAjax.Uri.IGNORE,
      { 'model': model },
      callQuery,
      errorHandler);
};

PostingAjax.keep = function (model, successCallback, errorHandler) {
  function getCheckExistenceModel(modelFromServer) {
    successCallback(modelFromServer);
  }
  function callQuery() {
    var models = [];
    var correctModel = {
      FileDateTime: new Date(JSON.parseDateOnly(model.FileDateTime)),
      FileIDModifier: model.FileIDModifier
    };
    models.push(correctModel);
    PostingAjax.query(PostingAjax.Uri.GET_CHECK_EXISTENCE_MODEL,
      { 'models': models },
        getCheckExistenceModel, errorHandler);
  }

  PostingAjax.command(PostingAjax.Uri.KEEP,
      { 'model': model },
      callQuery,
      errorHandler);
};

PostingAjax.post = function (model, successCallback, errorHandler, undoHandler) {
  var correctModel = {
    FileDateTime: new Date(JSON.parseDateOnly(model.FileDateTime)),
    FileIDModifier: model.FileIDModifier
  };
  var userAgreesToPost = confirm("Confirm Post for:\n  File Date: "
    + correctModel.FileDateTime + "\n  FileIDModifier: " + correctModel.FileIDModifier);
  if (!userAgreesToPost) {
    undoHandler();
    return;
  }


  function getCheckExistenceModel(modelFromServer) {
    successCallback(modelFromServer);
  }
  function callQuery() {
    var models = [];
    models.push(correctModel);
    PostingAjax.query(PostingAjax.Uri.GET_CHECK_EXISTENCE_MODEL,
      { 'models': models },
      getCheckExistenceModel, errorHandler);
  }

  var postModel = {
    FileIdentifier: correctModel,
    GlAccounts: {
      SettlementAccount: {
        Department: $('#settlements-gl-department-textbox').val(),
        Account: $('#settlements-gl-account-textbox').val()
      },
      ACHCashAccount: {
        Department: $('#cash-gl-department-textbox').val(),
        Account: $('#cash-gl-account-textbox').val()
      }
    }
  };
  PostingAjax.command(PostingAjax.Uri.POST,
      { 'model': postModel },
      callQuery,
      errorHandler);
};

PostingAjax.getRequestGlModel = function (successCallback, errorHandler) {
  PostingAjax.get(PostingAjax.Uri.DEFAULT_PAYMENT_ACCOUNTS, successCallback, errorHandler);
};


function callAjax(methodName, dataToSend, successCallback) {
  //var host = location.protocol + '/Settlements';
  var host = "http://localhost:22768";
  var uri = host + "/Posting.svc/" + methodName;
  jQuery.support.cors = true;
  $.ajax({
    url: uri,
    type: "POST",
    contentType: "application/json; charset=utf-8", /* To Server */
    data: dataToSend,
    success: function (data) {
      successCallback(data);
    },
    dataType: 'json', /* Received from server */
    error: function (XMLHttpRequest, textStatus, errorThrown) {
      alert('textStatus = ' + textStatus + '/nerrorThrown = ' + errorThrown);
    }
  });
}

function callAjaxOneWay(methodName, dataToSend, successCallback) {
  //var host = location.protocol + '/Settlements';
  var host = "http://localhost:22768";
  var uri = host + "/Posting.svc/" + methodName;
  jQuery.support.cors = true;
  $.ajax({
    url: uri,
    type: "POST",
    contentType: "application/json; charset=utf-8", /* To Server */
    data: dataToSend,
    success: function () {
      successCallback();
    },
    error: function (XMLHttpRequest, textStatus, errorThrown) {
      alert('textStatus = ' + textStatus + '/nerrorThrown = ' + errorThrown);
    }
  });
}

function postMenuShow(td, model) {
  function defaultAction() {
    $(td).find('.post-link')
      .closest('.post-menu-group').show();
  }
  $(td).find('.post-menu-group').hide();
  if (model.Exists) {
    /* Show Check Number */
    $(td).find('.ach-check-number').text(model.CheckNumber).show();
  } else if (model.Ignore) {
    /* Show Keep Text */
    $(td).find('.keep-link').closest('.post-menu-group').show();
  } else {
    /* standard post */
    defaultAction();
  }
}
/*
class post-menu-item  is what shows/hides link groups. 
There are 3 groups:
- Ignore / post
- Check Number
- Keep
*/
function renderPostMenu(td, model) {
  function getLink(text, className) {
    var link = $('<a>').attr('href', '#')
      .addClass('post-menu-item')
      .addClass(className)
      .text(text);
    return link;
  }

  var achFileCheckNumber = $('<span>').addClass('ach-check-number')
    .addClass('post-menu-group');
  var postLink = getLink('Post', 'post-link');
  var ignoreLink = getLink('Ignore', 'ignore-link');
  var keepLinkAndMenuGroup = getLink('Keep', 'keep-link')
      .addClass('post-menu-group');

  var postMenuGroup = $('<span>').addClass('post-menu-group')
    .append(postLink)
    .append('|')
    .append(ignoreLink);

  $(td).append(achFileCheckNumber)
      .append(postMenuGroup)
      .append(keepLinkAndMenuGroup);
  postMenuShow(td, model);
}




function getFileIdentifierModel(model) {
  //var achFileIdentifierModel = { 'FileDateTime': new Date(creationDate), FileIDModifier: fileIdModifier };
  var achFileIdentifierModel = { 'FileDateTime': new Date(model.creationDate), FileIDModifier: model.fileIdModifier };

  return achFileIdentifierModel;
}
function getCheckModel(td) {
  var fileIdentifier = $(td).data('fileIdentifier');
  var fileIdentifierModel = JSON.stringifyWcf({ 'model': fileIdentifier });
  return fileIdentifierModel;
}

$(document).ready(function () {

  function successCallback(validatedModels) {

    var msg = "";
    var models = {};

    $.each(validatedModels, function (idx, model) {
      model.FileIdentifier.FileDateTime = JSON.parseDateOnly(model.FileIdentifier.FileDateTime);

      if (models[model.FileIdentifier.FileDateTime] === undefined) {
        models[model.FileIdentifier.FileDateTime] = {};
      }
      models[model.FileIdentifier.FileDateTime][model.FileIdentifier.FileIDModifier] = model;
    });


    $('#payments-table tbody tr').each(function () {
      $(this).find('td').eq(6).each(function () {
        var parent = $(this).parent();
        var creationDate = new Date($(parent).find('td').eq(0).text());
        var fileIdModifier = $(parent).find('td').eq(1).text();
        var fileIdentifier = { 'FileDateTime': new Date(creationDate), FileIDModifier: fileIdModifier };
        if (models[creationDate] === undefined) {
          $(this).append('error');
        } else if (models[creationDate][fileIdModifier] === undefined) {
          $(this).append('error');
        }
        else {
          renderPostMenu(this, models[creationDate][fileIdModifier]);
        }
      });
    });
    /* set click for all menu items just rendered */
    $('.post-menu-item').on('click', function () {
      var td = $(this).closest('td');
      var model = $(td).data('fileIdentifier');

      function togglePostItemMenu(modelFromServer) {
        postMenuShow(td, modelFromServer[0]);
      }

      function notifyAjaxError() {
        alert('Error calling the server. Please try again');
      }
      function postCallBack(modelFromServer) {
        if (modelFromServer.length === 0) {
          notifyAjaxError();
          return;
        }
        var postedModel = modelFromServer[0];
        if (postedModel.Exists) {
          postMenuShow(td, postedModel);
        } else {
          $(td).find('.post-menu-group').hide();
          $(td).find('.ach-check-number').text('Post Failed').fadeIn(2000, function () {
            $(td).find('.post-menu-group').hide();
            postMenuShow(td, postedModel);
          });
        }
      }
      function undoHandler() {
        model.Exists = false;
        model.Ignore = false;
        postMenuShow(td, model);
      }

      var menuText = $(this).text().toLowerCase();
      $(td).find('.post-menu-group').hide();
      if (menuText === 'post') {
        /* pending */
        PostingAjax.post(model, postCallBack, notifyAjaxError, undoHandler);
      } else if (menuText === 'ignore') {
        PostingAjax.ignore(model, togglePostItemMenu, notifyAjaxError);
      } else if (menuText === 'keep') {
        PostingAjax.keep(model, togglePostItemMenu, notifyAjaxError);
      } else {
        throw 'Invalid link';
      }
      return false;
    });

  }


  var fileIdentifierModels = [];
  $('#payments-table tbody tr').each(function () {
    $(this).find('td').eq(6).each(function () {
      var parent = $(this).parent();
      var creationDate = $(parent).find('td').eq(0).text();
      var fileIdModifier = $(parent).find('td').eq(1).text();

      var achFileIdentifierModel = { 'FileDateTime': new Date(creationDate), FileIDModifier: fileIdModifier };
      fileIdentifierModels.push(achFileIdentifierModel);
      $(this).data('fileIdentifier', achFileIdentifierModel);
    });
  });

  //var fullModel = JSON.stringifyWcf({ 'models': fileIdentifierModels });
  var fullModel = { 'models': fileIdentifierModels };
  var methodName = "GetCheckExistenceModels";
  //callAjax(methodName, fullModel, successCallback);
  PostingAjax.query(PostingAjax.Uri.GET_CHECK_EXISTENCE_MODEL, fullModel, successCallback, function () { alert("error"); });
  function setGlAccounts(paymentGlModel) {
    $('#cash-gl-department-textbox').val(paymentGlModel.ACHCashAccount.Department);
    $('#cash-gl-account-textbox').val(paymentGlModel.ACHCashAccount.Account);
    $('#settlements-gl-department-textbox').val(paymentGlModel.SettlementAccount.Department);
    $('#settlements-gl-account-textbox').val(paymentGlModel.SettlementAccount.Account);
  }

  PostingAjax.getRequestGlModel(setGlAccounts, function () { alert("error"); });
  $('#payments-table tbody tr:even').css('background-color', '#dee6f3');
});
