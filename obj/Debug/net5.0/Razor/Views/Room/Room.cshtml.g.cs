#pragma checksum "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "972be4f4ef7b01a5c665d331bea61a784c73b7d4"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Room_Room), @"mvc.1.0.view", @"/Views/Room/Room.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\_ViewImports.cshtml"
using ChatApp.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\_ViewImports.cshtml"
using ChatApp.Controllers;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Html;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"972be4f4ef7b01a5c665d331bea61a784c73b7d4", @"/Views/Room/Room.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d1cab75cfe10a27103e259ffc2630fd7ed5fb803", @"/Views/_ViewImports.cshtml")]
    public class Views_Room_Room : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<RoomViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", "~/js/room-signalr.js", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.ScriptTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div id=\"div-main\">\r\n    <h1>");
#nullable restore
#line 4 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
    Write(Model.Message=="" ? $"Комната {Model.RoomName}" : Model.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h1>\r\n    <input type=\"text\" id=\"msg\" />\r\n    <input type=\"submit\" id=\"btnsendmsg\" value=\"Отправить\" /> \r\n    <br />\r\n    <div id=\"messages\">\r\n");
#nullable restore
#line 9 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
         if(Model.LastMessages is not null){
            

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
             foreach(var msg in Model.LastMessages)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <p>");
#nullable restore
#line 12 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
               Write(msg.DateTime.ToShortTimeString());

#line default
#line hidden
#nullable disable
            WriteLiteral(" ");
#nullable restore
#line 12 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
                                                   Write(msg.SenderName);

#line default
#line hidden
#nullable disable
            WriteLiteral(": ");
#nullable restore
#line 12 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
                                                                     Write(msg.Text);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n");
#nullable restore
#line 13 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
             
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </div>\r\n        <div id=\"users\">\r\n\r\n");
#nullable restore
#line 19 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
         foreach(var v in Model.UsersInRoom)
            {
                if (Model.RoomAdmins.FirstOrDefault(a=>a.Equals(v.User))!=null)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <p style=\"color:crimson;\"");
            BeginWriteAttribute("id", " id=\"", 740, "\"", 756, 1);
#nullable restore
#line 23 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
WriteAttributeValue("", 745, v.UserName, 745, 11, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">Админ ");
#nullable restore
#line 23 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
                                                                Write(v.UserName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n");
#nullable restore
#line 24 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
                }
                else
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <p");
            BeginWriteAttribute("id", " id=\"", 863, "\"", 879, 1);
#nullable restore
#line 27 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
WriteAttributeValue("", 868, v.UserName, 868, 11, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">");
#nullable restore
#line 27 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
                                   Write(v.UserName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n");
#nullable restore
#line 28 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
                }
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </div>\r\n</div>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "972be4f4ef7b01a5c665d331bea61a784c73b7d47894", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.ScriptTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper.Src = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 32 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Room\Room.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper.AppendVersion = true;

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-append-version", __Microsoft_AspNetCore_Mvc_TagHelpers_ScriptTagHelper.AppendVersion, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(" ");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<RoomViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
