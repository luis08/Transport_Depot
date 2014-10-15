<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ACHFilePreview.aspx.cs" Inherits="ACH_ACHFilePreview" %>

<asp:Content ID="ACHPReviewHeaderContent" ContentPlaceHolderID="HeadContent" Runat="Server">
  <link href="../Styles/ACHFilePreview.aspx.css" rel="stylesheet" type="text/css" />
  <title>ACH File Preview</title>
</asp:Content>
<asp:Content ID="ACHPreviewContent" ContentPlaceHolderID="MainContent" Runat="Server">
  <h2>ACH File Preview</h2>
  <div id="payment-container" class="preview-container" >
      <table id="payments-due-table" >
        <thead>
        </thead>
        <tbody id="SettlementsDueTableBody" runat="server">
          
        </tbody>
      </table>    
  </div>

  <div id="file-container" class="preview-container" >
    <asp:TextBox ID="FileTextBox" runat="server" TextMode="MultiLine" Width="900px" Height="500px" />
  </div>
  <div id="menu-container"></div>
</asp:Content>

