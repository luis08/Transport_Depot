<%@ Page Title=""  Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PaidSettlements.aspx.cs" Inherits="ACH_PaidSettlements" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
  <link href="/Styles/PaidTransactions.css" rel="stylesheet" type="text/css" />
  <link href="../Styles/PaymentsPending.aspx.css" rel="stylesheet" 
    type="text/css" />
</asp:Content>
<asp:Content ID="PaidSettlementsContent" ContentPlaceHolderID="MainContent" Runat="Server">

  <div class="list-title">
    <h3>Paid Settlements  
      Created On  <asp:Label ID="FileCreationDateLabel"   runat="server" 
        CssClass="list-title-data" /> &nbsp;&nbsp;
      File ID Modifier: <asp:Label ID="FileIdModifierLabel" runat="server" 
        CssClass="list-title-data" />
    </h3>
  </div>
  <asp:ListView runat="server" ID="PaidSettlementsListView"
     DataKeyNames="ACHBatchNumber, TraceNumberSequence" 
    ondatabound="PaidSettlementsListView_DataBound" >
    <LayoutTemplate>
      <table class="standard-table" >
        <thead>
          <tr>
            <th class="table-header">Type</th>
            <th class="table-header">Batch Number</th>
            <th class="table-header">Trace Number</th>
            <th class="table-header">LessorId</th>
            <th class="table-header">Amount</th>
          </tr>
          <tr>
            <td colspan="6">       
              <asp:DataPager runat="server" ID="PaidSettlementsPagerTop" PageSize="100">
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
            <td colspan="6">       
              <asp:DataPager runat="server" ID="PaidSettlementsPagerBottom" PageSize="100">
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
        <td class="centered-data"><%# Eval("TransactionType")%></td>
        <td class="centered-data"><%# Eval("ACHBatchNumber")%></td>
        <td class="centered-data"><%# Eval("TraceNumberSequence")%></td>
        <td class="centered-data"><%# Eval("LessorId")%></td>
        <td class="currency"><%# Eval("Amount", "{0:C2}") %></td>
      </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
      <tr class="alternating-row" >
        <td class="centered-data"><%# Eval("TransactionType")%></td>
        <td class="centered-data"><%# Eval("ACHBatchNumber")%></td>
        <td class="centered-data"><%# Eval("TraceNumberSequence")%></td>
        <td class="centered-data"><%# Eval("LessorId")%></td>
        <td class="currency"><%# Eval("Amount", "{0:C2}") %></td>
      </tr>    
    </AlternatingItemTemplate>
  </asp:ListView>
</asp:Content>

