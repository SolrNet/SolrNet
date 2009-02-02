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

    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <div>
        <% foreach (var p in Model.Products) { %>
        <div class="product">
            <div class="productName"><%= p.Name %></div>
            Price: <span class="price"><%= p.Price.ToString("C") %></span><br />
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
        <% foreach (var ps in new[] { 5, 10, 20 }) { %>
            <% if (ps == Model.Search.PageSize) { %>
            <span><%= ps%></span>
            <% } else { %>
            <a href="<%= Url.SetParameters(new {pagesize = ps, page = 1}) %>"><%= ps%></a>
            <% } %>
        <% } %>
        items per page
    </div>
</asp:Content>
