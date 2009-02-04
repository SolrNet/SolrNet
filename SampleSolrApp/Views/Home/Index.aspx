<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ProductView>" %>
<%@ Import Namespace="SampleSolrApp.Helpers"%>
<%@ Import Namespace="SampleSolrApp.Models"%>

<asp:Content ID="indexHead" ContentPlaceHolderID="head" runat="server">
    <title>Welcome to Megastore! 'Mega' means 'good', 'store' means 'thing'.</title>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <form method="get" action="<%= Url.Action("Index") %>">
        <%= Html.TextBox("q", Model.Search.FreeSearch) %>
        <input type="submit" value="Search" />
    </form>
    
    <div class="leftColumn">
        <ul>
            
        </ul>
    </div>

    <div class="rightColumn">
        <div>
            <% foreach (var p in Model.Products) { %>
            <div class="product">
                <div class="productName"><%= p.Name %></div>
                Price: <span class="price"><%= p.Price.ToString("C") %></span><br />
                Categories: <% Html.Repeat(p.Categories, cat => { %>
                    <a href="<%= Url.SetParameters(new {f_cat = cat, page = 1 }) %>"><%= cat %></a><% }, () => {%>, <% }); %>
            </div>
            <%} %>
        </div>
        
        <% Html.RenderPartial("Pagination", new PaginationInfo {
            PageUrl = Url.SetParameter("page", "!0"),
            CurrentPage = Model.Search.PageIndex, 
            PageSize = Model.Search.PageSize,
            TotalItemCount = Model.TotalCount,
        }); %>
        
        <div class="pagesize">
            <% Html.Repeat(new[] { 5, 10, 20 }, ps => { %>
                <% if (ps == Model.Search.PageSize) { %>
                <span><%= ps%></span>
                <% } else { %>
                <a href="<%= Url.SetParameters(new {pagesize = ps, page = 1}) %>"><%= ps%></a>
                <% } %>
            <% }, () => { %> | <% }); %>
            items per page
        </div>
    </div>
</asp:Content>
