<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ProductView>" %>
<%@ Import Namespace="SampleSolrApp.Helpers"%>
<%@ Import Namespace="SampleSolrApp.Models"%>

<asp:Content ID="indexHead" ContentPlaceHolderID="head" runat="server">
    <title>Welcome to Megastore! 'Mega' means 'good', 'store' means 'thing'.</title>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <div>
        <% foreach (var p in Model.Products) { %>
        <div class="product">
            <div class="productName"><%= p.Name %></div>
            Price: <span class="price"><%= p.Price.ToString("C") %></span><br />
        </div>
        <%} %>
    </div>
    <div>
        Results <%= Model.Search.FirstItemIndex+1 %> - <%= Model.Search.LastItemIndex%> of <b><%= Model.TotalCount %></b>
    </div>
    
    <% Html.RenderPartial("Pagination", new PaginationInfo {
        PageUrl = Url.SetParameter("page", "!0"),
        CurrentPage = Model.Search.PageIndex, 
        PageSize = Model.Search.PageSize,
        TotalItemCount = Model.TotalCount,
    }); %>
</asp:Content>
