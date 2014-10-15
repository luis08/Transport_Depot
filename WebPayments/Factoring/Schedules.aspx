<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
  CodeFile="Schedules.aspx.cs" Inherits="Factoring_Schedules" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="Server">
  <link href="../Styles/Schedules.aspx.css" rel="stylesheet" type="text/css" />
  <link href="../Styles/jQueryUI/smoothness/jquery-ui-1.10.4.custom.min.css" rel="stylesheet"
    type="text/css" />
  <script src="../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
  <script src="../Scripts/jquery-ui-1.10.4.custom.js" type="text/javascript"></script>
  <script src="../Scripts/mustache.js" type="text/javascript"></script>
  <script src="../Scripts/Utilities.js" type="text/javascript"></script>
  <script src="../Scripts/json.js" type="text/javascript"></script>
  <script src="../Scripts/JSON.NET.js" type="text/javascript"></script>
  <script src="../Scripts/Pager.js" type="text/javascript"></script>
  <script id='invoice-template' type="text/template">
    {{#invoices}}
    <tr class='invoice-row'>
      <td class='invoice-selected centered-data'>
        <input type='checkbox'{{#checked}}checked='checked'{{/checked}} class='invoice-selector'/>
        <input type='hidden' value='{{index}}'/>
      </td>
      <td class='centered-data'>{{Number}}</td>
      <td class='centered-data'>{{Date}}</td>
      <td class='centered-data'>{{CustomerName}}</td>
      <td class='currency'>{{Amount}}</td>
    </tr>
    {{/invoices}}
    <tr>
      <td colspan='5'>
        {{#pagerButtons}}
        <a href='#' onclick='SchedulePopup.renderInvoices({{startRowIndex}}, true);' {{#hide}}style='display:none;'{{/hide}}>{{text}}</a>
        {{/pagerButtons}}
        <span>Page&nbsp;</span>{{thisPageNumber}}<span>&nbsp;of&nbsp;</span>{{totalPages}}
      </td>
    </tr>
  </script>
  <script type="text/javascript">
  
  </script>
  <script src="../Scripts/Schedules.aspx.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="Server">
  <div id='schedules-container'>
    <asp:ListView ID="SchedulesListView" runat="server" DataSourceID="SchedulesObjectDataSource"
      OnPagePropertiesChanging="SchedulesListView_PagePropertiesChanging">
      <LayoutTemplate>
        <asp:DataPager ID="SchedulesDataPager" runat="server" PageSize="15">
          <Fields>
            <asp:NumericPagerField ButtonCount="10" ButtonType="Link" />
          </Fields>
        </asp:DataPager>
        <table id='schedules-container-table'>
          <thead>
            <tr>
              <th>
                Schedule
              </th>
              <th>
                Date
              </th>
              <th>
                Amount
              </th>
              <th>&nbsp;</th>
            </tr>
          </thead>
          <tbody>
            <tr runat="server" id="itemPlaceholder">
            </tr>
          </tbody>
          <tfoot>
            <tr>
              <td colspan="3">
              </td>
            </tr>
          </tfoot>
        </table>
      </LayoutTemplate>
      <ItemTemplate>
        <tr runat="server">
          <td class='centered-data'>
            <a href="#" class="open-schedule-link schedule-row-menu-item">
              <%#Eval("Id") %></a>
          </td>
          <td class='date-column'>
            <%#Eval("Date", "{0:MM/dd/yyyy}")%>
          </td>
          <td class='currency'>
            <%#Eval("Amount", "{0:C}") %>
          </td>
          <td>
            <a class="schedule-row-menu-item" href='http://netgateway/settlements/FactoringPdf.svc/pdf?id=<%#Eval("Id") %>' target="_blank" type="application/pdf">Pdf</a>
          </td>
        </tr>
      </ItemTemplate>
      <AlternatingItemTemplate>
        <tr id="alternatingItemTemp" runat="server" class="alternating-row">
          <td class='centered-data'>
            <a href="#" class="open-schedule-link schedule-row-menu-item">
              <%#Eval("Id") %></a>
          </td>
          <td class='date-column'>
            <%#Eval("Date", "{0:MM/dd/yyyy}")%>
          </td>
          <td class='currency'>
            <%#Eval("Amount", "{0:C}") %>
          </td>
          <td>
            <a class="schedule-row-menu-item" href='http://netgateway/settlements/FactoringPdf.svc/pdf?id=<%#Eval("Id") %>' target="_blank" type="application/pdf">Pdf</a>
          </td>
        </tr>
      </AlternatingItemTemplate>
    </asp:ListView>
  </div>
  <div id="paging-container">
  </div>
  <div id='options-container'>
    Items Per Page&nbsp;
    <asp:DropDownList runat="server" ID="ItemsPerPageDropDown" OnSelectedIndexChanged="ItemsPerPageDropDown_SelectedIndexChanged"
      AutoPostBack="True">
      <asp:ListItem Text="15" Value="15" Selected="True" />
      <asp:ListItem Text="20" Value="20" />
      <asp:ListItem Text="25" Value="25" />
      <asp:ListItem Text="30" Value="30" />
      <asp:ListItem Text="35" Value="35" />
    </asp:DropDownList>
    <a href="#" id='new-schedule-menu-item'>New Schedule</a>
    <fieldset id='filters-selector-container'>
      <legend>Filters</legend>
      <label>
        <input type="checkbox" id='UseDatesCheckbox' runat="server" clientidmode="Static" />
        Use Dates
      </label>
      <br />
      <label>
        <input type="checkbox" id='UseScheduleNumberCheckbox' runat="server" clientidmode="Static" />
        Use Schedule Number
      </label>
    </fieldset>
    <fieldset class='schedule-options' id='date-range-container'>
      <legend>Date Range</legend>
      <label for="FromDateInput">
        From:</label>
      <input type="text" runat="server" id="FromDateInput" clientidmode="Static" />
      <br />
      <label>
        To:</label>
      <input type="text" runat="server" id="ToDateInput" clientidmode="Static" />
    </fieldset>
    <fieldset class='schedule-options' id='schedule-range-container'>
      <legend>Schedules</legend>
      <label for="FromScheduleInput">
        From:</label>
      <input type="text" runat="server" id="FromScheduleInput" clientidmode="Static" />
      <br />
      <label for="ToScheduleInput">
        To:</label>
      <input type="text" runat="server" id="ToScheduleInput" clientidmode="Static" />
    </fieldset>
    <asp:LinkButton runat="server" ID="ApplyFiltersButton" ClientIDMode="Static" OnClick="ApplyFiltersButton_Click">Apply Filters</asp:LinkButton>
  </div>
  <div class='clear' />
  <div id='dialog-overlay'>
  </div>
  <div id='schedule-dialog'>
    <div>
      <fieldset id='schedule-date-container' class='schedule-details-header'>
        <label for='schedule-date-textbox'>
          Date:</label><input type='text' id='schedule-date-textbox' />
      </fieldset>
      <div id='schedule-menu-container'>
        <a id='view-schedule-pdf-link' href="#">Schedule Pdf</a> 
      </div>
      <fieldset id='schedule-info-container' class='schedule-details-header'>
        <label class='schedule-form'>
          Schedule &#35;:</label><input id='schedule-number-textbox' type='text' readonly='readonly' class='schedule-form' />
        <br />
        <label class='schedule-form'>
          Total:</label><input id='schedule-total-textbox' type='text' readonly='readonly' class='schedule-form'/>
      </fieldset>
      <div id='schedule-changes-menu-container' style='width: 100%; clear: both;'>
        <a href="#" id='import-invoices-menu-item'>Import Invoices</a> 
        <a href="#" id='remove-unselected-menu-item'>Remove Unselected</a>
        <a href="#" id='update-schedule-menu-item' >Update Schedule</a>
        <a id='close-schedule-link' href="#">Close</a>
        <div class="clear">
        </div>
      </div>
    </div>
    <div id='invoice-list'>
      <!-- InvoiceViewModel -->
      <table>
        <thead class='invoice-list-header'>
          <tr>
            <th>
              Select
            </th>
            <th>
              Number
            </th>
            <th>
              Date
            </th>
            <th>
              CustomerName
            </th>
            <th>
              Amount
            </th>
          </tr>
        </thead>
        <tbody id='invoice-list-container'>
        </tbody>
      </table>
    </div>
  </div>
  <div id='invoice-filter-dialog'>
    <h2>Invoices</h2>
    <fieldset id='invoice-filter-container' class='invoice-filter-options'>
      <legend>Filters</legend>
      <label >
        <input type="checkbox" id='use-invoice-date-filter' />
        Use Dates
      </label>
      <br />
      <label >
        <input type="checkbox" id='use-invoice-number-filter' />
        Use Invoice Number
      </label><br />
      <label >
        <input type="checkbox" checked="checked" id='only-no-schedule-invoices-filter' />
        Only Without Schedule
      </label>
    </fieldset>
    <div id='import-menu'>
      <a href='#' id='import-invoices-dialog-button'>Import</a>
      <a href="#" id='close-invoice-filter-dialog-button'>Close</a>
    </div>
    <fieldset id='invoice-date-filter' class='invoice-filter-options'>
      <legend>Date Range</legend>
      <label for="invoice-from-date-textbox" class='schedule-form'>
        From:</label>
      <input type="text" id="invoice-from-date-textbox"  class='schedule-form' />
      <br />
      <label for='invoice-to-date-checkbox' class='schedule-form'>
        To:</label>
      <input type="text" id="invoice-to-date-textbox"  class='schedule-form' />
    </fieldset>
    <fieldset id='invoice-number-filter' class='invoice-filter-options'>
      <legend>Invoice Numbers</legend>
      <label for="from-invoice-number-textbox" class='schedule-form'>
        From:</label>
      <input type="text" id="from-invoice-number-textbox" class='schedule-form' />
      <br />
      <label for="to-invoice-number-textbox" class='schedule-form'>
        To:</label>
      <input type="text" id="to-invoice-number-textbox" class='schedule-form'  />
    </fieldset>
    
  </div>
  <asp:ObjectDataSource ID="SchedulesObjectDataSource" runat="server" DataObjectTypeName="FactoringServiceReference.ScheduleSummaryViewModel"
    SelectMethod="GetSchedules" TypeName="WebPayments.Factoring.FactoringServiceDataSource"
    EnablePaging="true" MaximumRowsParameterName="maxRows" StartRowIndexParameterName="startRowIndex"
    SelectCountMethod="GetScheduleCount" OnSelecting="SchedulesObjectDataSource_Selecting">
    <SelectParameters>
      <asp:Parameter Name="fromDate" DbType="DateTime" />
      <asp:Parameter Name="toDate" DbType="DateTime" />
      <asp:Parameter Name="fromSchedule" DbType="Int32" />
      <asp:Parameter Name="toSchedule" DbType="Int32" />
    </SelectParameters>
  </asp:ObjectDataSource>
</asp:Content>
