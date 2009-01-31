<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ProductView>" %>
<%@ Import Namespace="SampleSolrApp.Models"%>

<asp:Content ID="indexHead" ContentPlaceHolderID="head" runat="server">
    <title>Welcome to Megastore! 'Mega' means 'good', 'store' means 'thing'.</title>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <div>
        <% foreach (var p in ViewData.Model.Products) { %>
        <div class="product">
            <div class="productName"><%= p.Name %></div>
            Price: <span class="price"><%= p.Price.ToString("C") %></span><br />
        </div>
        <%} %>
    </div>
    <div>
        Results <%= ViewData.Model.FirstResultIndex %> - <%= ViewData.Model.LastResultIndex %> of <b><%= ViewData.Model.TotalCount %></b>
    </div>
</asp:Content>
