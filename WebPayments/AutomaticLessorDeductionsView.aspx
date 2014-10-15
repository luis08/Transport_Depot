<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AutomaticLessorDeductionsView.aspx.cs" Inherits="AutomaticLessorDeductionsView" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script src="Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <style type="text/css">
        div.header_ald 
        {
            text-align: center; 
            margin-left: auto; 
            margin-right: auto; 
            margin-bottom: 50px; 
            font-family: Times New Roman; 
            font-size: x-large; 
            font-weight: bold;
            background-color: transparent;
        }
        div.select_button
        {
            text-align: left;
            padding: 0 0 20px; 
            margin-left: auto; 
            margin-right: auto;
			float: left;
        }
        tr.top_row
        {
            background-color: #CCFFCC;
        }
        tr.alt_row
        {
            background-color: #fff;
        }
        span.default 
        {
            margin-left: 20px;
            visibility: hidden;
        }
        input.select {}
        .gridview
        {
            background-color: #5185FF;
            margin-left: auto;
            margin-right: auto;
            padding: 0;
            border-collapse: separate;
            border-color: Black;
			display: block;
			clear: both;
        }
        table.gridview th
        {
            color: #fff;
            background-color: #4b6c9e;/*#5185FF;*/
            font-size: 15px;
            font-weight: 900;
            padding: 12px 25px;
        }
        table.gridview td
        {
            text-align: center;
            font-size: 13px;
            color: Black;
            padding: 8px 0;
            margin: 0;
            border-width: 1px;
            border-color: Black;
            height: 30px;
            vertical-align: middle;
        }
        table.gridview tr
        {
            background-color: #FDFEFA;
            margin: 0;
            font-size: 11px;
            padding: 0;
            margin: 0;   
            vertical-align: middle;   
        }
        table.gridview td.dListColumn 
        {
            padding-right: 20px;   
        }
        .h_cBox 
        {
            line-height: 22px;
        }
        .label_message
        {
            text-align:right;
            color: #3D7DDE;
            font-weight: bolder; 
            padding-right: 20px;
            padding-top: 10px; 
            float: right;
        }
        .error_chkBox 
        {
            visibility: hidden;
        }
    </style>
    <script type="text/javascript">
	  
      /*$(document).ready(function () {
        $('form:first').submit(function (onSubmit) {
          var selectedDeductions = $('.update_checkboxes input:checked');
          if (selectedDeductions.length == 0) {
            onSubmit.preventDefault();
            alert('You must select at least one row in order to update!');
            return;
          }
          $('#submit_button').attr('disabled', 'disabled');
        });
        $(".setDefault").each(function () {
          var showThis = (!$(this).has('input:checked'));
          $(this).css('visibility', showThis ? 'visible' : 'hidden');
        });
        $('#selectAll').click(function () {
          if ($(this).val() === 'Select All') {
            $(this).val('Unselect All');
            $('.update_checkboxes input[type=checkbox]').prop('checked', true);
          } else {
            $(this).val('Select All');
            $('.update_checkboxes input[type=checkbox]').prop('checked', false);
          }
        });
      });
      function dListChanged(ctrl) {
        $(ctrl).parent().siblings('.setDefault').css('visibility', 'visible');
    }*/
        $(document).ready(function () {
            $("#main_form").submit(function () {
                return SubmitButton();
            });
            $("#selectAll").click(function () {
                select_all();
            });
            
            $(".setDefault").each(function () {
                var showThis = (!$(this).has('input:checked'));
                $(this).css('visibility', showThis ? 'visible' : 'hidden');
            });
        });
        $(window).ready(function () {
            $("#outer_div").css("width", $(".outer_table").css("width") + 2 );            
            var empty = $("#empty_table").get(0);
            if (empty != null) {
                $("#empty_table").removeAttr("border");
            }
        });
        function OnSaveClicked(val) {
            $("#submit_button").get(0).disabled = val;
        }
        function SubmitButton() {
            OnSaveClicked(true);
            var checkBoxes = $(".update_checkboxes");
            for (var i = 0; i < checkBoxes.length; i++) {
                var checkBox = checkBoxes[i].firstElementChild || checkBoxes[i].children[0];
                if (checkBox.checked) {
                    return true;
                }
            }
            alert("You must select at least one row in order to update!");
            OnSaveClicked(false);
            return false;
        }
        function select_all() {
            var checkBoxes = $(".update_checkboxes");
            if (checkBoxes.length <= 0) {
                return;
            }       
            var s = $("#selectAll").get(0);
            var checked = true;
            if (s.value == "Select All") {
                s.value = "Unselect All";
            }
            else {
                s.value = "Select All";
                checked = false;
            }
            for (var i = 0; i < checkBoxes.length; i++) {
                var cxBox = checkBoxes[i].firstElementChild || checkBoxes[i].children[0];
                cxBox.checked = checked;
            }
        }
        function dListChanged(ctrl) {
            $(ctrl).parent().siblings('.setDefault').css('visibility', 'visible');
        }
</script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <div id="Grid_View" style="padding: 20px 0; text-align: center;">
        <div class="header_ald" id="head">
            Use the table to select automatic deductions to apply
        </div>
        <div id="outer_div" style="display: inline-block; margin: 0 auto; width: auto;">
            <div class="select_button">
                <input id="selectAll" type="button" value="Unselect All" style="padding: 15px 0; width: 150px"/>
                <asp:Label id="label_message" runat="server" CssClass="label_message"></asp:Label>
            </div>
            <asp:GridView 
                ID="LessorsGridView"   
                runat="server"
                AutoGenerateColumns="False" 
                OnRowDataBound="GridView_RowDataBound"
                AllowPaging="False"
                AlternatingRowStyle-BackColor="#EAFDE1"
                BorderWidth="1"
                CssClass="gridview outer_table">
                <AlternatingRowStyle BackColor="#EAFDE1"></AlternatingRowStyle>
                <EmptyDataRowStyle BorderWidth="0" BorderStyle="None" />
                <EmptyDataTemplate>
                    <table id="empty_table" class="gridview" style="border: none">
                        <tr style="padding: 0; margin: 0; border: none">
                            <th style="width:100px">Update?</th>
                            <th>Lessor ID</th>
                            <th>Trip Number</th>
                            <th>Commission</th>
                            <th>Liability</th>
                            <th>Cargo</th>
                        </tr>
                        <tr>
                            <td colspan="6">There is no data to update!</td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <EmptyDataRowStyle CssClass="EmptyData" />
                <Columns>
                    <asp:TemplateField AccessibleHeaderText="LessorSelector" HeaderText="Update?" ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:checkbox ID="updateSelector" class="update_checkboxes" runat="server" AutoPostBack="false" checked='<%#Eval("Update")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField  DataField="LessorId" HeaderText="Lessor ID" ReadOnly="true" />
                    <asp:BoundField DataField="TripNumber" HeaderText="Trip Number" ReadOnly="true" />
                    <asp:TemplateField AccessibleHeaderText="RateSelector" HeaderText="Commission" ItemStyle-Width="200px" ItemStyle-CssClass="dListColumn" >
                        <ItemTemplate>
                            <div style="float: left; margin-left: 20px; vertical-align: middle; line-height: 30px">
                                <asp:DropDownList ID="rateSelector" Width="70" runat="server" AutoPostBack="false" onchange="dListChanged(this)" ></asp:DropDownList>
                                <div style="clear:both; "></div>  
                            </div>
                            <div class="setDefault" style="float: right; margin-right: 0; clear: right; visibility: collapse">
                                <div style="height: 8px; line-height: 8px; margin-top: -5px;"><b><u>Set as Default?</u></b></div>
                                <div style="margin-top: 4px;">
                                    <asp:checkbox ID="changeCommission" CssClass="change_default_checkboxes" runat="server" AutoPostBack="false" checked='<%#Eval("ChangeCommissionDefault")%>' />
                                </div>
                                <div style="clear:both; "></div>                              
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="LiabilityField" HeaderText="Liability" ReadOnly="true" />
                    <asp:BoundField DataField="CargoField" HeaderText="Cargo" ReadOnly="true" />
                </Columns>
                <HeaderStyle BackColor="#5185FF" BorderStyle="Solid" BorderColor="Black" BorderWidth="1px" />
            </asp:GridView>
            <div style="padding: 30px 0; margin-left: auto; margin-right: auto;">
                <input id="submit_button" type="submit" value="Save" name="ChangeDeductions" style="padding: 10px 20px; float: right" />
            </div>
            <div style="display: block; clear: both"></div>
        </div>
    </div>    
</asp:Content>
