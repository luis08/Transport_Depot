<%@ Page Title="Change Defaults" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
  CodeFile="ChangeDefaults.aspx.cs" Inherits="ChangeDefaults" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server"> 
    <script type='text/javascript'>

    </script>
    <style type="text/css">
        div.header_message 
        {
            text-align: center; 
            margin-left: auto; 
            margin-right: auto; 
            margin-top: 40px;
            font-family: Times New Roman; 
            font-size: x-large; 
            font-weight: bold;
            background-color: transparent;
        }
        .message
        {
            color: Red;
            text-align: center;
            clear: both;
            font-weight: bold;
        }
        table.gridview
        {
            background-color: #5185FF;
            padding: 0;
            border-collapse: separate;
            border-color: Black;
            margin-top: 30px;
        }
        table.gridview th
        {
            color: #fff;
            background-color: #5185FF;
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
            background-color: #EAFDE1;
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
        .drop_downList
        {
            padding: 5px 0 5px 2px;
            float: left;
        }
        .save_button
        {
            padding: 10px 20px; 
            float: right;
            margin-top: 30px;
        }
        .add_default_button
        {
            padding: 10px 20px; 
            float: right;
        }
        .save_default_button
        {
            padding: 10px 25px;  
        }
        .cancel_default_button
        {
            padding: 10px 20px; 
        }
        div.opacity_div 
        {
            width: 100%; 
            height: 100%; 
            background-color: #fff; 
            position: absolute; 
            left: 0; 
            top: 0; 
            z-index: 4; 
            opacity: 0.3;
            filter: alpha(opacity=30);
        }
        div.add_default_outter
        {
            width: 500px; 
            height: 220px; 
            position: absolute; 
            left: 50%; 
            top: 50%; 
            z-index: 5;
            opacity: 1;
            filter: alpha(opacity=1);
        }
        div.add_default_inner
        {
            position: relative; 
            left: -50%; 
            top: -50%; 
            width: 100%; 
            height: 100%; 
            border: solid red 1px; 
            background-color: #fff;
            text-align: center;
            padding: 20px;
        }
        .default_textbox
        {
            padding: 6px 0 6px 2px;
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div runat="server" id="main_working_area">
        <div class="header_message">
            Use the form below to update Lessor's defaults
        </div>
        <div style="margin-left: auto; margin-right: auto; text-align: center; min-width: 60%;">
            <div style="display:inline-table;">
                <div style="padding: 30px 0 5px 0;">
                    <asp:Label runat="server" ID="lbMessage" CssClass="message" ></asp:Label><br /><br />
                </div>
                <asp:DropDownList ID="LessorIDs" CausesValidation="false" ClientIDMode="Static" 
                    Width="200" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Change_Lessor" CssClass="drop_downList" ></asp:DropDownList>
                <asp:Button ID="add_defaults" runat="server" Text="Add New Rate" CssClass="add_default_button" OnClick="Add_Defaults_Open"/>
                <div style="padding: 30px 0 20px;">
                <span runat="server" id="work_area">
                    <span runat="server" id="inner_work_area">
                        <asp:GridView 
                            ID="LessorsDefaultGridView"   
                            runat="server"
                            AutoGenerateColumns="False" 
                            OnRowDataBound="GridView_RowDataBound"
                            AllowPaging="False"
                            AlternatingRowStyle-BackColor="#FDFEFA"
                            CssClass="gridview outer_table" BorderWidth="1">
                            <AlternatingRowStyle BackColor="#FDFEFA"></AlternatingRowStyle>
                            <Columns>
                                <asp:BoundField  DataField="LessorId" HeaderText="Lessor ID" ReadOnly="true" />
                                <asp:TemplateField AccessibleHeaderText="CommissionSelector" HeaderText="Commission" ItemStyle-Width="200px" ItemStyle-CssClass="dListColumn" >
                                    <ItemTemplate>
                                        <asp:DropDownList ID="commissionSelector" Width="70" runat="server" AutoPostBack="false" ></asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="LiabilitySelector" HeaderText="Liability" ItemStyle-Width="200px" ItemStyle-CssClass="dListColumn" >
                                    <ItemTemplate>
                                        <asp:DropDownList ID="liabilitySelector" Width="70" runat="server" AutoPostBack="false" ></asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="CargoSelector" HeaderText="Cargo" ItemStyle-Width="200px" ItemStyle-CssClass="dListColumn" >
                                    <ItemTemplate>
                                        <asp:DropDownList ID="cargoSelector" Width="70" runat="server" AutoPostBack="false" ></asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle BackColor="#5185FF" BorderStyle="Solid" BorderColor="Black" BorderWidth="1px" />
                        </asp:GridView>
                        <asp:Button ID="save_defaults" runat="server" Text="Save" CssClass="save_button" OnClick="Save_Defaults" />
                    </span> 
                </span>
                </div>
            </div>
        </div>
    </div>
    <div runat="server" id="add_default" class="add_default_outter">
        <div class="add_default_inner">
            <div style="height: 60px">
                <label style="font-size: 16px; font-weight: 800;">Use the form below to add a default setting.</label>
            </div>
            <div style="height: 100px;">
                <table style="width: 100%">
                    <tr>
                        <th>Type</th>
                        <th>Value</th>
                    </tr>
                    <tr>
                        <td style="width: 50%">
                            <asp:DropDownList ID="DropDownListAddDefault" Width="200" runat="server" AutoPostBack="false" CssClass="drop_downList" ></asp:DropDownList>
                        </td>
                        <td style="width: 50%">
                            <asp:TextBox runat="server" ID="Default_Val_TextBox" CssClass="default_textbox"></asp:TextBox><label>  %</label>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>Example: 20 = 20%</td>
                    </tr>
                </table>
                <div style="padding-top: 10px; text-align: center;">
                    <asp:Label runat="server" ID="add_default_message" CssClass="message" ></asp:Label>                    
                </div>
            </div>
            <div style="height: 60px; width: 100%; margin-top: 20px;">
                <div style="width: 50%; text-align: center; float:left;">
                    <asp:Button ID="Save_Add_Default" runat="server" Text="Save" CssClass="save_default_button" OnClick="Add_Default_Dialog_Clicked" />                    
                </div>
                <div style="width: 50%; text-align: center; float: right;">
                    <asp:Button ID="Cancel_Add_Default" runat="server" Text="Cancel" CssClass="cancel_default_button" OnClick="Close_Add_Default_Dialog" />                                    
                </div>
            </div>
        </div>    
    </div>
    <div runat="server" id="opacity_div" class="opacity_div" ></div>
</asp:Content>
