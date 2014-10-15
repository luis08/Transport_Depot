<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PaidPayrollBatches.aspx.cs" Inherits="ACH_PaidPayrollBatches" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
  <link href="/Styles/PaidTransactions.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="list-title">
    <h3>Paid Payroll Batches
        Created On  <asp:Label ID="FileCreationDateLabel"   runat="server" 
          CssClass="list-title-data" /> &nbsp;&nbsp;
        File ID Modifier: <asp:Label ID="FileIdModifierLabel" runat="server" 
          CssClass="list-title-data" />
    </h3>
    <asp:ListView runat="server" ID="PaidPayrollItemsListView"
       DataKeyNames="ACHBatchNumber, TraceNumberSequence" 
      ondatabound="PaidPayrollItemsListView_DataBound" >
      <LayoutTemplate>
        <table class="tabular-data">
          <thead>
            <tr>
              <th>Type</th>
              <th>ACH Batch</th>
              <th>Trace Number</th>
              <th>Payroll Batch</th>
              <th>Employee ID</th>
              <th>Receiver Name</th>
              <th>Amount</th>
            </tr>
            <tr>
              <td colspan="7">       
                <asp:DataPager runat="server" ID="PaidPayrollItemsListViewTop" PageSize="100">
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
                <asp:DataPager runat="server" ID="PaidPayrollItemsListViewBottom" PageSize="100">
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
        <tr class="centered-data">
          <td><%# Eval("TransactionType")%></td>
          <td><%# Eval("ACHBatchNumber")%></td>
          <td><%# Eval("TraceNumberSequence")%></td>
          <td><%# Eval("PayrollBatchNumber")%></td>
          <td><%# Eval("EmployeeId")%></td>
          <td class="currency"><%# Eval("Amount", "{0:C2}") %></td>
        </tr>
      </ItemTemplate>
  </asp:ListView>
  </div>
</asp:Content>

