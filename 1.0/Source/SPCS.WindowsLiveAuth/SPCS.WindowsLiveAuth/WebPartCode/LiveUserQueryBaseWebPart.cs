/*
 * 
 * Windows Live ID Membership Provider for SharePoint
 * originally developed for SharePointCommunity.se
 * http://www.sharepointcommunity.se/
 * ------------------------------------------
 * Copyright (c) 2009, Wictor Wilén
 * http://spwla.codeplex.com/
 * http://www.wictorwilen.se/
 * ------------------------------------------
 * Licensed under the Microsoft Public License (Ms-PL) 
 * http://www.opensource.org/licenses/ms-pl.html
 * 
 */
using System;
using System.Web.UI.WebControls.WebParts;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using System.Web.UI;

namespace SPCS.WindowsLiveAuth {
    public abstract class LiveUserQueryBaseWebPart : System.Web.UI.WebControls.WebParts.WebPart {

        protected LinkButton lbPrevious;
        protected LinkButton lbNext;
        protected Label lCurrentPage;
        protected Label lLastPage;
        protected Table tTable;
        protected int iCurrentPage;
        protected TableRow pagerRow;

        [WebBrowsable]
        [WebDisplayName("Number of users")]
        [WebDescription("The maximum number of users to display")]
        [Category("Configuration")]
        [Personalizable(PersonalizationScope.Shared)]
        public int NumberOfUsers {
            get;
            set;
        }
        [WebBrowsable]
        [WebDisplayName("Columns")]
        [WebDescription("The number of columns to use")]
        [Category("Configuration")]
        [Personalizable(PersonalizationScope.Shared)]
        public int Columns {
            get;
            set;
        }
        [WebBrowsable]
        [WebDisplayName("Image size")]
        [WebDescription("The size of the image")]
        [Category("Configuration")]
        [Personalizable(PersonalizationScope.Shared)]
        public int ImageSize {
            get;
            set;
        }
        [WebBrowsable]
        [WebDisplayName("Users per page")]
        [WebDescription("The number of user per page")]
        [Category("Configuration")]
        [Personalizable(PersonalizationScope.Shared)]
        public int UsersPerPage {
            get;
            set;
        }
        [WebBrowsable]
        [WebDisplayName("Show Pager")]
        [WebDescription("Show the pager")]
        [Category("Configuration")]
        [Personalizable(PersonalizationScope.Shared)]
        public bool ShowPager{
            get;
            set;
        }
        [WebBrowsable]
        [WebDisplayName("Require logged in user")]
        [WebDescription("Only shows the information for logged in users")]
        [Category("Configuration")]
        [Personalizable(PersonalizationScope.Shared)]
        public bool RequireLogin {
            get;
            set;
        }


        public LiveUserQueryBaseWebPart() {
            this.ExportMode = WebPartExportMode.All;
        }


        protected override void CreateChildControls() {

            base.CreateChildControls();

            if (this.RequireLogin && !this.Page.User.Identity.IsAuthenticated) {
                this.Controls.Add(new LiteralControl("<div class='ms-vb'>The information is only shown to authenticated users.</div>"));
                return;
            }

            try {

                

                tTable = new Table();
                
                tTable.Width = new Unit("100%");
                tTable.CellPadding = 0;
                tTable.CellSpacing = 0;
                tTable.BorderWidth = new Unit("0");

                TableRow row = new TableRow();
                tTable.Rows.Add(row);
                TableCell cell = new TableCell();
                cell.ColumnSpan = this.Columns;
                cell.CssClass = "ms-partline";
                row.Cells.Add(cell);
                cell.Controls.Add(new LiteralControl("<img alt='' src='/_layouts/images/blank.gif' width='1' height='1' complete='complete'/>"));
                pagerRow = new TableRow();
                tTable.Rows.Add(pagerRow);
                cell = new TableCell();
                pagerRow.Cells.Add(cell);
                cell.CssClass = "ms-addnew";
                cell.Attributes.Add("style", "padding-bottom: 3px;");
                cell.ColumnSpan = this.Columns;
                
                lCurrentPage = new Label();
                lCurrentPage.Text = "1";
                lCurrentPage.EnableViewState = true;
                cell.Controls.Add(new LiteralControl("<img alt='' src='/_layouts/images/rect.gif' complete='complete'/>&nbsp;"));
                cell.Controls.Add(lCurrentPage);
                cell.Controls.Add(new LiteralControl("/"));
                lLastPage = new Label();
                cell.Controls.Add(lLastPage);
                cell.Controls.Add(new LiteralControl("&nbsp;|&nbsp;"));

                lbNext = new LinkButton();
                lbNext.Text = "Next";
                lbNext.Click += new EventHandler(lbNext_Click);
                
                
                lbPrevious = new LinkButton();
                lbPrevious.Text = "Previous";
                lbPrevious.Click += new EventHandler(lbPrevious_Click);
                
                cell.Controls.Add(lbPrevious);
                cell.Controls.Add(new LiteralControl("&nbsp;|&nbsp;"));
                cell.Controls.Add(lbNext);

                if (!this.ShowPager) {
                    pagerRow.Visible = false;
                }
                          
                this.Controls.Add(tTable);
               
            }
            catch (Exception ex) {
                HandleException(ex);
            }
        }
        protected override void LoadViewState(object savedState) {
            if (savedState != null) {
                iCurrentPage = (int)savedState;
            }
        }
        protected override object SaveViewState() {
            return iCurrentPage;
        }
        protected override object SaveControlState() {
            return base.SaveControlState();
        }
        protected override void OnPreRender(EventArgs e) {
            if (this.RequireLogin && !this.Page.User.Identity.IsAuthenticated) {
                return;
            }
            base.OnPreRender(e);

            try {
                LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);
                if (settings == null) {
                    return;
                }
                SPSecurity.RunWithElevatedPrivileges(delegate() {
                    using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                        using (SPWeb web = site.OpenWeb()) {
                            SPList list = web.Lists[settings.ProfileListName];
                            SPQuery query = new SPQuery();
                            query.Query = this.Query;
                            if (this.NumberOfUsers > 0) {
                                query.RowLimit = (uint)this.NumberOfUsers;
                            }
                            SPListItemCollection items = list.GetItems(query);
                            int count = items.Count;
                            int added = 0;
                            TableRow row = null;
                            // paging support                            
                            int maxPage = (int)Math.Ceiling((double)list.ItemCount / this.UsersPerPage);
                            if (iCurrentPage == 0) {
                                iCurrentPage = 1;
                            }
                            lbPrevious.Enabled = !(iCurrentPage == 1);
                            lbNext.Enabled = !(maxPage == iCurrentPage);
                            lCurrentPage.Text = iCurrentPage.ToString();    
                            lLastPage.Text = maxPage.ToString();
                            for (int i = 0; 
                                    i < this.UsersPerPage && (iCurrentPage-1)*this.UsersPerPage +i < items.Count; 
                                    i++) {

                                SPListItem item = items[(iCurrentPage - 1) * this.UsersPerPage + i];
                                added++;
                                if (i % this.Columns == 0) {
                                    row = new TableRow();
                                    tTable.Rows.AddAt(tTable.Rows.Count-2, row);
                                }
                                TableCell cell = new TableCell();
                                LiveCommunityUserControl ctrl = new LiveCommunityUserControl();
                                ctrl.LiveCommunityUser = LiveCommunityUser.GetUser(item["UUID"].ToString());                                
                                //ctrl.Width = new Unit((100 / this.Columns).ToString() + "%");
                                ctrl.ImageSize = this.ImageSize;
                                cell.Controls.Add(ctrl);
                                row.Controls.Add(cell);
                            }
                            while (added % this.Columns != 0) {
                                row.Cells.Add(new TableCell());
                                added++;
                            }
                        }
                    }
                });
                
            }
            catch (Exception ex) {
                HandleException(ex);
            }
            
        }

        void lbNext_Click(object sender, EventArgs e) {
            iCurrentPage++;
        }

        void lbPrevious_Click(object sender, EventArgs e) {
            iCurrentPage--;
        }

        /// <summary>
        /// Clear all child controls and add an error message for display.
        /// </summary>
        /// <param name="ex"></param>
        private void HandleException(Exception ex) {
            this.Controls.Clear();
            this.Controls.Add(new LiteralControl(ex.Message));
        }
        public abstract string Query { 
            get; 
        }

    }
}
