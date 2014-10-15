<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PaymentsPending.aspx.cs" Inherits="ACH_PaymentsPending" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
<script src="/WebPayments/Scripts/jquery-1.10.2.js" type="text/javascript"></script>
<script type="text/javascript">
  $(document).ready(function () {
    $("#PreviewACHFileButton")
    .click(function () {
      $('#SettlementIdsHiddenField').val(getIds());
    });

    $('#select-all-payments-button').on('click', function () {
      $('#payments-due-container')
        .find('input[type=checkbox]:not([disabled])')
        .attr('checked', 'checked');
    });
    $('#select-no-payments-button').on('click', function () {
      $('#payments-due-container')
        .find('input[type=checkbox]')
        .removeAttr('checked');
    });
  });

  function getIds() {
    var settlmentPaymentIds = [];
    $("#SettlementsDueTableBody")
      .find("td input[type=checkbox]")
      .each(function () {
        if (this.checked) {
          settlmentPaymentIds.push($(this).next().val());
        }
      });
      return settlmentPaymentIds;
    }


</script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Payments Due
    </h2>
    <div id="main-interactive-container">
      <input type="hidden" id="Thething" value="TTT" />
      <div id="payments-due-container" >
        <a id="select-all-payments-button" onclick="return false;" >Select All</a>
        &nbsp;&#124;&nbsp;
        <a id="select-no-payments-button" onclick="return false;" >Select None</a>
        &nbsp;&#124;&nbsp;
        <asp:LinkButton runat="server"  ClientIDMode="Static" ID="GoToACHFilesButton" 
          Text="Create New File & Download Files" 
          OnClick="GoToACHFilesButton_Click"/>
        <asp:ListView ID="SettlementPaymentsPending" 
          ViewStateMode="Enabled" runat="server" 
          DataKeyNames="Id" onitemdatabound="SettlementPaymentsPending_ItemDataBound" 
          ondatabound="SettlementPaymentsPending_DataBound" >
          
          <LayoutTemplate>
          <table id="payments-grid" class="standard-table" >
            <thead>
              <tr>
                <th class="table-header"></th>
                <th class="table-header">ID</th>
                <th class="table-header">Lessor</th>
                <th class="table-header">Accepted</th>
                <th class="table-header">Amount</th>
              </tr>
              <tr>
                <td colspan="5" >
                  <asp:DataPager runat="server" ID="PaymentsPendingPagerTop"
                     PageSize="100">
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
                <td colspan="5">     
                  <asp:DataPager runat="server" ID="PaymentsPendingPagerBottom"
                    PageSize="100">
                    <Fields >
                      <asp:NumericPagerField  NextPageText=">" PreviousPageText="<" />
                    </Fields>
                  </asp:DataPager>
                </td>
              </tr>
            </tfoot>
          </table>    
          </LayoutTemplate>

          <ItemTemplate>
            <tr >
              <td class="centered-data"><asp:CheckBox runat="server" ID="SelectPaymentCheckBox"  /></td>
              <td class="centered-data"><asp:Label runat="server" ID="PaymentIdLabel" Text='<%# Eval("Id") %>' /></td>
              <td class="centered-data"><asp:Label runat="server" ID="LessorIdLabel" Text='<%# Eval("LessorId")%>' /></td>
              <td class="centered-data"><%# Eval("AcceptedDate", "{0:MM/dd/yyyy}")%></td>
              <td class="currency"><%# Eval("Amount", "{0:C2}")%></td>
            </tr>
          </ItemTemplate>
          <AlternatingItemTemplate>
            <tr class="alternating-row" >
              <td class="centered-data"><asp:CheckBox runat="server" ID="SelectPaymentCheckBox"  /></td>
              <td class="centered-data"><asp:Label runat="server" ID="PaymentIdLabel" Text='<%# Eval("Id") %>' /></td>
              <td class="centered-data"><asp:Label runat="server" ID="LessorIdLabel" Text='<%# Eval("LessorId")%>' /></td>
              <td class="centered-data"><%# Eval("AcceptedDate", "{0:MM/dd/yyyy}")%></td>
              <td class="currency"><%# Eval("Amount", "{0:C2}")%></td>
            </tr>
          </AlternatingItemTemplate>
          <EmptyDataTemplate>
            <p>No Scheduled Selements to pay</p>
          </EmptyDataTemplate>
      
      </asp:ListView>
      <br />

      </div>    
      <div id="options-container">
        <h2>ACH File Date</h2>
        <asp:Calendar ID="ACHFileDateCalendar" runat="server" 
          ondayrender="ACHFileDateCalendar_DayRender"></asp:Calendar>


      </div>
      <div>
        <asp:Label ID="ErrorLabel" runat="server" EnableViewState="false" Text="" />
      </div>
    </div>
</asp:Content>


