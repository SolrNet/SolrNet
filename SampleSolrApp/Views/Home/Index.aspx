<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ProductView>" %>
<%@ Import Namespace="SampleSolrApp.Models"%>

<asp:Content ID="indexHead" ContentPlaceHolderID="head" runat="server">
    <title>Welcome to Megastore! 'Mega' means 'good', 'store' means 'thing'.</title>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <p>
        <% foreach (var p in ViewData.Model.Products) { %>
        <%= p.Name %><br />
        <%= p.Price.ToString("C") %><br />
        <%} %>
    </p>
    <p>
        Results <%= ViewData.Model.FirstResultIndex %> - <%= ViewData.Model.LastResultIndex %> of <b><%= ViewData.Model.TotalCount %></b>
    </p>
</asp:Content>
