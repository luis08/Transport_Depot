<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
<style type="text/css">
  .links { list-style-type:none; }
  .links a { display:block; padding:0.25em; list-style-type:none; }
  .links-container { width: 30%; float:left; }
</style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<h2>Only Authorized Users</h2>
<p>You need access to this system.</p>
<asp:Label ID="TestLabel" runat="server" Text="" />
<div class="links-container">
<ul id='links-list' class="links" style="list-style-type:none">
</ul>
</div>
<div class="links-container">
  <ul class="links" >
    <li><a target="_blank" href="http://transportserver/json-reports/">Custom Reports</a>
      http://192.168.1.242/trans/Safety/Maintenance.aspx
    <li><a target="_blank" href="http://transportserver/trans/Safety/Maintenance.aspx">Maintenance</a></li>
  </ul>
  <br/>
  <p><strong>J on Server</strong></p>
  <p>\\TRANSPORTSERVER\shared</p>
  <br/>
</div>
</asp:Content>
<asp:Content ID="JSContent" runat="server" ContentPlaceHolderID="JavaScriptPlaceHolder">
  <script type="text/javascript">
    $(document).ready(function () {
      var uri = "http://transportserver/test-svc/Business.svc/ajax/GetMainLinks";
      var ajaxRequest = $.ajax({
        type: "GET",
        url: uri
      });
      ajaxRequest.success(function (data) {

        $.each(data.GetMainLinksResult, function (idx, l) {
          var li = $('<li/>').append($('<a/>').attr('href', l.Link).attr('target', '_blank').append(l.Text));
          $(li).appendTo('#links-list');
        });
      });
    });
  </script>
</asp:Content>