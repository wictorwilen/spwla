<%@ Page Language="c#" AutoEventWireup="false" Inherits="SPCS.WindowsLiveAuth.LiveAuthProfile, SPCS.WindowsLiveAuth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4b37e1a71dac8e81"
    MasterPageFile="~/_layouts/application.master" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" Src="/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" Src="/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" Src="/_controltemplates/ButtonSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBar" Src="/_controltemplates/ToolBar.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ToolBarButton" src="~/_controltemplates/ToolBarButton.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    User information
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea"
    runat="server">
    <SharePoint:EncodedLiteral ID="EncodedLiteral2" runat="server" Text="<%$Resources:wss,userdisp_pagetitleintitle%>"
        EncodeMethod='HtmlEncode' />
    <asp:Label ID="LabelTitle" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table cellspacing="0" cellpadding="0">
        <tr>
            <td>
                <span id='part1'>
                    <wssuc:ToolBar  id="toolBarTbltop" RightButtonSeparator="&nbsp;"
                        runat="server">
                                                <template_buttons>
                          <wssuc:ToolBarButton runat="server"
						Text="Edit"
						id="tbbEdit"
						NavigateUrl="liveauth-editprofile.aspx"
						AccessKey="E"/>
                    </template_buttons>
                        <template_rightbuttons>
                          <asp:Button runat="server" class="ms-ButtonHeightWidth" OnClick="Close_Click" Text="Close" id="butClose2" />
                        </template_rightbuttons>
                    </wssuc:ToolBar>
                    
                    <table class="ms-formtable" style="margin-top: 8px;" border="0" cellpadding="0" cellspacing="0"
                        width="800">                  
                        
                        <wssuc:InputFormSection runat="server" Title="Information" id="idProfileDisplayName" Visible="True">
                            <template_description>           
                            </template_description>
                            <template_inputformcontrols>
                            <asp:Image runat="Server" id="imImage" ImageAlign="Right" />
                                <h3 class="ms-standardheader">Name:</h3><asp:Label ID="lblDisplayName" Runat="Server" /><br /><br />
                                <h3 class="ms-standardheader">E-mail:</h3><asp:HyperLink ID="hlEmail" Runat="Server" /><br /><br />
                                <h3 class="ms-standardheader">About:</h3><asp:Label ID="lbAbout" Runat="Server" /><br /><br />
                                <h3 class="ms-standardheader">Company:</h3><asp:Label ID="lbCompany" Runat="Server" /><br /><br />
                                <h3 class="ms-standardheader">Title:</h3><asp:Label ID="lbTitle" Runat="Server" /><br /><br />
                                <h3 class="ms-standardheader">Country:</h3><asp:Label ID="lbCountry" Runat="Server" /><br /><br />
                                <h3 class="ms-standardheader">City:</h3><asp:Label ID="lbCity" Runat="Server" /><br /><br />                                
                                <h3 class="ms-standardheader">Web Site:</h3><asp:HyperLink ID="hlWebsite" Runat="Server" /><br /><br />
                                <h3 class="ms-standardheader">Feed:</h3><asp:HyperLink ID="hlFeed" Runat="Server" /><br /><br />
                                <h3 class="ms-standardheader">Twitter:</h3><asp:HyperLink ID="hlTwitter" Runat="Server" /><br />
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
                   
                </span>
            </td>        </tr>
            <tr> 
                <td class="ms-descriptiontext">
                        Created by <a href="http://www.wictorwilen.se">Wictor Wilén</a>, source and license at <a href="http://spwla.codeplex.com/">http://spwla.codeplex.com/</a>
                </td> 
            </tr>
    </table>
  
</asp:Content>
