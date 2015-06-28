<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
<style type="text/css">
  #links-list a { display:block; padding:0.25em; }
</style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<h2>Only Authorized Users</h2>
<p>You need access to this system.</p>
<asp:Label ID="TestLabel" runat="server" Text="" />

<ul id='links-list' style="list-style-type:none">
</ul>
</asp:Content>
<asp:Content ID="JSContent" runat="server" ContentPlaceHolderID="JavaScriptPlaceHolder">
  <script type="text/javascript">
    $(document).ready(function () {
      var uri = "http://192.168.1.200/ft-svc/Business.svc/ajax/GetMainLinks";
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