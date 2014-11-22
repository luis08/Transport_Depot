<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="AccountsReceivable_Default" %>

<asp:Content ID="ARHeadnerContent" ContentPlaceHolderID="HeadContent" Runat="Server">
  <link href="../Styles/AccountsReceivable_Default.aspx.css" rel="stylesheet" type="text/css" />
  <link href="../Styles/AccountsReceivable_Default.aspx.css" rel="stylesheet" type="text/css" />
  <script src="../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
  <script src="../Scripts/Utilities.js" type="text/javascript"></script>
  <script src="../Scripts/mustache.js" type="text/javascript"></script>
  <script src="../Scripts/AccountsReceivable_Default.aspx.js" type="text/javascript"></script>
  <script type="text/javascript">

  </script>
  <script id='collections-template' type="text/template">
  {{#customers}}
  <tr class='collections-data-row'>
    <td class='customer-selection-container centered-data'>
      <input type='hidden' class='customer-id-contiainer' value='{{CustomerId}}'/>
      <input type='checkbox' class='customer-selection-element'/>
    </td>
    <td class='centered-data'>
      <span class='customer-id'>{{CustomerId}}</span>  
    </td>
    <td class='customer-name-container'>
      <span class='customer-name'>{{CustomerName}}</span>  
    </td>
    <td class='currency'>
      {{Balance}}
    </td>
    <td class='currency'>
      {{OverdueBalance}}
    </td>
    <td class='centered-data'>
      {{TotalInvoiceCount}}
    </td>
    <td class='centered-data'>
      {{OverdueInvoiceCount}}
    </td>
    <td class='centered-data'>
      {{MaxAging}}
    </td>
  </tr>
  {{/customers}}
  </script>
</asp:Content>
<asp:Content ID="ARContent" ContentPlaceHolderID="MainContent" Runat="Server">
  <div id='collection-customers-container'>
    <table>
      <thead>
        <tr>
          <th></th>
          <th>CustomerID</th>
          <th>Customer Name</th>
          <th>Total Balance</th>
          <th>Overdue Balance</th>
          <th>Invoice<br />Cnt</th>
          <th>Overdue<br />Inv Cnt</th>
          <th>Max<br />Aging</th>
        </tr>
      </thead>
      <tbody id='collection-data-container'>
      </tbody>
    </table>
  </div>

  <div id='collections-menu-container'>
    <fieldset>
    <legend>Print Letters</legend>
    <a href="#" id='print-collection-letters-link' class='print-command' target="_blank" >Collection Letters</a><br />
    <a href="#" id='print-final-letters-link' class='print-command' target="_blank">Final Notice</a><br />
    </fieldset>
    <fieldset>
      <legend >Grid Contents</legend>
      <label>
      <input type="radio" name="overdue-selection"  value='overdue-by-days'  /><span>By Aging Days</span>
      </label>
      <input class='overdue-by-days' type="text" id='days-overdue-textbox' value='30' /><br />
      <label>
      <input type='radio' name="overdue-selection" class='overdue-all' checked="checked" value='overdue-all' /><span>All Overdue</span>
      </label>
      <br /><br />
      <a href="#" id='refresh-collections-link' >Refresh</a>
    </fieldset>
    <fieldset id='collections-report-menu'>
      <legend>Collections Report</legend>
      <label>Terms:</label>
      <select id='collections-report-type'>
        <option selected="selected" value="QuickPay">Quick-Pay</option>
        <option value="Factored">Factored</option>
        <option  value="All">All</option>
      </select>
      <br /><br />
      <label>Sort:</label>
      <select id='collections-report-sort'>
          <option selected="selected" value="CustomerName" >Customer Name</option>
          <option value="Aging">Oldest Invoice</option>
          <option value="Balance" value="Balance">Balance</option>
        </select>
      <a href="#" id='print-collections-report' class='print-command' target="_blank">Print</a>
  </fieldset>
  </div>


</asp:Content>

