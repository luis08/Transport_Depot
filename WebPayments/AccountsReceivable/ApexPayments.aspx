<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ApexPayments.aspx.cs" Inherits="AccountsReceivable_ApexPayments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
  <link href="//maxcdn.bootstrapcdn.com/bootstrap/3.3.2/css/bootstrap.min.css" rel="stylesheet">
  <style stype='text/css'>
    .main-container {font-size:small;}
    .main-container ul { margin-left: 0; }
    .main-container button {float: left; }
    .main-container input[type=file] { float:left; }
    .the-menu { clear:both;}
    .payments-container { list-style-type: none; overflow: auto; max-height:22em; clear:both;}
    .payment-group { width:100%; display: block;  clear: both; height:2em; }
    .payment-group div { float: left; height:2em; padding:0.25em 0.2em; overflow:hidden;}
    .payment-header { font-weight:bold; }
    .exclude { width: 4em; text-align:center; }
    .invoice{ width: 5em; text-align:center; }
    .check{ width: 9em; text-align:center; }
    .client{ width: 27em; text-align:left; overflow:hidden; }
    .effective{ width: 5.5em; text-align:center; }
    .amount{ width: 5.5em; text-align:right; }
    .type { width: 8em; text-align:center; }
    .alternating-item {background-color: #dee6f3;}
    .clear { width:100%; clear:both;}
  </style>
  <script src="../Scripts/json.js" type="text/javascript"></script>
  <script src="../Scripts/JSON.NET.js" type="text/javascript"></script>
  <script src="//code.jquery.com/jquery-1.11.1.min.js"></script>
  <script src="//ajax.googleapis.com/ajax/libs/angularjs/1.2.26/angular.min.js"></script>  
  <script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.2/js/bootstrap.min.js"></script>
  <script src="../Scripts/ApexPayments.aspx.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <div ng-app="apexUpload" ng-controller="uploader" class='main-container'>
      <input type='file' 
           onchange="angular.element(this).scope().fileChanged(this)"  />
    
    <button ng-click="save($event)" >Save</button>
    <div class='clear'></div>
    
    
    <ul class="nav nav-tabs the-menu" ng-init="activeTab = 'valid'">
        <li ng-class="{ active: activeTab === 'valid' }">
            <a ng-click="activeTab = 'valid'" href="#">Valid Invoices</a>
        </li>
        <li ng-class="{ active: activeTab === 'invalid' }"  ng-show='showInvalid'>
            <a ng-click="activeTab = 'invalid'" href="#">Invalid Invoices</a>
        </li>
    </ul>    
    
  <ul class='payments-container' >
    <li class='payment-group payment-header'>
      <div class='exclude'>Exclude</div>
      <div class='invoice'>Invoice</div>
      <div class='check'>Check</div>
      <div class='client'>Client</div>
      <div class='effective'>Effective</div>
      <div class='amount'>Amount</div>
      <div class='type'>Type</div>
    </li>
  </ul>
    <div class='tab-content'>
    <div class="tab-pane" ng-class="{ active: activeTab === 'valid' }">
    <ul class='payments-container'>
      <li class='payment-group' ng-repeat="payment in ValidPayments | orderBy:predicate" >
         <div clickbox class='exclude' ng-class-even="'alternating-item'"><input name='unselected' type="checkbox" ng-model="payment.exclude" noclick/></div>
          <!-- Invoice -->
          <div clickbox class='invoice' ng-class-even="'alternating-item'">{{payment.InvoiceNumber}}</div>
          <!-- Check -->
          <div clickbox class='check' ng-class-even="'alternating-item'">{{payment.CheckNumber}}</div>
          <!-- Client -->
          <div clickbox class='client' ng-class-even="'alternating-item'">{{payment.Debtor}}</div>
          <!-- Effective Date -->
          <div clickbox class='effective' ng-class-even="'alternating-item'">{{payment.EffectiveDate | date:'MM/dd/yyyy'}}</div>
          <!-- Invoice Amount-->
          <div clickbox class='amount' ng-class-even="'alternating-item'">{{payment.Amount | currency }}</div>
          <div class='type' ng-class-even="'alternating-item'">{{payment.displayType }}</div>
      </li>
    </ul>
    </div>
    <div class="tab-pane" ng-class="{ active: activeTab === 'invalid' }">
    
    <ul class='payments-container'>
      <li class='payment-group' ng-repeat="payment in InvalidPayments | orderBy:predicate" >
         <div clickbox class='exclude' ng-class-even="'alternating-item'">&nbsp;</div>
          <!-- Invoice -->
          <div clickbox class='invoice' ng-class-even="'alternating-item'">{{payment.InvoiceNumber}}</div>
          <!-- Check -->
          <div clickbox class='check' ng-class-even="'alternating-item'">{{payment.CheckNumber}}</div>
          <!-- Client -->
          <div clickbox class='client' ng-class-even="'alternating-item'">{{payment.Debtor}}</div>
          <!-- Effective Date -->
          <div clickbox class='effective' ng-class-even="'alternating-item'">{{payment.EffectiveDate | date:'MM/dd/yyyy'}}</div>
          <!-- Invoice Amount-->
          <div clickbox class='amount' ng-class-even="'alternating-item'">{{payment.Amount | currency }}</div>
          <div class='type' ng-class-even="'alternating-item'">{{payment.displayType }}</div>
      </li>
    </ul>  
    </div>
  </div>
  </div>
  
</asp:Content>

