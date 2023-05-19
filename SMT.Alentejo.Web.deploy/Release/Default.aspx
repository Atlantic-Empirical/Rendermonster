<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="SMT.Alentejo.Web._Default" %>

<%@ Register Assembly="System.Web.Silverlight" Namespace="System.Web.UI.SilverlightControls"
    TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" style="height:100%;">
<head id="Head1" runat="server">
    <title>SMT Render Monster</title>
    <link rel="shortcut icon" href="Resources/RM.ico"/>
</head>
<body style="background-color: #000000">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div style="top: 50%; margin-top:0px; margin: auto; width: 930px; height: 600px"> 
            <asp:Silverlight ID="Xaml1" runat="server" Source="~/ClientBin/Alentejo_SLA.xap" MinimumVersion="2.0.31005.0" Width="930" Height="600" />
        </div> 
        <iframe runat="server" id="ifDownloader" height="0" width="0" frameborder="0" />
    </form>
</body>
</html>