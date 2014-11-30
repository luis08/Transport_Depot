<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ApexPayments.aspx.cs" Inherits="AccountsReceivable_ApexPayments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
  <style stype='text/css'>
    .payments-container { list-style-type: none; overflow: auto; max-height:22em;}
    .payment-group { width:100%; display: block;  clear: both; }
    .payment-group div { float: left; height:1.5em; padding:0.25em; }
    .payment-header { font-weight:bold; }
    .exclude { width: 4em; text-align:center; }
    .invoice{ width: 5em; text-align:center; }
    .check{ width: 9em; text-align:center; }
    .client{ width: 27em; text-align:left; overflow:hidden; }
    .effective{ width: 5em; text-align:center; }
    .amount{ width: 5.5em; text-align:right; }
    .alternating-item {background-color: #dee6f3;}
  </style>
  <script src="../Scripts/json.js" type="text/javascript"></script>
  <script src="../Scripts/JSON.NET.js" type="text/javascript"></script>
  <script src="//code.jquery.com/jquery-1.11.1.min.js"></script>
  <script src="//ajax.googleapis.com/ajax/libs/angularjs/1.2.26/angular.min.js"></script>  
  <script src="../Scripts/ApexPayments.aspx.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <div ng-app="apexUpload" ng-controller="uploader">
      <input type='file' 
           onchange="angular.element(this).scope().fileChanged(this)"  />
    <button ng-click="upload($event)" >Upload and Review Payments</button>
    <button ng-click="save($event)" >Save</button>
    <br />
  <ul class='payments-container' >
    <li class='payment-group payment-header'>
      <div class='exclude'>Exclude</div>
      <div class='invoice'>Invoice</div>
      <div class='check'>Check</div>
      <div class='client'>Client</div>
      <div class='effective'>Effective</div>
      <div class='amount'>Amount</div>
    </li>
  </ul>
  
    <ul class='payments-container'>
      <li class='payment-group' ng-repeat="payment in payments" >
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
      </li>
    </ul>
  
  </div>
</asp:Content>

