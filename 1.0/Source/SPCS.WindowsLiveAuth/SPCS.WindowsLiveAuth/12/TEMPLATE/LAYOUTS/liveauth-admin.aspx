<%@ Page Language="C#" Inherits="SPCS.WindowsLiveAuth.LiveAuthAdmin, SPCS.WindowsLiveAuth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4b37e1a71dac8e81"
    MasterPageFile="~/_layouts/application.master"
     AutoEventWireup="false" 
     EnableViewState="true"%>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="LiveControls" Namespace="SPCS.WindowsLiveAuth" Assembly="SPCS.WindowsLiveAuth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4b37e1a71dac8e81" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBar" Src="~/_controltemplates/ToolBar.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBarButton" src="~/_controltemplates/ToolBarButton.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" Src="/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" Src="/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" Src="/_controltemplates/ButtonSection.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
	Windows Live ID Administration
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
	<SharePoint:DelegateControl runat="server" id="DelctlProfileRedirection" ControlId="ProfileRedirection" Scope="Farm" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
	Windows Live ID Administration
</asp:Content>
<asp:content ID="Content5" contentplaceholderid="PlaceHolderPageDescription" runat="server">
	Configure your Windows Live ID profiles
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
            <td>
                <img src="/_layouts/images/blank.gif" width="10" height="1" alt="" />
            </td> 
        </tr> 
    </table>
    <input type="hidden" id="HidVirtualServerUrl" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" width="100%">
        <wssuc:InputFormSection runat="server" Title="Announcements" Description="Enable announcements for Windows Live ID profile changes. Available lists are lists having a 'Body' and an 'Expires' field. If the list contains an 'Image' field, profile images will be published also." >
            <Template_InputFormControls>
                <wssuc:InputFormControl runat="server" LabelText="Enable" Visible="True">
                    <Template_Control>
                        <asp:CheckBox id="cbEnable" runat="server" CssClass="ms-input" />
                    </Template_Control>
                </wssuc:InputFormControl>
                <wssuc:InputFormControl runat="server" LabelText="Announcement list" Visible="True">
                    <Template_Control>
                        <asp:DropDownList id="ddAnnouncementList" runat="server" CssClass="ms-input" />
                    </Template_Control>
                </wssuc:InputFormControl>
                <wssuc:InputFormControl runat="server" LabelText="New profiles" Visible="True">
                    <Template_Control>
                        <asp:CheckBox id="cbNewProfiles" runat="server" CssClass="ms-input" />
                    </Template_Control>
                </wssuc:InputFormControl>
                <wssuc:InputFormControl runat="server" LabelText="Profile updates" Visible="True">
                    <Template_Control>
                        <asp:CheckBox id="cbProfileUpdates" runat="server" CssClass="ms-input" />
                    </Template_Control>
                </wssuc:InputFormControl>

            </Template_InputFormControls>            
        </wssuc:InputFormSection>
        
        <wssuc:ButtonSection runat="server">
            <template_buttons>
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
