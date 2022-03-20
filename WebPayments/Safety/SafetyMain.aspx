<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="Safety_SafetyMain" CodeFile="SafetyMain.aspx.cs"%>

<asp:Content ID="SafetyHeaderContent" ContentPlaceHolderID="HeadContent" runat="Server">
  <link href="../Styles/SafetyMain.aspx.css" rel="stylesheet" type="text/css" />
  <link href="../Styles/jQueryUI/smoothness/jquery-ui-1.10.4.custom.min.css" rel="stylesheet"
    type="text/css" />
  <style type="text/css">
      .ui-front{
          z-index: 5010;
      }
  </style>
  <script src="../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
  <script src="../Scripts/jquery-ui-1.10.4.custom.js" type="text/javascript"></script>
  <script type="text/javascript" src="../Scripts/json.js"></script>
  <script src="../Scripts/Utilities.js" type="text/javascript"></script>
  <script src="../Scripts/JSON.NET.js" type="text/javascript"></script>
  <script src="../Scripts/mustache.js" type="text/javascript"></script>
  <script id='driver-safety-template' type="text/template">
    {{#drivers}}
    <tr class='driver-row'>
      <td class='driver-id-container'>
        <span id='{{Id}}' class='driver-id'  >{{Id}}</span><br/>
        ({{#Active}}Active{{/Active}}{{^Active}}Inactive{{/Active}})
        <input type='hidden' class='driver-active' value='{{Active}}' />

        <a href="#" class='safety-entity-menu'>Save</a>
        <a href="#" class='safety-entity-menu'>Reset</a>
        <span class='last-saved-message'></span>
      </td>
      <td class='yesno-container'>
        <fieldset class='driver-fieldset'>
          <label><input type='checkbox' class='driver-application' {{#Application}} checked='checked' {{/Application}} />
            Application
          </label>
          <br/>
          <label><input type='checkbox' class='driver-on-duty-hours' {{#OnDutyHours}} checked='checked' {{/OnDutyHours}} />
            Duty Hrs
          </label>
          <br/>
          <label><input type='checkbox' class='driver-drug-test' {{#DrugTest}} checked='checked' {{/DrugTest}} />
            Drug Test
          </label>
          <br/>
          <label><input type='checkbox' class='driver-police-report' {{#PoliceReport}} checked='checked' {{/PoliceReport}} />
            Police Rpt
          </label>
          <br/>
          <label><input type='checkbox' class='driver-previous-employer' {{#PreviousEmployerForm}} checked='checked' {{/PreviousEmployerForm}} />
            Prev Emp
          </label>
          <br/>
          <label><input type='checkbox' class='driver-social-security' {{#SocialSecurity}} checked='checked' {{/SocialSecurity}} />
            SSN
          </label>
          <br/>
          <label><input type='checkbox' class='driver-agreement' {{#Agreement}} checked='checked' {{/Agreement}} />
            Agreement
          </label>
          <br/>
          <label><input type='checkbox' class='driver-w9' {{#W9}} checked='checked' {{/W9}} />
            W9
          </label>
        </fieldset>
      </td>
      <td class='dates-container'>
        <fieldset class='driver-fieldset'>
        <label>Physical:</label><input type='text' class='driver-physical-expires' value='{{PhysicalExamExpiration}}' /><br/>
        <label>Last Log:</label><input type='text' class='driver-last-valid-log' value='{{LastValidLogDate}}' /><br/>
        <label>DL Exp:</label><input type='text' class='driver-licence-expiration' value='{{DriversLicenseExpiration}}' /><br/>
        <label>Annual V:</label><input type='text' class='driver-annual-certificate-violations' value='{{AnnualCertificationOfViolations}}' /><br/>
        <label>MVR:</label><input type='text' class='driver-mvr-expiration' value='{{MVRExpiration}}' />
        </fieldset>
      </td>
      <td>
        <fieldset class='driver-fieldset'>
          <label>Comments</label>
          <textarea cols='26' rows='8' class='driver-comments'>{{Comments}}</textarea>
      </td>
    
    </tr>
    {{/drivers}}
  </script>
  <script id='tractor-safety-template' type="text/template">
    {{#Tractors}}
    <tr class='tractor-row'>
      <td class='tractor-identification'>
        <input class="tractorId" type="hidden" value="{{Id}}" />
        <span id='tractor-{{Id}}' class='tractor-id'>{{Id}}</span>
        (<span class='tractor-unit'>{{Unit}}</span>)<br/>
        <span class='vin-number'>{{VIN}}</span>
        <a href='#' class='save-tractor-button' >Save</a>&nbsp;
        <a href='#' class='reset-tractor-button'>Reset</a>
        <br/>
        <br/>
        <span class='last-saved-message'>&nbsp;</span>
        <br />
        <a href="javascript:void(0)" class="open-maintenance-dialog">Maintenance</a>
      </td>
      <td>
        <!--<fieldset>-->
          <label>Lease Agrmt</label>
          <input type='text' class='lease-agreement' value='{{LeaseAgreementDue}}' />
          <br/>
          <label>Maintenance</label>
          <span class='maintenance-due'>{{LastMaintenance}}</span>
          <br/>
          <label>Ins Name</label>
          <input type='text' class='insurance-name' value='{{InsuranceName}}' />
          <br/>
          <label>Ins Exp</label>
          <input type='text' class='insurance-expiration' value='{{InsuranceExpiration}}' />
          <br/>
          <label>Registration</label>
          <input type='text' class='registration-expiration' value='{{RegistrationExpiration}}' />
        <!--</fieldset>-->
      </td>
      <td>
        <fieldset>
          <label>Inspection</label>
          <input type='text' class='inspection-due' value='{{InspectionDue}}' />
          <br/>
          <label>
          <input type='checkbox' class='tractor-w9' {{#HasW9}} checked='checked' {{/HasW9}} />
          W9
          </label>
          <br/>
          <label class='lessor-owner-label'>Owner</label>
          <select class='lessor-owner'>
            <option value=''></option>
            {{#Lessors}}
            <option value='{{Name}}' {{Selected}}>{{Name}}</option>
            {{/Lessors}}
          </select>
        </fieldset>
      </td>
      <td>
        <textarea cols='24' rows='8' class='tractor-comments'>{{Comments}}</textarea>
      </td>
    </tr>   
    {{/Tractors}}
  </script>
  <script id='driver-tracking-template' type='text/template'>
    <select id='untracked-drivers-select' class='tracking-select'>
      {{#untrackedDrivers}}
      <option value='{{Id}}'>{{Name}}</option>
      {{/untrackedDrivers}}
    </select>
    <a id='track-driver-button' href='#'>Track Driver</a>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <select id='tracked-drivers-select' class='tracking-select'>
      {{#trackedDrivers}}
      <option value='{{Id}}'>{{Name}}</option>
      {{/trackedDrivers}}
    </select>
    <a id='untrack-driver-button' href='#'>Untrack Driver</a>
  </script>
  <script id='trailer-safety-template' type="text/template">
    {{#Trailers}}
    <tr class='trailer-row'>
      <td class='trailer-identification' style="border-bottom:1px solid">
        
        <span id='trailer-{{Id}}' class='trailer-id'>{{Id}}</span>
        (<span class='trailer-unit'>{{Unit}}</span>)<br/>
        <span class='vin-number'>{{VIN}}</span>
        <a href='#' class='save-trailer-button' >Save</a>&nbsp;
        <a href='#' class='reset-trailer-button'>Reset</a>
        <br/>
        <br/>
        <span class='last-saved-message'>&nbsp;</span>
        <br/>
        <a href="javascript:void(0)" class="open-maintenance-dialog">Maintenance</a>
      </td>
      <td style="border-bottom:1px solid">
        <!--<fieldset>-->
          <label>Registration</label>
          <input type='text' class='registration-expiration' value='{{RegistrationExpiration}}' />
          <br/>
		  <label>Maintenance</label>
          <span class='maintenance-due'>{{LastMaintenance}}</span>
          <br/>
          <label>Inspection</label>
          <input type='text' class='inspection-due' value='{{InspectionDue}}' />
          <br/>
		  <label class='lessor-owner-label'>Owner</label>
          <select class='lessor-owner'>
            <option value=''></option>
            {{#Lessors}}
            <option value='{{Name}}' {{Selected}}>{{Name}}</option>
            {{/Lessors}}
          </select>
        <!--</fieldset>-->
      </td>
      <td>
        <textarea cols='24' rows='7' class='trailer-comments'>{{Comments}}</textarea>
      </td>
    </tr>   
    {{/Trailers}}
  </script>
  <script src="../Scripts/SafetyMain.aspx.js" type="text/javascript"></script>
  <script type="text/javascript">
    $(document).ready(SafetyMain.initialize);
  </script>
</asp:Content>
<asp:Content ID="SafetyBodyContent" ContentPlaceHolderID="MainContent" runat="Server">
  <div id='safety-main-container'>
    <div id='safety-menu-container'>
      <input type="radio" id="driver-safety-menu-option" name="radio" checked="checked" /><label for="driver-safety-menu-option">Driver</label>
      <input type="radio" id="tractor-safety-menu-option" name="radio"  /><label for="tractor-safety-menu-option">Tractor</label>
      <input type="radio" id="trailer-safety-menu-option" name="radio"  /><label for="trailer-safety-menu-option">Trailer</label>
    </div>
    <div id='safety-report-menu'>
      <a href="#" 
          id='open-safety-report' target="_blank">Driver Safety Report</a>
          &nbsp;&nbsp;|&nbsp;&nbsp;
          <a href="http://transportserver/trans/Safety/SafetyMain.aspx" target="_blank" id="pending-maintenance-report">Maintenance</a>
    </div>
    <div id='driver-track-menu'>
      
    </div>
    <div id='driver-safety-container'>
      <div id='scrollable-drivers-container'>
      <table >
        <tbody id='driver-list-container'>
        </tbody>
      </table>
      </div>
      <div id='driver-menu-container'>
        <h4>Go To Driver</h4>
        <ul id='driver-picker'>
        </ul>
      </div>
    </div>
    <div id='tractor-safety-container'>
      <div id='scrollable-tractors-container'>
        <table >
          <tbody id='tractor-list-container'>
          </tbody>
        </table>
      </div>
      <div id='tractor-menu-container'>
        <h4>Go To Tractor</h4>
        <ul id='tractor-picker'>
        </ul>
      </div>
    </div>
    <div id='trailer-safety-container'>
      <div id='scrollable-trailers-container'>
        <table>
          <tbody id='trailer-list-container'>
          </tbody>
        </table>
      </div>
      <div id='trailer-menu-container'>
        <h4>Go To Trailer</h4>
        <ul id='trailer-picker'>
        </ul>
      </div>
    </div>
  </div>
  <div class="dialog-overlay"></div>
  <div id="maintenance-dialog" class="overlaid-dialog tractor-maintenance-dialog">
    <!--Header-->
    <div></div>
    <!--Body-->
    <div>
        <fieldset>
            <legend id="dialog-title"></legend>
            <label id="dialog-id-label"></label>
            <input name="vehicleId" type="text" />
            <br/>
            <label>Performed Date</label>
            <input name="performedDate" type="text" />
            <br />
            <label>Mileage</label>
            <input name="mileage" type="number" />
            <input name="maintenanceType" type="hidden" value="MAINT"/>
            <br />
            <div>
              <label style="width:auto">
                Description</label><br />
              <textarea name="maintenanceDescription" cols='35' rows='8' class='driver-comments'></textarea>
            </div>
        </fieldset>
        <div class="menu">
        <a href="javascript:void(0)" name="save-maintenance">Save</a>
        <a href="javascript:void(0)" name="cancel-maintenance">Cancel</a>
        </div>
    </div>
  </div>
</asp:Content>

