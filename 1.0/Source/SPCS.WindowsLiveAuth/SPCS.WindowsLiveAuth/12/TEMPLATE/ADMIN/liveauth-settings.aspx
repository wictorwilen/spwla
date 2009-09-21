<%@ Page Language="C#" Inherits="SPCS.WindowsLiveAuth.LiveAuthSettings, SPCS.WindowsLiveAuth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4b37e1a71dac8e81"
    MasterPageFile="~/_admin/admin.master"
     AutoEventWireup="false" 
     EnableViewState="true"%>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.SharePoint.ApplicationPages.Administration, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="LiveControls" Namespace="SPCS.WindowsLiveAuth" Assembly="SPCS.WindowsLiveAuth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4b37e1a71dac8e81" %>
<%@ Register Tagprefix="AdminControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint.ApplicationPages.Administration" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBar" Src="~/_controltemplates/ToolBar.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBarButton" src="~/_controltemplates/ToolBarButton.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" Src="/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" Src="/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" Src="/_controltemplates/ButtonSection.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
	Windows Live ID Configuration
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
	<SharePoint:DelegateControl runat="server" id="DelctlProfileRedirection" ControlId="ProfileRedirection" Scope="Farm" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
	Windows Live ID Configuration
</asp:Content>
<asp:content ID="Content5" contentplaceholderid="PlaceHolderPageDescription" runat="server">
	Use this page to configure Windows Live Authentication for the Web Application
</asp:content>
<asp:Content ID="Content4" ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <table width="100%" class="propertysheet" cellspacing="0" cellpadding="0" border="0">
        <tr> 
            <td class="ms-descriptionText"> 
                <asp:Label ID="LabelMessage" Runat="server" EnableViewState="False" CssClass="ms-descriptionText"/> 
            </td> 
        </tr> 
        <tr> 
            <td class="ms-error">
                <asp:Label ID="LabelErrorMessage" Runat="server" EnableViewState="False" />
            </td> 
        </tr> 
        <tr> 
            <td class="ms-descriptionText"> 
                <asp:ValidationSummary ID="ValSummary" ValidationGroup="valgroup1" HeaderText="<%$SPHtmlEncodedResources:spadmin, ValidationSummaryHeaderText%>" DisplayMode="BulletList" ShowSummary="True" runat="server"> </asp:ValidationSummary> 
                <asp:ValidationSummary ID="ValSummary2" ValidationGroup="valgroup2" HeaderText="<%$SPHtmlEncodedResources:spadmin, ValidationSummaryHeaderText%>" DisplayMode="BulletList" ShowSummary="True" runat="server"> </asp:ValidationSummary> 
            </td> 
        </tr> 
        <tr> 
            <td>
                <img src="/_layouts/images/blank.gif" width="10" height="1" alt="" />
            </td> 
        </tr> 
    </table>
    <input type="hidden" id="HidVirtualServerUrl" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" width="100%">
        <wssuc:InputFormSection runat="server" Title="<%$Resources:spadmin, multipages_webapplication_title%>" Description="<%$Resources:spadmin, multipages_webapplication_desc%>" >
            <Template_InputFormControls>
                <tr>
                    <td>
                        <SharePoint:WebApplicationSelector id="Selector" runat="server" OnContextChange="OnContextChange" />
                    </td>
                </tr>
            </Template_InputFormControls>
        </wssuc:InputFormSection>
        <wssuc:InputFormSection runat="server" Title="Status" >
            <Template_Description>
                Status of current Web Application
            </Template_Description>
            <Template_InputFormControls>                
                <asp:Panel id="statusPanel" runat="server"></asp:Panel>
                
            </Template_InputFormControls>            
        </wssuc:InputFormSection>
        <wssuc:InputFormSection runat="server" Title="Windows Live ID login zone" >
            <Template_Description>
                Choose which zone to configure to use the Windows Live ID Authentication. Leave (none) if you only would like to add the membership providers.
            </Template_Description>
            <Template_InputFormControls>                
            
                <wssuc:InputFormControl runat="server" LabelText="Choose Zone" Visible="True">
                    <Template_Control>
                        <asp:DropDownList id="ddZones" runat="server" CssClass="ms-input"/>
                    </Template_Control>
                </wssuc:InputFormControl>
            </Template_InputFormControls>            
        </wssuc:InputFormSection>
        
        <wssuc:InputFormSection runat="server" Title="Windows Live ID Authentication Configuration" >
            <Template_Description>
                Enter the Windows Live ID settings that you get from the Windows Live ID configuration portal.
            </Template_Description>
            <Template_InputFormControls>
                <wssuc:InputFormControl runat="server" LabelText="Application ID" Visible="True">
                    <Template_Control>
                        <asp:TextBox id="tbApplicationId" runat="server" CssClass="ms-input" Width="250px"/>
                        <asp:RequiredFieldValidator runat="server" id="tbApplicationIdReq" ControlToValidate="tbApplicationId" Text="*" ErrorMessage="Application Id Required"  ValidationGroup="valgroup1" EnableClientScript="false"/>
                    </Template_Control>
                </wssuc:InputFormControl>
            
                <wssuc:InputFormControl runat="server" LabelText="Secret Key" Visible="True">
                    <Template_Control>
                        <asp:TextBox id="tbApplicationKey" runat="server" CssClass="ms-input" Width="250px"/>
                        <asp:RequiredFieldValidator runat="server" id="tbApplicationKeyReq" ControlToValidate="tbApplicationKey" Text="*" ErrorMessage="Application Key Required"  ValidationGroup="valgroup1" EnableClientScript="false"/>
                    </Template_Control>
                </wssuc:InputFormControl>
            </Template_InputFormControls>
            
        </wssuc:InputFormSection>

        <wssuc:InputFormSection runat="server" Title="Windows Live ID Profile Settings" >
            <Template_Description>
                Fill in the settings for your Windows Live ID enabled site. <em>Domain</em> must match the Windows Live registered domain. The <em>Profile Site</em> should be a secured SharePoint site where all user data will be stored.
                
            </Template_Description>
            <Template_InputFormControls>
                <wssuc:InputFormControl runat="server" LabelText="Domain" Visible="True">
                    <Template_Control>
                        <asp:TextBox id="tbDomain" runat="server" CssClass="ms-input" Width="250px"/>
                        <asp:RequiredFieldValidator runat="server" id="tbDomainReq" ControlToValidate="tbDomain" Text="*" ErrorMessage="Domain needs to be specified"  ValidationGroup="valgroup1" EnableClientScript="false"/>
                    </Template_Control>
                </wssuc:InputFormControl>
                <wssuc:InputFormControl runat="server" LabelText="Create/overwrite Windows Live ID Profile lists" Visible="True">
                    <Template_Control>
                        <asp:CheckBox ID="cbCreateLists" runat="server" Text="" />
                    </Template_Control>
                </wssuc:InputFormControl>
                <wssuc:InputFormControl runat="server" LabelText="Profile Site Url" Visible="True" >
                    <Template_Control>
                        <asp:TextBox id="tbProfileSiteUrl" runat="server" CssClass="ms-input" Width="250px"/>
                        <asp:RequiredFieldValidator runat="server" id="tbProfileSiteUrlReq" ControlToValidate="tbProfileSiteUrl" Text="*" ErrorMessage="Profile Site Url required"  ValidationGroup="valgroup1" EnableClientScript="false"/>
                    </Template_Control>
                </wssuc:InputFormControl>
            
                <wssuc:InputFormControl runat="server" LabelText="Profile List Name" Visible="True">
                    <Template_Control>
                        <asp:TextBox id="tbProfileListName" runat="server" CssClass="ms-input"/>
                        <asp:RequiredFieldValidator runat="server" id="tbProfileListNameReq" ControlToValidate="tbProfileListName" Text="*" ErrorMessage="Profile List needs a name" ValidationGroup="valgroup1" EnableClientScript="false"/>
                    </Template_Control>
                </wssuc:InputFormControl>
                
                <wssuc:InputFormControl runat="server" LabelText="Profile Sync List Name" Visible="True">
                    <Template_Control>
                        <asp:TextBox id="tbSyncListName" runat="server" CssClass="ms-input"/>
                        <asp:RequiredFieldValidator runat="server" id="tbSyncListNameReq" ControlToValidate="tbSyncListName" Text="*" ErrorMessage="Profile Sync List needs a name" ValidationGroup="valgroup1" EnableClientScript="false"/>
                    </Template_Control>
                </wssuc:InputFormControl>
                
                <wssuc:InputFormControl runat="server" LabelText="Url for locked accounts" Visible="True">
                    <Template_Control>
                        <asp:TextBox id="tbLockedUrl" runat="server" CssClass="ms-input" Width="250px"/>
                    </Template_Control>
                </wssuc:InputFormControl>
                
            </Template_InputFormControls>            
        </wssuc:InputFormSection>
        
        <wssuc:InputFormSection runat="server" Title="Windows Live ID Advanced Settings" >
            <Template_Description>
                
            </Template_Description>
            <Template_InputFormControls>
                
                <wssuc:InputFormControl runat="server" LabelText="Approve all new users" Visible="True">
                    <Template_Control>
                        <asp:CheckBox ID="cbApprove" runat="server" Text="" />
                    </Template_Control>
                </wssuc:InputFormControl>
                
                <wssuc:InputFormControl runat="server" LabelText="Use HTTPS" Visible="True">
                    <Template_Control>
                        <asp:CheckBox ID="cbHttps" runat="server" Text="" />
                    </Template_Control>
                </wssuc:InputFormControl>
                
                <wssuc:InputFormControl runat="server" LabelText="Enable Delegated Authentication" Visible="True">
                    <Template_Control>
                        <asp:CheckBox ID="cbDelegated" runat="server" Text="" />
                    </Template_Control>
                </wssuc:InputFormControl>
                <wssuc:InputFormControl runat="server" LabelText="Url to policy page" Visible="True">
                    <Template_Control>
                        <asp:TextBox ID="tbPolicyPage" runat="server" Text="" CssClass="ms-input" Width="250px"/>
                        <asp:RequiredFieldValidator runat="server" id="tbPolicyPageReq" ControlToValidate="tbPolicyPage" Text="*" ErrorMessage="Policy page is required when using Delegated Authentication"  ValidationGroup="valgroup2" EnableClientScript="false"/>
                    </Template_Control>
                </wssuc:InputFormControl>
            
            </Template_InputFormControls>            
        </wssuc:InputFormSection>
        <wssuc:ButtonSection runat="server">
            <template_buttons>
                <asp:Button runat="server" class="ms-ButtonHeightWidth" OnClick="Delete_Click" Text="Clear settings" id="butDelete" />
                <asp:Button runat="server" class="ms-ButtonHeightWidth" OnClick="Submit_Click" Text="Submit" id="butSubmit" />
            </template_buttons>
        </wssuc:ButtonSection>
        <tr> 
            <td style="width:30%">&nbsp;</td> <td class="ms-descriptiontext" colspan="2">
                    Created by <a href="http://www.wictorwilen.se">Wictor Wilén</a>, source and license at <a href="http://spwla.codeplex.com/">http://spwla.codeplex.com/</a>
            </td> 
        </tr>
    </table>
</asp:Content>
