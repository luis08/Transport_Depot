<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SavedACHFiles.aspx.cs" Inherits="ACH_SavedACHFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
  <script src="../Scripts/jquery-ui-1.10.4.custom.min.js" type="text/javascript"></script>
  <script src="../Scripts/SavedACHFiles.aspx.js" type="text/javascript"></script>
  <script src="../Scripts/json.js" type="text/javascript"></script>
  <script src="../Scripts/JSON.NET.js" type="text/javascript"></script>
  <style type="text/css">
    #file-list-container
    {
      overflow:auto;
      width:100%;
      height:30em;
      margin:0;
      
    }
    .account-container
    {
      width:17em;
      float:left;
      margin-right:1em;
      margin-left:1em;
      margin-top:0;
      
    }
    .account-container input
    {
      width:4em;
      margin:0.2em;
      text-align:left;
      padding:0.2em;
    }
    .account-container label
    {
      width:7em;
      display:inline-block;
      text-align:right;
      margin:0.2em;
    }
    #gl-accounts-container
    {
      background-color:#edf2f9;
      padding:0.1em;
      float:left;
    }
    .clear
    {
      clear:both;
    }
    #payments-table
    {
      width:55em;
    }
    .post-menu-item
    {
      display:inline-block;
      padding:0 0.2em;
    }
    #menu-column
    {
      width:10em;
    }
  </style>
</asp:Content>
<asp:Content ID="PageMainContent" ContentPlaceHolderID="MainContent" Runat="Server">
  <input id="AJAXCallDomainInput" type="hidden" runat="server" />
  <div id='gl-accounts-container' >
    <%--<a href="PaymentsPending.aspx" >Payments Pendinig</a>--%>
    <fieldset class='account-container' ><legend>Cash</legend>
      <label for='cash-gl-department-textbox'>Department:</label><input type="text" id='cash-gl-department-textbox' value=''/><br />
      <label for='cash-gl-account-textbox'>Account:</label><input type="text" id='cash-gl-account-textbox' value=''/>
    </fieldset>
    <fieldset class='account-container'><legend>Settlements</legend>
      <label for='settlements-gl-department-textbox'>Department:</label><input type="text" id='settlements-gl-department-textbox' value=''/><br />
      <label for='settlements-gl-account-textbox'>Account:</label><input type="text" id='settlements-gl-account-textbox' value=''/>
    </fieldset>
    <div class="clear"></div>
  </div>
  <div id='file-list-container'>
  <asp:ListView runat="server" ID="ACHPaymentsListView"
     DataKeyNames="CreationDate, FileIDModifier" 
     OnItemCommand="ACHPaymentsListView_OnItemCommand" 
    ondatabound="ACHPaymentsListView_DataBound" >
    <LayoutTemplate>
      <table id='payments-table' class="standard-table">
        <thead>
          <tr>
            <th class="table-header">Date</th>
            <th class="table-header">FileIDModifier</th>
            <th class="table-header">Time</th>
            <th class="table-header">Selements</th>
            <th class="table-header">Payrol Batches</th>
            <th class="table-header"></th>
            <th id="menu-column" class="table-header" ></th>
          </tr>
          <tr>
            <td colspan="7">       
              <asp:DataPager runat="server" ID="SaveACHFilesPagerTop" PageSize="100">
                <Fields>
                  <asp:NumericPagerField  NextPageText=">" PreviousPageText="<" />
                </Fields>
              </asp:DataPager>
            </td>
          </tr>    
        </thead>
        <tbody>
          <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
        </tbody>
        <tfoot>
          <tr>
            <td colspan="7">       
              <asp:DataPager runat="server" ID="SavedACHFilesPagerBottom" PageSize="100">
                <Fields>
                  <asp:NumericPagerField  NextPageText=">" PreviousPageText="<" />
                </Fields>
              </asp:DataPager>
            </td>
          </tr>   
        </tfoot>
      </table>
    </LayoutTemplate>
  
    <ItemTemplate>
      <tr>
        <td class="centered-data"><%# Eval("CreationDate", "{0:MM/dd/yyyy}")%></td>
        <td class="centered-data"><%# Eval("FileIDModifier") %></td>
        <td class="centered-data"><%# Eval("CreationTime") %></td>
        <td class="centered-data">
          <asp:LinkButton ID="OpenSettlementsLinkButton" runat="server" 
            CommandName="OpenSettlements">
            <%# Eval("SettlementCount")%>
          </asp:LinkButton></td>
        <td class="centered-data">
          <asp:LinkButton runat="server" ID="PayrollBatchCountLinkButton"  
            CommandName="OpenPayrollItems" >
            <%# Eval("PayrollEmployeeCount")%>
          </asp:LinkButton>
        </td>
        <td class="centered-data">
          <a  href='ACHFileGenerator.ashx?<%# ACHController.DownloadFileCreationDateKey %>=<%# Eval("CreationDate", "{0:yyyyMMdd}") %>&<%# ACHController.DownloadFileIdModiferKey %>=<%# Eval("FileIDModifier") %>' >
            Download
          </a>
        </td> 
        <td class="centered-data"></td>
      </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
      <tr>
        <td class="centered-data alternating-row"><%# Eval("CreationDate", "{0:MM/dd/yyyy}")%></td>
        <td class="centered-data alternating-row"><%# Eval("FileIDModifier") %></td>
        <td class="centered-data alternating-row"><%# Eval("CreationTime") %></td>
        <td class="centered-data alternating-row">
          <asp:LinkButton ID="OpenSettlementsLinkButton" runat="server" 
            CommandName="OpenSettlements">
            <%# Eval("SettlementCount")%>
          </asp:LinkButton></td>
        <td class="centered-data alternating-row">
          <asp:LinkButton runat="server" ID="PayrollBatchCountLinkButton"  
            CommandName="OpenPayrollItems" >
            <%# Eval("PayrollEmployeeCount")%>
          </asp:LinkButton>
        </td>
        <td class="centered-data alternating-row">
          <a  href='ACHFileGenerator.ashx?<%# ACHController.DownloadFileCreationDateKey %>=<%# Eval("CreationDate", "{0:yyyyMMdd}") %>&<%# ACHController.DownloadFileIdModiferKey %>=<%# Eval("FileIDModifier") %>' >
            Download
          </a>
        </td> 
        <td class="centered-data alternating-row"> </td>  
      </tr>    
    </AlternatingItemTemplate>
  </asp:ListView>
  </div>
</asp:Content>

