<%@ Page Language="c#" AutoEventWireup="false" Inherits="SPCS.WindowsLiveAuth.LiveAuthProfileEdit, SPCS.WindowsLiveAuth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4b37e1a71dac8e81"
    MasterPageFile="~/_layouts/application.master" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" Src="/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" Src="/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" Src="/_controltemplates/ButtonSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBar" Src="/_controltemplates/ToolBar.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Edit your profile
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea"
    runat="server">
    Edit your profile: 
    <asp:Label ID="LabelTitle" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table cellspacing="0" cellpadding="0">
        <tr>
            <td>
                <span id='part1'>
                    <wssuc:ToolBar CssClass="ms-formtoolbar" id="toolBarTbltop" RightButtonSeparator="&nbsp;"
                        runat="server">
                        <template_rightbuttons>
                          <asp:Button runat="server" class="ms-ButtonHeightWidth" OnClick="Submit_Click" Text="Submit" id="butSubmit2" />
                        </template_rightbuttons>
                    </wssuc:ToolBar>
                    
                    <table class="ms-formtable" style="margin-top: 8px;" border="0" cellpadding="0" cellspacing="0"
                        width="800">
                        <wssuc:InputFormSection runat="server" Title="Information" id="idProfileDisplayName" Visible="True">
                            <template_description>           
            You need a display name and an e-mail address.            
        </template_description>
                            <template_inputformcontrols>
            <wssuc:InputFormControl runat="server" LabelText="Display Name" Visible="True">
                <Template_Control>
	                <SharePoint:InputFormTextBox Title="Display name for user profile" class="ms-input" ID="tbDisplayName" Runat="server" width="390" />
	                <SharePoint:InputFormRequiredFieldValidator  ID="reqtbDisplayName" runat="server" ControlToValidate="tbDisplayName" ErrorMessage="Required" />
                </Template_Control>
            </wssuc:InputFormControl>
            <wssuc:InputFormControl runat="server" LabelText="E-Mail" Visible="True">
                <Template_Control>
	                <SharePoint:InputFormTextBox Title="E-Mail for user profile" class="ms-input" ID="tbEmail" Runat="server" width="390" />
	                <SharePoint:InputFormRequiredFieldValidator  ID="reqtbEmail" runat="server" ControlToValidate="tbEmail" ErrorMessage="Required" />
                </Template_Control>
            </wssuc:InputFormControl>
        </template_inputformcontrols>
                        </wssuc:InputFormSection>
                        <wssuc:InputFormSection runat="server" Title="" id="idProfileAboutMe" Visible="True">
                            <template_description>
            <b>About Me</b><br />
            Describe yourself and upload a photo.
        </template_description>
                            <template_inputformcontrols>
            <wssuc:InputFormControl runat="server" LabelText="About Me" Visible="True">
                <Template_Control>
	                <SharePoint:InputFormTextBox Title="About me information for user profile" class="ms-input" ID="tbAboutMe" RichText="true" RichTextMode="FullHtml" runat="server" TextMode="MultiLine" Rows="5" />
                </Template_Control>
            </wssuc:InputFormControl>
            <wssuc:InputFormControl runat="server" LabelText="Picture" Visible="True">
                <Template_Control>
                    <asp:Image ID="userpicture" runat="server" ImageAlign="Right"/>
                    <asp:FileUpload ID="fileupload" runat="server" CssClass="ms-input" />
                    
	                
                </Template_Control>
            </wssuc:InputFormControl>
        </template_inputformcontrols>
                        </wssuc:InputFormSection>
                        <wssuc:InputFormSection runat="server" Title="" id="idProfileDepartment" Visible="True">
                            <template_description>
            <b>Business info</b>
        </template_description>
                            <template_inputformcontrols>
            <wssuc:InputFormControl runat="server" LabelText="Company" Visible="True">
                <Template_Control>
	                <SharePoint:InputFormTextBox Title="Company for user profile" class="ms-input" ID="tbDepartment" Runat="server" width="390" />
                </Template_Control>
            </wssuc:InputFormControl>
                        <wssuc:InputFormControl runat="server" LabelText="Job Title" Visible="True">
                <Template_Control>
	                <SharePoint:InputFormTextBox Title="Job title for user profile" class="ms-input" ID="tbJobTitle" Runat="server" width="390" />
                </Template_Control>
            </wssuc:InputFormControl>
            <wssuc:InputFormControl runat="server" LabelText="Country" Visible="True">
                <Template_Control>
	                <SharePoint:InputFormTextBox Title="Country" class="ms-input" ID="tbCountry" Runat="server" width="390" />
                </Template_Control>
            </wssuc:InputFormControl>
            <wssuc:InputFormControl runat="server" LabelText="City" Visible="True">
                <Template_Control>
	                <SharePoint:InputFormTextBox Title="City" class="ms-input" ID="tbCity" Runat="server" width="390" />
                </Template_Control>
            </wssuc:InputFormControl>

        </template_inputformcontrols>
                        </wssuc:InputFormSection>
                        <wssuc:InputFormSection runat="server" Title="" id="idUrls" Visible="True">
                            <template_description>
            <b>Other information</b>
        </template_description>
                            <template_inputformcontrols>
            <wssuc:InputFormControl runat="server" LabelText="Home page Url" Visible="True">
                <Template_Control>
	                <SharePoint:InputFormTextBox Title="Url for home page" class="ms-input" ID="tbHomepage" Runat="server" width="390" />
                </Template_Control>
            </wssuc:InputFormControl>
            <wssuc:InputFormControl runat="server" LabelText="Feed Url" Visible="True">
                <Template_Control>
	                <SharePoint:InputFormTextBox Title="Url for feed" class="ms-input" ID="tbFeed" Runat="server" width="390" />
                </Template_Control>
            </wssuc:InputFormControl>
            <wssuc:InputFormControl runat="server" LabelText="Twitter account name" Visible="True">
                <Template_Control>
	                <SharePoint:InputFormTextBox Title="Twitter account name" class="ms-input" ID="tbTwitter" Runat="server" width="390" />
                </Template_Control>
            </wssuc:InputFormControl>
            <wssuc:InputFormControl runat="server" LabelText="SIP Address" Visible="True">
                <Template_Control>
	                <SharePoint:InputFormTextBox Title="SIP Address for user profile" class="ms-input" ID="tbSipAddress" Runat="server" width="390" />
                </Template_Control>
            </wssuc:InputFormControl>
        </template_inputformcontrols>
                        </wssuc:InputFormSection>
                    </table>
                    <table cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td class="ms-formline">
                                <img src="/_layouts/images/blank.gif" width="1" height="1" alt="">
                            </td>
                        </tr>
                    </table>
                    <table cellpadding="0" cellspacing="0" width="100%" style="padding-top: 7px">
                        <tr>
                            <td width="100%">
                                
                                <wssuc:ToolBar CssClass="ms-formtoolbar" id="toolBarTbl" RightButtonSeparator="&nbsp;"
                                    runat="server">
                                    <template_buttons>
                                    
                                </template_buttons>
                                    <template_rightbuttons>
                                    <asp:Button runat="server" class="ms-ButtonHeightWidth" OnClick="Submit_Click" Text="Submit" id="butSubmit" />
                                </template_rightbuttons>
                                </wssuc:ToolBar>
                            </td>
                        </tr>
                    </table>
                </span>
            </td>        </tr>
            <tr> 
                <td class="ms-descriptiontext">
                        Created by <a href="http://www.wictorwilen.se">Wictor Wilén</a>, source and license at <a href="http://spwla.codeplex.com/">http://spwla.codeplex.com/</a>
                </td> 
            </tr>

    </table>
</asp:Content>
