<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Commissions.aspx.cs" Inherits="AccountsPayable_Commissions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <table>
    <thead>
    </thead>
    <tbody>
    </tbody>
      <asp:ListView ID="DispatcherListView" runat="server" 
      OnItemDataBound="DispatcherListView_ItemDataBound">
        <LayoutTemplate>
          <tr runat="server" id="itemPlaceholder" />
        </LayoutTemplate>
        <ItemTemplate>
          <tr runat="server" >
            <td>
              <asp:Label runat="server" ID="InitialsLabel" Text='<%# Bind("Initials") %>' />
            </td>
            <td>
              <asp:DropDownList runat="server" ID="VendorIdDropDown" CssClass="vendor"/>
            </td>
          </tr>
        </ItemTemplate>
    
      </asp:ListView>
  </table>
</asp:Content>

