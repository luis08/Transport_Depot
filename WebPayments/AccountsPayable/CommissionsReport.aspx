<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="CommissionsReport.aspx.cs" Inherits="AccountsPayable_CommissionsReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
  <style type="text/css">
    #main-menu { width: 14em; float:left; overvlow:hidden; padding-left:1em;}    
    #main-menu div { float:left; }
    #source-menu { width:15em; }
    #source-menu label { width:12em; float:left; display:inline-block; height:1.5em; }
    #source-menu input { float:left; display:inline-block; width:2em; }
    #extension-menu{   }
    #extension-menu div { float:left;}
    #dispatcher-menu{  }
    #dispatcher-selector{ width:12em; }
    #paydate-selector{ width:12em;}
    #report-container{ width:56em; float:left;}
    #commission-header{ font-weight:bold; }
    #report-container ul
    {
      margin: 0;
      padding: 0;
      list-style-type: none;
    }
    #report-container ul li { display:block; clear: both; }
    #report-container ul li div 
    { 
      float:left; height:1.2em; 
      padding:0.25em 0.15em 0.35em 0.15em; 
      overflow:hidden;
    }
    #commission-details { height:29em; clear:both; overflow:auto; }
    #commission-details:nth-child(odd) { background-color:#ccc; }
    .trip-number { width:6em; }
    .invoice-number{ width: 5em; text-align:center; }
    .invoice-date{ width:7em; text-align:center;}
    .lane{ width:5em; text-align:center;}
    .tractor-id{ width:5em; text-align:center; }
    .customer-name{ width:15em; overflow:hidden; }
    .commission-total{ width:6em; text-align:right;}
    .invoice-amount{ width:8em; text-align:right;}
    #totals-container { margin-top:2em; }
    #totals-container label { font-weight:bold; width: 9em; display:inline-block; text-align:right; }
  </style>
  <script src="//code.jquery.com/jquery-1.11.1.min.js"></script>
  <script type="text/javascript" src="../Scripts/json.js"></script>
  <script src="../Scripts/Utilities.js" type="text/javascript"></script>
  <script src="../Scripts/JSON.NET.js" type="text/javascript"></script>
  <script src="../Scripts/jquery-ui-1.10.4.custom.min.js" type="text/javascript"></script>
  <script src="../Scripts/mustache.js" type="text/javascript"></script>
  <script id='commission-report-template' type="text/template">
      {{#commissions}}
      <li class='commission-detail'>
        <div class='trip-number'>{{TripNumber}}</div>
        <div class='invoice-date'>{{InvoiceDate}}</div>
        <div class='lane'>{{Lane}}</div>
        <div class='tractor-id'>{{LessorId}}</div>
        <div class='customer-name'>{{CustomerName}}</div>
        <div class='commission-total'>{{CommissionTotal}}</div>
        <div class='invoice-amount'>{{InvoiceAmount}}</div>

      </li>
      {{/commissions}}
  </script>
  <script src="../Scripts/Payables/commissions.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

  
  <div id='report-container'>
    <ul id='commission-header'>
      <li class='commission-detail'>
        <div class='trip-number'>Trip</div>
        <div class='invoice-date'>Inv. Date</div>
        <div class='lane'>Lane</div>
        <div class='tractor-id'>Lessor</div>
        <div class='customer-name'>Customer Name</div>
        <div class='commission-total'>Commission</div>
        <div class='invoice-amount'>Inv. Amt</div>
      </li>
    </ul>
    <ul id='commission-details'>

    </ul>
    
  </div>
  <div id='main-menu'>
    <div id='source-menu'>
      <input type="radio" id='all-dispatchers' name='commission-source' value="all" checked="checked" />
      <label for='all-dispatchers'>All Dispatchers</label><br />
      <input type="radio" id='specific-dispatcher' name='commission-source' value="specific" />
      <label for='specific-dispatcher'>Specific Dispatcher</label>
      <br />
      
    </div>
    <div id='extension-menu'>
      <div id='dispatcher-menu'>
        <h3>Dispatcher</h3>
        <select id='dispatcher-selector' size='4'></select>
      </div>
      <div>
        <div><h3>Pay Date</h3>
         <select id='paydate-selector' size='5'></select>
        </div>
      </div>
      
    </div>
    <div id='totals-container'>
      <label id='load-count-label'>Load Count:</label>
      <span id='load-count'></span>
      <br/><br/>
      <label id='commission-total-label'>Commission Total:</label>
      <span id='commission-total'></span>
      
    </div>
  </div>
  <div class='loading-widget'><!-- Always goes at the bottom --></div>  
</asp:Content>

