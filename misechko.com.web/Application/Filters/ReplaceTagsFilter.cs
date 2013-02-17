using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using HtmlAgilityPack;

namespace misechko.com.Application.Filters
{
    public class ReplaceTagsFilter : MemoryStream
    {
        private readonly Stream _response;
        public ReplaceTagsFilter(Stream response)
        {
            _response = response;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var html = Encoding.UTF8.GetString(buffer);
            html = ReplaceTags(html);
            buffer = Encoding.UTF8.GetBytes(html);
            _response.Write(buffer, offset, buffer.Length);
        }

        private string ReplaceTags(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var divs = doc.DocumentNode.Descendants("div").Where(d =>
                d.Attributes.Contains("class")).ToList(); //&& d.Attributes["class"].Value.Contains("editable-wrapper"));


            var editableDivs = new List<HtmlNode>();

            foreach (var div in divs)
            {
                if(div.Attributes["class"].Value.Contains("editable-wrapper"))
                {
                    editableDivs.Add(div);
                }
            }


            foreach (var editableDiv in editableDivs)
            {
                editableDiv.AppendChild(GetEditButtonElement());
                editableDiv.AppendChild(GetSaveButtonElement());
            }

            return doc.DocumentNode.OuterHtml;
        }

        private HtmlNode GetEditButtonElement()
        {
            var editButtonHtml = "<button class=\"btn edit-button\" type=\"button\" data-bind=\"click: EnableEdit, fadeVisible: EditButtonVisible\"><span>Редактировать блок</span></button>";
           return HtmlNode.CreateNode(editButtonHtml);
        }

        private HtmlNode GetSaveButtonElement()
        {
            var saveButtonHtml = "<button class=\"btn save-button\" type=\"button\" data-bind=\"click: SaveEditedContent, fadeVisible: SaveButtonVisible\"><span>Сохранить изменения</span></button>";
            return HtmlNode.CreateNode(saveButtonHtml);
        }
    }
}