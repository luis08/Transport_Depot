<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Payees.aspx.cs" Inherits="ACH_Payees" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
  <script src="/WebPayments/Scripts/jquery-1.10.2.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="PayeeContent" ContentPlaceHolderID="MainContent" Runat="Server">
  <asp:RadioButton ID="LessorsRadioButton" Text="Lessors" AutoPostBack="true" 
     runat="server" GroupName="PayeeType" ClientIDMode="Static" 
     oncheckedchanged="LessorEmployee_CheckedChanged" />
  <asp:RadioButton ID="EmployeesRadioButton" Text="Employees" AutoPostBack="true" 
    runat="server" GroupName="PayeeType" ClientIDMode="Static" 
    ViewStateMode="Disabled" oncheckedchanged="LessorEmployee_CheckedChanged" /> 
  <asp:RadioButton name="EditAddCheckBoxGroup" runat="server" id="SaveInput" value="Save" enableviewstate="false" ClientIDMode="Static" CssClass="hidden-element" />
  <asp:RadioButton name="EditAddCheckBoxGroup" runat="server" id="AddInput" value="Add" enableviewstate="false" ClientIDMode="Static" CssClass="hidden-element"  />
<div id="payee-dialog-container-menu">
  <fieldset class='editfieldset'>
    <legend runat="server" id="PayeeEditContainer" clientidmode="Static"></legend>
    
    <asp:HiddenField ID="EditPayeeIdHiddenField" runat="server" Value="" ClientIDMode="Static" />
    <label for="EditTruckWinId" class='editlabel'>Truckwin ID:</label>
    <input readonly="readonly" type="text" id="EditTruckWinId" runat="server" clientidmode="Static" enableviewstate="false" />
    <asp:DropDownList runat="server" ID="NewPayeesDropDown" ClientIDMode="Static"></asp:DropDownList>
    <br />
    <label for="EditDFIIdentification" class='editlabel'>Routing Number:</label>
    <asp:TextBox ID="EditDFIIdentification"  runat="server" ClientIDMode="Static" Text="" MaxLength="9"  />
    <asp:RequiredFieldValidator ID="DFIRequiredFieldValidator" runat="server"
      ControlToValidate="EditDFIIdentification"
      EnableClientScript="false"
      Text="*"  />
    <asp:RegularExpressionValidator ID="DFIIdentificationValidator"
      runat="server" 
      ControlToValidate="EditDFIIdentification"
      EnableClientScript="false"
      Text="9 num"
      ValidationExpression="\d{9}" >
    </asp:RegularExpressionValidator>
    <br />
    <label for="EditAccountNumber" class='editlabel'>Account Number:</label>
    <asp:TextBox ID="EditAccountNumber"  runat="server" ClientIDMode="Static" Text="" MaxLength="17" />
    <asp:RequiredFieldValidator ID="AccountFieldValidator" runat="server"
      ControlToValidate="EditAccountNumber"
      EnableClientScript="false"
      Text="*"  />   
    <asp:RegularExpressionValidator ID="EditAccountNumberValidator"
      runat="server" 
      ControlToValidate="EditAccountNumber"
      EnableClientScript="false"
      Text="17 num"
      ValidationExpression="\d{2,17}">
    </asp:RegularExpressionValidator>
    <br />
    <div id='edit-payee-container-menu'>
    <asp:LinkButton runat="server" ID="SaveEditedPayeeLinkButton" 
      CssClass="payee-save-button" ClientIDMode="Static" Text="" 
      ViewStateMode="Disabled" 
      onclick="SaveEditedPayeeLinkButton_Click"  />
    <a href="#" id='clear-link' class="payee-save-button" >Cancel</a>

    <asp:Label ID="MessageLabel" runat="server" Text="" />
    </div>
  </fieldset>
  </div>
  <asp:ListView runat="server" ID="PayeeListView"
     OnItemDeleting="PayeeListView_ItemDeleting" 
    ondatabound="PayeeListView_DataBound">
    <LayoutTemplate>
      <table id='payees-table' class="standard-table" >
        <thead>
          <tr>
            <th class="table-header">Id</th>            
            <th class="table-header">Routing Number</th>
            <th class="table-header">Account</th>
            <th class="table-header"></th>
          </tr>
          <tr>
            <td colspan="4">       
              <asp:DataPager runat="server" ID="PayeesPagerTop" PageSize="100">
                <Fields>
                  <asp:NumericPagerField  NextPageText=">" PreviousPageText="<" />
                </Fields>
              </asp:DataPager>
            </td>
          </tr>
        </thead>
        <tbody>
          <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
        </tbody>
        <tfoot>
          <tr>
            <td colspan="4">       
              <asp:DataPager runat="server" ID="PayeesPagerBottom" PageSize="100">
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
        <td class="payee-data-column">
          <asp:HiddenField ID="PayeeIDHiddenField" runat="server"
            Value='<%# Eval("PayeeID") %>'/>
          <span><%# Eval("TruckwinId") %></span></td>
        <td class="payee-data-column"><%# Eval("DFIIdentification")%></td>
        <td class="payee-data-column"><%# Eval("AccountNumber") %></td>
        <td>
          <asp:LinkButton ID="DeleteButton" runat="server" CommandName="Delete" Text="Delete" CssClass="payee-delete-button" />
        </td>
      </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
       <tr class="alternating-row" >
        <td class="payee-data-column">
          <asp:HiddenField ID="PayeeIDHiddenField" runat="server"
            Value='<%# Eval("PayeeID") %>'/>
          <span><%# Eval("TruckwinId") %></span></td>
        <td class="payee-data-column"><%# Eval("DFIIdentification")%></td>
        <td class="payee-data-column"><%# Eval("AccountNumber") %></td>
        <td>
          <asp:LinkButton ID="DeleteButton" runat="server" CommandName="Delete" Text="Delete" CssClass="payee-delete-button" />
        </td>
      </tr>
    </AlternatingItemTemplate>
  </asp:ListView>
  <input type="hidden" runat="server" id="PayeeTypeKeyHiddenInput" />
  <input type="hidden" runat="server" id="IdKeyHiddenInput" />

  
</asp:Content>


