<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="WebUI.Payment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            height: 414px;
            width: 786px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>  
            <table class="auto-style1">  
                 <asp:Label ID="order_error" runat="server" Text=" " ForeColor="Red" BackColor="Black"></asp:Label> 
                <tr>  
                    <td> Full Name :</td>  
                    <td>  
                        <asp:TextBox ID="TextBox1" runat="server" Height="30" Width="500px"></asp:TextBox>  
                    </td>  
  
               </tr>  
                <tr>  
                    <td>Card Numbers</td>  
                     <td> <asp:TextBox ID="TextBox2" runat="server" Width="500px" Height="30"></asp:TextBox></td>  
                </tr>  
                <tr>  
                    <td>Email</td>  
                    <td>  
                        <asp:TextBox ID="TextBox3" runat="server" Width="500px" Height="30" TextMode="Email"></asp:TextBox>  
                    </td>  
                </tr>  
                <tr>  
                    <td>City</td>  
                    <td>  
                        <asp:DropDownList ID="DropDownList1" runat="server">  
                            <asp:ListItem Text="Select City" Value="select" Selected="True"></asp:ListItem>  
                            <asp:ListItem Text="Kristianstad" Value="Kristianstad"></asp:ListItem>  
                            <asp:ListItem Text="Malmo" Value="Malmo"></asp:ListItem>  
                            <asp:ListItem Text="Stockholm" Value="Stockholm"></asp:ListItem>  
                        </asp:DropDownList>  
                    </td>  
                </tr>  
               
                <tr>  
                    <td>Price</td>  
                    <td>  
                        <asp:Label ID="Label_offer" runat="server"></asp:Label>  
                    </td>
                     <td>  
                       <asp:Label ID="error" runat="server" Text=" "></asp:Label> 
                    </td>
                </tr>  
                <tr>  
                    <td>  
                         <asp:Label ID="Label1" runat="server" Text="Click [Get Offer] to see total price and [Pay] to finalize order" ForeColor="Red" Width="450px"></asp:Label>
                        <br />
                        <br />

                        <asp:Button ID="Button1" runat="server" OnClick="get_result" Text="GET OFFER" Width="125px" Height="35"  BackColor="Orange"/> 
                        <asp:Button ID="Button2" runat="server" OnClick="btn_pay" Text="PAY" Width="125px" Height="35"  BackColor="Orange"/> 
                    </td>  
                </tr>  
            </table>  
        </div>  
    </form>
</body>
</html>
