#pragma checksum "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Home\Chat.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c3a98173abdd959e9b01bc6347cfe8c5a15464d0"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Chat), @"mvc.1.0.view", @"/Views/Home/Chat.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c3a98173abdd959e9b01bc6347cfe8c5a15464d0", @"/Views/Home/Chat.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d1cab75cfe10a27103e259ffc2630fd7ed5fb803", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Chat : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ChatViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/microsoft/signalr/dist/browser/signalr.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<style>\r\n    #messages{\r\n        border: 1px solid black;\r\n        border-radius: 5px;\r\n    }\r\n</style>\r\n<input id=\"msg\" type=\"text\" placeholder=\"Сообщение\" />\r\n<button id=\"btn\">Отправить</button>\r\n<div id=\"messages\"></div>\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c3a98173abdd959e9b01bc6347cfe8c5a15464d03890", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
<script>
    var connection = new signalR.HubConnectionBuilder().withUrl(""/chathub"").build();
    connection.start();
    
    document.getElementById(""btn"").addEventListener(""click"", (e) => {
        e.preventDefault();
        let input = document.getElementById(""msg"");
        let msg = input.value;
        input.value = """";
        connection.invoke(""NewMessageRequest"", msg, """);
#nullable restore
#line 22 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Home\Chat.cshtml"
                                                Write(Model.Username);

#line default
#line hidden
#nullable disable
            WriteLiteral(@""");

    });
    connection.on(""NewMessage"", (time, sender, message)=>{
        let elem = document.createElement(""p"");
        elem.innerText = `${time} ${sender}: ${message}`;
        let firstElem = document.getElementById(""messages"").firstChild;
        document.getElementById(""messages"").insertBefore(elem, firstElem);
    });

    connection.on(""MemberJoined"", username=>{
        let elem = document.createElement(""p"");
        elem.style.backgroundColor = ""lightgreen"";
        elem.innerText = `Пользователь ${username} подключился к чату`;
        let firstElem = document.getElementById(""messages"").firstChild;
        document.getElementById(""messages"").insertBefore(elem, firstElem);
    });

    connection.on(""MemberLeft"", username=>{
        let elem = document.createElement(""p"");
        elem.style.backgroundColor=""red"";
        elem.innerText = `Пользователь ${username} покинул чат`;
        let firstElem = document.getElementById(""messages"").firstChild;
        document.getEle");
            WriteLiteral("mentById(\"messages\").insertBefore(elem, firstElem);\r\n    });\r\n    window.addEventListener(\"unload\", e=>{\r\n        connection.invoke(\"MemberLeft\", \"");
#nullable restore
#line 48 "C:\Users\timur\source\repos\ChatApp\ChatApp\Views\Home\Chat.cshtml"
                                    Write(Model.Username);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n    });\r\n</script>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ChatViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
